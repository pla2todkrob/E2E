using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class EForm_Files
    {
        public EForm_Files()
        {
            EForm_File_Id = Guid.NewGuid();
        }

        public string EForm_File_Extension { get; set; }

        [Key]
        public Guid EForm_File_Id { get; set; }

        public string EForm_File_Name { get; set; }
        public string EForm_File_Path { get; set; }
        public int EForm_File_Seq { get; set; }

        [Index]
        public Guid EForm_Id { get; set; }

        public virtual EForms EForms { get; set; }
    }
}
