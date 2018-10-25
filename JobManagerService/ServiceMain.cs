using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Edata.CommonLibrary;

namespace JobManagerService
{
    public partial class ServiceMain : ServiceBase
    {
        JobManagerServiceCore.JobManagement m_xJobManager = null;

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                m_xJobManager = new JobManagerServiceCore.JobManagement();
                m_xJobManager.bStart();
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
                m_xJobManager.bStop();
                m_xJobManager.Dispose();
                m_xJobManager = null;

            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
