using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using NolekAPI.Model.Dto.Junction;

namespace NolekAPI.Model.Dto
{
    public class ServiceDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceID { get; set; }
        public DateTime ServiceDate { get; set; }
        public int CustomerID { get; set; }
        public int MachineID { get; set; }
        public string MachineSerialNumber { get; set; }
        public List<ServicePartJunctionDto> ServiceParts { get; set; }

        public int TransportTimeUsed { get; set; }
        public int TransportKmUsed { get; set; }
        public int WorkTimeUsed { get; set; }
        public string Note { get; set; }
        public string MachineStatus { get; set; }
    }

}
