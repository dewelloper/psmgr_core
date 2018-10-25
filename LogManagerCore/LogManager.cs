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
using Edata.LogManager;
using Edata.SystemInformation;

namespace LogManagerCore
{
    /// <summary>
    /// The class provides to manage LogManager service. Purpose of the class is take control for start or stop tasks with interface disposable.
    /// </summary>
    public class LogManager : IDisposable
    {
        private bool m_bDisposed;
        private SocketServer<LogListener> m_xSocketServer;
        private LogListener m_xLogListener;

        private SystemInformationController m_xSystemInformationController;
        private PingController m_xPingController = null;

        /// <summary>
        /// It's constructor of service main. Make creating initial stiuation.
        /// It has created "LogManager" protocol class.
        /// IF has not any error, It has opened a socket server with "LogManager" protocol and its specified port. Number of port must get from database.
        /// </summary>
        public LogManager()
        {
            m_xPingController = new PingController();
            m_bDisposed = false;
            m_xLogListener = new LogListener();
            m_xSystemInformationController = new SystemInformationController();
            m_xSocketServer = new SocketServer<LogListener>(m_xLogListener, Parameters.xGetInstance().iGetParameter("ListenerPort", "10028")); // Veri tabanından port numarası alınması gerekiyor.
            
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
                    // Dispose disposable objects    
                    m_xPingController.Dispose();
                    m_xSystemInformationController.Dispose();
                }

                // null the disposable objects
                m_xLogListener = null;
                m_xSocketServer = null;

                // Indicate that the instance has been disposed.
                m_bDisposed = true;
            }
        }

        /// <summary>
        /// The method is using for start LogManager service. And also the method provide to service remind itself to system for is live or isn't live in time period.
        /// Start socket server that was created.
        /// </summary>
        /// <returns>Return boolean value. If start server, will return 'True'. Started server.</returns>
        public bool bStart()
        {
            m_xPingController.bStart();
            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Log Manager will be started...");
            m_xSystemInformationController.bStart();
            return m_xSocketServer.bStartServer();
        }

        /// <summary>
        /// The method is using for stop to the service. If stop service, also can't make to remind itself to system.And socket server will be close.
        /// </summary>
        /// <returns>Return socket server make stop and return true.</returns>
        public bool bStop()   
        {
            m_xPingController.bStop();
            m_xSystemInformationController.bStop();
            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Log Manager will be stopped...");
            return m_xSocketServer.bStopServer();
        }
    }
}

