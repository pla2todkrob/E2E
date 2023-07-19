using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class ClsSatisfaction
    {
        public ClsSatisfaction()
        {
            Satisfactions = new Satisfactions();
            SatisfactionDetails = new List<SatisfactionDetails>();

            Satisfactions_BusinessCards = new Satisfactions_BusinessCards();
            SatisfactionDetails_BusinessCards = new List<SatisfactionDetails_BusinessCards>();
        }

        public List<SatisfactionDetails> SatisfactionDetails { get; set; }
        public Satisfactions Satisfactions { get; set; }


        public Satisfactions_BusinessCards Satisfactions_BusinessCards { get; set; }
        public List<SatisfactionDetails_BusinessCards> SatisfactionDetails_BusinessCards { get; set; }
    }
}
