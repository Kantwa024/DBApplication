using System.ComponentModel.DataAnnotations;

namespace DBApplication.Models
{
    public class Auth
    {
        [Key]
        public Guid Uid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool isAdmin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
    }
}
