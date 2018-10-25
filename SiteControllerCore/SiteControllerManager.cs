using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Edata.CommonLibrary;
using Edata.Edip;
using Edata.TcpIpSocket;
using Edata.DataTypes.DataSets;
using Edata.DataTypes;
using Edata.DataAccessLayer;
using Edata.ServicePingController;
using Edata.SiteControllerManager;
using Edata.SystemInformation;


namespace SiteControllerCore
{
    /// <summary>
    /// The class provides to manage SiteController service. Purpose of the class is take control for start or stop tasks with interface disposable.
    /// </summary>
    public class SiteControllerManager : IDisposable
    {
        private bool m_bDisposed;
        //private PingController m_xPingController = null;
        private SiteController m_xSiteController;
        private SocketServer<SiteController> m_xSocketServer;
        private MasterSiteCheckUpControl m_SitesCheckUpControl;
        private DisasterSiteCheckUpControl m_xDisasterSiteCheckUpControl;
        //private SystemInformationController m_xSystemInformationController;

        /// <summary>
        /// It's constructor of service main. Make creating initial stiuation.
        /// It has created "SiteController" protocol class.
        /// IF has not any error, It has opened a socket server with "SiteController" protocol and its specified port. Number of port must get from database.
        /// </summary>
        public SiteControllerManager()
        {
            m_bDisposed = false;
            //m_xPingController = new PingController();
            //m_xSystemInformationController = new SystemInformationController();

            m_xSiteController = new SiteController();
            m_xSocketServer = new SocketServer<SiteController>(m_xSiteController, Parameters.xGetInstance().iGetParameter("DispatcherPort", "10017")); // Veri tabanından port numarası alınması gerekiyor.

            m_SitesCheckUpControl = new MasterSiteCheckUpControl();
            m_xDisasterSiteCheckUpControl = new DisasterSiteCheckUpControl();
        }

        /// <summary>
        /// The method is using to dispose tasks.It will be killed all objects these don't have referance via Garbage Collector(GC).
        /// </summary>
        public void Dispose()
        {
            // stop the service
            bStop();

            // dispose objects and variables
            Dispose(true);

            // Call SupressFinalize in case a subclass implements a finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  The method manage to dispose tasks with conditions. Socket server and the service equal to null.
        /// </summary>
        /// <param name="prm_bDisposing">Parameter using to decide for dispose tasks.</param>
        protected virtual void Dispose(bool prm_bDisposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (m_bDisposed == false)
            {
                if (prm_bDisposing == true)
                {
                   // m_xPingController.Dispose();
                   // m_xSystemInformationController.Dispose();
                    // Dispose disposable objects                    
                }

                // null the disposable objects
                m_xSiteController = null;
                m_xSocketServer = null;

                // Indicate that the instance has been disposed.
                m_bDisposed = true;
            }
        }

        /// <summary>
        /// The method is using for start SiteControllerManager service. And also the method provide to service remind itself to system for is live or isn't live in time period.
        /// Start socket server that was created.
        /// </summary>
        /// <returns>Return boolean value. If start server, will return 'True'. Started server.</returns>
        public bool bStart()
        {
            m_SitesCheckUpControl.vStartThread();
            m_xDisasterSiteCheckUpControl.vStartThread();
           // m_xPingController.bStart();
            //m_xSystemInformationController.bStart();
            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "SiteController Manager will be started...");

            return m_xSocketServer.bStartServer();
        }

        /// <summary>
        /// The method is using for stop to the service. If stop service, also can't make to remind itself to system.And socket server will be close.
        /// </summary>
        /// <returns>Return socket server make stop and return true.</returns>
        public bool bStop()
        {
            if (m_SitesCheckUpControl != null)
            {
                m_SitesCheckUpControl.vStopThread();
                m_SitesCheckUpControl = null;
            }

            if (m_xDisasterSiteCheckUpControl != null)
            {
                m_xDisasterSiteCheckUpControl.vStopThread();
                m_xDisasterSiteCheckUpControl = null;
            }

            //m_xPingController.bStop();
            //m_xSystemInformationController.bStop();
            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "SiteController Manager will be stopped...");

            return m_xSocketServer.bStopServer();
        }
    }
}

