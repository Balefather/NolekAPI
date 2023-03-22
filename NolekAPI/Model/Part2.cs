using System.ComponentModel.DataAnnotations;
namespace NolekAPI.Model
{
    public class Part2
    {
        [Key]
        public int PartID { get; set; }
        public string PartName { get; set; }
        public int NumberInStock { get; set; }
        public int AmountPartMachine { get; set; }
        public double PartPrice { get; set; }
    }
}
