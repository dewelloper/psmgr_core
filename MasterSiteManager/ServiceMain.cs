using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Edata.CommonLibrary;
using Microsoft.Win32;
using MasterSiteManagerCore;

namespace MasterSiteManager
{
    /// <summary>
    /// The class is service main. It host all properties of service application properties.
    /// </summary>
    partial class ServiceMain : ServiceBase
    {
        MasterSiteManagerCore.MasterSiteManager m_xSiteManager = null;

        /// <summary>
        /// Constructor of service main. Make creating initial components.
        /// </summary>
        public ServiceMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The method is using to start dispatcher service. Creating "DominosDispatcherManager" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="args">The parameter is string argument object.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                m_xSiteManager = new MasterSiteManagerCore.MasterSiteManager();
                m_xSiteManager.bStart();

            }
            catch (Exception xException)
            {
                //System.IO.File.WriteAllText("c:\\hata.txt", xException.Message);
                xException.TraceError();
            }
        }

        /// <summary>
        /// The method is using to stop dispatcher service. Was created "DominosDispatcherManager" class whenever used trigger for stop service tasks.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (m_xSiteManager.bStop() == true)
                {
                    m_xSiteManager.Dispose();
                    m_xSiteManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
