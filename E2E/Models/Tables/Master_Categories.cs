using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Tables
{
    public class Master_Categories
    {
        
        [Key, Display(Name = "Category")]
        public Guid Category_Id { get; set; }

        [Display(Name = "Category")]
        public string Category_Name { get; set; }
        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
        public bool Active { get; set; }

        public Master_Categories()
        {
            Category_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }
    }
}