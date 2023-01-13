using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace E2E.Controllers
{
    public class VoidController : Controller
    {
        private readonly ClsContext db = new ClsContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MovePlant2User()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    Dictionary<string, Guid> divisionList = new Dictionary<string, Guid>();

                    foreach (var item in db.Users.AsNoTracking().Select(s => s.User_Id))
                    {
                        Users users = db.Users.Find(item);
                        users.Plant_Id = users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Plant_Id.Value;
                        if (!divisionList.Any(a => a.Key == users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Name))
                        {
                            divisionList.Add(users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Name, users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Id);
                        }
                        else
                        {
                            Guid divisionId = divisionList.Where(a => a.Key == users.Master_Processes.Master_Sections.Master_Departments.Master_Divisions.Division_Name)
                                .Select(s => s.Value)
                                .SingleOrDefault();

                            Master_Departments department = db.Master_Departments
                                .Where(w => w.Division_Id == divisionId &&
                                w.Department_Name == users.Master_Processes.Master_Sections.Master_Departments.Department_Name)
                                .FirstOrDefault();
                            if (department == null)
                            {
                                department = new Master_Departments()
                                {
                                    Department_Name = users.Master_Processes.Master_Sections.Master_Departments.Department_Name,
                                    Division_Id = divisionId
                                };
                                db.Entry(department).State = System.Data.Entity.EntityState.Added;
                                db.SaveChanges();
                            }

                            Master_Sections section = db.Master_Sections
                                .Where(w => w.Department_Id == department.Department_Id &&
                               w.Section_Name == users.Master_Processes.Master_Sections.Section_Name)
                                .FirstOrDefault();
                            if (section == null)
                            {
                                section = new Master_Sections()
                                {
                                    Department_Id = department.Department_Id,
                                    Section_Name = users.Master_Processes.Master_Sections.Section_Name
                                };
                                db.Entry(section).State = System.Data.Entity.EntityState.Added;
                                db.SaveChanges();
                            }

                            Master_Processes process = db.Master_Processes
                                .Where(w => w.Section_Id == section.Section_Id &&
                                w.Process_Name == users.Master_Processes.Process_Name)
                                .FirstOrDefault();
                            if (process == null)
                            {
                                process = new Master_Processes()
                                {
                                    Process_Name = users.Master_Processes.Process_Name,
                                    Section_Id = section.Section_Id
                                };
                                db.Entry(process).State = System.Data.Entity.EntityState.Added;
                                db.SaveChanges();
                            }

                            users.Process_Id = process.Process_Id;
                        }
                        if (db.SaveChanges() > 0)
                        {
                            continue;
                        }
                    }

                    foreach (var item in db.WorkRoots)
                    {
                        item.Section_Id = db.Master_Sections
                            .Where(w => divisionList.Values.Contains(w.Master_Departments.Division_Id) &&
                            w.Section_Name == item.Master_Sections.Section_Name)
                            .Select(s => s.Section_Id)
                            .FirstOrDefault();
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            continue;
                        }
                    }
                    if (!Equals(db.Master_Divisions.Count(), divisionList.Count()))
                    {
                        List<Master_Divisions> delDivision = db.Master_Divisions.Where(w => !divisionList.Values.Contains(w.Division_Id)).ToList();
                        db.Master_Divisions.RemoveRange(delDivision);
                        db.SaveChanges();
                    }

                    scope.Complete();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public ActionResult ReReadFile()
        {
            ClsSwal swal = new ClsSwal();
            try
            {
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.MaxValue
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    ClsManageMaster cls = new ClsManageMaster();
                    string lastFile = db.UserUploadHistories.OrderByDescending(o => o.Create).Select(s => s.UserUploadHistoryFile).FirstOrDefault();
                    if (cls.Users_AdjustMissing(cls.Users_ReadFile(lastFile)))
                    {
                        scope.Complete();
                        swal.DangerMode = false;
                        swal.Icon = "success";
                        swal.Text = "รีโหลดข้อมูลสำเร็จแล้ว";
                        swal.Title = "Successful";
                    }
                    else
                    {
                        swal.Icon = "warning";
                        swal.Text = "รีโหลดข้อมูลไม่สำเร็จ";
                        swal.Title = "Warning";
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
    }
}
