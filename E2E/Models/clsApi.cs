using E2E.Models.Tables;

namespace E2E.Models
{
    public class clsApi
    {
        public clsApi()
        {
            isSuccess = new bool();
        }

        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public dynamic Value { get; set; }
    }

    public class responseUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Users Users { get; set; }
    }
}
