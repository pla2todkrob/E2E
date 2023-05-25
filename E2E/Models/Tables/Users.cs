using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class Users
    {
        public Users()
        {
            User_Id = Guid.NewGuid();
            Active = true;
            Create = DateTime.Now;
        }

        public bool Active { get; set; }

        [Display(Name = "Business card group")]
        public bool BusinessCardGroup { get; set; }

        public DateTime Create { get; set; }

        [Required]
        [Display(Name = "Grade"), Index]
        public Guid Grade_Id { get; set; }

        public virtual Master_Grades Master_Grades { get; set; }

        public virtual Master_Plants Master_Plants { get; set; }
        public virtual Master_Processes Master_Processes { get; set; }

        [Required]
        [Display(Name = "Plant"), Index]
        public Guid Plant_Id { get; set; }

        [Required]
        [Display(Name = "Process"), Index]
        public Guid Process_Id { get; set; }

        [Required]
        [Display(Name = "Role"), Index]
        public int Role_Id { get; set; }

        public virtual System_Roles System_Roles { get; set; }

        public DateTime? Update { get; set; }

        [Display(Name = "Code"), Required, StringLength(100), Index(IsUnique = true)]
        public string User_Code { get; set; }

        [Display(Name = "Cost center")]
        public string User_CostCenter { get; set; }

        [Display(Name = "Email"), StringLength(100), Index]
        public string User_Email { get; set; }

        [Key]
        public Guid User_Id { get; set; }

        [Display(Name = "Point")]
        public int User_Point { get; set; }

        [StringLength(100), Index]
        public string Username { get; set; }

        public int YearSetPoint { get; set; }
    }
}
