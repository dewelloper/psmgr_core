using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Edata.CommonLibrary;
using Edata.TcpIpClient;
using Edata.DataAccessLayer;
using Edata.DataTypes;
using Edata.DataTypes.DataSets;
using Edata.TLV;
using System.Security.Cryptography.X509Certificates;
using Edata.GMP;
using System.IO;

namespace GmpStressTestingTool
{
    public class GmpEcrSimulator
    {
        int m_iPacketSize = 9999;
        string m_strDestinationIPAddress = "213.153.255.102";
        int m_iDestinationPort = 2573;
        int m_iThreadIndex = 0;
        private X509Certificate2 m_xCertificateTSM = null;
        private X509Certificate2 m_xCertificateTSM_SIGN = null;
        private X509Certificate2 m_xCertificateGIB = null;
        private X509Certificate2 m_xCertificateGIB_SIGN = null;
        private X509Certificate2 m_xCertificateECR = null;
        private Dictionary<string, X509Certificate2> m_xCertificateDictionary = new Dictionary<string, X509Certificate2>();

        public EventHandler eventCountersRefresh;

        public GmpEcrSimulator()
        {

        }

        public X509Certificate2 xLoadCertificates(string prm_strPath, string prm_strCertificatePassword)
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            // X509Certificate2 xCertificate = new X509Certificate2(AppPath + "\\" + prm_strPath, prm_strCertificatePassword,  X509KeyStorageFlags.Exportable);
            X509Certificate2 xCertificate = new X509Certificate2(@"c:\Edata\Certificates\" + prm_strPath, prm_strCertificatePassword, X509KeyStorageFlags.Exportable);
            Trace.vInsertMethodTrace(enumTraceLevel.Normal, "Certificate Loaded" + prm_strPath);
            return xCertificate;
        }

        public GmpEcrSimulator(int prm_iThreadIndex, string prm_strDestinationIPAddress, int prm_iDestinationPort, int prm_iPacketSize = 9999)
        {
            m_iThreadIndex = prm_iThreadIndex;
            m_strDestinationIPAddress = prm_strDestinationIPAddress;
            m_iDestinationPort = prm_iDestinationPort;
            m_iPacketSize = prm_iPacketSize;

            m_xCertificateDictionary.Clear();
            m_xCertificateTSM = xLoadCertificates(Parameters.xGetInstance().strGetParameter("CertificateTSM", "rad.pfx"), Parameters.xGetInstance().strGetParameter("CertificateTSM_Password", "123123"));
            m_xCertificateTSM_SIGN = xLoadCertificates(Parameters.xGetInstance().strGetParameter("CertificateTSM_SIGN", "rad.pfx"), Parameters.xGetInstance().strGetParameter("CertificateTSM_SIGN_Password", "123123"));
            m_xCertificateGIB = xLoadCertificates(Parameters.xGetInstance().strGetParameter("CertificateGIB", "rad.pfx"), Parameters.xGetInstance().strGetParameter("CertificateGIB_Password", "123123"));
            m_xCertificateGIB_SIGN = xLoadCertificates(Parameters.xGetInstance().strGetParameter("CertificateGIB_SIGN", "rad.pfx"), Parameters.xGetInstance().strGetParameter("CertificateGIB_SIGN_Password", "123123"));
            m_xCertificateDictionary.Add("CertificateTSM", m_xCertificateTSM);
            m_xCertificateDictionary.Add("CertificateTSM_SIGN", m_xCertificateTSM_SIGN);
            m_xCertificateDictionary.Add("CertificateGIB", m_xCertificateGIB);
            m_xCertificateDictionary.Add("CertificateGIB_SIGN", m_xCertificateGIB_SIGN);
        }

        private byte[] baSendAndReceiveMessage(byte[] prm_baInputString)
        {
            SocketClient xSocketClient = new SocketClient();

            xSocketClient.glb_intSendTimeOut = 4000;
            byte[] baResponseMessage = xSocketClient.baSendReceiveData(m_strDestinationIPAddress, m_iDestinationPort, prm_baInputString);
            
            return baResponseMessage;
        }      

        public int iSendDeviceCertificate(string prm_strDeviceSerialNumber)
        {
            try
            {
                byte[] xTLVMessage = null;
                TLVDataObject xBase = new TLVDataObject();
                xBase.baTag = Converter.HexToByteArray(StringEnum.GetStringValue(Enums.GMP_EDIP_DATA_GROUP.GMP_CERTIFICATE_TRMK));

                byte[] baCertificate = File.ReadAllBytes(string.Format("C:\\Edata\\Certificates\\{0}.pfx", prm_strDeviceSerialNumber));
                Byte[] baTRMK = GMPParserV1.xGetInstance().baCopyOfRange(m_xCertificateDictionary.SingleOrDefault(e => e.Key == StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM)).Value.GetPublicKey(), 0, 32); //baDecryptData(baCertificateTRMK, StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM));
                xBase.baValue = GMPParserV1.xGetInstance().baSignDataWithAesKeyWithoutIV(baTRMK, baCertificate);
                xBase.iLength = xBase.baValue.Length;  
                TLVBuilder xTlvBuilder = new TLVBuilder();
                xTLVMessage = GMPBuilder.xGetInstance().baCreateRequestPacket(xBase, Converter.StrToAscii(prm_strDeviceSerialNumber));
                byte[] baResponseMessage = baSendAndReceiveMessage(xTLVMessage);

                if (baResponseMessage == null)
                    return 1;            

                return 0;
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return -1;
            }
        }

        public int iSendTDKRequest(string prm_strDeviceSerialNumber)
        {
            try
            {
                if (m_xCertificateDictionary.Count == 4)
                {
                    X509Certificate2 m_xCertificateECR = xLoadCertificates(string.Format("{0}.pfx", prm_strDeviceSerialNumber), "456087");
                    m_xCertificateDictionary.Add("CertificateECR", m_xCertificateECR);
                }
                else
                {
                    m_xCertificateDictionary.Remove("CertificateECR");
                    X509Certificate2 m_xCertificateECR = xLoadCertificates(string.Format("{0}.pfx", prm_strDeviceSerialNumber), "456087");
                    m_xCertificateDictionary.Add("CertificateECR", m_xCertificateECR);
                }

                byte[] baGmpRequestMessage = new byte[1];
                iCreateGmpPacket(prm_strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES.REQUEST_TDK_MESSAGE, ref baGmpRequestMessage);
                byte[] baResponseMessage = baSendAndReceiveMessage(baGmpRequestMessage);

                if (baResponseMessage == null)
                    return 1;

                return 0;
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return -1;
            }
        }

        public int iSendTrekTrakRequest(string prm_strDeviceSerialNumber)
        {
            try
            {
                byte[] baGmpRequestMessage = new byte[1];
                iCreateGmpPacket(prm_strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES.REQUEST_PRIMARY_KEY_TERMS_AND_CREATING_DATA_ENCRYPTION, ref baGmpRequestMessage);
                byte[] baResponseMessage = baSendAndReceiveMessage(baGmpRequestMessage);

                if (baResponseMessage == null)
                    return 1;

                return 0;
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return -1;
            }
        }

        public int iSendCheckDevice(string prm_strDeviceSerialNumber)
        {
            try
            {
                int iPacketSize = m_iPacketSize;
                Edata.EdipV2.Builder xEDIPBuilder = new Edata.EdipV2.Builder(iPacketSize);

                EdipV2Dataset.CheckDeviceRequestDataDataTable xCheckDeviceRequestData = new EdipV2Dataset.CheckDeviceRequestDataDataTable();
                xCheckDeviceRequestData.AddCheckDeviceRequestDataRow("1", "ABCDEF1234567890ABCDEF12", "1000", "10000");
                byte[] baRequestMessage = xEDIPBuilder.baBuildCheckDeviceRequest(DateTime.Now, prm_strDeviceSerialNumber, 0, xCheckDeviceRequestData);
                byte[] baGmpRequestMessage = new byte[1];
                iCreateGmpPacket(prm_strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, baRequestMessage, ref baGmpRequestMessage);
                byte[] baResponseMessage = baSendAndReceiveMessage(baGmpRequestMessage);

                if (baResponseMessage == null)
                    return 1;

                Edata.EdipV2.Parser xEdipParser = new Edata.EdipV2.Parser();
                Edata.DataTypes.DataSets.EdipV2Dataset xEdipV2Dataset;

                xEdipV2Dataset = xEdipParser.ParseMessage(baRequestMessage, baRequestMessage.Length);

                if (xEdipV2Dataset != null)
                {
                    if (xEdipV2Dataset.PacketInfoV2[0].bPacketIsSuccessfullyParsed == true)
                    {
                        if (xEdipV2Dataset.PacketInfoV2[0].iCommandType == (int)Enums.MessageType.SEND_ERROR)
                            return 1;
                        else
                            return 0;
                    }
                    else
                        return 2;
                }

                return 3;
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return -1;
            }
        }

        public int iRequestParameters(string prm_strDeviceSerialNumber)
        {
            try
            {
                int iPacketSize = m_iPacketSize;
                Edata.EdipV2.Builder xEDIPBuilder = new Edata.EdipV2.Builder(iPacketSize);

                EdipV2Dataset.ParameterRequestDataDataTable xRequestData = new EdipV2Dataset.ParameterRequestDataDataTable();
                xRequestData.AddParameterRequestDataRow("1234567890123456", "12345678", prm_strDeviceSerialNumber.Trim(), "123456789012345678901234", "123456789012345678901234", "123456789012345678901234",
                    "12345678901", "1234567890", "1234567890123456", "123456", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "2402141100", "2402141100", "1234567890123456", "12345678901234567890");

                EdipV2Dataset.ParameterRequestApplicationDataDataTable xRequestApplicationData = new EdipV2Dataset.ParameterRequestApplicationDataDataTable();
                xRequestApplicationData.AddParameterRequestApplicationDataRow("APP1", "QT680120");
                xRequestApplicationData.AddParameterRequestApplicationDataRow("APP2", "02.00.00.07");
                xRequestApplicationData.AddParameterRequestApplicationDataRow("APP3", "1.7.3.0");

                byte[] baRequestMessage = xEDIPBuilder.baBuildRequestParameters(DateTime.Now, prm_strDeviceSerialNumber, 0, xRequestData, xRequestApplicationData);
                byte[] baGmpRequestMessage = new byte[1];
                iCreateGmpPacket(prm_strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, baRequestMessage, ref baGmpRequestMessage);
                byte[] baResponseMessage = baSendAndReceiveMessage(baGmpRequestMessage);

                if (baResponseMessage == null)
                    return 1;

                Edata.EdipV2.Parser xEdipParser = new Edata.EdipV2.Parser();
                Edata.DataTypes.DataSets.EdipV2Dataset xEdipV2Dataset;

                // GÖNDERİLEN REQUEST PARAMETERS MESAJI
                xEdipV2Dataset = xEdipParser.ParseMessage(baRequestMessage, baRequestMessage.Length);

                if (xEdipV2Dataset != null)
                {
                    if (xEdipV2Dataset.PacketInfoV2[0].bPacketIsSuccessfullyParsed == true)
                    {
                        if (xEdipV2Dataset.PacketInfoV2[0].iCommandType == (int)Enums.MessageType.SEND_ERROR)
                            return 1;
                        else
                            return 0;
                    }
                    else
                        return 2;
                }

                return 3;
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return -1;
            }
        }

        public int iRequestFullParameters(string prm_strDeviceSerialNumber)
        {
            try
            {
                int iPacketSize = m_iPacketSize;
                Edata.EdipV2.Builder xEDIPBuilder = new Edata.EdipV2.Builder(iPacketSize);

                EdipV2Dataset.ParameterRequestDataDataTable xRequestData = new EdipV2Dataset.ParameterRequestDataDataTable();
                xRequestData.AddParameterRequestDataRow("1234567890123456", "12345678", prm_strDeviceSerialNumber.Trim(), "123456789012345678901234", "123456789012345678901234", "123456789012345678901234",
                    "12345678901", "1234567890", "1234567890123456", "123456", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "2402141100", "2402141100", "1234567890123456", "12345678901234567890");

                EdipV2Dataset.ParameterRequestApplicationDataDataTable xRequestApplicationData = new EdipV2Dataset.ParameterRequestApplicationDataDataTable();
                xRequestApplicationData.AddParameterRequestApplicationDataRow("APP1", "QT680120");
                xRequestApplicationData.AddParameterRequestApplicationDataRow("APP2", "02.00.00.07");
                xRequestApplicationData.AddParameterRequestApplicationDataRow("APP3", "1.7.3.0");

                byte[] baRequestMessage = xEDIPBuilder.baBuildRequestFullParameters(DateTime.Now, prm_strDeviceSerialNumber.Replace(" ", ""), 0, xRequestData, xRequestApplicationData);
                byte[] baGmpRequestMessage = new byte[1];
                iCreateGmpPacket(prm_strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, baRequestMessage, ref baGmpRequestMessage);
                byte[] baResponseMessage = baSendAndReceiveMessage(baGmpRequestMessage);

                if (baResponseMessage == null)
                    return 1;

                Edata.EdipV2.Parser xEdipParser = new Edata.EdipV2.Parser();
                Edata.DataTypes.DataSets.EdipV2Dataset xEdipV2Dataset;

                // GÖNDERİLEN REQUEST FULL PARAMETERS MESAJI
                xEdipV2Dataset = xEdipParser.ParseMessage(baRequestMessage, baRequestMessage.Length);

                if (xEdipV2Dataset != null)
                {
                    if (xEdipV2Dataset.PacketInfoV2[0].bPacketIsSuccessfullyParsed == true)
                    {
                        if (xEdipV2Dataset.PacketInfoV2[0].iCommandType == (int)Enums.MessageType.SEND_ERROR)
                            return 1;
                        else
                            return 0;
                    }
                    else
                        return 2;
                }

                return 3;
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return -1;
            }
        }      

        public int iCreateGmpPacket(string prm_strDeviceSerialNumber, Enums.GMP_MESSAGE_TYPES prm_GMP_MESSAGE_TYPE, ref byte[] baGmpRequestMessage)
        {
            Edata.TLV.TLVParserV2 xTLVParserV2 = new Edata.TLV.TLVParserV2();
            baGmpRequestMessage = GMPBuilder.xGetInstance().baCreatePacket(prm_GMP_MESSAGE_TYPE, prm_strDeviceSerialNumber, "6000000001", xTLVParserV2, m_xCertificateDictionary);

            return 0;
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
            byte[] baEncryptedData = xGMPParserV1.baSignDataWithAesKeyWithoutIV(xGMPParserV1.baGetTDK(prm_strDeviceSerialNumber), baMessage, true);
            byte[] baEncryptedDataLength = Converter.baGetLengthValueAsByte(baEncryptedData.Length);

            Array.Resize<byte>(ref xTLVMessage, baEncryptedData.Length + (xTLVMessage.Length > 255 ? 6 : 4));
            Array.Copy(baEncryptedDataLength, 0, xTLVMessage, 3, baEncryptedDataLength.Length);
            Array.Copy(baEncryptedData, 0, xTLVMessage, (xTLVMessage.Length > 255 ? 6 : 4), baEncryptedData.Length);

            baGmpRequestMessage = GMPBuilder.xGetInstance().baCreatePacket(prm_GMP_MESSAGE_TYPE, prm_strDeviceSerialNumber, "6000000001", xTLVParserV2, m_xCertificateDictionary, xTLVMessage);

            return 0;
        }
    }
}
