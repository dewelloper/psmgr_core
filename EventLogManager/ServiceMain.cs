using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Edata.CommonLibrary;
//using EventLogManagerCore;
using Microsoft.Win32;

namespace EventLogManager
{
    /// <summary>
    /// The class is main of service. It host all properties of service application. Purpose of class is using for noticing when has any problem.
    /// </summary>
    public partial class ServiceMain : ServiceBase
    {
      EventLogManagerCore.EventLogManager m_xEventLogManager = null;

        /// <summary>
        /// Its's constructor of service main. Make creating initial components.
        /// </summary>
        public ServiceMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The method is using to start service. It is creating "EventLogManager" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="args">The parameter is string object.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                m_xEventLogManager = new EventLogManagerCore.EventLogManager();
                m_xEventLogManager.bStart();

            }
            catch (Exception xException)
            {
                //System.IO.File.WriteAllText("c:\\hata.txt", xException.Message);
                xException.TraceError();
            }
        }

        /// <summary>
        /// The method is using to stop service. It was created "EventLogManager" class whenever used trigger for stop service tasks.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (m_xEventLogManager.bStop() == true)
                {
                    m_xEventLogManager.Dispose();
                    m_xEventLogManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
