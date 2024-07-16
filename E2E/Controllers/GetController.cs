using E2E.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace E2E.Controllers
{
    public class GetController : ApiController
    {
        private readonly ClsContext db = new ClsContext();

        [HttpGet]
        public async Task<IHttpActionResult> GetAllUser()
        {
            try
            {
                var userDetails = await db.UserDetails.ToListAsync();
                if (userDetails.Any())
                {
                    return Ok(userDetails);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> NotifyAuto()
        {
            try
            {
                ClsManageService service = new ClsManageService();
                await service.JobDaily();

                int currentDay = DateTime.Today.Day;
                if (currentDay == 8)
                {
                    await service.JobMonthly();
                }

                return Ok("Notification process completed successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
