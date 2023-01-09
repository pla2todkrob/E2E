using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class System_Prefix_TH
    {
        [Key]
        public int Prefix_TH_Id { get; set; }

        [StringLength(100), Index(IsUnique = true)]
        public string Prefix_TH_Name { get; set; }
    }
}
