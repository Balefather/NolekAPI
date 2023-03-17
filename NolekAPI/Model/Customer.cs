using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Machine> Machines { get; set; }

    }
}
