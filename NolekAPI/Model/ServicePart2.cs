namespace NolekAPI.Model
{
    public class ServicePart2
    {
        public int PartID { get; set; }
        public int PartsUsed { get; set; }
        public string PartName { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ServicePart2 other = (ServicePart2)obj;
            return PartID == other.PartID
                && PartName == other.PartName
                && PartsUsed == other.PartsUsed;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + PartID.GetHashCode();
                hash = hash * 23 + (PartName ?? "").GetHashCode();
                hash = hash * 23 + PartsUsed.GetHashCode();
                return hash;
            }
        }

    }
}
