using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model.Dto
{
    public class UserDto
    {
        [Key]
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
