using System.ComponentModel.DataAnnotations;
namespace NolekAPI.Model
{
    public class tblParts
    {
        [Key]
        public int PartID { get; set; }
        public string PartName { get; set; }
        public int NumberInStock { get; set; }
        public int PartPrice { get; set; }
    }
}
