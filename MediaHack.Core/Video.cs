using System;

namespace MediaHack.Core
{
    public class Video
    {
        public String Title { get; set; }
        public String Name { get; set; }
        public String StorageUrl { get; set; }
        public String ContentDeliveryNetworkUrl { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
