using E2E.Models.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class ClsGPT
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public int Tokens { get; set; }
        public int Amount { get; set; }

    }

    public class SearchFilter
    {
        private ClsContext db = new ClsContext();

        [Display(Name = "To")]
        public DateTime Date_To { get; set; }

        [Display(Name = "From")]
        public DateTime? Date_From { get; set; }

        public SearchFilter()
        {
            Date_To = DateTime.Now;
            Date_From = DateTime.Today.AddDays( - DateTime.Today.Day +1) ;
        }


        public SearchFilter DeserializeFilter(string filter)
        {
            try
            {
                SearchFilter res = JsonConvert.DeserializeObject<SearchFilter>(filter);

                if (!res.Date_From.HasValue)
                {
                    res.Date_From = db.ChatGPTs.OrderBy(o => o.Create).Select(s => s.Create).FirstOrDefault();
                }

                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }



    public class ClsGPT_Sum
    {
        public List<ClsGPT> ClsGPTs { get; set; }
        public int TotalAmount { get; set; }
        public int TotalToken { get; set; }
        public ClsGPT_Sum()
        {
            ClsGPTs = new List<ClsGPT>();
        }

    }
}