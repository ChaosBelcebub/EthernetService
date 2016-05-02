using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
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
        private bool first = true;
        private bool ethernetUp = false;
        private bool wirelessUp = false;
        private bool ethernetUpBefore;
        private bool wirelessUpBefore;
        private ManagementObjectSearcher objadapter;
        private ManagementObject ethernet = null;
        private ManagementObject wireless = null;

        public EthernetService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            objadapter = new ManagementObjectSearcher("select * from Win32_NetworkAdapter");
            timer = new Timer();
            this.timer.Interval = 5000;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.checkTimer);
            timer.Enabled = true;
            Library.deleteLog();
            Library.writeErrorLog("EthernetService started");
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            try
            {
                wireless.InvokeMethod("Enable", null);
                Library.writeErrorLog("Wireless connection enabled.");
            }
            catch (Exception ex)
            {
                Library.writeErrorLog(ex);
            }
            Library.writeErrorLog("EthernetService stopped");
        }

        private void checkTimer(object sender, ElapsedEventArgs e)
        {
            foreach (ManagementObject obj in objadapter.Get())
            {
                // Check for ethernet
                if (obj["Name"].ToString() == "Realtek PCIe GBE Family Controller")
                {
                    if (ethernet == null)
                    {
                        ethernet = obj;
                    }

                    // Set values
                    ethernetUpBefore = ethernetUp;
                    if (obj["NetEnabled"].ToString() == "True")
                    {
                        ethernetUp = true;
                    }
                    else
                    {
                        ethernetUp = false;
                    }
                    // Check for wireless
                } else if (obj["Name"].ToString() == "Intel(R) Centrino(R) Wireless-N 100")
                {
                    if (wireless == null)
                    {
                        wireless = obj;
                    }

                    // Set values
                    wirelessUpBefore = wirelessUp;
                    if (obj["NetEnabled"].ToString() == "True")
                    {
                        wirelessUp = true;
                    }
                    else
                    {
                        wirelessUp = false;
                    }
                }
            }
            if (ethernetUp && (!ethernetUpBefore || first))
            {
                try
                {
                    wireless.InvokeMethod("Disable", null);
                    Library.writeErrorLog("Wireless connection disabled.");
                    first = false;
                }
                catch (Exception ex)
                {
                    Library.writeErrorLog(ex);
                }
            } else if (!ethernetUp && ethernetUpBefore)
            {
                try
                {
                    wireless.InvokeMethod("Enable", null);
                    Library.writeErrorLog("Wireless connection enabled.");
                    first = false;
                }
                catch (Exception ex)
                {
                    Library.writeErrorLog(ex);
                }
            } else if (!ethernetUp && !wirelessUp)
            {
                // If both connections are disabled wireless should be enabled
                try
                {
                    wireless.InvokeMethod("Enable", null);
                    Library.writeErrorLog("Wireless connection enabled.");
                    first = false;
                }
                catch (Exception ex)
                {
                    Library.writeErrorLog(ex);
                }
            }
        }
    }
}
