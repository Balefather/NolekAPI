namespace NolekAPI.Model
{
    public class Invoice
    {
        public DateTime ServiceDate { get; set; }
        public string TotalPartsName { get; set; }
        public int WorkTimeUsed { get; set; }
        public decimal WorkPrice { get; set; }
        public int TransportTimeUsed { get; set; }
        public decimal TransportTimePrice { get; set; }
        public int TransportKmUsed { get; set; }
        public decimal TransportKmPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
