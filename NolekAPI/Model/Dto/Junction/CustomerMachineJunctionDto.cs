using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model.Dto.Junction
{
    public class CustomerMachineJunctionDto
    {
        //Represents the relationship between customers and specific machines, as seen in the database
        [Key]
        public string MachineSerialNumber { get; set; }
        public int CustomerID { get; set; }
        public DateTime NextService { get; set; }
    }
}
