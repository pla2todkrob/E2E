using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [Authorize]
    public class BusinessCardsController : Controller
    {
        private readonly ClsApi clsApi = new ClsApi();
        private readonly ClsManageService data = new ClsManageService();
        private readonly ClsManageBusinessCard dataCard = new ClsManageBusinessCard();
        private readonly ClsContext db = new ClsContext();

        private static Guid? UserAuthorized { get; set; }

        public BusinessCardsController()
        {
        }

        public ActionResult BusinessCard_Create(Guid? id)
        {
            ClsBusinessCard businessCards;
            if (!id.HasValue)
            {
                Guid userId = Guid.Parse(HttpContext.User.Identity.Name);
                businessCards = new ClsBusinessCard()
                {
                    User_id = userId,
                    UserDetails = db.UserDetails.Where(w => w.User_Id == userId).FirstOrDefault()
                };
            }
            else
            {
                businessCards = new ClsBusinessCard()
                {
                    User_id = id.Value,
                    UserDetails = db.UserDetails.Where(w => w.User_Id == id.Value).FirstOrDefault()
                };
            }

            ViewBag.UserList = data.SelectListItems_User();
            return View(businessCards);
        }

        [HttpPost]
        public ActionResult BusinessCard_Create(ClsBusinessCard Model)
        {
            ClsSwal swal = new ClsSwal();
            if (Model.User_id.HasValue)
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    try
                    {
                        if (dataCard.BusinessCard_SaveCreate(Model))
                        {
                            scope.Complete();
                            swal.DangerMode = false;
                            swal.Icon = "success";
                            swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.Title = "Successful";
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.Title = ex.Source;
                        swal.Text = ex.Message;
                        Exception inner = ex.InnerException;
                        while (inner != null)
                        {
                            swal.Title = inner.Source;
                            swal.Text += string.Format("\n{0}", inner.Message);
                            inner = inner.InnerException;
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.Icon = "warning";
                swal.Title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.Text))
                        {
                            swal.Text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.Text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BusinessCard_Detail(Guid id)
        {
            UserAuthorized = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.UserCardList = dataCard.SelectListItems_CardGroup();
            ViewBag.GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            ViewBag.OrderBusinessCard = db.BusinessCards.Where(w => w.BusinessCard_Id == id).Select(s => s.System_Statuses.OrderBusinessCard).FirstOrDefault();
            ViewBag.authorized = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();
            ViewBag.UserCHK = db.BusinessCardFiles.Any(a => a.BusinessCard_Id == id && a.Confirm == true);
            var clsBusinessCard = QueryClsBusinessCard().Where(w => w.BusinessCard_Id == id);

            return View(clsBusinessCard.FirstOrDefault());
        }

        public ActionResult BusinessCard_Model()
        {
            ClsBusinessCardModel cardModel = new ClsBusinessCardModel
            {
                Company_en = "THAI PARKERIZING CO.,LTD.",
                Parent_company = "NIHON PARKERIZING CO.,LTD. GROUP",
                NameEN = "SOMBOONLAP KHAMSAN",
                NameTH = "สมบูรณ์ลาภ คำสาร",
                Position = "Staff",
                Office_Number = "6157"
            };

            List<ClsBusinessCardModel> Cards = new List<ClsBusinessCardModel>();

            for (int i = 0; i < 10; i++)
            {
                Cards.Add(cardModel);
            }

            return View(Cards);
        }

        public ActionResult BusinessCard_UploadFile(Guid id)
        {
            return View(id);
        }

        public ActionResult Cancel(Guid? id)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 6;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.BusinessCard_SaveLog(businessCards);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public List<ClsBusinessCardModel> CardBack(string res, Guid id)
        {
            List<ClsBusinessCardModel> models = new List<ClsBusinessCardModel>();
            var BusinessCard = db.BusinessCards.Find(id);
            var UserDetail = db.UserDetails.Where(w => w.User_Id == BusinessCard.User_id).FirstOrDefault();

            string plant1 = string.Empty;
            string plant2 = string.Empty;

            switch (res)
            {
                case "Bangpoo12":

                    plant1 = "ESIE1";
                    plant2 = "Gateway";

                    break;

                case "ESIE1":

                    plant1 = "Bangpoo12";
                    plant2 = "Gateway";

                    break;

                case "Gateway":

                    plant1 = "Bangpoo12";
                    plant2 = "ESIE1";

                    break;

                default:

                    plant1 = "ESIE1";
                    plant2 = "Gateway";

                    break;
            }

            var SelectPlant1 = db.PlantDetails.Where(w => w.Master_Plants.Plant_Name == plant1).FirstOrDefault();
            var SelectPlant2 = db.PlantDetails.Where(w => w.Master_Plants.Plant_Name == plant2).FirstOrDefault();

            ClsBusinessCardModel cardModel1 = new ClsBusinessCardModel
            {
                Company_en = "THAI PARKERIZING CO., LTD.",
                Company_th = "บริษัท ไทยปาร์คเกอร์ไรซิ่ง จำกัด",
                Parent_company = "NIHON PARKERIZING CO.,LTD. GROUP",
                Company_Web = "www.thaiparker.co.th",

                NameEN = string.Format("{0} {1}", UserDetail.Detail_EN_FirstName, UserDetail.Detail_EN_LastName),
                NameTH = string.Format("{0} {1}", UserDetail.Detail_TH_FirstName, UserDetail.Detail_TH_LastName),
                Position = UserDetail.Users.Master_Grades.Grade_Position,
                Dept = string.Format("{0} Department", UserDetail.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name),
                Address1 = SelectPlant1.OfficeAddress1,
                Address2 = SelectPlant1.OfficeAddress2,
                Office_Number = string.Format("Office: {0}", SelectPlant1.OfficeNumber),
                Fax = string.Format("Fax: {0}", SelectPlant1.OfficeFax)
            };
            cardModel1.Email = string.Format("{0} / {1}", UserDetail.Users.User_Email, cardModel1.Company_Web);
            cardModel1.HeadOffice = SelectPlant1.OfficeName;

            ClsBusinessCardModel cardModel2 = new ClsBusinessCardModel
            {
                Company_en = "THAI PARKERIZING CO., LTD.",
                Company_th = "บริษัท ไทยปาร์คเกอร์ไรซิ่ง จำกัด",
                Parent_company = "NIHON PARKERIZING CO.,LTD. GROUP",
                Company_Web = "www.thaiparker.co.th",

                NameEN = string.Format("{0} {1}", UserDetail.Detail_EN_FirstName, UserDetail.Detail_EN_LastName),
                NameTH = string.Format("{0} {1}", UserDetail.Detail_TH_FirstName, UserDetail.Detail_TH_LastName),
                Position = UserDetail.Users.Master_Grades.Grade_Position,
                Dept = string.Format("{0} Department", UserDetail.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name),
                Address1 = SelectPlant2.OfficeAddress1,
                Address2 = SelectPlant2.OfficeAddress2,
                Office_Number = string.Format("Office: {0}", SelectPlant2.OfficeNumber),
                Fax = string.Format("Fax: {0}", SelectPlant2.OfficeFax)
            };
            cardModel2.Email = string.Format("{0} / {1}", UserDetail.Users.User_Email, cardModel2.Company_Web);
            cardModel2.HeadOffice = SelectPlant2.OfficeName;

            models.Add(cardModel1);
            models.Add(cardModel2);

            return models;
        }

        // id businessCard
        public ActionResult DownloadPDF(Guid id)
        {
            try
            {
                var BusinessCard = db.BusinessCards.Find(id);
                var UserDetail = db.UserDetails.Where(w => w.User_Id == BusinessCard.User_id).FirstOrDefault();
                var SelectPlant = db.PlantDetails.Where(w => w.Plant_Id == UserDetail.Users.Master_Plants.Plant_Id).FirstOrDefault();

                ClsBusinessCardModel cardModel = new ClsBusinessCardModel
                {
                    //default
                    Company_en = "THAI PARKERIZING CO., LTD.",
                    Company_th = "บริษัท ไทยปาร์คเกอร์ไรซิ่ง จำกัด",
                    Parent_company = "NIHON PARKERIZING CO.,LTD. GROUP",
                    Company_Web = "www.thaiparker.co.th",

                    NameEN = string.Format("{0} {1}", UserDetail.Detail_EN_FirstName, UserDetail.Detail_EN_LastName),
                    NameTH = string.Format("{0} {1}", UserDetail.Detail_TH_FirstName, UserDetail.Detail_TH_LastName),
                    Position = UserDetail.Users.Master_Grades.Grade_Position,
                    Dept = string.Format("{0} Department", UserDetail.Users.Master_Processes.Master_Sections.Master_Departments.Department_Name),
                    Address1 = SelectPlant.OfficeAddress1,
                    Address2 = SelectPlant.OfficeAddress2,
                    Office_Number = string.Format("Office: {0}, Ext. {1}", SelectPlant.OfficeNumber, BusinessCard.Tel_Internal)
                };
                if (string.IsNullOrEmpty(BusinessCard.Tel_Internal))
                {
                    cardModel.Office_Number = string.Format("Office: {0}", SelectPlant.OfficeNumber);
                }
                cardModel.Fax = string.Format("Fax: {0}, Mobile: {1}", SelectPlant.OfficeFax, FormatPhoneNumber(BusinessCard.Tel_External));
                if (string.IsNullOrEmpty(BusinessCard.Tel_External))
                {
                    cardModel.Fax = string.Format("Fax: {0}", SelectPlant.OfficeFax);
                }

                cardModel.Email = string.Format("{0} / {1}", UserDetail.Users.User_Email, cardModel.Company_Web);
                cardModel.HeadOffice = SelectPlant.OfficeName;

                List<ClsBusinessCardModel> Cards = new List<ClsBusinessCardModel>();
                List<ClsBusinessCardModel> Cards2 = new List<ClsBusinessCardModel>();
                Cards2 = CardBack(SelectPlant.Master_Plants.Plant_Name, id);

                for (int i = 0; i < 10; i++)
                {
                    Cards.Add(cardModel);
                }

                MemoryStream stream = new MemoryStream();

                // สร้างไฟล์ PDF โดยใช้ PDFSharp
                PdfSharp.Pdf.PdfDocument pdf = new PdfSharp.Pdf.PdfDocument();
                PdfPage page = pdf.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics graphics = XGraphics.FromPdfPage(page);

                PdfPage page2 = pdf.AddPage();
                page2.Size = PdfSharp.PageSize.A4;
                XGraphics graphics2 = XGraphics.FromPdfPage(page2);

                double CardWidth = page.Width / 2;
                double yLeft = 14.173228346;
                double yRight = 14.173228346;
                double xLeft = 35.433070866142;
                double xRight = 297.63779527559;

                double xLeft_txt = 35.433070866142;
                double xRight_txt = 297.63779527559;

                PdfSharp.Drawing.XImage LogoTPs = PdfSharp.Drawing.XImage.FromStream(LogoTP());
                PdfSharp.Drawing.XImage QrCodeTPs = PdfSharp.Drawing.XImage.FromStream(QrCodeTP());
                //806.45669291339
                graphics.DrawRectangle(XPens.Black, new XRect(xLeft, yLeft, 524.4094488189, 809.45669291339));
                //graphics2.DrawRectangle(XPens.Black, new XRect(xLeft, yLeft, 524.4094488189, 809.45669291339));

                // สร้างหน้าเอกสาร
                for (int i = 0; i < Cards.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        double yLogo_L = yLeft + 11;
                        graphics.DrawImage(LogoTPs, xLeft + 10, yLogo_L, 53, 33);

                        graphics.DrawString(Cards[i].Company_en ?? "", new XFont("Roboto", 12, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft + 70, yLogo_L += 15, 60, 0));
                        graphics.DrawString(Cards[i].Parent_company ?? "", new XFont("Roboto", 9), XBrushes.Black, new XRect(xLeft + 70, yLogo_L += 13, 60, 0));

                        graphics.DrawString(Cards[i].NameTH ?? "", new XFont("AngsanaDSE", 17, XFontStyle.Bold), XBrushes.Black, new XRect(17, yLogo_L, CardWidth, 40), XStringFormats.Center);
                        graphics.DrawString(Cards[i].NameEN.ToUpper() ?? "", new XFont("Myriad Pro Semibold", 11), XBrushes.Black, new XRect(17, yLogo_L + 5, CardWidth, 60), XStringFormats.Center);
                        graphics.DrawString(Cards[i].Position ?? "", new XFont("Myriad Pro Light", 10), XBrushes.Black, new XRect(17, yLogo_L + 5, CardWidth, 80), XStringFormats.Center);

                        double yQR_L = yLeft + 88;
                        graphics.DrawImage(QrCodeTPs, 246, yQR_L, 40, 60);

                        graphics.DrawString(Cards[i].Dept ?? "", new XFont("Roboto Light", 8), XBrushes.Black, new XRect(xLeft + 10, yLogo_L += 64, CardWidth, 0), XStringFormats.TopLeft);

                        graphics.DrawRectangle(XPens.Blue, new XRect(xLeft_txt + 10, yLogo_L += 11, 130, 0));

                        graphics.DrawString(Cards[i].HeadOffice ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft_txt + 10, yLogo_L += 1, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Address1 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 10, yLogo_L += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Address2 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 10, yLogo_L += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Office_Number ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 10, yLogo_L += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Fax ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 10, yLogo_L += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Email.ToLower() ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft_txt + 10, yLogo_L += 6, CardWidth, 0), XStringFormats.TopLeft);

                        double yPosition_B = yLeft + 23.66;
                        graphics2.DrawString(Cards[i].Company_th ?? "", new XFont("AngsanaDSE", 10), XBrushes.Black, new XRect(xLeft + 40, yPosition_B, page2.Width, 0), XStringFormats.TopLeft);

                        graphics2.DrawString(Cards2[0].HeadOffice ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 15, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Address1 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Address2 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Office_Number ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Fax ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Company_Web.ToLower() ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);

                        graphics2.DrawString(Cards2[1].HeadOffice ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 17, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Address1 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Address2 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Office_Number ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Fax ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Company_Web.ToLower() ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);

                        // 161.338
                        graphics.DrawRectangle(XPens.Black, new XRect(xLeft, yLeft, 262.20472440945, 162));
                        //graphics2.DrawRectangle(XPens.Black, new XRect(xLeft, yLeft, 262.20472440945, 162));
                        yLeft += 162;
                    }
                    else
                    {
                        double yLogo_R = yRight + 11;
                        graphics.DrawImage(LogoTPs, xRight + 10, yLogo_R, 53, 33);

                        graphics.DrawString(Cards[i].Company_en ?? "", new XFont("Roboto", 12, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft + 332, yLogo_R += 15, 60, 0));
                        graphics.DrawString(Cards[i].Parent_company ?? "", new XFont("Roboto", 9), XBrushes.Black, new XRect(xLeft + 332, yLogo_R += 13, 60, 0));

                        graphics.DrawString(Cards[i].NameTH ?? "", new XFont("AngsanaDSE", 17, XFontStyle.Bold), XBrushes.Black, new XRect(xLeft + 243.5, yLogo_R, CardWidth, 40), XStringFormats.Center);
                        graphics.DrawString(Cards[i].NameEN.ToUpper() ?? "", new XFont("Myriad Pro Semibold", 11), XBrushes.Black, new XRect(xLeft + 243.5, yLogo_R + 5, CardWidth, 60), XStringFormats.Center);
                        graphics.DrawString(Cards[i].Position ?? "", new XFont("Myriad Pro Light", 10), XBrushes.Black, new XRect(xLeft + 243.5, yLogo_R + 5, CardWidth, 80), XStringFormats.Center);

                        double yQR_R = yRight + 88;
                        graphics.DrawImage(QrCodeTPs, 510, yQR_R, 40, 60);

                        graphics.DrawString(Cards[i].Dept ?? "", new XFont("Roboto Light", 8), XBrushes.Black, new XRect(xRight + 10, yLogo_R += 64, CardWidth, 0), XStringFormats.TopLeft);

                        graphics.DrawRectangle(XPens.Blue, new XRect(xRight_txt + 10, yLogo_R += 11, 130, 0));

                        graphics.DrawString(Cards[i].HeadOffice ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xRight_txt + 10, yLogo_R += 1, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Address1 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 10, yLogo_R += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Address2 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 10, yLogo_R += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Office_Number ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 10, yLogo_R += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Fax ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 10, yLogo_R += 6, CardWidth, 0), XStringFormats.TopLeft);
                        graphics.DrawString(Cards[i].Email.ToLower() ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xRight_txt + 10, yLogo_R += 6, CardWidth, 0), XStringFormats.TopLeft);

                        double yPosition_B = yRight + 23.66;
                        graphics2.DrawString(Cards[i].Company_th ?? "", new XFont("AngsanaDSE", 10), XBrushes.Black, new XRect(xRight + 40, yPosition_B, page2.Width, 0), XStringFormats.TopLeft);

                        graphics2.DrawString(Cards2[0].HeadOffice ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 15, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Address1 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Address2 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Office_Number ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Fax ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[0].Company_Web.ToLower() ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);

                        graphics2.DrawString(Cards2[1].HeadOffice ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 17, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Address1 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Address2 ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Office_Number ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Fax ?? "", new XFont("Roboto Light", 6), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);
                        graphics2.DrawString(Cards2[1].Company_Web.ToLower() ?? "", new XFont("Roboto", 6, XFontStyle.Bold), XBrushes.Black, new XRect(xRight_txt + 40, yPosition_B += 6, page2.Width, 0), XStringFormats.TopLeft);

                        // 161.338
                        graphics.DrawRectangle(XPens.Black, new XRect(xRight, yRight, 262.20472440945, 162));
                        //graphics2.DrawRectangle(XPens.Black, new XRect(xRight, yRight, 262.20472440945, 162));
                        yRight += 162;
                    }
                }

                // บันทึกไฟล์ PDF ลงใน MemoryStream
                pdf.Save(stream, false);
                stream.Position = 0;

                // ส่งไฟล์ PDF กลับไปยัง View เพื่อดาวน์โหลด
                return File(stream, "application/pdf", "BusinessCard.pdf");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string FormatPhoneNumber(string phoneNumber)
        {
            string formattedNumber = string.Empty;
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                if (phoneNumber.Length <= 10)
                {
                    formattedNumber = "+66" + phoneNumber.Substring(1, 1) + "-" + phoneNumber.Substring(2, 4) + "-" + phoneNumber.Substring(6);
                }
            }
            else
            {
                formattedNumber = phoneNumber;
            }

            return formattedNumber;
        }

        // GET: BusinessCards
        public ActionResult Index()
        {
            UserAuthorized = Guid.Parse(HttpContext.User.Identity.Name);
            ViewBag.GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            ViewBag.authorized = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();
            return View();
        }

        public MemoryStream LogoTP()
        {
            WebClient webClient = new WebClient();

            byte[] data = webClient.DownloadData("https://tp-portal.thaiparker.co.th/TP_Service/File/E2E/Topic/88ec4008-57e7-4486-902f-cb4a4d3fc246/Media/_Capture.PNG");

            MemoryStream mem = new MemoryStream(data);

            return mem;
        }

        public ActionResult LogStatus(Guid? id)
        {
            var LogsCard = db.Log_BusinessCards.Where(w => w.BusinessCard_Id == id).ToList();

            List<ClsLog_BusinessCards> clsLog_Businesses = db.Log_BusinessCards
                          .Where(w => w.BusinessCard_Id == id)
                          .Select(s => new ClsLog_BusinessCards()
                          {
                              User_id = s.User_Id,
                              Status_Id = s.Status_Id,
                              UserDetails = db.UserDetails.Where(w => w.User_Id == s.User_Id).FirstOrDefault(),
                              System_Statuses = db.System_Statuses.Where(w => w.Status_Id == s.Status_Id).FirstOrDefault(),
                              Create = s.Create,
                              Remark = s.Remark,
                              Undo = s.Undo
                          }).ToList();

            return View(clsLog_Businesses.OrderByDescending(O => O.Create));
        }

        public ActionResult ManagerGaApprove(Guid? id, Guid? SelectId)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 8;
                    businessCards.UserAction = SelectId;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.BusinessCard_SaveLog(businessCards);
                        dataCard.SendMail(businessCards, SelectId);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManagerGaReject(Guid? id, string remark)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 5;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.BusinessCard_SaveLog(businessCards, remark);
                        dataCard.SendMail(businessCards, null, null, "", remark);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Rejected";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManagerUserApprove(Guid? id)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 7;

                    DateTime currentDate = DateTime.Now;
                    int daysToAdd = 3;

                    while (daysToAdd > 0)
                    {
                        currentDate = currentDate.AddDays(1);

                        if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            daysToAdd--;
                        }
                    }

                    businessCards.DueDate = currentDate;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.BusinessCard_SaveLog(businessCards);
                        dataCard.SendMail(businessCards);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManagerUserReject(Guid? id, string remark)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 5;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.BusinessCard_SaveLog(businessCards, remark);
                        dataCard.SendMail(businessCards, null, null, "", remark);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Rejected";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public MemoryStream QrCodeTP()
        {
            WebClient webClient = new WebClient();

            string URL = "https://tp-portal.thaiparker.co.th/TP_Service/File/E2E/Topic/88ec4008-57e7-4486-902f-cb4a4d3fc246/Media/QR CODE TP.JPG";

            byte[] data = webClient.DownloadData(URL);

            MemoryStream mem = new MemoryStream(data);

            return mem;
        }

        public List<ClsBusinessCard> QueryClsBusinessCard()
        {
            List<ClsBusinessCard> clsBusinessCards = db.BusinessCards
                .OrderByDescending(o => o.System_Statuses.OrderBusinessCard)
                .Select(s => new ClsBusinessCard()
                {
                    Key = s.Key,
                    Create = s.Create,
                    User_id = s.User_id,
                    Status_Id = s.Status_Id,
                    BothSided = s.BothSided,
                    Amount = s.Amount,
                    UserAction = s.UserAction.Value,
                    UserRef_id = s.UserRef_id,
                    UserActionName = db.UserDetails.Where(w => w.User_Id == s.UserAction).Select(ss => ss.Users.Username).FirstOrDefault(),
                    Tel_External = s.Tel_External,
                    Tel_Internal = s.Tel_Internal,
                    System_Statuses = db.System_Statuses.Where(w => w.Status_Id == s.Status_Id).FirstOrDefault(),
                    UserDetails = db.UserDetails.Where(w => w.User_Id == s.User_id).FirstOrDefault(),
                    UserRefName = db.UserDetails.Where(w => w.User_Id == s.UserRef_id).Select(ss => ss.Users.Username).FirstOrDefault(),
                    BusinessCard_Id = s.BusinessCard_Id,
                    DueDate = s.DueDate,
                    Update = s.Update
                }).ToList();

            return clsBusinessCards;
        }

        public ActionResult StaffComplete(Guid? id)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 3;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.SendMail(businessCards);
                        dataCard.BusinessCard_SaveLog(businessCards);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffStart(Guid? id)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 2;
                    businessCards.UserAction = Guid.Parse(HttpContext.User.Identity.Name);
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.BusinessCard_SaveLog(businessCards);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffUndo(Guid? id, string remark)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 7;
                    businessCards.UserAction = null;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.SendMail(businessCards, null, null, "", remark, "7");
                        dataCard.BusinessCard_SaveLog(businessCards, remark, true);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Table_AllRequest()
        {
            Guid MyDeptId = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Processes.Master_Sections.Department_Id).FirstOrDefault();
            var GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            var authorized = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();
            var clsBusinessCard = QueryClsBusinessCard();

            //user
            if (authorized == 3 && GA == false)
            {
                clsBusinessCard = clsBusinessCard.Where(w => w.User_id == UserAuthorized).OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }
            //mg user
            else if (authorized == 2 && GA == false)
            {
                clsBusinessCard = clsBusinessCard.Where(w => w.UserDetails.Users.Master_Processes.Master_Sections.Department_Id == MyDeptId).OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }
            //mg ga
            else if (authorized == 2 && GA == true)
            {
                clsBusinessCard = clsBusinessCard.OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }
            // staff ga
            else if (authorized == 3 && GA == true)
            {
                clsBusinessCard = clsBusinessCard.Where(w => w.User_id == UserAuthorized).OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }

            return View(clsBusinessCard);
        }

        public ActionResult Table_AllTask()
        {
            var GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            var authorized = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();
            var clsBusinessCard = QueryClsBusinessCard();

            //staff ga
            if (authorized == 3 && GA == true)
            {
                clsBusinessCard = clsBusinessCard.Where(w => w.Status_Id == 8 && w.UserAction == null).OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }

            return View(clsBusinessCard);
        }

        public ActionResult Table_Approval()
        {
            Guid MyDeptId = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Processes.Master_Sections.Department_Id).FirstOrDefault();
            var GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            var authorized = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();
            var clsBusinessCard = QueryClsBusinessCard();

            //mg user
            if (authorized == 2 && GA == false)
            {
                clsBusinessCard = clsBusinessCard.Where(w => w.Status_Id == 1 && w.UserDetails.Users.Master_Processes.Master_Sections.Department_Id == MyDeptId).OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }
            //mg ga
            else if (authorized == 2 && GA == true)
            {
                clsBusinessCard = clsBusinessCard.Where(w => w.Status_Id == 1 && w.UserDetails.Users.Master_Processes.Master_Sections.Department_Id == MyDeptId || w.Status_Id == 7).OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }

            return View(clsBusinessCard);
        }

        public ActionResult Table_MyTask()
        {
            var GA = db.Users.Any(a => a.User_Id == UserAuthorized && a.BusinessCardGroup == true);
            var authorized = db.Users.Where(w => w.User_Id == UserAuthorized).Select(s => s.Master_Grades.Master_LineWorks.Authorize_Id).FirstOrDefault();
            var clsBusinessCard = QueryClsBusinessCard();

            //staff ga
            if (authorized == 3 && GA == true)
            {
                clsBusinessCard = clsBusinessCard.Where(w => w.UserAction == UserAuthorized).OrderBy(o => o.System_Statuses.OrderBusinessCard).ToList();
            }

            return View(clsBusinessCard);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, Guid? id)
        {
            ClsSwal swal = new ClsSwal();

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (file != null && file.ContentLength > 0 && id.HasValue)
                    {
                        string dir = "BusinessCard/" + id.Value;
                        string FileName = file.FileName;

                        bool cardFiles = db.BusinessCardFiles.Any(a => a.BusinessCard_Id == id.Value && a.FileName == file.FileName);

                        if (cardFiles)
                        {
                            FileName = string.Concat("_", file.FileName);
                        }

                        string filepath = data.UploadFileToString(dir, file, FileName);

                        if (!string.IsNullOrEmpty(filepath))
                        {
                            var sql = db.BusinessCards.Find(id);
                            if (dataCard.BusinessCard_SaveFile(filepath, sql))
                            {
                                scope.Complete();
                                swal.DangerMode = false;
                                swal.Icon = "success";
                                swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                                swal.Title = "Successful";
                            }
                        }
                        else
                        {
                            swal.Icon = "warning";
                            swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.Title = "Warning";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                swal.Title = ex.Source;
                swal.Text = ex.Message;
                Exception inner = ex.InnerException;
                while (inner != null)
                {
                    swal.Title = inner.Source;
                    swal.Text += string.Format("\n{0}", inner.Message);
                    inner = inner.InnerException;
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadHistory(Guid? id)
        {
            var res = db.BusinessCardFiles.Where(w => w.BusinessCard_Id == id).OrderByDescending(o => o.Create).ToList();

            return View(res);
        }

        public ActionResult UserClose(Guid? id)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 4;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.SendMail(businessCards);
                        dataCard.BusinessCard_SaveLog(businessCards);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserConfirmApprove(Guid? id)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCardFiles businessCardFiles = db.BusinessCardFiles.Find(id);
                    businessCardFiles.Confirm = true;
                    businessCardFiles.Create = DateTime.Now;

                    BusinessCards businessCards = db.BusinessCards.Find(businessCardFiles.BusinessCard_Id);
                    businessCards.Status_Id = 9;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.SendMail(businessCards);
                        dataCard.BusinessCard_SaveLog(businessCards);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserConfirmCancel(Guid? id, string remark)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCardFiles businessCardFiles = db.BusinessCardFiles.Find(id);
                    businessCardFiles.Confirm = false;
                    businessCardFiles.Create = DateTime.Now;
                    businessCardFiles.Remark = remark;

                    BusinessCards businessCards = db.BusinessCards.Find(businessCardFiles.BusinessCard_Id);
                    businessCards.Status_Id = 2;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.SendMail(businessCards, null, businessCardFiles, "", remark);
                        dataCard.BusinessCard_SaveLog(businessCards, remark);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserUndo(Guid? id, string remark)
        {
            ClsSwal swal = new ClsSwal();
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                try
                {
                    BusinessCards businessCards = db.BusinessCards.Find(id);
                    businessCards.Status_Id = 2;
                    businessCards.Update = DateTime.Now;

                    if (db.SaveChanges() > 0)
                    {
                        dataCard.SendMail(businessCards, null, null, "", remark, "2");
                        dataCard.BusinessCard_SaveLog(businessCards, remark, true);
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "บันทึกข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
                    }
                }
                catch (Exception ex)
                {
                    swal.Title = ex.Source;
                    swal.Text = ex.Message;
                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        swal.Title = inner.Source;
                        swal.Text += string.Format("\n{0}", inner.Message);
                        inner = inner.InnerException;
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }
    }
}
