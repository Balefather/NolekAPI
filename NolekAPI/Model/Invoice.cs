namespace NolekAPI.Model
{
    public class Invoice
    {
        public DateTime ServiceDate { get; set; }
        public string TotalPartsName { get; set; }
        public int WorkTimeUsed { get; set; }
        public double WorkPrice { get; set; }
        public int TransportTimeUsed { get; set; }
        public double TransportTimePrice { get; set; }
        public int TransportKmUsed { get; set; }
        public double TransportKmPrice { get; set; }
        public double TotalPrice { get; set; }
    }
}
