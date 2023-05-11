using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Newtonsoft.Json;

namespace NolekAPI.Model.Dto.Junction
{
    public class ServicePartJunctionDto
    {
        public int PartID { get; set; }
        public int PartsUsed { get; set; }

        public int ServiceID { get; set; }

    }
}
