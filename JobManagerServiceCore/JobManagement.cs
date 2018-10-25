using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Edata.CommonLibrary;
using Edata.TcpIpSocket;
using Edata.SystemInformation;

namespace JobManagerServiceCore
{
    public class JobManagement : IDisposable
    {
        private bool m_bDisposed;
        private JobManager m_xJobManager;
        private SocketServer<JobManager> m_xSocketServer;
        Timer m_xPingTimer = new Timer();
        ScheduledTasks m_xScheduleTasks = new ScheduledTasks();
        private SystemInformationController m_xSystemInformationController = null;

        public JobManagement()
        {
            m_xSystemInformationController = new SystemInformationController();
            m_bDisposed = false;
            m_xJobManager = new JobManager();
            m_xSocketServer = new SocketServer<JobManager>(m_xJobManager, Parameters.xGetInstance().iGetParameter("ServicePort", "10022")); // Veri tabanından port numarası alınması gerekiyor.
        }

        public bool bStart()
        {
            m_xSystemInformationController.bStart();
            m_xPingTimer.Elapsed += new ElapsedEventHandler(vPingTimer_Elapsed);
            m_xPingTimer.AutoReset = true;
            m_xPingTimer.Interval = Parameters.xGetInstance().iGetParameter("PingUpdateInterval", "30000");
            m_xPingTimer.Start();
            m_xScheduleTasks.vStartThread();
            m_xSocketServer.bStartServer();
            return true;
        }

        public bool bStop()
        {
            m_xSystemInformationController.bStop();
            m_xScheduleTasks.bKillThreads(); 
            m_xPingTimer.Stop();
            m_xSocketServer.bStopServer();
            return true;
        }

        public void Dispose()
        {
            bStop();
            m_xPingTimer.Dispose();
            Dispose(true);
            m_xJobManager = null;
            m_xSocketServer = null;
            // Call SupressFinalize in case a subclass implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool prm_bDisposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (m_bDisposed == false)
            {
                if (prm_bDisposing == true)
                {
                    m_xSystemInformationController.Dispose();
                    // Dispose disposable objects
                }

                // null the disposable objects
                m_xScheduleTasks = null;

                // Indicate that the instance has been disposed.
                m_bDisposed = true;
            }
        }

        void vPingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int iErrorCode = 0;
            Edata.DataAccessLayer.DbFunctionDAO.bUpdateAppInformationPingTime(ref iErrorCode);
        }
    }
}
