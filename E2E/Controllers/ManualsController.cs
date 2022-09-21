using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ManualsController : Controller
    {
        private clsContext db = new clsContext();
        private clsServiceFTP ftp = new clsServiceFTP();

        // GET: Manuals
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manuals_Table()
        {
            ViewBag.Language_Name = SelectListItems_Language_Name();

            ViewBag.Manual_TypeName = SelectListItems_Manual_TypeName();

            var system_Manuals = db.Manuals.OrderByDescending(o => o.Create).ToList();
            return View(system_Manuals);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Manuals_Table(Manuals model)
        {
            clsSwal swal = new clsSwal();
            bool res = new bool();
            HttpFileCollectionBase files = Request.Files;
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        Manuals system_Manuals = new Manuals();

                        if (files[0].ContentLength != 0)
                        {
                            HttpPostedFileBase file = files[0];
                            string dir = "Manuals/" + system_Manuals.Manual_Id;
                            string FileName = file.FileName;

                            string filepath = ftp.Ftp_UploadFileToString(dir, file, FileName);

                            system_Manuals.Manual_Path = filepath;
                            system_Manuals.Manual_Extension = Path.GetExtension(FileName);
                            system_Manuals.Manual_Name = FileName;
                        }

                        system_Manuals.Language_Id = model.Language_Id;
                        system_Manuals.Manual_Type_Id = model.Manual_Type_Id;
                        system_Manuals.User_Id = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
                        system_Manuals.Ver = db.Manuals.Where(w => w.Language_Id == model.Language_Id & w.Manual_Type_Id == model.Manual_Type_Id).Count() + 1;

                        db.Manuals.Add(system_Manuals);
                        if (db.SaveChanges() > 0)
                        {
                            res = true;
                        }

                        if (res)
                        {
                            scope.Complete();

                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                        }
                        else
                        {
                            swal.icon = "warning";
                            swal.text = "บันทึกข้อมูลไม่สำเร็จ";
                            swal.title = "Warning";
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        foreach (var item in ex.EntityValidationErrors)
                        {
                            foreach (var item2 in item.ValidationErrors)
                            {
                                if (string.IsNullOrEmpty(swal.text))
                                {
                                    swal.text = item2.ErrorMessage;
                                }
                                else
                                {
                                    swal.text += "\n" + item2.ErrorMessage;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.text = ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.text = ex.InnerException.InnerException.Message;
                            }
                        }
                    }
                }
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                   .Where(y => y.Count > 0)
                                   .ToList();
                swal.icon = "warning";
                swal.title = "Warning";
                foreach (var item in errors)
                {
                    foreach (var item2 in item)
                    {
                        if (string.IsNullOrEmpty(swal.text))
                        {
                            swal.text = item2.ErrorMessage;
                        }
                        else
                        {
                            swal.text += "\n" + item2.ErrorMessage;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> SelectListItems_Language_Name()
        {
            IQueryable<System_Language> query = db.System_Language;

            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "Select Language", Value = "" });
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Language_Id.ToString(),
                    Text = s.Language_Name
                }).OrderBy(o => o.Text).ToList());

            return item;
        }

        public List<SelectListItem> SelectListItems_Manual_TypeName()
        {
            IQueryable<System_ManualType> query = db.System_ManualType;

            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem() { Text = "Select Type", Value = "" });
            item.AddRange(query
                .Select(s => new SelectListItem()
                {
                    Value = s.Manual_Type_Id.ToString(),
                    Text = s.Manual_TypeName
                }).OrderBy(o => o.Text).ToList());

            return item;
        }
    }
}
