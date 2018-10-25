using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Edata.CommonLibrary;


namespace LogManager
{
    public partial class ServiceMain : ServiceBase
    {
        LogManagerCore.LogManager m_xLogManager = null;

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
                m_xLogManager = new LogManagerCore.LogManager();
                m_xLogManager.bStart();

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
                if (m_xLogManager.bStop() == true)
                {
                    m_xLogManager.Dispose();
                    m_xLogManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
