using System;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class EForm_Galleries
    {
        public EForm_Galleries()
        {
            EForm_Gallery_Id = Guid.NewGuid();
        }

        public string EForm_Gallery_Extension { get; set; }

        [Key]
        public Guid EForm_Gallery_Id { get; set; }

        public string EForm_Gallery_Name { get; set; }
        public string EForm_Gallery_Original { get; set; }

        [Display(Name = "No.")]
        public int EForm_Gallery_Seq { get; set; }

        public string EForm_Gallery_Thumbnail { get; set; }
        public Guid EForm_Id { get; set; }
        public virtual EForms EForms { get; set; }
    }
}
