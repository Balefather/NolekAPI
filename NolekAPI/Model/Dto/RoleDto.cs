using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model.Dto
{
    public class RoleDto
    {
        [Key]
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
