using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using SystemInformationServiceCore;
using Edata.DataAccessLayer;
using Edata.CommonLibrary;
namespace SystemInformationService
{

    /// <summary>
    /// The class is main of service. It host all properties of service application. Purpose of class is using for collect and update information of system.
    /// </summary>
    public partial class ServiceMain : ServiceBase
    {
        SystemInformationManagement m_xSystemInformationManagement =null;

        /// <summary>
        /// It's constructor of service main. Make creating initial components.
        /// </summary>
        public ServiceMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The method is using to start service. It is creating "SystemInformationManagement" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="args">The parameter is string object.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                m_xSystemInformationManagement = new SystemInformationManagement();
                m_xSystemInformationManagement.bStart();
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        /// <summary>
        /// The method is using to stop service. Was created "SystemInformationManagement" class whenever used trigger for stop service tasks.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                m_xSystemInformationManagement.bStop();
                m_xSystemInformationManagement.Dispose();
                m_xSystemInformationManagement = null;

            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
