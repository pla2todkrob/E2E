using E2E.Models.Tables;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class ClsBusinessCard
    {
        public int Amount { get; set; }
        public bool BothSided { get; set; }
        public Guid BusinessCard_Id { get; set; }
        public DateTime Create { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DueDate { get; set; }

        [DisplayName("Job no")]
        public long Key { get; set; }

        [DisplayName("Status")]
        public int Status_Id { get; set; }

        public System_Statuses System_Statuses { get; set; }

        public string Tel_External { get; set; }

        public string Tel_Internal { get; set; }

        public DateTime? Update { get; set; }

        [DisplayName("Requestor")]
        public Guid? User_id { get; set; }

        public Guid? UserAction { get; set; }

        [DisplayName("Staff action")]
        public string UserActionName { get; set; }

        public UserDetails UserDetails { get; set; }
        public Guid? UserRef_id { get; set; }
        public string UserRefName { get; set; }
    }
}
