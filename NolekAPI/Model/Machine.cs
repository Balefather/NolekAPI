using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model
{
    public class Machine
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string PartsMustChange { get; set; }
        public int ServiceInterval { get; set; }
        public List<Part> Parts { get; set; }
    }
}
