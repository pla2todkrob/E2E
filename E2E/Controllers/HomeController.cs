using E2E.Models;
using E2E.Models.Tables;
using E2E.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace E2E.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ClsContext db = new ClsContext();

        public ActionResult ChangeFileUrl()
        {
            try
            {
                var TopicGalleries = db.TopicGalleries.ToList();
                var TopicFiles = db.TopicFiles.ToList();
                var EForm_Files = db.EForm_Files.ToList();
                var EForm_Galleries = db.EForm_Galleries.ToList();
                var Manuals = db.Manuals.ToList();
                var Configurations = db.System_Configurations.ToList();
                var ServiceCommentFiles = db.ServiceCommentFiles.ToList();
                var ServiceDocuments = db.ServiceDocuments.ToList();
                var ServiceFiles = db.ServiceFiles.ToList();
                var UserUploadHistories = db.UserUploadHistories.ToList();
                var Master_DocumentVersions = db.Master_DocumentVersions.ToList();

                if (TopicGalleries?.Count > 0)
                {
                    foreach (var item in TopicGalleries)
                    {
                        if (item.TopicGallery_Original != null)
                        {
                            item.TopicGallery_Original = item.TopicGallery_Original.Replace("tpfiles", "tp_service/File");
                        }
                        if (item.TopicGallery_Thumbnail != null)
                        {
                            item.TopicGallery_Thumbnail = item.TopicGallery_Thumbnail.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (TopicFiles?.Count > 0)
                {
                    foreach (var item in TopicFiles)
                    {
                        if (item.TopicFile_Path != null)
                        {
                            item.TopicFile_Path = item.TopicFile_Path.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (EForm_Files?.Count > 0)
                {
                    foreach (var item in EForm_Files)
                    {
                        if (item.EForm_File_Path != null)
                        {
                            item.EForm_File_Path = item.EForm_File_Path.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (EForm_Galleries?.Count > 0)
                {
                    foreach (var item in EForm_Galleries)
                    {
                        if (item.EForm_Gallery_Original != null)
                        {
                            item.EForm_Gallery_Original = item.EForm_Gallery_Original.Replace("tpfiles", "tp_service/File");
                        }
                        if (item.EForm_Gallery_Thumbnail != null)
                        {
                            item.EForm_Gallery_Thumbnail = item.EForm_Gallery_Thumbnail.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (Manuals?.Count > 0)
                {
                    foreach (var item in Manuals)
                    {
                        if (item.Manual_Path != null)
                        {
                            item.Manual_Path = item.Manual_Path.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (Configurations?.Count > 0)
                {
                    foreach (var item in Configurations)
                    {
                        if (item.Configuration_Brand != null)
                        {
                            item.Configuration_Brand = item.Configuration_Brand.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (ServiceCommentFiles?.Count > 0)
                {
                    foreach (var item in ServiceCommentFiles)
                    {
                        if (item.ServiceCommentFile_Path != null)
                        {
                            item.ServiceCommentFile_Path = item.ServiceCommentFile_Path.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (ServiceDocuments?.Count > 0)
                {
                    foreach (var item in ServiceDocuments)
                    {
                        if (item.ServiceDocument_Path != null)
                        {
                            item.ServiceDocument_Path = item.ServiceDocument_Path.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (ServiceFiles?.Count > 0)
                {
                    foreach (var item in ServiceFiles)
                    {
                        if (item.ServiceFile_Path != null)
                        {
                            item.ServiceFile_Path = item.ServiceFile_Path.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (UserUploadHistories?.Count > 0)
                {
                    foreach (var item in UserUploadHistories)
                    {
                        if (item.UserUploadHistoryFile != null)
                        {
                            item.UserUploadHistoryFile = item.UserUploadHistoryFile.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (Master_DocumentVersions?.Count > 0)
                {
                    foreach (var item in Master_DocumentVersions)
                    {
                        if (item.DocumentVersion_Path != null)
                        {
                            item.DocumentVersion_Path = item.DocumentVersion_Path.Replace("tpfiles", "tp_service/File");
                        }
                    }
                }

                if (db.SaveChanges() > 0)
                {
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        public ActionResult Index()
        {
            ClsHome clsHome = new ClsHome()
            {
                Topics = db.Topics.Where(w => w.Topic_Pin).OrderBy(o => o.Create).ToList(),
                EForms = db.EForms.Take(10).OrderByDescending(o => o.Create).ToList(),
            };
            clsHome.Topics.AddRange(db.Topics.Where(w => !w.Topic_Pin).Take(10).OrderByDescending(o => o.Create).ToList());

            return View(clsHome);
        }

        public ActionResult Manual_Table()
        {
            List<Manuals> system_Manuals = new List<Manuals>();
            List<Guid> List_Language = db.System_Language.Select(s => s.Language_Id).ToList();
            List<Guid> List_ManualType = db.System_ManualType.Select(s => s.Manual_Type_Id).ToList();

            foreach (var item1 in List_Language)
            {
                foreach (var item2 in List_ManualType)
                {
                    var CHK = db.Manuals.Where(w => w.Manual_Type_Id == item2 & w.Language_Id == item1).OrderByDescending(o => o.Create).FirstOrDefault();

                    if (CHK != null)
                    {
                        system_Manuals.Add(CHK);
                    }
                }
            }

            return View(system_Manuals);
        }
    }
}
