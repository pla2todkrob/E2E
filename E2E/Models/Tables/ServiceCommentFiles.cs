using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class ServiceCommentFiles
    {
        [Key]
        public Guid ServiceCommentFile_Id { get; set; }
        public Guid ServiceComment_Id { get; set; }
        public string ServiceCommentFile_Path { get; set; }
        public string ServiceCommentFile_Name { get; set; }
        public ServiceCommentFiles()
        {
            ServiceCommentFile_Id = Guid.NewGuid();
        }
    }
}