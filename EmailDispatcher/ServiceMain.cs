using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;

using Edata.CommonLibrary;
using EmailDispatcherCore;

namespace EmailDispatcher
{

    /// <summary>
    /// The class is main of service. It host all properties of service application. Purpose of class is using for send mail that content daily Z report of dispatchers and sales information.
    /// </summary>
    public partial class ServiceMain : ServiceBase
    {
        EmailDispatcherManager m_xEmailDispatcherManager = null;
      
        /// <summary>
        /// It's constructor of service main. Make creating initial components.
        /// </summary>
   
        public ServiceMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The method is using to start service. It is creating "EmailDispatcherManager" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="args">The parameter is string argument object.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                m_xEmailDispatcherManager = new EmailDispatcherManager();
                m_xEmailDispatcherManager.bStart();

            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        /// <summary>
        /// The method is using to stop service. It was created "EmailDispatcherManager" class whenever used trigger for stop service tasks.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (m_xEmailDispatcherManager.bStop() == true)
                {
                    m_xEmailDispatcherManager.Dispose();
                    m_xEmailDispatcherManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
