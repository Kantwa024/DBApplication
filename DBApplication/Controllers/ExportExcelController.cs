using DBApplication.Data;
using DBApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Cors;

namespace DBApplication.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ExportExcelController : Controller
    {
        private readonly ApiDBContext dbContext;
        public ExportExcelController(ApiDBContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        [Route("{uid:guid}/{query}")]
        public async Task<ActionResult> GetExcel([FromRoute] string? query, [FromRoute] Guid uid)
        {
            var user = await dbContext.Users.FindAsync(uid);
            if (user != null)
            {
                if(user.isAdmin)
                {
                    List<Report> obj = new List<Report>();
                    if (query == null)
                    {
                        obj = await dbContext.Reports.ToListAsync();
                    }
                    else
                    {
                        obj = await dbContext.Reports.Where(x =>
                                       x.Name.ToLower().Trim().Contains(query.ToLower().Trim())
                                    || query.ToLower().Trim().Contains(x.Name.ToLower().Trim())
                                    || x.StartDate.ToLower().Trim().Contains(query.ToLower().Trim())
                                    || query.ToLower().Trim().Contains(x.StartDate.ToLower().Trim())
                                    || x.EndDate.ToLower().Trim().Contains(query.ToLower().Trim())
                                    || query.ToLower().Trim().Contains(x.EndDate.ToLower().Trim())).ToListAsync();
                    }


                    StringBuilder str = new StringBuilder();
                    str.Append("<table border=`" + "1px" + "`b>");
                    str.Append("<tr>");
                    str.Append("<td><b><font face=Arial Narrow size=3>Name</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>Start Date</font></b></td>");
                    str.Append("<td><b><font face=Arial Narrow size=3>End Date</font></b></td>");
                    str.Append("</tr>");

                    foreach(Report report in obj)
                    {
                        str.Append("<tr>");
                        str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + report.Name.ToString() + "</font></td>");
                        str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + report.StartDate.ToString() + "</font></td>");
                        str.Append("<td><font face=Arial Narrow size=" + "14px" + ">" + report.EndDate.ToString() + "</font></td>");
                        str.Append("</tr>");
                    }

                    str.Append("</table>");


                    this.Response.Headers.Add("content-disposition", "attachment; filename=Reports" + DateTime.Now.ToString() + ".xls");
                    this.Response.ContentType = "application/vnd.ms-excel";
                    byte[] temp = System.Text.Encoding.UTF8.GetBytes(str.ToString());
                    return File(temp, "application/vnd.ms-excel");

                }

                return NotFound();
            }

            return NotFound();
        }
     
    }
}
