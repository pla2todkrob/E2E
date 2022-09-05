using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class ServiceCommentFiles
    {
        public ServiceCommentFiles()
        {
            ServiceCommentFile_Id = Guid.NewGuid();
        }

        public Guid ServiceComment_Id { get; set; }

        public int ServiceComment_Seq { get; set; }

        [Display(Name = "File extension")]
        public string ServiceCommentFile_Extension { get; set; }

        [Key]
        public Guid ServiceCommentFile_Id { get; set; }

        [Display(Name = "File name")]
        public string ServiceCommentFile_Name { get; set; }

        [Display(Name = "File path")]
        public string ServiceCommentFile_Path { get; set; }
    }
}