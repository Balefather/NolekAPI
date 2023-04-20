using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model.View
{
    public class MachineView
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string PartsMustChange { get; set; }
        public int ServiceInterval { get; set; }
        public int PartID { get; set; }
        public string PartName { get; set; }
        public int NumberInStock { get; set; }
        public double PartPrice { get; set; }
    }
}
