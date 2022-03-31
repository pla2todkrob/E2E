using E2E.Models;
using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace E2E.Controllers
{
    [Authorize]
    public class SelectController : ApiController
    {
        private clsManageData data = new clsManageData();

        public Master_Divisions Division_Get(Guid id)
        {
            return data.Division_Get(id);
        }

        public List<Master_Divisions> Division_GetAll()
        {
            return data.Division_GetAll();
        }
    }
}
