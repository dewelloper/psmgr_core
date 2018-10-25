using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Edata.CommonLibrary;
using InDemoCore;


namespace InDemo
{
    public partial class ServiceMain : ServiceBase
    {
        InDemoManager m_xInDemoManager = null;

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                m_xInDemoManager = new InDemoManager();
                m_xInDemoManager.bStart();
            } 
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (m_xInDemoManager != null && m_xInDemoManager.bStop() == true)
                {
                    m_xInDemoManager.Dispose();
                    m_xInDemoManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
