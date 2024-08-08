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
        public async Task<IHttpActionResult> GetUser(string userCode)
        {
            try
            {
                var userDetail = await db.UserDetails.FirstOrDefaultAsync(f => f.Users.User_Code == userCode);
                if (userDetail != null)
                {
                    return Ok(userDetail);
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
                ClsManageService clsManageService = new ClsManageService();
                await clsManageService.JobDaily();

                //currentDay default is 8
                //currentDay test is 5
                int currentDay = DateTime.Today.Day;

                if (currentDay == 8)
                {
                    await clsManageService.JobMonthly();
                }

                return Ok("Notification process completed successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> CloseSelectedJob(string key)
        {
            try
            {
                ClsManageService clsManageService = new ClsManageService();

                var service = await db.Services
                    .AsNoTracking()
                    .Where(w => w.Service_Key == key)
                    .FirstOrDefaultAsync();

                var result = await clsManageService.Services_SetClose(service, true);

                return Ok("The desired Job has been closed successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
