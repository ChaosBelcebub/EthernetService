using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EthernetService
{
    public partial class EthernetService : ServiceBase
    {
        private Timer timer = null;

        public EthernetService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new Timer();
            this.timer.Interval = 30000;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timerTest);
            timer.Enabled = true;
            Library.writeErrorLog("Test window service started");
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            Library.writeErrorLog("Test window service stopped");
        }

        private void timerTest(object sender, ElapsedEventArgs e)
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    Library.writeErrorLog(ni.Name + ":");
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            Library.writeErrorLog("  " + ip.Address.ToString());
                        }
                    }
                }
            }
        }
    }
}
