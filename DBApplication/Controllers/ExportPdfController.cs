using DBApplication.Data;
using DBApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;
using Syncfusion.Pdf.Grid;
using Microsoft.AspNetCore.Cors;

namespace DBApplication.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ExportPdfController : Controller
    {
        private readonly ApiDBContext dbContext;
        public ExportPdfController(ApiDBContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        [Route("{uid:guid}/{query}")]
        public async Task<ActionResult> GetExcel([FromRoute] Guid uid, [FromRoute] string? query )
        {
            var user = await dbContext.Users.FindAsync(uid);
            if (user != null)
            {
                if (user.isAdmin)
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

                    PdfDocument doc = new PdfDocument();
                    PdfPage page = doc.Pages.Add();

                    List<AddReportData> data = new List<AddReportData>();
                    foreach(Report report in obj)
                    {
                        AddReportData addReportData = new AddReportData();
                        addReportData.Name = report.Name;
                        addReportData.StartDate = report.StartDate;
                        addReportData.EndDate = report.EndDate;
                        data.Add(addReportData);
                    }
                    PdfGrid pdfGrid = new PdfGrid();
                    IEnumerable<object> dataTable = data;

                    pdfGrid.DataSource = dataTable;
                    pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));
                    MemoryStream stream = new MemoryStream();
                    doc.Save(stream);
                    stream.Position = 0;
                    doc.Close(true);

                    string contentType = "application/pdf";
                    string fileName = "Reports" + DateTime.Now.ToString() + ".pdf";
                    return File(stream, contentType, fileName);

                }

                return NotFound();
            }

            return NotFound();
        }
    }
}
