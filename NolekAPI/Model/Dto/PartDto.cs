using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace NolekAPI.Model.Dto
{
    public class PartDto
    {

        [Key]
        public int PartID { get; set; }
        public string PartName { get; set; }
        public int NumberInStock { get; set; }
        public double PartPrice { get; set; }
    }
}
