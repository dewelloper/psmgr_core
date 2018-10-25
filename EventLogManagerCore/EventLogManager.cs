using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Edata.CommonLibrary;
using Edata.EdipV2;
using Edata.TcpIpSocket;
using Edata.DataTypes.DataSets;
using Edata.DataTypes;
using Edata.DataAccessLayer;
using System.Timers;
using Edata.SystemInformation;

namespace EventLogManagerCore
{
    /// <summary>
    /// The class provides to manage event logs. Purpose of the class is take control for start or stop tasks with interface disposable.
    /// </summary>
    public class EventLogManager
    {
        private bool m_bDisposed;
        private EdipEventLogManager m_xEdipEventLogManager;
        private SocketServer<EdipEventLogManager> m_xSocketServer;
        private SystemInformationController m_xSystemInformationController;
        Timer m_tmLastTime = new Timer();

        /// <summary>
        /// It's constructor of service main. Make creating initial stiuation.
        /// It has created "Event Log" protocol class.
        /// It also has opened a socket server with "Event Log" protocol and its specified port. Number of port must get from database.
        /// </summary>
        public EventLogManager()
        {
            m_bDisposed = false;
            m_xEdipEventLogManager = new EdipEventLogManager();
            m_xSystemInformationController = new SystemInformationController();

            int iErrorCode = -1;
            SqlSelectDataset.DeviceManagerSelectRow dr_EventLogManagerSelectRow = DbFunctionDAO.xGetDeviceManagerIpPort("EventLogManager",ref iErrorCode);

            if (iErrorCode == -1)
            {
                int iPortNo = 9998;
                if (Parameters.xGetInstance().iGetParameter("EventLogManagerNo", "1") == 1)
                    iPortNo = int.Parse(dr_EventLogManagerSelectRow.PORT1);
                else
                    iPortNo = int.Parse(dr_EventLogManagerSelectRow.PORT2);

                Trace.vInsertMethodTrace(enumTraceLevel.Necessary, "Event Log Manager will be started at port no {0}", iPortNo);

                m_xSocketServer = new SocketServer<EdipEventLogManager>(m_xEdipEventLogManager, iPortNo);
            }
            else
            {
                Trace.vInsertError(new Exception("Event Log Manager GetEventLogManagerIpPort Error:" + iErrorCode));
            }
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
        /// The method manage to dispose tasks with conditions. Socket server and the service equal to null.
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
                    m_xSystemInformationController.Dispose();
                }

                // null the disposable objects
                m_xEdipEventLogManager = null;
                m_xSocketServer = null;

                // Indicate that the instance has been disposed.
                m_bDisposed = true;
            }
        }

        /// <summary>
        /// The method is using for start "Event Log" service. And also the method provide to service remind itself to system for is live or isn't live in time period.
        /// Start socket server that was created.
        /// </summary>
        /// <returns>Return boolean value. If start server, will return 'True'. Started server.</returns>
        public bool bStart()
        {
            m_tmLastTime.Elapsed += new ElapsedEventHandler(m_tmLastTime_Elapsed);
            m_tmLastTime.AutoReset = true;
            m_tmLastTime.Interval = Parameters.xGetInstance().iGetParameter("PingUpdateInterval", "30000");
            m_tmLastTime.Start();

            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Event Log Manager will be started...");
            m_xSystemInformationController.bStart();

            return m_xSocketServer.bStartServer(); 
        }

        /// <summary>
        /// he method is using for update ping time information of application. If has any problem, the error can looking up via trace method. 
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event arguments. </param>
        void m_tmLastTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            int iErrorCode = 0;
            Edata.DataAccessLayer.DbFunctionDAO.bUpdateAppInformationPingTime(ref iErrorCode);
        }

        /// <summary>
        /// The method is using for stop to the service. If stop service, also can't make to remind itself to system.And socket server will be close.
        /// </summary>
        /// <returns>Return socket server make stop and return true.</returns>
        public bool bStop()
        {
            m_tmLastTime.Stop();

            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Event Log Manager will be stopped...");
            m_xSystemInformationController.bStop();
            return m_xSocketServer.bStopServer();
        }
    }
}
