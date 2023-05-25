using E2E.Models.Tables;

namespace E2E.Models.Views
{
    public class ClsBusinessCardModel
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public BusinessCards businessCards { get; set; }
        public string Company_en { get; set; }
        public string Company_th { get; set; }
        public string Company_Web { get; set; }
        public string Dept { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string HeadOffice { get; set; }
        public Master_Plants Master_Plants { get; set; }

        //Loop 10 Cards
        public string NameEN { get; set; }

        public string NameTH { get; set; }
        public string Office_Number { get; set; }
        public string Parent_company { get; set; }
        public PlantDetail plantDetail { get; set; }
        public string Position { get; set; }
    }
}
