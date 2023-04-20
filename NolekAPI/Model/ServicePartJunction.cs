using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NolekAPI.Model
{
    public class ServicePartJunction
    {
        public int PartID { get; set; }
        public int PartsUsed { get; set; }
        [Key]
        public int ServiceID { get; set; }
    }
}
