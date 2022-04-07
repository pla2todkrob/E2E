using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E2E.Models.Views
{
    public class clsTopic
    {
        public Topics Topics { get; set; }
        public TopicComments topicComments_NoList { get; set; }
        public List<TopicComments> TopicComments { get; set; }
        public List<TopicFiles> TopicFiles { get; set; }
        public List<TopicGalleries> TopicGalleries { get; set; }
        public clsTopic()
        {
            TopicComments = new List<TopicComments>();
            TopicFiles = new List<TopicFiles>();
            TopicGalleries = new List<TopicGalleries>();
        }
    }
}