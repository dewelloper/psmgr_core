using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Edata.CommonLibrary;
//using TerminalManagerCore;
using Microsoft.Win32;

namespace TerminalManager
{
    /// <summary>
    /// The class is service main. It host all properties of service application properties.
    /// </summary>
    public partial class ServiceMain : ServiceBase
    {
      TerminalManagerCore.TerminalManager m_xTerminalManager = null;

        /// <summary>
        /// It's constructor of service main. Make creating initial components.
        /// </summary>
        public ServiceMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The method is using to start dispatcher service. Creating "TerminalManager" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="args">The parameter is string argument object.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                m_xTerminalManager = new TerminalManagerCore.TerminalManager();
                m_xTerminalManager.bStart();

            }
            catch (Exception xException)
            {
                //System.IO.File.WriteAllText("c:\\hata.txt", xException.Message);
                xException.TraceError();
            }
        }

        /// <summary>
        /// The method is using to stop dispatcher service. Was created "TerminalManager" class whenever used trigger for stop service tasks.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (m_xTerminalManager.bStop() == true)
                {
                    m_xTerminalManager.Dispose();
                    m_xTerminalManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
