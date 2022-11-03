using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace DBApplication.Models
{
    public class Report
    {
        [Key]
        public Guid Rid { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
