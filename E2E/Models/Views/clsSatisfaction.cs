using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class clsSatisfaction
    {
        public clsSatisfaction()
        {
            Satisfactions = new Satisfactions();
            SatisfactionDetails = new List<SatisfactionDetails>();
        }

        public List<SatisfactionDetails> SatisfactionDetails { get; set; }
        public Satisfactions Satisfactions { get; set; }
    }
}
