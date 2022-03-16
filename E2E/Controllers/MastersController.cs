using E2E.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E2E.Models.Tables;
using E2E.Models.Views;
using System.Transactions;
using OfficeOpenXml;

namespace E2E.Controllers
{
    public class MastersController : Controller
    {
        private clsManageData data = new clsManageData();
        private clsContext db = new clsContext();

        public ActionResult Departments()
        {
            return View();
        }

        public ActionResult Divisions()
        {
            return View();
        }

        public ActionResult Grades()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LineWorks()
        {
            return View();
        }

        public ActionResult Nationalities()
        {
            return View();
        }

        public ActionResult Plants()
        {
            return View();
        }

        public ActionResult Processes()
        {
            return View();
        }

        public ActionResult Sections()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult Users_Form(Guid? id)
        {
            if (id != null)
            {
                return View(data.UserDetail_Get(id.Value));
            }

            return View(new UserDetails());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Users_Form(UserDetails model)
        {
            clsSwal swal = new clsSwal();
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    try
                    {
                        if (data.User_Save(model))
                        {
                            scope.Complete();
                            swal.dangerMode = false;
                            swal.icon = "success";
                            swal.text = "บันทึกข้อมูลเรียบร้อยแล้ว";
                            swal.title = "Successful";
                        }
                    }
                    catch (Exception ex)
                    {
                        swal.title = ex.TargetSite.Name;
                        swal.text = ex.Message;
                        if (ex.InnerException != null)
                        {
                            swal.text += "\n " + ex.InnerException.Message;
                            if (ex.InnerException.InnerException != null)
                            {
                                swal.text += "\n " + ex.InnerException.InnerException.Message;
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

        public ActionResult Users_Table()
        {
            return View(data.User_GetAll());
        }

        public ActionResult Users_Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Users_Upload(HttpPostedFileBase file)
        {
            clsSwal swal = new clsSwal();
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    using (ExcelPackage package = new ExcelPackage(file.InputStream))
                    {
                        foreach (var sheet in package.Workbook.Worksheets)
                        {
                            for (int row = 1; row <= sheet.Dimension.End.Row; row++)
                            {
                                if (row > 3)
                                {
                                    UserDetails userDetails = new UserDetails();
                                    userDetails.Detail_EN_FirstName = sheet.Cells[row, 4].Text;
                                    userDetails.Detail_EN_LastName = sheet.Cells[row, 5].Text;
                                    userDetails.Detail_EN_Prefix = sheet.Cells[row, 3].Text;
                                    userDetails.Detail_TH_FirstName = sheet.Cells[row, 7].Text;
                                    userDetails.Detail_TH_LastName = sheet.Cells[row, 8].Text;
                                    userDetails.Detail_TH_Prefix = sheet.Cells[row, 6].Text;
                                    userDetails.Users.LineWork_Id = data.LineWork_GetId(sheet.Cells[row, 10].Text, true).Value;
                                    userDetails.Users.Grade_Id = data.Grade_GetId(userDetails.Users.LineWork_Id, sheet.Cells[row, 11].Text, sheet.Cells[row, 12].Text, true).Value;
                                    userDetails.Users.Plant_Id = data.Plant_GetId(sheet.Cells[row, 13].Text, true).Value;
                                    userDetails.Users.Division_Id = data.Division_GetId(userDetails.Users.Plant_Id, sheet.Cells[row, 14].Text, true);
                                    userDetails.Users.Department_Id = data.Department_GetId(userDetails.Users.Division_Id.Value,sheet.Cells[row, 15].Text,true);
                                    userDetails.Users.Section_Id = data.Section_GetId(userDetails.Users.Department_Id.Value, sheet.Cells[row, 16].Text,true);
                                    userDetails.Users.Process_Id = 
                                }
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
                        swal.text += "\n " + ex.InnerException.Message;
                        if (ex.InnerException.InnerException != null)
                        {
                            swal.text += "\n " + ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }

            return Json(swal, JsonRequestBehavior.AllowGet);
        }
    }
}
