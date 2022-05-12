using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class EForm_Files
    {
        [Key]
        public Guid EForm_File_Id { get; set; }
        public Guid EForm_Id { get; set; }
        public virtual EForms EForms { get; set; }
        public string EForm_File_Path { get; set; }
        public string EForm_File_Name { get; set; }
        public string EForm_File_Extension { get; set; }
        public int EForm_File_Seq { get; set; }
        public EForm_Files()
        {
            EForm_File_Id = Guid.NewGuid();
        }
    }
}