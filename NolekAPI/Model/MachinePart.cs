using System.ComponentModel.DataAnnotations;
using NolekAPI.Model.Dto;

namespace NolekAPI.Model
{
    public class MachinePart : PartDto
    {
        public int AmountPartMachine { get; set; }
    }
}
