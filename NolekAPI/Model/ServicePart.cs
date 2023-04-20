using NolekAPI.Model.Dto;

namespace NolekAPI.Model
{
    public class ServicePart : PartDto
    {
        public int PartsUsed { get; set; }




        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ServicePart other = (ServicePart)obj;
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
