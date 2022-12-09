namespace E2E.Models.Views
{
    public class ClsSwal
    {
        public ClsSwal()
        {
            Title = "Error";
            Icon = "error";
            Button = "OK";
            DangerMode = true;
        }

        public string Button { get; set; }
        public bool DangerMode { get; set; }
        public string Icon { get; set; }
        public dynamic Option { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
    }
}
