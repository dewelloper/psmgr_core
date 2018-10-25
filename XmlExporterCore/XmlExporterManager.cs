using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Edata.Protocol;
using Edata.CommonLibrary;
using Edata.TcpIpSocket;
using Edata.TcpIpClient;
using Edata.DataAccessLayer;
using Edata.SystemInformation;


namespace XmlExporterCore
{
    public class XmlExporterManager: IDisposable
    {
        private bool m_bDisposed;
        private XmlExporterScheduler m_xXmlExporterScheduler;
        private SystemInformationController m_xSystemInformationController = null; 
        
        Timer m_tmLastTime = new Timer();

        public XmlExporterManager()
        {
            m_bDisposed = false;
            m_xXmlExporterScheduler = new XmlExporterScheduler();
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
                    m_xSystemInformationController.Dispose();          
                }

                m_xXmlExporterScheduler.vStopThread();
                m_xXmlExporterScheduler = null;     
                m_bDisposed = true;
            }
        }

        public bool bStart()
        {
            m_tmLastTime.Elapsed += new ElapsedEventHandler(vLastTimeElapsed);
            m_tmLastTime.AutoReset = true;
            m_tmLastTime.Interval = Parameters.xGetInstance().iGetParameter("PingUpdateInterval", "30000");
            m_tmLastTime.Start();

            m_xXmlExporterScheduler.vStartThread();
            m_xSystemInformationController.bStart();
            return true;
        }


        void vLastTimeElapsed(object sender, ElapsedEventArgs e)
        {
            int iErrorCode = 0;
            Edata.DataAccessLayer.DbFunctionDAO.bUpdateAppInformationPingTime(ref iErrorCode);
            
            
        }

        public bool bStop()
        {
            bool bResult = false;
            try
            {
                if (m_xXmlExporterScheduler != null )
                {
                    m_xXmlExporterScheduler.vStopThread();
                    m_xXmlExporterScheduler = null;
                    m_tmLastTime.Stop();
                    m_xSystemInformationController.bStop();
                    bResult = true;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
                bResult = false;
            }

            return bResult;
        }
    }
}
