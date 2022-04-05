using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsServices
    {
        public Guid Service_Id { get; set; }
        public string Service_Key { get; set; }
        public string Service_Subject { get; set; }
        public string Service_Description { get; set; }
        public string Plant_Name { get; set; }
        public string Division_Name { get; set; }
        public string Department_Name { get; set; }
        public string User_Name { get; set; }
        public string User_Code { get; set; }
        public string User_Email { get; set; }
        public string Ref_Service_Key { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public string Commit_User_Name { get; set; }
        public string Commit_User_Code { get; set; }
        public string Commit_User_Email { get; set; }
        public DateTime? Commit_DateTime { get; set; }
        public string Approve_User_Name { get; set; }
        public string Approve_User_Code { get; set; }
        public string Approve_User_Email { get; set; }
        public DateTime? Approve_DateTime { get; set; }
        public string Status_Name { get; set; }
        public string Status_Class { get; set; }
        public string Priority_Name { get; set; }
        public string Priority_Class { get; set; }
    }
}