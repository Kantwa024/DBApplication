using DBApplication.Data;
using DBApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace DBApplication.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("/api/[controller]")]
    public class ReportsController : Controller
    {
        private readonly ApiDBContext dbContext;
        public ReportsController(ApiDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("{uid:guid}")]
        public async Task<IActionResult> GetReports([FromRoute] Guid uid)
        { 
            var user = await dbContext.Users.FindAsync(uid);
            if(user != null)
            {
                return Ok(await dbContext.Reports.ToListAsync());
            }

            return NotFound();
        }

        [HttpGet]
        [Route("/search/{query}/{uid:guid}")]
        public async Task<IActionResult> GetSearchedReports([FromRoute] string query, [FromRoute] Guid uid)
        {
            var user = await dbContext.Users.FindAsync(uid);
            if (user != null)
            {
                var reports = await dbContext.Reports.Where(x =>
                    x.Name.ToLower().Trim().Contains(query.ToLower().Trim())
                || query.ToLower().Trim().Contains(x.Name.ToLower().Trim())
                || x.StartDate.ToLower().Trim().Contains(query.ToLower().Trim())
                || query.ToLower().Trim().Contains(x.StartDate.ToLower().Trim())
                || x.EndDate.ToLower().Trim().Contains(query.ToLower().Trim())
                || query.ToLower().Trim().Contains(x.EndDate.ToLower().Trim())).ToListAsync();

                return Ok(reports);
            }

            return NotFound();
        }

        [HttpPost]
        [Route("{uid:guid}")]
        public async Task<IActionResult> AddReport([FromRoute] Guid uid, AddReportData addReportData)
        {
            var user = await dbContext.Users.FindAsync(uid);
            if (user != null)
            {
                if(user.isAdmin)
                {
                    var report = new Report()
                    {
                        Rid = Guid.NewGuid(),
                        Name = addReportData.Name,
                        StartDate = addReportData.StartDate,
                        EndDate = addReportData.EndDate
                    };

                    await dbContext.Reports.AddAsync(report);
                    await dbContext.SaveChangesAsync();

                    return Ok(report);
                }

                return NotFound();
            }

            return NotFound();
        }

        [HttpPut]
        [Route("{rid:guid}/{uid:guid}")]
        public async Task<IActionResult> UpdateReport([FromRoute] Guid rid, [FromRoute] Guid uid, UpdateReportData updateReportData)
        {
            var user = await dbContext.Users.FindAsync(uid);
            if (user != null)
            {
                if (user.isAdmin)
                {
                    var reports = await dbContext.Reports.FindAsync(rid);

                    if (reports != null)
                    {
                        reports.Name = updateReportData.Name;
                        reports.StartDate = updateReportData.StartDate;
                        reports.EndDate = updateReportData.EndDate;

                        await dbContext.SaveChangesAsync();

                        return Ok(reports);
                    }

                    return NotFound();
                }

                return NotFound();
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{rid:guid}/{uid:guid}")]
        public async Task<IActionResult> DeleteReport([FromRoute] Guid rid, [FromRoute] Guid uid)
        {
            var user = await dbContext.Users.FindAsync(uid);
            if (user != null)
            {
                if (user.isAdmin)
                {
                    var report = await dbContext.Reports.FindAsync(rid);

                    if (report != null)
                    {
                        dbContext.Remove(report);
                        await dbContext.SaveChangesAsync();

                        return Ok(report);
                    }

                    return NotFound();
                }

                return NotFound();
            }

            return NotFound();
        }
        
    }
}
