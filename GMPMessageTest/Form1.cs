using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Edata.CommonLibrary;
using Edata.DataValidity;
using Edata.GMP;
using Edata.GIB;
using Edata.TLV;
using Edata.DataTypes.DataSets;
using System.Security.Cryptography.X509Certificates;
using System.Net;


namespace GMPMessageTest
{

    public partial class Form1 : Form
    {
        public TLVParser xTLVParser = new TLVParser();
        public GMPParserV1 xGMPParser = new GMPParserV1();
        public GMPBuilder xGMPBuilder = new GMPBuilder();
        public GIBParser xGIBParser = new GIBParser();
        public GIBBuilder xGIBBuilder = new GIBBuilder();
        public string strTerminalSerialNumber = "TEST00010101";
        public TLVBuilder xTLVBuilder = new TLVBuilder();
        public Form1()
        {
            InitializeComponent();
            xTLVParser = new TLVParser();
            xGMPParser = new GMPParserV1();
            xGMPBuilder = new GMPBuilder();

        }
        internal static string FormatByteArray(byte[] b)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder();
            int i = 0;
            for (i = 0; i < b.Length; i++)
            {
                if (i != 0 && i % 16 == 0)
                    sb1.Append("\n");
                sb1.Append(System.String.Format("{0:X2} ", b[i]));
            }
            return sb1.ToString();
        }


        public byte[] GenerateTLVBaseObject()
        {
            StringBuilder sbExchangeBlock = new StringBuilder();
            sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(strTerminalSerialNumber.PadLeft(12, '0'))));


            string sss = Converter.StrToHexStr(sbExchangeBlock.ToString());

            byte[] ddd = Converter.ToByteArray(sss);

            List<TLVBaseObject> xItems = new List<TLVBaseObject>();

            TLVObject xTerminalSerialNumber = new TLVObject(StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_SERIAL_NUMBER), ddd);

            #region Process Group
            TLVBaseObject xBaseTerminalSerialNumber = new TLVBaseObject();
            xBaseTerminalSerialNumber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalSerialNumber.strTag = xTerminalSerialNumber.strTag;
            xBaseTerminalSerialNumber.baValue = xTerminalSerialNumber.baValue;

            string strHexTerminalSerialNumberLength = Converter.IntToHexStr(xBaseTerminalSerialNumber.baValue.Length);
            xBaseTerminalSerialNumber.baLength = Converter.HexToByte(strHexTerminalSerialNumberLength);

            TLVObject xProcessNumber = new TLVObject(StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER), Converter.ToByteArray("123"));
            TLVBaseObject xBaseProcessNUmber = new TLVBaseObject();
            xBaseProcessNUmber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseProcessNUmber.strTag = xProcessNumber.strTag;
            xBaseProcessNUmber.baValue = Converter.StrToBcd(xProcessNumber.strValue);
            string strHexProcessNUmberLength = Converter.IntToHexStr(xBaseProcessNUmber.baValue.Length);
            xBaseProcessNUmber.baLength = Converter.HexToByte(strHexProcessNUmberLength);

            string strDate = DateTime.Now.ToString("yyMMdd");
            TLVObject xTerminalDate = new TLVObject(StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE), Converter.StrToBcd(strDate));
            TLVBaseObject xBaseTerminalDate = new TLVBaseObject();
            xBaseTerminalDate.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalDate.strTag = xTerminalDate.strTag;
            xBaseTerminalDate.baValue = Converter.StrToBcd(xTerminalDate.strValue);
            string strHexTerminalDateLength = Converter.IntToHexStr(xBaseTerminalDate.baValue.Length);
            xBaseTerminalDate.baLength = Converter.HexToByte(strHexTerminalDateLength);

            string strDtime = DateTime.Now.ToString("HHmmss");
            TLVObject xTerminalTime = new TLVObject(StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME), Converter.StrToBcd(strDtime));
            TLVBaseObject xBaseTerminalTime = new TLVBaseObject();
            xBaseTerminalTime.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalTime.strTag = xTerminalTime.strTag;
            xBaseTerminalTime.baValue = Converter.StrToBcd(xTerminalTime.strValue);
            string strHexTerminalTimeLength = Converter.IntToHexStr(xBaseTerminalTime.baValue.Length);
            xBaseTerminalTime.baLength = Converter.HexToByte(strHexTerminalTimeLength);



            /****/
            #endregion
            #region GibDataSecurity Group

            byte[] prm_baIV = xGMPParser.baCalculateIV(xBaseTerminalTime.baValue[2]);
            //prm_baTDK = baCreateKey(baFiscalNumber.ToString(), prm_baIV);

            byte[] baTrmkd = xGMPParser.baCreateKey(strTerminalSerialNumber,prm_baIV);

            TLVObject xtlvTrmkd = new TLVObject(StringEnum.GetStringValue(Enums.GIB_DATA_SECURITY_GROUP.TRMKD),
                                                 xGMPParser.baEncryptDataWithPrivateKey(baTrmkd, StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM))
                                               );
            string strHexreturn = Converter.ByteToHex(xtlvTrmkd.baValue);
            TLVBaseObject xTRMKD = new TLVBaseObject();
            xTRMKD.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.GIB_DATA_GROUP);
            xTRMKD.strTag = xtlvTrmkd.strTag;
            xTRMKD.baValue = xtlvTrmkd.baValue;
            xTRMKD.baLength = Converter.baGetLengthValueAsByte(xTRMKD.baValue.Length); 


            TLVObject xtlvFiscalNo = new TLVObject(StringEnum.GetStringValue(Enums.GIB_DATA_SECURITY_GROUP.FISCAL_NO),
                                                 xGMPParser.baEncryptDataWithPrivateKey(xTerminalSerialNumber.baValue, StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM))
                                               );
            TLVBaseObject xFiscalNo = new TLVBaseObject();
            xFiscalNo.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.GIB_DATA_GROUP);
            xFiscalNo.strTag = xtlvFiscalNo.strTag;
            xFiscalNo.baValue = xtlvFiscalNo.baValue;
            xFiscalNo.baLength = Converter.baGetLengthValueAsByte(xFiscalNo.baValue.Length); 



            byte[] baTerminalSign = new byte[1];
            int iSize = xtlvTrmkd.baValue.Length + xTerminalSerialNumber.baValue.Length + xProcessNumber.baValue.Length + xTerminalDate.baValue.Length + xTerminalTime.baValue.Length;
            Array.Resize(ref baTerminalSign, iSize);
            Array.Copy(xtlvTrmkd.baValue, 0, baTerminalSign, 0, xtlvTrmkd.baValue.Length);
            Array.Copy(xFiscalNo.baValue, 0, baTerminalSign, xtlvTrmkd.baValue.Length, xTerminalSerialNumber.baValue.Length);
            Array.Copy(xProcessNumber.baValue, 0, baTerminalSign, xtlvTrmkd.baValue.Length + xTerminalSerialNumber.baValue.Length, xProcessNumber.baValue.Length);
            Array.Copy(xTerminalDate.baValue, 0, baTerminalSign, xtlvTrmkd.baValue.Length + xTerminalSerialNumber.baValue.Length + xProcessNumber.baValue.Length, xTerminalDate.baValue.Length);
            Array.Copy(xTerminalTime.baValue, 0, baTerminalSign, xtlvTrmkd.baValue.Length + xTerminalSerialNumber.baValue.Length + xProcessNumber.baValue.Length + xTerminalDate.baValue.Length, xTerminalTime.baValue.Length);

            TLVObject xtlvTerminalSign = new TLVObject(StringEnum.GetStringValue(Enums.GIB_DATA_SECURITY_GROUP.TERMINAL_SIGN),
                                                xGMPParser.baSignDataWithPrivateKey(baTerminalSign
                                                                                    , StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM)
                                                                                   )
                                              );

            TLVBaseObject xTerminalSign = new TLVBaseObject();
            xTerminalSign.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.GIB_DATA_GROUP);
            xTerminalSign.strTag = xtlvTerminalSign.strTag;
            xTerminalSign.baValue = xtlvTerminalSign.baValue;
            xTerminalSign.baLength = Converter.baGetLengthValueAsByte(xTerminalSign.baValue.Length); 




            #endregion

            xItems.Add(xTerminalSign);
            xItems.Add(xFiscalNo);
            xItems.Add(xTRMKD);

            xItems.Add(xBaseTerminalTime);
            xItems.Add(xBaseTerminalDate);
            xItems.Add(xBaseProcessNUmber);
            xItems.Add(xBaseTerminalSerialNumber);

            TLVBuilder xTlvBuilder = new TLVBuilder();
            return xTlvBuilder.baCreate(Enums.GMP_MESSAGE_TYPES.REQUEST_TDK_MESSAGE, xItems.ToArray<TLVBaseObject>());
        }
        private byte[] CreateRequestPacket()
        {

            byte[] prm_baTLVMessage = GenerateTLVBaseObject();
            TLVBaseObject xTLVBaseObject = new TLVBaseObject();

            StringBuilder strMessageData = new StringBuilder();
            byte[] baMessageData = new byte[2];
            string str = string.Empty;
            int i = 17;
            i = i + prm_baTLVMessage.Length;
            if (i > 255)
            {
                baMessageData[0] = (byte)(i / 256);
                baMessageData[1] = (byte)(i % 256);
            }
            else
            {
                baMessageData[0] = (byte)0;
                baMessageData[1] = (byte)i;
            }
            strMessageData.Append(Converter.ByteToHex(baMessageData));

            strMessageData.Append("6000010000");

            strMessageData.Append(Converter.StrToHexStr(strTerminalSerialNumber));

            strMessageData.Append(Converter.ByteToHex(prm_baTLVMessage));

            byte[] baLRCBody;
            byte[] baLrc8 = new byte[1];

            baLRCBody = Converter.HexToByte(strMessageData.ToString());

            int iLength = baLRCBody.Length + 1;
            Array.Resize<byte>(ref baLRCBody, iLength);
            baLRCBody[baLRCBody.Length - 1] = Lrc8.byteCalculateLrc(baLRCBody, 2, baLRCBody.Length - 3);

            return baLRCBody;

        }


        public byte[] baSignDataWithPrivateKey(byte[] prm_baSource, string prm_strCertificateName,X509Certificate2 xCertificate, bool hash = true)
        {
            RSACryptoServiceProvider xRSACryptoServiceProvider = new RSACryptoServiceProvider();
            //X509Certificate2 xCertificate = m_xCertificateDictionary.Single(e => e.Key == prm_strCertificateName).Value;

            //var privKey = (RSACryptoServiceProvider)xCertificate.PrivateKey;

            //var enhCsp = new RSACryptoServiceProvider().CspKeyContainerInfo;
            //var cspparams = new CspParameters(enhCsp.ProviderType, enhCsp.ProviderName, privKey.CspKeyContainerInfo.KeyContainerName);
            //privKey = new RSACryptoServiceProvider(privKey.KeySize, cspparams);

            //return privKey.SignHash(prm_baSource, CryptoConfig.MapNameToOID("SHA256"));





            RSACryptoServiceProvider key = new RSACryptoServiceProvider();
            key.FromXmlString(xCertificate.PrivateKey.ToXmlString(true));
            if (hash) return key.SignHash(prm_baSource, CryptoConfig.MapNameToOID("SHA256"));
            else return key.SignData(prm_baSource, CryptoConfig.MapNameToOID("SHA256"));





        }
        private void button1_Click(object sender, EventArgs e)
        {

            GIBListener listenergb = new GIBListener();
            listenergb.ReadCertificates();
            xGIBParser.xSetCertificateDictionary(listenergb.xGetCertificatesList());


            GMPParserV1 xgmb = new GMPParserV1();

            GMPListener listener = new GMPListener();
            listener.ReadCertificates();
            xgmb.xSetCertificateDictionary(listener.xGetCertificatesList());
            
            byte[] prm_baTRMK_TREK = Converter.HexToByteArray("3A96EE6DC14B38D32B811895D95EAE0BAE364A29E4F6DB215224B9E1DF8C6644");
            byte[] prm_baTRMK_TRAK = Converter.HexToByteArray("ACA428903358EEA0E10F8EEB391E8DC3B7D42A1DA8822303E3AD4ABA8351D76E");



            byte[] fff = Converter.HexToByteArray("971F6845E845D4F1E38A9E034B38631F07162F2A2ABE729801DF7A5D918C2D936B059FA8F34035441DCC5C4BA4B989E9BA2928D687FA667C1420E662666F17961AB71D2C1F3EBB81D095C112B3AC13D563FB23A5F3FEA2D7F6B072EB71A7A8137C751963D9553938F37FA62E6E03EF7CCD2341650AD8BF4CB58DAD9CEF43F7A4DB3A8CCEA2BA1676098BC2E789F337B3DFC75E2A2233120C9B5FE4C67EE8B36D59933394AB6FD4A84B5D070BEF3AB7205F4D412E919B52F23FF6BB17DB2ED76AEEB83D32F64F34C080D246F3365396056D519A9451A0F818C7DD4DD10333290ED01183826B461DFE3550BDACA2FA072DBC23FC7EA339A65D25EE35917E973279");
               
            byte[] baGibSignContent = new byte[prm_baTRMK_TREK.Length + prm_baTRMK_TRAK.Length];
            X509Certificate2 dd = new X509Certificate2("GIB-SIGN.pfx", "123123", X509KeyStorageFlags.Exportable);



           

            Array.Copy(prm_baTRMK_TRAK, 0, baGibSignContent, 0, prm_baTRMK_TRAK.Length);
            Array.Copy(prm_baTRMK_TREK, 0, baGibSignContent, prm_baTRMK_TRAK.Length, prm_baTRMK_TREK.Length);

             bool ttyy = xgmb.VerifySignature_2048_Bit_PKCS1_v1_5(baGibSignContent,fff,dd);

             byte[] prm_baGIBSign2 = xGIBParser.baSignDataWithPrivateKey(baGetHashValue(baGibSignContent), "CertificateGIB_SIGN", true);

             byte[] prm_baGIBSign1 = baSignDataWithPrivateKey(baGetHashValue(baGibSignContent), "", dd, true);

            byte[] paramrequest4 = null;
            int paramrequest4i = 0;
            byte[] prm_baPacketCollectorBuffer=null;
            int prm_iPacketCollectorBufferLength=0;
            EndPoint prm_EndPoint = null;

            byte[] paramrequest = Converter.HexToByteArray("00756000010000544553543030303030393531FF880260A394F0AE615B998749E184CEB2048E4B6A84134B884761B96088C34F751FF7E0D4599D0F557BCC76833D944F1E0D74C4BB1FF999607414AF2EEE5A5F18268E0959CF48D6E5E8E7E6802C6ADF4FA5379A6C3F81F225879CEBFC2D6346871291424C");
             listener.iAnalyseIncommingMessageAndPrepareResponse(paramrequest, paramrequest.Length, out paramrequest4, out paramrequest4i, ref prm_baPacketCollectorBuffer, ref  prm_iPacketCollectorBufferLength, prm_EndPoint);


        }

        private byte[] baGetHashValue(byte[] baGibSignContent)
        {
            SHA256 xSHA256 = SHA256.Create();

          //  Trace.vInsertMethodTraceDataLog(enumTraceLevel.LowLevel, "HASH TRMKD(TDK):\n" + Converter.ToHexStr(xSHA256.ComputeHash(prm_baSource, 0, prm_baSource.Length), 0, xSHA256.ComputeHash(prm_baSource, 0, prm_baSource.Length).Length) + "(" + xSHA256.ComputeHash(prm_baSource, 0, prm_baSource.Length).Length + ")");

            return xSHA256.ComputeHash(baGibSignContent, 0, baGibSignContent.Length);
        }
        public byte[] encryptdata(byte[] bytearraytoencrypt, string key, string iv)
        {
            AesCryptoServiceProvider dataencrypt = new AesCryptoServiceProvider();
            //Block size : Gets or sets the block size, in bits, of the cryptographic operation.  
            dataencrypt.BlockSize = 128;
            //KeySize: Gets or sets the size, in bits, of the secret key  
            dataencrypt.KeySize = 128;
            //Key: Gets or sets the symmetric key that is used for encryption and decryption.  
            dataencrypt.Key = System.Text.Encoding.UTF8.GetBytes(key);
            //IV : Gets or sets the initialization vector (IV) for the symmetric algorithm  
            dataencrypt.IV = System.Text.Encoding.UTF8.GetBytes(iv);
            //Padding: Gets or sets the padding mode used in the symmetric algorithm  
            dataencrypt.Padding = PaddingMode.PKCS7;
            //Mode: Gets or sets the mode for operation of the symmetric algorithm  
            dataencrypt.Mode = CipherMode.CBC;
            //Creates a symmetric AES encryptor object using the current key and initialization vector (IV).  
            ICryptoTransform crypto1 = dataencrypt.CreateEncryptor(dataencrypt.Key, dataencrypt.IV);
            //TransformFinalBlock is a special function for transforming the last block or a partial block in the stream.   
            //It returns a new array that contains the remaining transformed bytes. A new array is returned, because the amount of   
            //information returned at the end might be larger than a single block when padding is added.  
            byte[] encrypteddata = crypto1.TransformFinalBlock(bytearraytoencrypt, 0, bytearraytoencrypt.Length);
            crypto1.Dispose();
            //return the encrypted data  
            return encrypteddata;
        }

        //code to decrypt data
        private byte[] decryptdata(byte[] bytearraytodecrypt, string key, string iv)
        {

            AesCryptoServiceProvider keydecrypt = new AesCryptoServiceProvider();
            keydecrypt.BlockSize = 128;
            keydecrypt.KeySize = 128;
            keydecrypt.Key = System.Text.Encoding.UTF8.GetBytes(key);
            keydecrypt.IV = System.Text.Encoding.UTF8.GetBytes(iv);
            keydecrypt.Padding = PaddingMode.PKCS7;
            keydecrypt.Mode = CipherMode.CBC;
            ICryptoTransform crypto1 = keydecrypt.CreateDecryptor(keydecrypt.Key, keydecrypt.IV);

            byte[] returnbytearray = crypto1.TransformFinalBlock(bytearraytodecrypt, 0, bytearraytodecrypt.Length);
            crypto1.Dispose();
            return returnbytearray;
        }
        public void vSEND_GIB_PARAMETER_MESSAGE()
        {
            //create packet
            //send to TSM-Gib Listener 2574

            string strDate = DateTime.Now.ToString("yyMMdd");
            string strDtime = DateTime.Now.ToString("HHMMss");


            Dictionary<Enums.GMP_PROCESS_GROUP, byte[]> processGroup = new Dictionary<Enums.GMP_PROCESS_GROUP, byte[]>();
            Dictionary<Enums.GMP_GIB_SECRET_DATA_GROUP, byte[]> specialDataGroup = new Dictionary<Enums.GMP_GIB_SECRET_DATA_GROUP, byte[]>();
            Dictionary<Enums.GIB_DATA_SECURITY_GROUP, byte[]> dataSecurityGroup = new Dictionary<Enums.GIB_DATA_SECURITY_GROUP, byte[]>();

            byte[] ParameterSequenceNumber = Converter.StrToBcd("000001");
            byte[] SystemDate = Converter.StrToBcd(strDate);
            byte[] SystemTime = Converter.StrToBcd(strDtime);

            processGroup.Add(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER, ParameterSequenceNumber);
            processGroup.Add(Enums.GMP_PROCESS_GROUP.SYSTEM_DATE, SystemDate);
            processGroup.Add(Enums.GMP_PROCESS_GROUP.SYSTEM_TIME, SystemTime);

            byte[] TerminalId = Converter.ToByteArray("00000128");
            byte[] PARAMETER_DOWNLOAD_DATE = Converter.StrToBcd(DateTime.Now.ToString("yyMMdd") + DateTime.Now.ToString("HHMM"));

            EdipV2Dataset dsEdipV2Dataset = new EdipV2Dataset();
            EdipV2Dataset.ParameterRequestDataDataTable dtParameterRequestData = new EdipV2Dataset.ParameterRequestDataDataTable();
            DateTime prm_xExchangeDownloadDate = DateTime.Now;
            DateTime prm_xParameterDownloadDate = DateTime.Now;
            int ErrorCode = -1;



             
            dtParameterRequestData.AddParameterRequestDataRow("1234567890123456", Converter.BcdToStr(TerminalId, 0, TerminalId.Length),
                                                              "TEST00000128",
                                                              "", "", "", "", "", "",
                                                             "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", DateTime.Now.ToString(), "", "", "");



             xGIBParser.vGetParameters(ref dsEdipV2Dataset, dtParameterRequestData, prm_xParameterDownloadDate, prm_xExchangeDownloadDate, ref ErrorCode);
           // byte[] baParameter = Converter.HexToByteArray("9999414106230300360006230300360002230300360036000300360000013600000000003600990800050001001000120015001800200000080001080000020500000301000004100000051200000615000007180000082000");//
             byte[] baParameter =  xGIBParser.baParameterBlock(dsEdipV2Dataset.ParameterDeviceData, dsEdipV2Dataset.ParameterVatData, dsEdipV2Dataset.ParameterDepartmentData);

            specialDataGroup.Add(Enums.GMP_GIB_SECRET_DATA_GROUP.TERMINAL_ID, TerminalId);
            specialDataGroup.Add(Enums.GMP_GIB_SECRET_DATA_GROUP.PARAMETER_DOWNLOAD_DATE, PARAMETER_DOWNLOAD_DATE);
            specialDataGroup.Add(Enums.GMP_GIB_SECRET_DATA_GROUP.PARAMETER_BLOCK, baParameter);

            byte[] baParameterHashValue = xGIBParser.baGetHashValue(baParameter);
            byte[] baParameterInfoSign = new byte[baParameterHashValue.Length + TerminalId.Length + ParameterSequenceNumber.Length];

            Array.Copy(baParameterHashValue, 0, baParameterInfoSign, 0, baParameterHashValue.Length);
            Array.Copy(TerminalId, 0, baParameterInfoSign, baParameterHashValue.Length, TerminalId.Length);
            Array.Copy(ParameterSequenceNumber, 0, baParameterInfoSign, baParameterHashValue.Length + TerminalId.Length, ParameterSequenceNumber.Length);



            dataSecurityGroup.Add(Enums.GIB_DATA_SECURITY_GROUP.GIB_PARAMETER_SIGN, xGIBParser.baSignDataWithPrivateKey(baParameterInfoSign, StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateGIB_SIGN)));





            
            byte[] baOutgoingMessage = xGIBBuilder.baCreatePacket(Enums.GMP_MESSAGE_TYPES.REQUEST_GIB_PARAMETER_MESSAGE, null, null, null, null, null, processGroup, specialDataGroup, dataSecurityGroup);

            byte[] baResponseMessage = xGIBParser.baUplodTsmPacket(baOutgoingMessage);
        }

        public void vSEND_GIB_CURRENCY_MESSAGE()
        {
            //create packet
            //send to TSM-Gib Listener 2574

            string strDate = DateTime.Now.ToString("yyMMdd");
            string strDtime = DateTime.Now.ToString("HHMMss");


            Dictionary<Enums.GMP_PROCESS_GROUP, byte[]> processGroup = new Dictionary<Enums.GMP_PROCESS_GROUP, byte[]>();
            Dictionary<Enums.GMP_GIB_SECRET_DATA_GROUP, byte[]> specialDataGroup = new Dictionary<Enums.GMP_GIB_SECRET_DATA_GROUP, byte[]>();
            Dictionary<Enums.GIB_DATA_SECURITY_GROUP, byte[]> dataSecurityGroup = new Dictionary<Enums.GIB_DATA_SECURITY_GROUP, byte[]>();

            byte[] baExchangeSequenceNumber = Converter.StrToBcd("000001");
            byte[] SystemDate = Converter.StrToBcd(strDate);
            byte[] SystemTime = Converter.StrToBcd(strDtime);

            processGroup.Add(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER, baExchangeSequenceNumber);
            processGroup.Add(Enums.GMP_PROCESS_GROUP.SYSTEM_DATE, SystemDate);
            processGroup.Add(Enums.GMP_PROCESS_GROUP.SYSTEM_TIME, SystemTime);

            byte[] TerminalId = Converter.ToByteArray("00000128");
            byte[] EXCHANGE_DOWNLOAD_DATE = Converter.StrToBcd(DateTime.Now.ToString("yyMMdd") + DateTime.Now.ToString("HHMM"));

            EdipV2Dataset dsEdipV2Dataset = new EdipV2Dataset();
            EdipV2Dataset.ParameterRequestDataDataTable dtParameterRequestData = new EdipV2Dataset.ParameterRequestDataDataTable();
            DateTime prm_xExchangeDownloadDate = DateTime.Now;
            DateTime prm_xParameterDownloadDate = DateTime.Now;
            int ErrorCode = -1;




            dtParameterRequestData.AddParameterRequestDataRow("1234567890123456", Converter.BcdToStr(TerminalId, 0, TerminalId.Length),
                                                              "TEST00000128",
                                                              "", "", "", "", "", "",
                                                             "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", DateTime.Now.ToString(), "", "", "");



           
            xGIBParser.vGetParameters(ref dsEdipV2Dataset, dtParameterRequestData, prm_xParameterDownloadDate, prm_xExchangeDownloadDate, ref ErrorCode);
            byte[] baExchange = xGIBParser.baExchangeBlock(dsEdipV2Dataset.ParameterCurrencyData);

            specialDataGroup.Add(Enums.GMP_GIB_SECRET_DATA_GROUP.TERMINAL_ID, TerminalId);
            specialDataGroup.Add(Enums.GMP_GIB_SECRET_DATA_GROUP.EXCHANGE_DOWNLOAD_DATE, EXCHANGE_DOWNLOAD_DATE);
            specialDataGroup.Add(Enums.GMP_GIB_SECRET_DATA_GROUP.EXCHANGE_BLOCK, baExchange);


            byte[] baExchangeHashValue = xGIBParser.baGetHashValue(baExchange);
            byte[] baExchangeInfoSign = new byte[baExchangeHashValue.Length + TerminalId.Length + baExchangeSequenceNumber.Length];

            Array.Copy(baExchangeHashValue, 0, baExchangeInfoSign, 0, baExchangeHashValue.Length);
            Array.Copy(TerminalId, 0, baExchangeInfoSign, baExchangeHashValue.Length, TerminalId.Length);
            Array.Copy(baExchangeSequenceNumber, 0, baExchangeInfoSign, baExchangeHashValue.Length + TerminalId.Length, baExchangeSequenceNumber.Length);



            dataSecurityGroup.Add(Enums.GIB_DATA_SECURITY_GROUP.GIB_CURRENCY_SIGN, xGIBParser.baSignDataWithPrivateKey(baExchangeInfoSign, StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateGIB_SIGN)));




            
           
            byte[] baOutgoingMessage = xGIBBuilder.baCreatePacket(Enums.GMP_MESSAGE_TYPES.REQUEST_GIB_CURRENCY_MESSAGE, null, null, null, null, null, processGroup, specialDataGroup, dataSecurityGroup);

            byte[] baResponseMessage = xGIBParser.baUplodTsmPacket(baOutgoingMessage);



        }

    }
}
