using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E2E.Models.Views
{
    public class clsWorkRoots
    {
        public clsWorkRoots()
        {
            WorkRoots = new WorkRoots();
            WorkRootDocuments = new List<WorkRootDocuments>();
        }

        [Display(Name = "Document control")]
        public List<Guid?> Document_Id { get; set; }

        public List<WorkRootDocuments> WorkRootDocuments { get; set; }
        public WorkRoots WorkRoots { get; set; }
    }
}
