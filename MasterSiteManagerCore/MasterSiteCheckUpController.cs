using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.ServiceProcess;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using Microsoft.Win32;

using Edata.DataAccessLayer;
using Edata.Edip;
using Edata.EdipV2;
using Edata.DataTypes.DataSets;
using Edata.TcpIpClient;
using Edata.DataTypes;
using Edata.CommonLibrary;
using Edata.GMP;
using Edata.TLV;

namespace MasterSiteManagerCore
{
    public class MasterSiteCheckUpController
    {
        private static int m_iCheckUpInterval = 0;
        private static int m_iCheckUpIntervalForAllApp = 0;
        private int m_iSiteChangeIntervalMinute = 0;
        public Thread m_xCheckUpMessageRefreshThread { get; set; }
        public Thread m_xCheckUpMessageForAllDispatcherRefreshThread { get; set; }
        private TLVParserV2 xTLVParser = new Edata.TLV.TLVParserV2();
        private Edata.EdipV2.Builder m_xEDIPBuilder = null;
        private Edata.GMP.GMPBuilder m_xGMPBuilder = null;
        private string prm_strGmpListenerStatus = null;
        SocketClient m_xSocketClient = null;
        EdipV2Dataset m_xEdipV2Dataset = null;
        private GMPDataSet dsReturnGMPPacketDataSet = new GMPDataSet();
        EdipV2Dataset.PacketInfoV2Row m_xPacketInfoWrapper = null;
        private Dictionary<string, X509Certificate2> m_xCertificateDictionary = new Dictionary<string, X509Certificate2>();
        private X509Certificate2 m_xCertificateTSM = null;
        string strDeviceSerialNumber = "TEST00000000";
        private string m_strmesageTypeTag = String.Empty;

        public MasterSiteCheckUpController()
        {
            m_xCertificateDictionary.Clear();
            m_xCertificateTSM = xLoadCertificates(Parameters.xGetInstance().strGetParameter("CertificateTSM", "rad.pfx"), Parameters.xGetInstance().strGetParameter("CertificateTSM_Password", "123123"));
            m_xCertificateDictionary.Add("CertificateTSM", m_xCertificateTSM);
        }

        public X509Certificate2 xLoadCertificates(string prm_strPath, string prm_strCertificatePassword)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string strSystemCertificatePath = Parameters.xGetInstance().strGetParameter("SystemCertificatePath", "C:\\Edata\\SystemCertificates\\");
            X509Certificate2 xCertificate = new X509Certificate2(strSystemCertificatePath + prm_strPath, prm_strCertificatePassword, X509KeyStorageFlags.Exportable);            
            Trace.vInsertMethodTrace(enumTraceLevel.Normal, "Certificate Loaded" + prm_strPath);
            return xCertificate;
        }

        public void vSendCheckUpMessageToGmpListener()
        {
            int iErrorCode = -1;
            byte[] byteaIncommingMessage = null;
            //SqlSelectDataset.DeviceManagerSelectRow drEdipListenerSelectRow = DbFunctionDAO.xGetDeviceManagerIpPort("EdipListener", ref iErrorCode);

            while (true)
            {
                try
                {
                    m_iSiteChangeIntervalMinute = Parameters.xGetInstance().iGetParameter("SiteChangeIntervalMinute", "3");
                    m_iCheckUpInterval = Parameters.xGetInstance().iGetParameter("CheckUp", "");

                    //byteaIncommingMessage = baSendCheckUpMessageToEdipListener(drEdipListenerSelectRow.IP1,drEdipListenerSelectRow.PORT1);
                    byteaIncommingMessage = baSendCheckUpMessageToGmpListener(Parameters.xGetInstance().strGetParameter("GmpListenerIp", ""), Parameters.xGetInstance().strGetParameter("GmpListenerPort", ""));

                    //Gmp listener çöktüğü durumda ,EdipListener çöktüğü durumlarda

                    if ((byteaIncommingMessage == null && Parameters.xGetInstance().strGetParameter("GmpListenerIp", "") != "" && Parameters.xGetInstance().strGetParameter("GmpListenerPort", "") != "" ) || (byteaIncommingMessage.Length == 0 && Parameters.xGetInstance().strGetParameter("GmpListenerIp", "") != "" && Parameters.xGetInstance().strGetParameter("GmpListenerPort", "") != ""))
                    {
                        if (DateTime.Parse(Parameters.xGetInstance().strGetParameter("GmpListenerLastCheckUpDateTime", DateTime.Now.ToString())).AddMinutes(m_iSiteChangeIntervalMinute) < DateTime.Now)
                        {
                            Parameters.xGetInstance().bSetParameter("GmpListenerStatus", "P");
                            Parameters.xGetInstance().bSetParameter("MasterSiteInActivationDateTime", DateTime.Now.ToString());
                            Trace.vInsertMethodTrace(enumTraceLevel.Important, "MasterSiteStatus was passive at {0}", DateTime.Now);
                        }
                    }
              
                        //GmpListener Ve EdipListener Ayaktayken
                    else if (byteaIncommingMessage != null && byteaIncommingMessage.Length > 0)
                    {

                        GMPPacketStructureParser xGMPPacketStructureParser = new GMPPacketStructureParser(byteaIncommingMessage, byteaIncommingMessage.Length);
                        xTLVParser.Parse(xGMPPacketStructureParser.baGMPMessageData);
                        TLVDataObject xTlvObject = xTLVParser.m_xResult[0];
                        List<TLVDataObject> ChildList = xTlvObject.xChildren;
                        try
                        {
                            byte[] baTDK = new byte[1];
                            if (strDeviceSerialNumber == "TEST00000000")
                                baTDK = GMPParserV1.xGetInstance().baCopyOfRange(m_xCertificateDictionary.SingleOrDefault(e => e.Key == StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM)).Value.GetPublicKey(), 0, 32);
                            else
                                baTDK = GMPParserV1.xGetInstance().baGetTDK(strDeviceSerialNumber);

                            byte[] DecreptedGibSecurityDataGroup = GMPParserV1.xGetInstance().baDecryptDataWithAesKey(baTDK, xTlvObject.baValue);
                            byte[] DecreptedValue = new byte[DecreptedGibSecurityDataGroup.Length + 3 + Converter.baGetLengthValueAsByte(DecreptedGibSecurityDataGroup.Length).Length];
                            Array.Copy(xTlvObject.baTag, 0, DecreptedValue, 0, xTlvObject.baTag.Length);
                            Array.Copy(Converter.baGetLengthValueAsByte(DecreptedGibSecurityDataGroup.Length), 0, DecreptedValue, xTlvObject.baTag.Length, Converter.baGetLengthValueAsByte(DecreptedGibSecurityDataGroup.Length).Length);
                            Array.Copy(DecreptedGibSecurityDataGroup, 0, DecreptedValue, xTlvObject.baTag.Length + Converter.baGetLengthValueAsByte(DecreptedGibSecurityDataGroup.Length).Length, DecreptedGibSecurityDataGroup.Length);

                            Trace.vInsertMethodTrace(enumTraceLevel.Important, "Enums.GMP_MESSAGE_TYPES GMPParserV1.xGetInstance().xTlvParser.Parse Start:" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                            xTLVParser.Parse(DecreptedValue, TLVParserV2.PARSER_TYPE.PARSE_GMP2);
                            Trace.vInsertMethodTrace(enumTraceLevel.Important, "Enums.GMP_MESSAGE_TYPES GMPParserV1.xGetInstance().xTlvParser.Parse End:" + DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                            List<TLVDataObject> xListTLVDataObject = xTLVParser.m_xResult.SingleOrDefault().xChildren;
                            var xProcessGroupy = xListTLVDataObject.Single(e => e.strTag == StringEnum.GetStringValue(Enums.GMP_EDIP_DATA_GROUP.GMP_EDIP_DATA_GROUP)).xChildren;
                            byte[] baResponsePacketx = null;
                            baResponsePacketx = xProcessGroupy.SingleOrDefault(e => e.strTag == StringEnum.GetStringValue(Enums.GMP_EDIP_DATA_GROUP.GMP_EDIP_RESPONSE)).baValue;
                            m_xEdipV2Dataset = Edata.EdipV2.Parser.xGetInstance().ParseMessage(baResponsePacketx, baResponsePacketx.Length);
                            EdipV2Dataset.CheckUpResponseDataRow xCheckUpResponseDataRow;
                            xCheckUpResponseDataRow = m_xEdipV2Dataset.CheckUpResponseData[0];
                            // prm_strGmpListenerStatus = xCheckUpResponseDataRow.SITE1_STATUS.ToString();
                            //  Parameters.xGetInstance().bSetParameter("GMPListenerStatus", prm_strGmpListenerStatus);
                            Parameters.xGetInstance().bSetParameter("GmpListenerLastCheckUpDateTime", DateTime.Now.ToString());

                        }
                        catch (Exception exc)
                        {
                            exc.TraceError("bGMPParseMessageV1_2", 2);

                        }


                    }

                    Thread.Sleep(m_iCheckUpInterval);
                }
                catch (Exception xException)
                {
                    xException.TraceError();
                   
                }
            }
        }


        //public void vSendCheckUpMessageToAllDispatcher()
        //{
        //    byte[] baIncommingMessage = null;
        //    byte[] prm_baOutgoingMessage = null;

        //    while (true)
        //    {
        //        SqlSelectDataset.DispatcherSelectRow drdispatcherSelectRow = null;
        //        SqlSelectDataset.SiteManagerSelectRow drSiteManagerSelectRow = null;
        //        try
        //        {
        //            int iErrorCode = -1;
        //            m_iCheckUpIntervalForAllApp = Parameters.xGetInstance().iGetParameter("CheckUpForAllDispatcher", "5000");
        //            DataTable dtApplicationNames = DbFunctionDAO.GetApplicationNames(ref iErrorCode);

        //            if (dtApplicationNames != null && dtApplicationNames.Rows.Count > 0)
        //            {

        //                for (int iIndex = 0; iIndex < dtApplicationNames.Rows.Count; iIndex++)
        //                {
        //                    try
        //                    {
        //                        string strApplicationName = dtApplicationNames.Rows[iIndex]["APP_NAME"].ToString();

        //                        if (Convert.ToBoolean(dtApplicationNames.Rows[iIndex]["HAVE_CHECK_UP_MESSAGE_STATISTICS"]) == true)
        //                        {
        //                            m_xEDIPBuilder = new Edata.EdipV2.Builder();
        //                            EdipV2Dataset.CheckUpDataDataTable xCheckUpData = new EdipV2Dataset.CheckUpDataDataTable();
        //                            xCheckUpData.AddCheckUpDataRow("2", DateTime.Now.ToString("ddMMyyhhmm"), "A");
        //                            prm_baOutgoingMessage = m_xEDIPBuilder.baBuildCheckUp(DateTime.Now, "TEST00000000", 0, xCheckUpData);

        //                            #region Uygulama İsimlerine Göre IP Ve Port Bilgilerini Al
        //                            SqlSelectDataset.DeviceManagerSelectRow drDeviceManagerSelectRow = Edata.DataAccessLayer.DbFunctionDAO.xGetDeviceManagerIpPort(strApplicationName, ref iErrorCode);

        //                            if (drDeviceManagerSelectRow["ID"] == System.DBNull.Value)
        //                            {
        //                                var DispatcherName = strApplicationName;
        //                                var resultDispatcherName = string.Concat(DispatcherName.Select(c => char.IsUpper(c) ? " " + c.ToString() : c.ToString())).TrimStart().Replace('i', 'ı');
        //                                drdispatcherSelectRow = Edata.DataAccessLayer.DbFunctionDAO.xGetDispatcherIpPort(resultDispatcherName.ToUpper(), ref iErrorCode);

        //                                if (drdispatcherSelectRow["ID"] == System.DBNull.Value)
        //                                {
        //                                    drSiteManagerSelectRow = Edata.DataAccessLayer.DbFunctionDAO.xGetServicesIpPort(strApplicationName, ref iErrorCode);
        //                                }
        //                            }
        //                            #endregion

        //                            try
        //                            {
        //                                m_xSocketClient = new SocketClient();
        //                                if (drDeviceManagerSelectRow["ID"] != System.DBNull.Value)
        //                                    baIncommingMessage = m_xSocketClient.baSendReceiveData(drDeviceManagerSelectRow.IP1, int.Parse(drDeviceManagerSelectRow.PORT1), prm_baOutgoingMessage);
        //                                else if (drdispatcherSelectRow["ID"] != System.DBNull.Value)
        //                                    baIncommingMessage = m_xSocketClient.baSendReceiveData(drdispatcherSelectRow.IP1, int.Parse(drdispatcherSelectRow.PORT1), prm_baOutgoingMessage);
        //                                else if (drSiteManagerSelectRow["ID"] != System.DBNull.Value)
        //                                    baIncommingMessage = m_xSocketClient.baSendReceiveData(drSiteManagerSelectRow.IP1, int.Parse(drSiteManagerSelectRow.PORT1), prm_baOutgoingMessage);

        //                            }
        //                            catch (Exception xException)
        //                            {
        //                                baIncommingMessage = null;
        //                                xException.TraceError();
        //                                m_xSocketClient.bCloseConnection();
        //                            }

        //                            #region Gelen Cevaba Göre Checkup Response Tarihlerini Güncelle
        //                            if (baIncommingMessage != null)
        //                            {
        //                                m_xEdipV2Dataset = Edata.EdipV2.Parser.xGetInstance().ParseMessage(baIncommingMessage, baIncommingMessage.Length);
        //                                m_xPacketInfoWrapper = m_xEdipV2Dataset.PacketInfoV2[0];

        //                                if (m_xPacketInfoWrapper.bPacketIsSuccessfullyParsed == true)
        //                                {
        //                                    EdipV2Dataset.CheckUpResponseDataRow xCheckUpResponseDataRow = m_xEdipV2Dataset.CheckUpResponseData[0];
        //                                    if (xCheckUpResponseDataRow.SERVER_DATE_TIME != null)
        //                                        Edata.DataAccessLayer.DbFunctionDAO.bUpdateCheckUpResponseDateTime(strApplicationName, ref iErrorCode);
        //                                }
        //                            }
        //                            #endregion
        //                        }

        //                    }
        //                    catch (Exception xException)
        //                    {
        //                        xException.TraceError();
        //                    }
        //                    Thread.Sleep(10);
        //                }
        //            }
        //        }
        //        catch (Exception xException)
        //        {
        //            xException.TraceError();
        //            return;
        //        }
        //        Thread.Sleep(m_iCheckUpIntervalForAllApp);
        //    }
        //}

        private byte[] baSendCheckUpMessageToGmpListener(string prm_strIp, string prm_strPort)
        {
            byte[] baEdipMessage = null;
            byte[] baIncommingMessage;
            string strDeviceSerialNumber = "TEST00000000";

            try
            {
                m_xEDIPBuilder = new Edata.EdipV2.Builder();
                m_xSocketClient = new SocketClient();
                EdipV2Dataset.CheckUpDataDataTable xCheckUpData = new EdipV2Dataset.CheckUpDataDataTable();
                xCheckUpData.AddCheckUpDataRow("2", DateTime.Now.ToString("ddMMyyhhmm"), Parameters.xGetInstance().strGetParameter("GmpListenerStatus", "A"));
                baEdipMessage = m_xEDIPBuilder.baBuildCheckUp(DateTime.Now, strDeviceSerialNumber, 0, xCheckUpData);
                byte[] baGmpRequestMessage = new byte[1];
                iCreateGmpPacket(strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, baEdipMessage, ref baGmpRequestMessage);
                baIncommingMessage = m_xSocketClient.baSendReceiveData(prm_strIp, int.Parse(prm_strPort), baGmpRequestMessage);
               
                    return baIncommingMessage;
                
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return null;
            }
        }

        public int iCreateGmpPacket(string prm_strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES prm_GMP_MESSAGE_TYPE, byte[] baRequestMessage, ref byte[] baGmpRequestMessage)
        {
            byte[] xTLVMessage = null;
            GMPParserV1 xGMPParserV1 = new GMPParserV1();
            byte[] prm_baIV = xGMPParserV1.baCalculateIV(Converter.StrToHexByte("0"));
            Edata.TLV.TLVParserV2 xTLVParserV2 = new Edata.TLV.TLVParserV2();

            TLVBaseObject xBase = new TLVBaseObject();
            xBase.strParentTag = StringEnum.GetStringValue(Enums.GMP_EDIP_DATA_GROUP.GMP_EDIP_DATA_GROUP);
            xBase.strTag = StringEnum.GetStringValue(Enums.GMP_EDIP_DATA_GROUP.GMP_EDIP_REQUEST);
            xBase.baValue = baRequestMessage;
            xBase.baLength = Converter.baGetLengthValueAsByte(xBase.baValue.Length);//Converter.HexToByte(strHexBaseLength);

            List<TLVBaseObject> xTlvBaseObjectList = new List<TLVBaseObject>();
            xTlvBaseObjectList.Add(xBase);
            TLVBuilder xTlvBuilder = new TLVBuilder();
            xTLVMessage = xTlvBuilder.baCreateV1(prm_GMP_MESSAGE_TYPE, xTlvBaseObjectList.ToArray<TLVBaseObject>());
            byte[] baMessage = new byte[1];
            Array.Resize<byte>(ref baMessage, xTLVMessage.Length - (xTLVMessage.Length > 255 ? 6 : 4));
            Array.Copy(xTLVMessage, (xTLVMessage.Length > 255 ? 6 : 4), baMessage, 0, xTLVMessage.Length - (xTLVMessage.Length > 255 ? 6 : 4));
            Byte[] baTRMK = GMPParserV1.xGetInstance().baCopyOfRange(m_xCertificateDictionary.SingleOrDefault(e => e.Key == StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM)).Value.GetPublicKey(), 0, 32);
            byte[] baEncryptedData = xGMPParserV1.baSignDataWithAesKeyWithoutIV(baTRMK, baMessage);
            byte[] baEncryptedDataLength = Converter.baGetLengthValueAsByte(baEncryptedData.Length);

            Array.Resize<byte>(ref xTLVMessage, baEncryptedData.Length + (xTLVMessage.Length > 255 ? 6 : 4));
            Array.Copy(baEncryptedDataLength, 0, xTLVMessage, 3, baEncryptedDataLength.Length);
            Array.Copy(baEncryptedData, 0, xTLVMessage, (xTLVMessage.Length > 255 ? 6 : 4), baEncryptedData.Length);

            baGmpRequestMessage = GMPBuilder.xGetInstance().baCreatePacket(prm_GMP_MESSAGE_TYPE, prm_strDeviceSerialNumber, "6000000001", xTLVParserV2, m_xCertificateDictionary, xTLVMessage);

            return 0;
        }
      
        public void vStartThread()
        {
            m_xCheckUpMessageRefreshThread = new Thread(new ThreadStart(vSendCheckUpMessageToGmpListener));
            m_xCheckUpMessageRefreshThread.Start();
            //m_xCheckUpMessageForAllDispatcherRefreshThread = new Thread(new ThreadStart(vSendCheckUpMessageToAllDispatcher));
            //m_xCheckUpMessageForAllDispatcherRefreshThread.Start();
        }

        public void vStopThread()
        {
            if (m_xCheckUpMessageRefreshThread != null) m_xCheckUpMessageRefreshThread.Abort();
            m_xCheckUpMessageRefreshThread = null;

            //if (m_xCheckUpMessageForAllDispatcherRefreshThread != null) m_xCheckUpMessageForAllDispatcherRefreshThread.Abort();
            //m_xCheckUpMessageForAllDispatcherRefreshThread = null;
        }


    }
}
