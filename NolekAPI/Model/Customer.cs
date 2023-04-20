using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model
{
    public class Customer
    {
        //Represents a customer as we want to be seen in the front-end
        [Key]
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<CustomerMachine> Machines { get; set; }

    }
}
