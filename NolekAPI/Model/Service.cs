using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using NolekAPI.Model.Dto;

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
        public List<ServicePart> Parts { get; set; }
        public List<ImageDto> Images { get; set; }
        public int TransportTimeUsed { get; set; }
        public int TransportKmUsed { get; set; }
        public int WorkTimeUsed { get; set; }
        public string Note { get; set; }
        public string MachineStatus { get; set; }
    }
}
