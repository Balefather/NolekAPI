﻿using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model
{
    public class MachineParts
    {
        [Key]
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public int PartID { get; set; }
        public string PartName { get; set; }
        public int AmountPartMachine { get; set; }
        public int NumberInStock { get; set; }
        public double PartPrice { get; set; }
    }
}
