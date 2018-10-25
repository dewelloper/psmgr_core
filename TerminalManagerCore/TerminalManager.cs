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
using System.Timers;

namespace TerminalManagerCore
{
    /// <summary>
    /// The class provides to manage TerminalManager service. Purpose of the class is take control for start or stop tasks with interface disposable.
    /// </summary>
    public class TerminalManager : IDisposable
    {
        private bool m_bDisposed;
        private EdipTerminalManager m_xEdipTerminalManager;
        private SocketServer<EdipTerminalManager> m_xSocketServer;
        Timer m_tmLastTime = new Timer();

        /// <summary>
        /// It's constructor of service main. Make creating initial stiuation.
        /// It has created "TerminalManager" protocol class.
        /// IF has not any error, It has opened a socket server with "TerminalManager" protocol and its specified port. Number of port must get from database.
        /// </summary>
        public TerminalManager()
        {
            m_bDisposed = false;
            m_xEdipTerminalManager = new EdipTerminalManager();

            int iErrorCode = -1;
            SqlSelectDataset.DeviceManagerSelectRow dr_TerminalManagerSelectRow = DbFunctionDAO.xGetDeviceManagerIpPort("TerminalManager", ref iErrorCode);

            if (iErrorCode == -1)
            {
                int iPortNo = 9999;
                if (Parameters.xGetInstance().iGetParameter("TerminalManagerNo", "1") == 1)
                    iPortNo = int.Parse(dr_TerminalManagerSelectRow.PORT1);
                else
                    iPortNo = int.Parse(dr_TerminalManagerSelectRow.PORT2);

                Trace.vInsertMethodTrace(enumTraceLevel.Necessary, "Terminal Manager will be started at port no {0}", iPortNo);

                m_xSocketServer = new SocketServer<EdipTerminalManager>(m_xEdipTerminalManager, iPortNo);
            }
            else
            {
                Trace.vInsertError(new Exception("Terminal Manager GetTerminalManagerIpPort Error:" + iErrorCode));
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
                }

                // null the disposable objects
                m_xEdipTerminalManager = null;
                m_xSocketServer = null;

                // Indicate that the instance has been disposed.
                m_bDisposed = true;
            }
        }

        /// <summary>
        /// The method is using for start TerminalManager service. And also the method provide to service remind itself to system for is live or isn't live in time period.
        /// Start socket server that was created.
        /// </summary>
        /// <returns>Return boolean value. If start server, will return 'True'. Started server.</returns>
        public bool bStart()
        {
            m_tmLastTime.Elapsed += new ElapsedEventHandler(m_tmLastTime_Elapsed);
            m_tmLastTime.AutoReset = true;
            m_tmLastTime.Interval = Parameters.xGetInstance().iGetParameter("PingUpdateInterval", "30000");
            m_tmLastTime.Start();

            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Terminal Manager will be started...");
            return m_xSocketServer.bStartServer();
        }

        /// <summary>
        /// The method is using for update ping time information of application. If has any problem, the error can looking up via trace method. 
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

            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Terminal Manager will be stopped...");
            return m_xSocketServer.bStopServer();
        }
    }
}
