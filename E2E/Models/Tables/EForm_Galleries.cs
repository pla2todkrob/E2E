using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class EForm_Galleries
    {
        [Key]
        public Guid EForm_Gallery_Id { get; set; }

        public Guid EForm_Id { get; set; }
        public virtual EForms EForms { get; set; }
        public string EForm_Gallery_Original { get; set; }
        public string EForm_Gallery_Thumbnail { get; set; }
        public string EForm_Gallery_Name { get; set; }
        public string EForm_Gallery_Extension { get; set; }

        [Display(Name ="No.")]
        public int EForm_Gallery_Seq { get; set; }

        public EForm_Galleries()
        {
            EForm_Gallery_Id = Guid.NewGuid();
        }
    }
}