using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model
{
    public class CustomersMachinesParts
    {
        
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string MachineName { get; set; }
        public string PartsMustChange { get; set; }
        public int ServiceInterval { get; set; }
        public int MachineID { get; set; }
        public string MachineSerialNumber { get; set; }

        public int PartID { get; set; }
        public string PartName { get; set; }
        public int AmountPartMachine { get; set; }
        public int NumberInStock { get; set; }
        public double PartPrice { get; set; }
    }
}
