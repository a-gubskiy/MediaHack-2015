using System;

namespace MediaHack.Core
{
    [Serializable]
    public class CloudConfig
    {
        public String StorageServiceAccountName { get; set; }
        public String StorageServiceAccessKey { get; set; }
        public String CdnEndpointName { get; set; }

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(StorageServiceAccountName)
                       && String.IsNullOrEmpty(StorageServiceAccessKey)
                       && String.IsNullOrEmpty(CdnEndpointName);
            }
        }
    }
}
