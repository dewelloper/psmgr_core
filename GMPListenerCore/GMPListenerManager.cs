/****************************************************************************************
 * Class		: GMP Listener Manager
 * Purpose		: Main form of GMP Listener debuging application
 * Create Date  : 08/04/2014 By Eylem Artut (BeterBocek)
 * Modify Date  : 

 Copyright(c) 2013 By Edata Electronics
 All Rights Reserved
 
 * Modifiying History
 * 08/04/2014 By Eylem Artut (BeterBocek)- Create Initial Class
 *****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;


using Edata.CommonLibrary;
using Edata.TcpIpSocket;
using Edata.TcpIpClient;
using Edata.GMP;
using Edata.DataAccessLayer;
using Edata.SecureTCP;

namespace GMPListenerCore
{
    /// <summary>
    /// The class provides to manage GMPListener service. Purpose of the class is take control for start or stop tasks with interface disposable.
    /// </summary>
    public class GMPListenerManager : IDisposable
    {
        private bool m_bDisposed;
        private GMPListener m_xGMPListener;
        private SocketServer <GMPListener> m_xSSLServer;
        Timer m_tmLastTime = new Timer();

        /// <summary>
        /// It's constructor of service main. Make creating initial stiuation.
        /// It has created "GMPListener" protocol class.
        /// It also has opened a socket server with "GMPListener" protocol and its specified port. Number of port must get from database.
        /// </summary>
        public GMPListenerManager()
        {
            m_bDisposed = false;
            m_xGMPListener = new GMPListener();// Veri tabanından port numarası alınması gerekiyor.
            m_xGMPListener.ReadCertificates();//Load all certificates
        }

        /// <summary>
        /// The method is using to dispose tasks.It will be killed all objects these don't have referance via Garbage Collector(GC).
        /// </summary>
        public void Dispose()
        {
            bStop();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The method manage to dispose tasks with conditions. Socket server and the service equal to null.
        /// </summary>
        /// <param name="prm_bDisposing">Parameter using to decide for dispose tasks.</param>
        protected virtual void Dispose(bool prm_bDisposing)
        {
            if (m_bDisposed == false)
            {
                if (prm_bDisposing == true)
                {}

                m_xGMPListener = null;
                m_xSSLServer = null;

                m_bDisposed = true;
            }
        }

        /// <summary>
        /// The method is using for start GMPListener service. And also the method provide to service remind itself to system for is live or isn't live in time period.
        /// Start socket server that was created.
        /// </summary>
        /// <returns>Return boolean value. If start server, will return 'True'. Started server.</returns>
        public bool bStart()
        {
            m_tmLastTime.Elapsed += new ElapsedEventHandler(m_tmLastTime_Elapsed);
            m_tmLastTime.AutoReset = true;
            m_tmLastTime.Interval = Parameters.xGetInstance().iGetParameter("PingUpdateInterval", "30000");
            m_tmLastTime.Start();
            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "GMP Listener will be started...");
            m_xSSLServer = new SocketServer<GMPListener>(m_xGMPListener, Parameters.xGetInstance().iGetParameter("ListenerPort", "2573"),true);
                //Parameters.xGetInstance().iGetParameter("ListenerPort", "2573"), strPath, Parameters.xGetInstance().strGetParameter("ListenerPfxPassword", "1234"));

            return m_xSSLServer.bStartServer();
        }

        /// <summary>
        /// The method is using for stop to the service. If stop service, also can't make to remind itself to system.And socket server will be close.
        /// </summary>
        /// <returns>Return socket server make stop and return true.</returns>
        public bool bStop()
        {
            m_tmLastTime.Stop();

            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "GMP Listener will be stopped...");
            
            return m_xSSLServer.bStopServer();
        }

        /// <summary>
        /// The method is using for update ping time information of application. If has any problem, the error can looking up via trace method. 
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event arguments. </param>
        void m_tmLastTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            int iErrorCode = 0;
            //Edata.DataAccessLayer.DbFunctionDAO.bUpdateAppInformationPingTime(ref iErrorCode);
        }

    }
}
