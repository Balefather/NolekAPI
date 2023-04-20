using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model.Dto
{
    public class MachineDto
    {
        [Key]
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string PartsMustChange { get; set; }
        public int ServiceInterval { get; set; }
    }
}
