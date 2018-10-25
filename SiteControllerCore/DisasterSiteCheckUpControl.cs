using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.Timers;
using System.ServiceProcess;
using System.Net;


using Edata.EdipV2;
using Edata.DataAccessLayer;
using Edata.Edip;
using Edata.DataTypes.DataSets;
using Edata.TcpIpClient;
using Edata.DataTypes;
using Edata.CommonLibrary;


namespace SiteControllerCore
{
    public class DisasterSiteCheckUpControl
    {
        private static int m_iCheckUpInterval = 0;
        public Thread m_xMasterSiteCheckUpRefreshThread { get; set; }
        public Thread m_xDisasterSiteCheckUpRefreshThread { get; set; }
        private string m_strDisasterSiteStatus = "";
        private Edata.EdipV2.Builder m_xEDIPBuilder = null;
        SocketClient m_xSocketClient = null;
        EdipV2Dataset.PacketInfoV2Row m_xPacketInfoWrapper = null;
        EdipV2Dataset m_xEdipV2Dataset = null;


        public DisasterSiteCheckUpControl()
        {
        }

        public void vSendCheckUpMessageToDisasterSiteManager()
        {

            byte[] baIncommingMessage = null;
            int iErrorCode = -1;
           // SqlSelectDataset.SiteManagerSelectRow drSiteManagerSelectRow = Edata.DataAccessLayer.DbFunctionDAO.xGetServicesIpPort("DisasterSiteManager", ref iErrorCode);

            while (true)
            {
                try
                {
                    m_xEDIPBuilder = new Edata.EdipV2.Builder();
                    m_iCheckUpInterval = Parameters.xGetInstance().iGetParameter("CheckUp", "5000");

                    baIncommingMessage = baSendCheckUpMessageToDisasterSiteManager(Parameters.xGetInstance().strGetParameter("DisasterSiteManagerIp", ""), Parameters.xGetInstance().strGetParameter("DisasterSiteManagerPort", ""));
                    if (baIncommingMessage != null)
                    {
                        m_xEdipV2Dataset = Edata.EdipV2.Parser.xGetInstance().ParseMessage(baIncommingMessage, baIncommingMessage.Length);
                        m_xPacketInfoWrapper = m_xEdipV2Dataset.PacketInfoV2[0];

                        if (m_xPacketInfoWrapper.bPacketIsSuccessfullyParsed == true)//GELEN VERİ PAKETİ SAĞLAMSA
                        {
                            EdipV2Dataset.CheckUpResponseDataRow xCheckUpResponseDataRow = m_xEdipV2Dataset.CheckUpResponseData[0];
                            m_strDisasterSiteStatus = xCheckUpResponseDataRow.SITE2_STATUS;
                            Parameters.xGetInstance().bSetParameter("DisasterSiteStatus", m_strDisasterSiteStatus);
                        }
                    }
                    Thread.Sleep(m_iCheckUpInterval);
                }
                catch (Exception xException)
                {
                    xException.TraceError();
                    return;
                }
            }
        }

        private byte[] baSendCheckUpMessageToDisasterSiteManager(string prm_strIp,string prm_strPort)
        {

            byte[] baIncommingMessage = null;
            byte[] baOutgoingMessage = null;
       

            try
            {
                m_xSocketClient = new SocketClient();
                EdipV2Dataset.CheckUpDataDataTable xCheckUpData = new EdipV2Dataset.CheckUpDataDataTable();
                xCheckUpData.AddCheckUpDataRow("6", DateTime.Now.ToString("ddMMyyhhmm"), "A");
                baOutgoingMessage = m_xEDIPBuilder.baBuildCheckUp(DateTime.Now, "TEST00000000", 0, xCheckUpData);

                baIncommingMessage = m_xSocketClient.baSendReceiveData(prm_strIp, int.Parse(prm_strPort), baOutgoingMessage);

                return baIncommingMessage;
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return null;
            }
        }

        public bool bCheckForInternetConnection(string prm_strWebAddress)
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(prm_strWebAddress))
                {
                    return true;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return false;
            }
        }

        public void vStartThread()
        {
            m_xDisasterSiteCheckUpRefreshThread = new Thread(new ThreadStart(vSendCheckUpMessageToDisasterSiteManager));
            m_xDisasterSiteCheckUpRefreshThread.Start();
        }

        public void vStopThread()
        {
            if (m_xDisasterSiteCheckUpRefreshThread != null) m_xDisasterSiteCheckUpRefreshThread.Abort();
            m_xDisasterSiteCheckUpRefreshThread = null;
        }
    }
}
