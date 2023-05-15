using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class ClsLog_BusinessCards
    {
        public Guid? User_id { get; set; }
        public int Status_Id { get; set; }
        public string Remark { get; set; }
        public DateTime Create { get; set; }
        public bool Undo { get; set; }
        public UserDetails UserDetails { get; set; }
        public System_Statuses System_Statuses { get; set; }
    }
}