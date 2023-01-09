using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class TopicFiles
    {
        public TopicFiles()
        {
            TopicFile_Id = Guid.NewGuid();
        }

        [Index]
        public Guid Topic_Id { get; set; }

        public string TopicFile_Extension { get; set; }

        [Key]
        public Guid TopicFile_Id { get; set; }

        public string TopicFile_Name { get; set; }
        public string TopicFile_Path { get; set; }
        public int TopicFile_Seq { get; set; }
        public virtual Topics Topics { get; set; }
    }
}
