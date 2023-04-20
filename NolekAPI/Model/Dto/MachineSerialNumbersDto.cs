using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model.Dto
{
    public class MachineSerialNumbersDto
    {
        [Key]
        public string MachineSerialNumber { get; set; }
        public int MachineID { get; set; }
    }
}
