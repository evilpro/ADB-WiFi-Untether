using System;

namespace ADB_WiFi_Untether_CLI
{
    class Program
    {
        static ADBDevice Device { get; set; }
        static void Main(string[] args)
        {
            AutoselectDevice();
            if(Device != null)
            {
                Console.WriteLine($"Selected Device: {Device.ToString()}");
                String Network_Interface = ADBUtility.GatherInterface(Device);

                if (!Network_Interface.Contains("wlan"))
                {
                    Console.WriteLine("Error: Your device isn't connected to WiFi.");
                    Console.WriteLine($"Active Interface: {Network_Interface}");
                }
                else
                {
                    String IP_Address = ADBUtility.GatherIPAddress(Device);
                    Console.WriteLine($"Device IP Address: {IP_Address}");
                    if(ADBUtility.IsDeviceAlreadyConnected(IP_Address))
                    {
                        Console.WriteLine("The device is already connected.");
                    }
                    else
                    {
                        Console.WriteLine("Changing ADB mode to TCP/IP...");
                        ADBUtility.ChangeADBModeToTCPIP(Device);
                        Console.WriteLine($"Connecting ADB to {IP_Address}...");
                        ADBUtility.ConnectADBToRemoteDevice(IP_Address);
                        Console.WriteLine("Done!");
                    }
                }
            }
            else
            {
                Console.WriteLine("It appears that there is no physical device connected to your computer.");
            }
            Console.ReadLine();
        }

        private static void AutoselectDevice()
        {
            foreach (ADBDevice device in ADBUtility.GetDevices())
            {
                if (!device.DEVICE_ID.Contains(".") && device.DEVICE_TYPE != "emulator")
                {
                    Device = device;
                    break;
                }
            }
        }
    }
}
