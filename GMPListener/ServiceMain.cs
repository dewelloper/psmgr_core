/****************************************************************************************
 * Class		: ServiceMain
 * Purpose		: Main routine of GMP Listener Service
 * Create Date  : 08/04/2014 By Eylem Artut (BeterBocek)
 * Modify Date  : 

 Copyright(c) 2013 By Edata Electronics
 All Rights Reserved
 
 * Modifiying History
 * 08/04/2014 By Eylem Artut (BeterBocek) - Create Initial Class
 *****************************************************************************************/
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
using GMPListenerCore;

namespace GMPListener
{
    /// <summary>
    /// The class is service main. It host all properties of service application properties.
    /// </summary>
    public partial class ServiceMain : ServiceBase
    {
        GMPListenerManager m_xGMPListenerManager = null;

        /// <summary>
        /// It's constructor of service main. Make creating initial components.
        /// </summary>
        public ServiceMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The method is using to start dispatcher service. Creating "GMPListenerManager" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="args">The parameter is string argument object.</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                m_xGMPListenerManager = new GMPListenerManager();
                m_xGMPListenerManager.bStart();
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        /// <summary>
        /// The method is using to stop dispatcher service. Was created "GMPListenerManager" class whenever used trigger for stop service tasks.
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                if (m_xGMPListenerManager.bStop() == true)
                {
                    m_xGMPListenerManager.Dispose();
                    m_xGMPListenerManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
