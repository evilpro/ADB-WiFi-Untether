using System;

namespace ADB_WiFi_Untether_CLI
{
    public class ADBDevice
    {
        public String DEVICE_ID;
        public String DEVICE_MODEL;
        public String DEVICE_TYPE;

        public override string ToString()
        {
            return $"{DEVICE_MODEL} ({DEVICE_ID})";
        }
    }
}
