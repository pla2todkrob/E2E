using E2E.Models.Tables;
using System;

namespace E2E.Models.Views
{
    public class ClsLog_BusinessCards
    {
        public DateTime Create { get; set; }
        public string Remark { get; set; }
        public int Status_Id { get; set; }
        public System_Statuses System_Statuses { get; set; }
        public bool Undo { get; set; }
        public Guid? User_id { get; set; }
        public UserDetails UserDetails { get; set; }
    }
}
