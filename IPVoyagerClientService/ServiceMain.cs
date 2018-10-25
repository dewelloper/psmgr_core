using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Win32;

using IPVoyagerClientCore;
using Edata.CommonLibrary;

namespace IPVoyagerClient
{
    public partial class ServiceMain : ServiceBase
    {
        IPVoyagerClientManager m_xIPVoyagerClientManager = null;
        //private System.Timers.Timer xTimer = null;

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {       
            //xTimer = new System.Timers.Timer();     
            try
            {    
                //xTimer.Interval = 60000;
                //xTimer.Elapsed += new System.Timers.ElapsedEventHandler(xTimer_Tick);
                //xTimer.Enabled = true;
                m_xIPVoyagerClientManager = new IPVoyagerClientManager();
                Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "IPVoyager Client Service will be started...");
            }
            catch (Exception xException)
            {
                //xTimer.Enabled = false; 
                xException.TraceError();
            }        
        }

        private void xTimer_Tick(object sender, ElapsedEventArgs e)
        {
            m_xIPVoyagerClientManager.bStart();
            Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Timer tick will be started...");
        }

        protected override void OnStop()
        {
            try
            {
                if (m_xIPVoyagerClientManager.bStop() == true)
                {
                    //xTimer.Enabled = false;
                    m_xIPVoyagerClientManager.Dispose();
                    m_xIPVoyagerClientManager = null;
                    // trace
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
