using E2E.Models.Tables;
using System.Collections.Generic;

namespace E2E.Models.Views
{
    public class ClsTopic
    {
        public ClsTopic()
        {
            TopicComments = new List<TopicComments>();
            TopicFiles = new List<TopicFiles>();
            TopicGalleries = new List<TopicGalleries>();
        }

        public List<TopicComments> TopicComments { get; set; }
        public TopicComments TopicComments_NoList { get; set; }
        public List<TopicFiles> TopicFiles { get; set; }
        public List<TopicGalleries> TopicGalleries { get; set; }
        public Topics Topics { get; set; }
        public List<TopicSections> TopicSections { get; set; }
    }
}
