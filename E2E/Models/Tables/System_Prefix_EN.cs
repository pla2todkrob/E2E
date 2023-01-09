using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E2E.Models.Tables
{
    public class System_Prefix_EN
    {
        [Key]
        public int Prefix_EN_Id { get; set; }

        [StringLength(100), Index(IsUnique = true)]
        public string Prefix_EN_Name { get; set; }
    }
}
