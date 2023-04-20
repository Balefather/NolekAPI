using NolekAPI.Model.Dto;

namespace NolekAPI.Model
{
    public class CustomerMachine : MachineDto
    {
        //Represents a specific machine as seen in the frontend
        public int CustomerID { get; set; }
        public string MachineSerialNumber { get; set; }
    }
}
