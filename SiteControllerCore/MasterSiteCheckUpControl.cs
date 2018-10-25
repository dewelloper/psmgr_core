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
    public class MasterSiteCheckUpControl
    {
        private int m_iSiteChangeIntervalMinute;
        private int m_iCheckUpInterval = 0;
        public Thread m_xMasterSiteCheckUpRefreshThread { get; set; }
        public Thread m_xDisasterSiteCheckUpRefreshThread { get; set; }
        private string m_strMasterSiteStatus = "";
        private Edata.EdipV2.Builder m_xEDIPBuilder = null;
        SocketClient m_xSocketClient = null;
        EdipV2Dataset.PacketInfoV2Row m_xPacketInfoWrapper = null;
        EdipV2Dataset m_xEdipV2Dataset = null;


        public MasterSiteCheckUpControl()
        {
        }

        public void vSendCheckUpMessageToMasterSiteManager()
        {

            byte[] baIncommingMessage = null;
            int iErrorCode = -1;
            //SqlSelectDataset.SiteManagerSelectRow drSiteManagerSelectRow = Edata.DataAccessLayer.DbFunctionDAO.xGetServicesIpPort("MasterSiteManager", ref iErrorCode);

            while (true)
            {
                try
                {
                    m_xEDIPBuilder = new Edata.EdipV2.Builder();
                    m_iCheckUpInterval = Parameters.xGetInstance().iGetParameter("CheckUp", "5000");
                    m_iSiteChangeIntervalMinute = Parameters.xGetInstance().iGetParameter("SiteChangeIntervalMinute", "30");

                    baIncommingMessage = baSendCheckUpMessageToMasterSiteManager(Parameters.xGetInstance().strGetParameter("MasterSiteManagerIp", ""), Parameters.xGetInstance().strGetParameter("MasterSiteManagerPort", ""));

                    if (baIncommingMessage == null && Parameters.xGetInstance().strGetParameter("MasterSiteManagerIp", "") != "" && Parameters.xGetInstance().strGetParameter("MasterSiteManagerPort", "")!="")
                    {
                        if (bCheckForInternetConnection("http://www.google.com") == true || bCheckForInternetConnection("https://www.turkiye.gov.tr/") == true)
                            if (DateTime.Parse(Parameters.xGetInstance().strGetParameter("MasterSiteLastCheckUpDateTime", DateTime.Now.ToString())).AddMinutes(m_iSiteChangeIntervalMinute) < DateTime.Now)
                                Parameters.xGetInstance().bSetParameter("MasterSiteStatus", "P");
                    }
                    else if (baIncommingMessage!=null)
                    {
                        m_xEdipV2Dataset = Edata.EdipV2.Parser.xGetInstance().ParseMessage(baIncommingMessage, baIncommingMessage.Length);
                        m_xPacketInfoWrapper = m_xEdipV2Dataset.PacketInfoV2[0];
                        Parameters.xGetInstance().bSetParameter("MasterSiteLastCheckUpDateTime", DateTime.Now.ToString());

                        if (m_xPacketInfoWrapper.bPacketIsSuccessfullyParsed == true)//GELEN VERİ PAKETİ SAĞLAMSA
                        {
                            EdipV2Dataset.CheckUpResponseDataRow xCheckUpResponseDataRow = m_xEdipV2Dataset.CheckUpResponseData[0];
                            m_strMasterSiteStatus = xCheckUpResponseDataRow.SITE1_STATUS;

                              Parameters.xGetInstance().bSetParameter("MasterSiteStatus", m_strMasterSiteStatus);
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

        private byte[] baSendCheckUpMessageToMasterSiteManager(string prm_strIp,string prm_strPort)
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

            m_xMasterSiteCheckUpRefreshThread = new Thread(new ThreadStart(vSendCheckUpMessageToMasterSiteManager));
            m_xMasterSiteCheckUpRefreshThread.Start();
        }

        public void vStopThread()
        {
            if (m_xMasterSiteCheckUpRefreshThread != null) m_xMasterSiteCheckUpRefreshThread.Abort();
            m_xMasterSiteCheckUpRefreshThread = null;
        }
    }
}
