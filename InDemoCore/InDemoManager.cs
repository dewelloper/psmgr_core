using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Edata.Protocol;
using Edata.CommonLibrary;
using Edata.TcpIpSocket;
using Edata.TcpIpClient;
using Edata.InDemo;
using Edata.ServicePingController;
using Edata.SystemInformation;

namespace InDemoCore
{
    public class InDemoManager: IDisposable
    {
        private bool m_bDisposed;
        private InDemoDispatcher m_xInDemoDispatcher;
        private SocketServer<InDemoDispatcher> m_xSocketServer;
        private PingController m_xPingController = null;
        private SystemInformationController m_xSystemInformationController = null; 

        public InDemoManager()
        {
           m_xPingController = new PingController();
            m_bDisposed = false;
            m_xInDemoDispatcher = new InDemoDispatcher();
            m_xSocketServer = new SocketServer<InDemoDispatcher>(m_xInDemoDispatcher, Parameters.xGetInstance().iGetParameter("DispatcherPort", "10010")); // Veri tabanından port numarası alınması gerekiyor.
            m_xSystemInformationController = new SystemInformationController();
        }

        public void Dispose()
        {
            bStop();

            Dispose(true);            

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool prm_bDisposing)
        {
            if (m_bDisposed == false)
            {
                if (prm_bDisposing == true)
                {
                    m_xPingController.Dispose();
                    m_xSystemInformationController.Dispose();    
                }

                m_xSocketServer = null;

                m_xInDemoDispatcher = null;     
                m_bDisposed = true;
            }
        }

        public bool bStart()
        {
            m_xPingController.bStart();
            m_xSystemInformationController.bStart();
            return m_xSocketServer.bStartServer();
        }

        public bool bStop()
        {
            m_xPingController.bStop();
            m_xSystemInformationController.bStop();
            return m_xSocketServer.bStopServer();
        }
    }
}
