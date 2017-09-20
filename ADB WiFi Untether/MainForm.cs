using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace ADB_WiFi_Untether
{
    public partial class MainForm : Form
    {
        List<ADBDevice> availableDevices;
        ADBDevice deviceForOperation;
        BackgroundWorker untetherWorker;
        BackgroundWorker disconnectWorker;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitializeWorkers();
            RefreshDevices();
        }

        private void InitializeWorkers()
        {
            untetherWorker = new BackgroundWorker();
            untetherWorker.WorkerReportsProgress = true;
            untetherWorker.ProgressChanged += ProgressChanged;
            untetherWorker.DoWork += UntetherWorker_DoWork;
            untetherWorker.RunWorkerCompleted += RunWorkerCompleted;
            disconnectWorker = new BackgroundWorker();
            disconnectWorker.WorkerReportsProgress = true;
            disconnectWorker.ProgressChanged += ProgressChanged;
            disconnectWorker.DoWork += DisconnectWorker_DoWork;
            disconnectWorker.RunWorkerCompleted += RunWorkerCompleted;
        }

        private void UntetherWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            untetherWorker.ReportProgress(0, "Gathering IP Address...");
            String IP_Address = ADBUtility.GatherIPAddress(deviceForOperation);
            untetherWorker.ReportProgress(0, "Changing ADB mode to TCP/IP...");
            ADBUtility.ChangeADBModeToTCPIP(deviceForOperation);
            untetherWorker.ReportProgress(0, $"Connecting ADB to {IP_Address}");
            ADBUtility.ConnectADBToRemoteDevice(IP_Address);
            untetherWorker.ReportProgress(0, "Waiting...");
            Thread.Sleep(5000);
            this.Invoke(new Action(() =>
            {
                RefreshDevices();
            }));
        }
        private void DisconnectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            disconnectWorker.ReportProgress(0, "Changing ADB mode to USB...");
            ADBUtility.ChangeADBModeToUSB(deviceForOperation);
            untetherWorker.ReportProgress(0, "Waiting...");
            Thread.Sleep(5000);
            this.Invoke(new Action(() =>
            {
                RefreshDevices();
            }));
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            statusLabel.Text = e.UserState.ToString();
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Operation completed", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshDevices()
        {
            availableDevices = ADBUtility.GetDevices();
            BindingSource bsource = new BindingSource();
            bsource.DataSource = availableDevices;
            comboBoxDevices.DataSource = bsource;

            if(availableDevices.Count == 0)
            {
                buttonUntether.Enabled = false;
                buttonUntether.Text = "Untether";
                statusLabel.Text = "No devices connected.";
            }
        }

        private void comboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxDevices.SelectedItem != null)
            {
                ADBDevice selectedDevice = (ADBDevice)comboBoxDevices.SelectedItem;
                if (selectedDevice.DEVICE_ID.Contains("."))
                {
                    buttonUntether.Text = "Disconnect";
                    statusLabel.Text = "This is a remote device connected to this computer.";
                }
                else if(ADBUtility.IsDeviceAlreadyConnected(ADBUtility.GatherIPAddress(selectedDevice)))
                {
                    buttonUntether.Text = "Disconnect";
                    statusLabel.Text = "This device is already connected to this computer over WiFi.";
                }
                else
                {
                    buttonUntether.Text = "Untether";
                    String Network_Interface = ADBUtility.GatherInterface(selectedDevice);
                    buttonUntether.Enabled = Network_Interface.Contains("wlan");
                    statusLabel.Text = Network_Interface.Contains("wlan") ? "Ready" : "The selecte device is not connected to a WiFi network.";
                }
            }
        }

        private void buttonUntether_Click(object sender, EventArgs e)
        {
            deviceForOperation = (ADBDevice)comboBoxDevices.SelectedItem;
            if (buttonUntether.Text == "Untether")
            {
                untetherWorker.RunWorkerAsync();
            }
            else if(buttonUntether.Text == "Disconnect")
            {
                disconnectWorker.RunWorkerAsync();
            }
        }

        private void buttonReload_Click(object sender, EventArgs e)
        {
            RefreshDevices();
        }
    }
}
