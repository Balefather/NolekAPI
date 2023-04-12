using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model
{
    public class Service
    {
        [Key]
        public int ServiceID { get; set; }
        public DateTime ServiceDate { get; set; }
        public string CustomerName { get; set; }
        public string MachineName { get; set; }
        public string MachineSerialNumber { get; set; }
        public int PartsUsed { get; set; }
        public string PartName { get; set; }
        public int TransportTimeUsed { get; set; }
        public int TransportKmUsed { get; set; }
        public int WorkTimeUsed { get; set; }
        public string ServiceImage { get; set; }
        public string Note { get; set; }
        public string MachineStatus { get; set; }
    }
    
}
