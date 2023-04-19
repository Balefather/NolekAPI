namespace NolekAPI.Model
{
    public class Image
    {
        public int ImageID { get; set; }
        public string ImagePath { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Image other = (Image)obj;
            return ImagePath == other.ImagePath;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                if (ImagePath != null)
                {
                    hash = hash * 23 + ImagePath.GetHashCode();
                }
                return hash;
            }
        }

    }
}
