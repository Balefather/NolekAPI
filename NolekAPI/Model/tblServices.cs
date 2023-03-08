using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model
{
    public class tblServices
    {
        [Key]
        public int ServiceID { get; set; }
        public int CustomerID { get; set; }
        public int MachineID { get; set; }
        public DateTime ServiceDate { get; set; }
        public string PartsUsed { get; set; }
        public int TransportTimeUsed { get; set; }
        public int TransportKmUsed { get; set; }
        public int WorkTimeUsed { get; set; }
        public string ImagePath { get; set; }
    }
}
