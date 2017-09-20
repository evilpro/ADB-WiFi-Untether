using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ADB_WiFi_Untether_CLI
{
    public static class ADBUtility
    {
        public static String ADB_Path = "%localappdata%\\Android\\sdk\\platform-tools\\adb.exe";

        public static List<ADBDevice> GetDevices()
        {
            //Get connected devices
            Process DeviceQuery = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = @"/C " + $"{ADB_Path} devices",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //Launch process
            DeviceQuery.Start();
            DeviceQuery.StandardOutput.ReadLine();
            List<ADBDevice> devices = new List<ADBDevice>();
            while (!DeviceQuery.StandardOutput.EndOfStream)
            {
                String line = DeviceQuery.StandardOutput.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    String[] split = line.Split('\t');
                    devices.Add(new ADBDevice { DEVICE_ID = split[0], DEVICE_MODEL = GetDeviceFriendlyName(split[0]), DEVICE_TYPE = split[1] });
                }
            }
            //Return device list
            return devices;
        }

        public static String GetDeviceFriendlyName(String DeviceID)
        {
            //Get device model
            Process InterfaceQuery = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $@"/C {ADB_Path} -s {DeviceID} shell getprop ro.product.model",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //Launch process
            InterfaceQuery.Start();
            //Return friendly name
            return InterfaceQuery.StandardOutput.ReadLine();
        }

        public static String GatherInterface(ADBDevice device)
        {
            //Get active interface
            Process InterfaceQuery = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $@"/C {ADB_Path} -s {device.DEVICE_ID} shell ip route",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //Launch process
            InterfaceQuery.Start();
            //Return currently used interface
            return InterfaceQuery.StandardOutput.ReadLine().Split(' ')[2];
        }

        public static String GatherIPAddress(ADBDevice device)
        {
            //Get IP address of the active interface
            Process IPQuery = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $@"/C {ADB_Path} -s {device.DEVICE_ID} shell ip route",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //Launch process
            IPQuery.Start();
            //Return IP address
            return IPQuery.StandardOutput.ReadLine().Split(' ')[11];
        }

        public static void ChangeADBModeToTCPIP(ADBDevice device)
        {
            //Change the ADB mode to TCP/IP
            Process TCPIPOperation = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = @"/C " + $"{ADB_Path} -s {device.DEVICE_ID} tcpip 5555",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //Launch process
            TCPIPOperation.Start();
        }

        public static void ChangeADBModeToUSB(ADBDevice device)
        {
            //Change the ADB mode to USB
            Process USBOperation = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = @"/C " + $"{ADB_Path} -s {device.DEVICE_ID} usb",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //Launch process
            USBOperation.Start();
        }

        public static void ConnectADBToRemoteDevice(String IP_Address)
        {
            //Get IP address of the active interface
            Process ConnectOperation = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = @"/C " + $"{ADB_Path} connect {IP_Address}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            //Launch process
            ConnectOperation.Start();
        }

        public static Boolean IsDeviceAlreadyConnected(String IP_Address)
        {
            //Loop through available devices
            foreach (ADBDevice device in GetDevices())
            {
                //Check if one of the remote devices has the IP address of our physical device
                if (device.DEVICE_ID.Contains(IP_Address))
                    return true;
            }
            return false;
        }
    }
}
