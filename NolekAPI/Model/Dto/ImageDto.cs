using System.ComponentModel.DataAnnotations;

namespace NolekAPI.Model.Dto
{
    public class ImageDto
    {
        [Key]
        public int ImageID { get; set; }
        public string ImagePath { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            ImageDto other = (ImageDto)obj;
            return ImagePath == other.ImagePath && ImageID == other.ImageID;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                if (ImagePath != null)
                {
                    hash = hash * 23 + ImagePath.GetHashCode();
                    hash = hash * 23 + ImageID.GetHashCode();
                }
                return hash;
            }
        }

    }
}
