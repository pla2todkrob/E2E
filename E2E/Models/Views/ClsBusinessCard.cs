using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class ClsBusinessCard
    {
        public Guid BusinessCard_Id { get; set; }
        public Guid? User_id { get; set; }
        public long Key { get; set; }
        public int Amount { get; set; }
        public string Tel_Internal { get; set; }
        public string Tel_External { get; set; }
        public bool BothSided { get; set; }
        public DateTime Create { get; set; }
        public Guid? UserAction { get; set; }
        public Guid? UserRef_id { get; set; }
        public string UserRefName { get; set; }
        public string UserActionName { get; set; }
        public  UserDetails UserDetails { get; set; }
        public int Status_Id { get; set; }
        public System_Statuses System_Statuses { get; set; }
    }
}