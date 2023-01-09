using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Master_Categories
    {
        public Master_Categories()
        {
            Category_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        [Key, Display(Name = "Category")]
        public Guid Category_Id { get; set; }

        [Display(Name = "Category"), StringLength(100), Index(IsUnique = true)]
        public string Category_Name { get; set; }

        public DateTime Create { get; set; }
        public DateTime? Update { get; set; }
    }
}
