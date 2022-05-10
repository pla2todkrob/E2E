using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class TopicFiles
    {
        [Key]
        public Guid TopicFile_Id { get; set; }

        public Guid Topic_Id { get; set; }
        public string TopicFile_Path { get; set; }
        public string TopicFile_Name { get; set; }
        public string TopicFile_Extension { get; set; }
        public int TopicFile_Seq { get; set; }

        public TopicFiles()
        {
            TopicFile_Id = Guid.NewGuid();
        }
    }
}