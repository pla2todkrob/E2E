using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class ManualsController : BaseController
    {
        private readonly ClsManageService clsManageService = new ClsManageService();

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
        public async Task<ActionResult> Manuals_Table(Manuals model)
        {
            ClsSwal swal = new ClsSwal();
            bool res = new bool();
            HttpFileCollectionBase files = Request.Files;
            if (ModelState.IsValid)
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        Manuals system_Manuals = new Manuals();

                        if (files[0].ContentLength != 0)
                        {
                            HttpPostedFileBase file = files[0];
                            string dir = "Manuals/" + system_Manuals.Manual_Id;
                            string FileName = file.FileName;

                            string filepath = await clsManageService.UploadFileToString(dir, file, FileName);

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
                        swal.Text = ex.GetBaseException().Message;
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

        public List<SelectListItem> SelectListItems_Language_Name()
        {
            IQueryable<System_Language> query = db.System_Language;

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Language", Value = "" }
            };
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

            List<SelectListItem> item = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Select Type", Value = "" }
            };
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
