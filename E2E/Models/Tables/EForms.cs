using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Tables
{
    public class EForms
    {
        public EForms()
        {
            EForm_Id = Guid.NewGuid();
            Create = DateTime.Now;
            Active = true;
        }

        public Guid? ActionUserId { get; set; }

        public bool Active { get; set; }

        public DateTime Create { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Content")]
        public string EForm_Description { get; set; }

        [DisplayName("End Date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? EForm_End { get; set; }

        [Key]
        public Guid EForm_Id { get; set; }

        [DisplayName("LINK")]
        public string EForm_Link { get; set; }

        [DisplayName("Start Date"), DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EForm_Start { get; set; }

        [DisplayName("Title")]
        public string EForm_Title { get; set; }

        public int? Status_Id { get; set; }
        public virtual System_Statuses System_Statuses { get; set; }
        public DateTime? Update { get; set; }
        public Guid User_Id { get; set; }
        public virtual Users Users { get; set; }
    }
}
