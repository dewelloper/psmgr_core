using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using Edata.CommonLibrary;
using Edata.DataTypes;
using Edata.DataTypes.DataSets;
using Edata.DataValidity;
using Edata.GMP;
using System.Security.Cryptography.X509Certificates;
using Edata.TLV;


namespace GmpStressTestingTool
{
    public class GMPBuilder
    {
        public static GMPBuilder m_xGMPBuilderGlobalsInstance { get; set; }
        public byte[] baCreateGibRequestPacket(TLVDataObject prm_xTLVObject, Enums.GMP_MESSAGE_TYPES prm_eMsgType)
        {
            StringBuilder strMessageData = new StringBuilder();
            byte[] baMessageData = new byte[2];
            string str = string.Empty;
            int i = 5;
            i = i + prm_xTLVObject.baValue.Length + Converter.baGetLengthValueAsByte(prm_xTLVObject.baValue.Length).Length + prm_xTLVObject.baTag.Length;
           
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
            strMessageData.Append(Converter.ByteToHex(prm_xTLVObject.baTag));
            strMessageData.Append(Converter.ByteToHex(Converter.baGetLengthValueAsByte(prm_xTLVObject.baValue.Length)));
            strMessageData.Append(Converter.ByteToHex(prm_xTLVObject.baValue));

            byte[] baLRCBody;
            byte[] baLrc8 = new byte[1];

            baLRCBody = Converter.HexToByte(strMessageData.ToString());
            int iLength = baLRCBody.Length + 1;
            Array.Resize<byte>(ref baLRCBody, iLength);
            baLRCBody[baLRCBody.Length - 1] = Lrc8.byteCalculateLrc(baLRCBody, 2, baLRCBody.Length - 3);

            return baLRCBody;
        }
        public static GMPBuilder xGetInstance()
        {
            if (m_xGMPBuilderGlobalsInstance == null)
                m_xGMPBuilderGlobalsInstance = new GMPBuilder();
            return m_xGMPBuilderGlobalsInstance;
        }
        
        ~GMPBuilder() 
        { 
        
        }

        public byte[] baCreateRequestPacket(TLVDataObject prm_xTLVObject, byte[] prm_baTerminalSerialNumber)
        {
            StringBuilder strMessageData = new StringBuilder();
            byte[] baMessageData = new byte[2];
            string str = string.Empty;
            int i = 17;
            i = i + prm_xTLVObject.iLength + 17;
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
            strMessageData.Append("6000000001");
            strMessageData.Append(Converter.ByteToHex(prm_baTerminalSerialNumber));          
            strMessageData.Append(Converter.ByteToHex(Converter.HexToByteArray(StringEnum.GetStringValue(Enums.GMP_MESSAGE_TYPES.REQUEST_CERTIFICATE_MESSAGE))));
            strMessageData.Append(Converter.ByteToHex(Converter.baGetLengthValueAsByte(prm_xTLVObject.iLength + 11)));
            strMessageData.Append(Converter.ByteToHex(Converter.HexToByteArray(StringEnum.GetStringValue(Enums.GMP_EDIP_CERTIFICATE_GROUP.GMP_EDIP_CERTIFICATE_GROUP))));
            strMessageData.Append(Converter.ByteToHex(Converter.baGetLengthValueAsByte(prm_xTLVObject.iLength + 6)));            
            strMessageData.Append(Converter.ByteToHex(prm_xTLVObject.baTag));
            strMessageData.Append(Converter.ByteToHex(Converter.baGetLengthValueAsByte(prm_xTLVObject.iLength)));
            strMessageData.Append(Converter.ToHexStr(prm_xTLVObject.baValue, 0, prm_xTLVObject.baValue.Length));
        
            byte[] baLRCBody;
            byte[] baLrc8 = new byte[1];

            baLRCBody = Converter.HexToByte(strMessageData.ToString());
            int iLength = baLRCBody.Length + 1;
            Array.Resize<byte>(ref baLRCBody, iLength);
            baLRCBody[baLRCBody.Length - 1] = Lrc8.byteCalculateLrc(baLRCBody, 2, baLRCBody.Length - 3);

            return baLRCBody;
        }

        public byte[] baCreatePacket(Enums.GMP_MESSAGE_TYPES prm_xMessageType, string prm_strSerialNumber, string prm_strTPDU, TLVParserV2 prm_xTlvParser, Dictionary<string, X509Certificate2> prm_xCertificateDictionary, byte[] prm_baTLVMessage = null)
        {
            byte[] xTLVMessage = null;
            int i = 17;
            bool bAddSerialNumber = true;

            Dictionary<Enums.GMP_PROCESS_GROUP, byte[]> xProcessGroupData = new Dictionary<Enums.GMP_PROCESS_GROUP, byte[]>();

            switch (prm_xMessageType)
            {
                case Enums.GMP_MESSAGE_TYPES.REQUEST_TDK_MESSAGE:
                    {
                        xTLVMessage = baSendRequest(prm_strSerialNumber, prm_xTlvParser, prm_xMessageType, xProcessGroupData, null, null, prm_xCertificateDictionary);
                    }
                    break;
                case Enums.GMP_MESSAGE_TYPES.REQUEST_PRIMARY_KEY_TERMS_AND_CREATING_DATA_ENCRYPTION:
                    {
                        xTLVMessage = baSendRequest(prm_strSerialNumber, prm_xTlvParser, prm_xMessageType, xProcessGroupData, null, null, prm_xCertificateDictionary);
                    }
                    break;
                case Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE: 
                {
                    ////Edip Base Object
                    //TLVBaseObject xBase = new TLVBaseObject();
                    //xBase.strParentTag = StringEnum.GetStringValue(Enums.GMP_EDIP_DATA_GROUP.GMP_EDIP_DATA_GROUP);
                    //xBase.strTag = StringEnum.GetStringValue(prm_xEdipDataGroup.First().Key);
                    //xBase.baValue = prm_xEdipDataGroup.First().Value;

                    //string strHexBaseLength = Converter.IntToHexStr(xBase.baValue.Length);
                    //xBase.baLength = Converter.baGetLengthValueAsByte(xBase.baValue.Length);//Converter.HexToByte(strHexBaseLength);

                    //List<TLVBaseObject> xTlvBaseObjectList = new List<TLVBaseObject>();
                    //xTlvBaseObjectList.Add(xBase);

                    //TLVBuilder xTlvBuilder = new TLVBuilder();
                    //xTLVMessage = xTlvBuilder.baCreateV1(prm_xMessageType, xTlvBaseObjectList.ToArray<TLVBaseObject>());
                    xTLVMessage = prm_baTLVMessage;
                    Array.Resize<byte>(ref xTLVMessage, prm_baTLVMessage.Length);
                    Array.Copy(prm_baTLVMessage, xTLVMessage, prm_baTLVMessage.Length);
                    bAddSerialNumber = true;
                }
                    break;
            }

           
            //if(prm_xMessageType == Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE)
            //{
            //    GMPParser xGMPParser = new GMPParser();
            //    prm_xTlvParser.Parse(xTLVMessage, TLVParserV2.PARSER_TYPE.PARSE_GMP2);
               
            //         #region encrypt Data with TDK

            //    byte[] prm_baIV = xGMPParser.baCalculateIV(Converter.StrToHexByte("0"));

            //    TLVDataObject xTlvObject = prm_xTlvParser.m_xResult.ToArray()[0];


            //    byte[] EncryptedData = xGMPParser.baSignDataWithAesKeyWithoutIV(xGMPParser.baGetTDK(prm_strSerialNumber), xTlvObject.baValue);
            //        byte[] EncryptedDataBlock = new byte[EncryptedData.Length + xTlvObject.baTag.Length + Converter.baGetLengthValueAsByte(EncryptedData.Length).Length];

            //        Array.Copy(xTlvObject.baTag, 0, EncryptedDataBlock, 0, xTlvObject.baTag.Length);
            //        Array.Copy(Converter.baGetLengthValueAsByte(EncryptedData.Length), 0, EncryptedDataBlock, xTlvObject.baTag.Length, Converter.baGetLengthValueAsByte(EncryptedData.Length).Length);
            //        Array.Copy(EncryptedData, 0, EncryptedDataBlock, xTlvObject.baTag.Length + Converter.baGetLengthValueAsByte(EncryptedData.Length).Length, EncryptedData.Length);
            //        Array.Resize<byte>(ref xTLVMessage, EncryptedDataBlock.Length);
            //        Array.Copy(EncryptedDataBlock, 0, xTLVMessage, 0, EncryptedDataBlock.Length);

            //    #endregion

            //}

            StringBuilder strMessageData = new StringBuilder();
            byte[] baMessageData = new byte[2];
            string str = string.Empty;

            i = i + xTLVMessage.Length;
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
            strMessageData.Append("6000000001");

            if (bAddSerialNumber) strMessageData.Append(Converter.StrToHexStr(prm_strSerialNumber));
            strMessageData.Append(Converter.ByteToHex(xTLVMessage));
            byte[] baLRCBody;
            byte[] baLrc8 = new byte[1];

            baLRCBody = Converter.HexToByte(strMessageData.ToString());
            int iLength = baLRCBody.Length + 1;
            Array.Resize<byte>(ref baLRCBody, iLength);
            baLRCBody[baLRCBody.Length - 1] = Lrc8.byteCalculateLrc(baLRCBody, 2, baLRCBody.Length - 3);

            return baLRCBody;
        }

        private byte[] baSendRequest(string prm_strSerialNumber, TLVParserV2 prm_xTlvParser, Enums.GMP_MESSAGE_TYPES prm_xMessageType,
                                            Dictionary<Enums.GMP_PROCESS_GROUP, byte[]> prm_xGmpProcessGroup,
                                            Dictionary<Enums.GMP_GIB_SECRET_DATA_GROUP, byte[]> prm_xGibSecretDataGroup,
                                            Dictionary<Enums.GIB_DATA_SECURITY_GROUP, byte[]> prm_xGibDataSecurityGroup,
                                            Dictionary<string, X509Certificate2> prm_xCertificateDictionary)
        {

            //List<TLVBaseObject> xTlvBaseObjectList = prm_xGibDataSecurityGroup != null ? xListGetBaseResponse(prm_xTlvParser, errorDataDataTable, prm_bAddAnswer) : null;
            List<TLVBaseObject> xTlvBaseObjectList = new List<TLVBaseObject>();
            
            vAddGmpProcessGroup(prm_strSerialNumber, ref xTlvBaseObjectList);
            
            if (prm_xMessageType == Enums.GMP_MESSAGE_TYPES.REQUEST_PRIMARY_KEY_TERMS_AND_CREATING_DATA_ENCRYPTION)
                vAddGibSecretDataGroup(prm_strSerialNumber, ref xTlvBaseObjectList);

            vAddGibDataSecurityGroup(prm_strSerialNumber, ref xTlvBaseObjectList, prm_xCertificateDictionary);
            
            TLVBuilder xTlvBuilder = new TLVBuilder();
            return xTlvBuilder.baCreate(prm_xMessageType, xTlvBaseObjectList.ToArray<TLVBaseObject>());
        }

        private void vAddGibSecretDataGroup(string prm_strSerialNumber, ref List<TLVBaseObject> xTlvBaseObjectList)  //DF40
        {
            TLVBaseObject xBase = new TLVBaseObject();
            xBase.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_SECRET_DATA_GROUP);
            xBase.strTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_TAX_ID);
            xBase.baValue = Converter.StrToBcd("101010101010");
            string strHexBaseLength = Converter.IntToHexStr(xBase.baValue.Length);
            xBase.baLength = Converter.HexToByte(strHexBaseLength);

            xTlvBaseObjectList.Add(xBase);
        }

        private void vAddGibDataSecurityGroup(string prm_strSerialNumber, ref List<TLVBaseObject> xTlvBaseObjectList, Dictionary<string, X509Certificate2> prm_xCertificateDictionary)   //DF42
        {
            TLVBaseObject xTlvBaseObjectProcessNumber = xTlvBaseObjectList.First(p => p.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER));
            TLVBaseObject xTlvBaseObjectTerminalDate = xTlvBaseObjectList.First(p => p.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE));
            TLVBaseObject xTlvBaseObjectTerminalTime = xTlvBaseObjectList.First(p => p.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME));

            GMPParserV1 xGMPParserV1 = GMPParserV1.xGetInstance();
            xGMPParserV1.xSetCertificateDictionary(prm_xCertificateDictionary);
            byte[] prm_baIV = xGMPParserV1.baCalculateIV(xTlvBaseObjectTerminalTime.baValue[2]);
            byte[] baTrmkd = xGMPParserV1.baCreateKey(prm_strSerialNumber, prm_baIV);

            TLVBaseObject xBase1 = new TLVBaseObject();
            xBase1.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_DATA_SECURITY_GROUP);
            xBase1.strTag = StringEnum.GetStringValue(Enums.GIB_DATA_SECURITY_GROUP.TRMKD);
            xBase1.baValue = xGMPParserV1.baEncryptDataWithPrivateKey(baTrmkd, StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM));
            xBase1.baLength = Converter.baGetLengthValueAsByte(xBase1.baValue.Length);
            xTlvBaseObjectList.Add(xBase1);

            TLVBaseObject xBase2 = new TLVBaseObject();
            xBase2.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_DATA_SECURITY_GROUP);
            xBase2.strTag = StringEnum.GetStringValue(Enums.GIB_DATA_SECURITY_GROUP.FISCAL_NO);
            xBase2.baValue = xGMPParserV1.baEncryptDataWithPrivateKey(Converter.StrToAscii(prm_strSerialNumber), StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateTSM));
            xBase2.baLength = Converter.baGetLengthValueAsByte(xBase2.baValue.Length);
            xTlvBaseObjectList.Add(xBase2);

            byte[] baTerminalDate = xTlvBaseObjectTerminalDate.baValue;
            string strTerminalDate = "20" + Converter.ByteToHex(baTerminalDate);
            baTerminalDate = Converter.StrToBcd(strTerminalDate);

            byte[] baTerminalSign = new byte[1];
            int iSize = xBase1.baValue.Length + Converter.StrToAscii(prm_strSerialNumber).Length + xTlvBaseObjectProcessNumber.baValue.Length + baTerminalDate.Length + xTlvBaseObjectTerminalTime.baValue.Length;

            Array.Resize(ref baTerminalSign, iSize);
            Array.Copy(xBase1.baValue, 0, baTerminalSign, 0, xBase1.baValue.Length);
            Array.Copy(Converter.StrToAscii(prm_strSerialNumber), 0, baTerminalSign, xBase1.baValue.Length, Converter.StrToAscii(prm_strSerialNumber).Length);
            Array.Copy(xTlvBaseObjectProcessNumber.baValue, 0, baTerminalSign, xBase1.baValue.Length + Converter.StrToAscii(prm_strSerialNumber).Length, xTlvBaseObjectProcessNumber.baValue.Length);
            Array.Copy(baTerminalDate, 0, baTerminalSign, xBase1.baValue.Length + Converter.StrToAscii(prm_strSerialNumber).Length + xTlvBaseObjectProcessNumber.baValue.Length, baTerminalDate.Length);
            Array.Copy(xTlvBaseObjectTerminalTime.baValue, 0, baTerminalSign, xBase1.baValue.Length + Converter.StrToAscii(prm_strSerialNumber).Length + xTlvBaseObjectProcessNumber.baValue.Length + baTerminalDate.Length, xTlvBaseObjectTerminalTime.baValue.Length);

            TLVBaseObject xBase3 = new TLVBaseObject();
            xBase3.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_DATA_SECURITY_GROUP);
            xBase3.strTag = StringEnum.GetStringValue(Enums.GIB_DATA_SECURITY_GROUP.TERMINAL_SIGN);
            xBase3.baValue = xGMPParserV1.baSignDataWithPrivateKey(xGMPParserV1.baGetHashValue(baTerminalSign), StringEnum.GetStringValue(Enums.GMP_CERTIFICATES_TYPE.CertificateECR));
            xBase3.baLength = Converter.baGetLengthValueAsByte(xBase3.baValue.Length);
            xTlvBaseObjectList.Add(xBase3);
        }

        private void vAddGmpProcessGroup(string prm_strDeviceSerialNumber, ref List<TLVBaseObject> xTlvBaseObjectList)  //DF02
        {
            TLVBaseObject xBase1 = new TLVBaseObject();
            xBase1.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBase1.strTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_SERIAL_NUMBER);
            xBase1.baValue = Converter.StrToAscii(prm_strDeviceSerialNumber);
            string strHexBaseLength = Converter.IntToHexStr(xBase1.baValue.Length);
            xBase1.baLength = Converter.HexToByte(strHexBaseLength);
            xTlvBaseObjectList.Add(xBase1);

            TLVBaseObject xBase2 = new TLVBaseObject();
            xBase2.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBase2.strTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER);
            xBase2.baValue = Converter.StrToBcd("000011");
            strHexBaseLength = Converter.IntToHexStr(xBase2.baValue.Length);
            xBase2.baLength = Converter.HexToByte(strHexBaseLength);
            xTlvBaseObjectList.Add(xBase2);

            TLVBaseObject xBase3 = new TLVBaseObject();
            xBase3.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBase3.strTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE);
            xBase3.baValue = Converter.StrToBcd(DateTime.Now.ToString("yyMMdd"));
            strHexBaseLength = Converter.IntToHexStr(xBase3.baValue.Length);
            xBase3.baLength = Converter.HexToByte(strHexBaseLength);
            xTlvBaseObjectList.Add(xBase3);

            TLVBaseObject xBase4 = new TLVBaseObject();
            xBase4.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBase4.strTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME);
            xBase4.baValue = Converter.StrToBcd(DateTime.Now.ToString("HHmmss"));
            strHexBaseLength = Converter.IntToHexStr(xBase4.baValue.Length);
            xBase4.baLength = Converter.HexToByte(strHexBaseLength);
            xTlvBaseObjectList.Add(xBase4);
           
        }
        public List<TLVBaseObject> xAddProcessGroup(TLVParser prm_xTlvParser)
        {
            List<TLVBaseObject> xTlvBaseObjectList = new List<TLVBaseObject>();
            TLVObject xTerminalSerialNumber = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_SERIAL_NUMBER)];
            TLVBaseObject xBaseTerminalSerialNumber = new TLVBaseObject();
            xBaseTerminalSerialNumber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalSerialNumber.strTag = xTerminalSerialNumber.strTag;
            xBaseTerminalSerialNumber.baValue = xTerminalSerialNumber.baValue;

            string strHexTerminalSerialNumberLength = Converter.IntToHexStr(xBaseTerminalSerialNumber.baValue.Length);
            xBaseTerminalSerialNumber.baLength = Converter.HexToByte(strHexTerminalSerialNumberLength);



            TLVObject xProcessNumber = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER)];
            TLVBaseObject xBaseProcessNUmber = new TLVBaseObject();
            xBaseProcessNUmber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseProcessNUmber.strTag = xProcessNumber.strTag;
            xBaseProcessNUmber.baValue = Converter.StrToBcd(xProcessNumber.strValue);
            string strHexProcessNUmberLength = Converter.IntToHexStr(xBaseProcessNUmber.baValue.Length);
            xBaseProcessNUmber.baLength = Converter.HexToByte(strHexProcessNUmberLength);

            TLVObject xTerminalDate = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE)];
            TLVBaseObject xBaseTerminalDate = new TLVBaseObject();
            xBaseTerminalDate.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalDate.strTag = xTerminalDate.strTag;
            xBaseTerminalDate.baValue = Converter.StrToBcd(xTerminalDate.strValue);
            string strHexTerminalDateLength = Converter.IntToHexStr(xBaseTerminalDate.baValue.Length);
            xBaseTerminalDate.baLength = Converter.HexToByte(strHexTerminalDateLength);



            TLVObject xTerminalTime = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME)];
            TLVBaseObject xBaseTerminalTime = new TLVBaseObject();
            xBaseTerminalTime.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalTime.strTag = xTerminalTime.strTag;
            xBaseTerminalTime.baValue = Converter.StrToBcd(xTerminalTime.strValue);
            string strHexTerminalTimeLength = Converter.IntToHexStr(xBaseTerminalTime.baValue.Length);
            xBaseTerminalTime.baLength = Converter.HexToByte(strHexTerminalTimeLength);

            xTlvBaseObjectList.Add(xBaseTerminalTime);
            xTlvBaseObjectList.Add(xBaseTerminalDate);
            xTlvBaseObjectList.Add(xBaseTerminalSerialNumber);

            return xTlvBaseObjectList;

        }

        private List<TLVBaseObject> xListGetBaseResponse(TLVParserV2 prm_xTlvParser, EdipV2Dataset.ErrorDataDataTable prm_dtEdipV2ErrorData, bool prm_bAddAnswer = true)
        {

            List<TLVBaseObject> xlTlvBaseObjectList = new List<TLVBaseObject>();
            List<TLVDataObject> xlProcessGroup = prm_xTlvParser.m_xResult.ToArray()[0]
                                                 .xChildren
                                                           .Single(e => e.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP))
                                                                .xChildren;                                    
                ;
            foreach (var xTlvItem in xlProcessGroup)
            {

                TLVBaseObject xTlvBaseObject = new TLVBaseObject();
                xTlvBaseObject.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
                xTlvBaseObject.strTag = xTlvItem.strTag;
               /* if (xTlvItem.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER))
                {
                    var longBytes = Convert.ToInt32(xTlvItem.strValue, 16);
                    longBytes++;
                    xTlvItem.baValue = Converter.StrToBcd(longBytes.ToString().PadLeft(6, '0'));


                }*/
                if (xTlvItem.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE))
                {
                   /* string strDate = DateTime.Now.ToString("yyMMdd");
                    TLVObject xTsmDate = new TLVObject(StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.SYSTEM_DATE), Converter.StrToBcd(strDate));
                    xTlvItem.baValue = xTsmDate.baValue;*/
                    continue;
                }
                if (xTlvItem.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME))
                {
                   /* string strDtime = DateTime.Now.ToString("HHMMss");
                    TLVObject xTsmTime = new TLVObject(StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.SYSTEM_TIME), Converter.StrToBcd(strDtime));
                    xTlvItem.baValue = xTsmTime.baValue;*/
                    continue;

                }
                xTlvBaseObject.baValue = xTlvItem.baValue;

                string strHexTerminalSerialNumberLength = Converter.IntToHexStr(xTlvBaseObject.baValue.Length);
                xTlvBaseObject.baLength = Converter.HexToByte(strHexTerminalSerialNumberLength);

                xlTlvBaseObjectList.Add(xTlvBaseObject);
            }



            if (prm_bAddAnswer)
            {

                TLVBaseObject xBaseProcessResponseCode = new TLVBaseObject();
                xBaseProcessResponseCode.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
                xBaseProcessResponseCode.strTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.RESPONSE_PROCESS_CODE);
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("00");

                EdipV2Dataset.ErrorDataDataTable dtErrorData = prm_dtEdipV2ErrorData;
                if (dtErrorData == null || dtErrorData.Rows.Count == 0)
                    xBaseProcessResponseCode.baValue = Converter.StrToAscii("00");
                else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.EJ_IS_NOT_RECOGNIZED).ToString())
                    xBaseProcessResponseCode.baValue = Converter.StrToAscii("03");
                else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.INVALID_COMMAND).ToString())
                    xBaseProcessResponseCode.baValue = Converter.StrToAscii("12");
                else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SEQUENCE_NUMBER_ERROR).ToString())
                    xBaseProcessResponseCode.baValue = Converter.StrToAscii("80");
                else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SYSTEM_TEMPORARY_CLOSED).ToString())
                    xBaseProcessResponseCode.baValue = Converter.StrToAscii("91");
                else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SYSTEM_ERROR).ToString())
                    xBaseProcessResponseCode.baValue = Converter.StrToAscii("96");
                string strProcessResponseCodeLength = Converter.IntToHexStr(xBaseProcessResponseCode.baValue.Length);
                xBaseProcessResponseCode.baLength = Converter.HexToByte(strProcessResponseCodeLength);


                xlTlvBaseObjectList.Add(xBaseProcessResponseCode);
            }
            return xlTlvBaseObjectList;


        }

        private byte[] baSendRequest(TLVParserV2 prm_xTlvParser, Enums.GMP_MESSAGE_TYPES prm_GibMessageType, EdipV2Dataset.ErrorDataDataTable prm_dtEdipV2ErrorData)
        {
            TLVDataObject xTerminalSerialNumber = prm_xTlvParser.m_xResult.ToArray<TLVDataObject>()[0].xChildren.Single(e=>e.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_SERIAL_NUMBER));
            TLVBaseObject xBaseTerminalSerialNumber = new TLVBaseObject();
            xBaseTerminalSerialNumber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalSerialNumber.strTag = xTerminalSerialNumber.strTag;
            xBaseTerminalSerialNumber.baValue = xTerminalSerialNumber.baValue;
            //xBaseTerminalSerialNumber.baValue = Converter.ToByteArray(dsEdipV2Dataset.PacketInfoV2[0].strDeviceSerialNumber);
            string strHexTerminalSerialNumberLength = Converter.IntToHexStr(xBaseTerminalSerialNumber.baValue.Length);
            xBaseTerminalSerialNumber.baLength = Converter.HexToByte(strHexTerminalSerialNumberLength);

            TLVDataObject xProcessNumber =  prm_xTlvParser.m_xResult.ToArray<TLVDataObject>()[0].xChildren.Single(e=>e.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER));
            TLVBaseObject xBaseProcessNUmber = new TLVBaseObject();
            xBaseProcessNUmber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseProcessNUmber.strTag = xProcessNumber.strTag;
            xBaseProcessNUmber.baValue = Converter.StrToBcd(xProcessNumber.strValue);
            string strHexProcessNUmberLength = Converter.IntToHexStr(xBaseProcessNUmber.baValue.Length);
            xBaseProcessNUmber.baLength = Converter.HexToByte(strHexProcessNUmberLength);

            TLVDataObject xTerminalDate =  prm_xTlvParser.m_xResult.ToArray<TLVDataObject>()[0].xChildren.Single(e=>e.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE));
            TLVBaseObject xBaseTerminalDate = new TLVBaseObject();
            xBaseTerminalDate.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalDate.strTag = xTerminalDate.strTag;
            xBaseTerminalDate.baValue = Converter.StrToBcd(xTerminalDate.strValue);
            string strHexTerminalDateLength = Converter.IntToHexStr(xBaseTerminalDate.baValue.Length);
            xBaseTerminalDate.baLength = Converter.HexToByte(strHexTerminalDateLength);

            TLVDataObject xTerminalTime =  prm_xTlvParser.m_xResult.ToArray<TLVDataObject>()[0].xChildren.Single(e=>e.strTag == StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME));
            TLVBaseObject xBaseTerminalTime = new TLVBaseObject();
            xBaseTerminalTime.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalTime.strTag = xTerminalTime.strTag;
            xBaseTerminalTime.baValue = Converter.StrToBcd(xTerminalTime.strValue);
            string strHexTerminalTimeLength = Converter.IntToHexStr(xBaseTerminalTime.baValue.Length);
            xBaseTerminalTime.baLength = Converter.HexToByte(strHexTerminalTimeLength);

            TLVBaseObject xBaseProcessResponseCode = new TLVBaseObject();
            xBaseProcessResponseCode.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseProcessResponseCode.strTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.RESPONSE_PROCESS_CODE);
            xBaseProcessResponseCode.baValue = Converter.StrToAscii("00");

            EdipV2Dataset.ErrorDataDataTable dtErrorData = prm_dtEdipV2ErrorData;
            if (dtErrorData == null || dtErrorData.Rows.Count == 0)
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("00");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.EJ_IS_NOT_RECOGNIZED).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("03");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.INVALID_COMMAND).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("12");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SEQUENCE_NUMBER_ERROR).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("80");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SYSTEM_TEMPORARY_CLOSED).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("91");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SYSTEM_ERROR).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("96");
            string strProcessResponseCodeLength = Converter.IntToHexStr(xBaseProcessResponseCode.baValue.Length);
            xBaseProcessResponseCode.baLength = Converter.HexToByte(strProcessResponseCodeLength);


            //xBaseTerminalID,
            //   xBaseProcessResponseCode, xBaseTerminalTime, xBaseTerminalDate, xBaseProcessNUmber, xBaseTerminalSerialNumber

            TLVBuilder xTlvBuilder = new TLVBuilder();
            return xTlvBuilder.baCreate(prm_GibMessageType,
                                        xBaseProcessResponseCode, xBaseTerminalTime, xBaseTerminalDate,
                                        xBaseProcessNUmber, xBaseTerminalSerialNumber);
        }

        public byte[] baCreateParameterRequestPacket(byte[] prm_baTLVData, string prm_strSerialNumber, string prm_strTPDU)
        {

            String xTLVMessage = Converter.ByteToHex(prm_baTLVData);

            StringBuilder strMessageData = new StringBuilder();
            byte[] baMessageData = new byte[2];

            int i = 17 + (xTLVMessage.Length / 2);
            if (i > 255)
            {
                baMessageData[0] = (byte)255;
                baMessageData[1] = (byte)(i - 255);
            }
            else
            {
                baMessageData[0] = (byte)0;
                baMessageData[1] = (byte)i;
            }

            strMessageData.Append(Converter.ByteToHex(baMessageData));

            strMessageData.Append(prm_strTPDU);

            strMessageData.Append(Converter.StrToHexStr(prm_strSerialNumber));

            strMessageData.Append(xTLVMessage);

            byte[] baLRCBody;
            byte[] baLrc8 = new byte[1];

            baLRCBody = Converter.HexToByte(strMessageData.ToString());
            int iLength = baLRCBody.Length + 1;
            Array.Resize<byte>(ref baLRCBody, iLength);
            baLRCBody[baLRCBody.Length - 1] = Lrc8.byteCalculateLrc(baLRCBody, 2, baLRCBody.Length - 3);

            return baLRCBody;
        }

        public byte[] baSendParameters(TLVParser prm_xTlvParser,
                                                EdipV2Dataset.ParameterDeviceDataDataTable prm_dtDeviceParameters,
                                                EdipV2Dataset.ParameterVatDataDataTable prm_dtVatParameters,
                                                EdipV2Dataset.ParameterDepartmentDataDataTable prm_dtDepartmentParameters,
                                                EdipV2Dataset.ParameterCurrencyDataDataTable prm_dtCurrencyParameters,
                                                EdipV2Dataset.ErrorDataDataTable prm_dtEdipV2ErrorData)
        {

          

            TLVBaseObject xBaseConnectionBlock = new TLVBaseObject();
            xBaseConnectionBlock.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_SECRET_DATA_GROUP);
            xBaseConnectionBlock.strTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.CONNECTION_BLOCK);
            xBaseConnectionBlock.baValue = baConnectionBlock(prm_dtDeviceParameters, prm_xTlvParser);
            string strConnectionBlockLength = Converter.IntToHexStr(xBaseConnectionBlock.baValue.Length);
            xBaseConnectionBlock.baLength = Converter.HexToByte(strConnectionBlockLength);




            TLVBaseObject xBaseParameterBlock = new TLVBaseObject();
            xBaseParameterBlock.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_SECRET_DATA_GROUP);
            xBaseParameterBlock.strTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.PARAMETER_BLOCK);
            xBaseParameterBlock.baValue = baParameterBlock(prm_dtDeviceParameters, prm_dtVatParameters, prm_dtDepartmentParameters);
            string strHexParameterBlockLength = Converter.IntToHexStr(xBaseParameterBlock.baValue.Length);
            xBaseParameterBlock.baLength = Converter.HexToByte(strHexParameterBlockLength);

            TLVBaseObject xBaseExchangeBlock = new TLVBaseObject();
            xBaseExchangeBlock.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_SECRET_DATA_GROUP);
            xBaseExchangeBlock.strTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.EXCHANGE_BLOCK);
            xBaseExchangeBlock.baValue = baExchangeBlock(prm_dtCurrencyParameters);
            string strHexExchangeBlockLength = Converter.IntToHexStr(xBaseExchangeBlock.baValue.Length);
            xBaseExchangeBlock.baLength = Converter.HexToByte(strHexExchangeBlockLength);

            TLVObject xTerminalID = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.TERMINAL_ID)];
            TLVBaseObject xBaseTerminalID = new TLVBaseObject();
            xBaseTerminalID.strParentTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.GIB_SECRET_DATA_GROUP);
            EdipV2Dataset.ParameterDeviceDataRow drDeviceParametersData = (EdipV2Dataset.ParameterDeviceDataRow)prm_dtDeviceParameters[0];
            xBaseTerminalID.strTag = StringEnum.GetStringValue(Enums.GMP_GIB_SECRET_DATA_GROUP.TERMINAL_ID);
            if (drDeviceParametersData.GIB_TERMINAL_ID != "")
            {
                xBaseTerminalID.baValue = Converter.StrToAscii(drDeviceParametersData.GIB_TERMINAL_ID.PadLeft(8, '0'));
                string strHexTerminalIDLength = Converter.IntToHexStr(xBaseTerminalID.baValue.Length);
                xBaseTerminalID.baLength = Converter.HexToByte(strHexTerminalIDLength);
            }
            else
            {
                xBaseTerminalID.baValue = xTerminalID.baValue;
                string strHexTerminalIDLength = Converter.IntToHexStr(xBaseTerminalID.baValue.Length);
                xBaseTerminalID.baLength = Converter.HexToByte(strHexTerminalIDLength);
            }

            TLVObject xTerminalSerialNumber = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_SERIAL_NUMBER)];
            TLVBaseObject xBaseTerminalSerialNumber = new TLVBaseObject();
            xBaseTerminalSerialNumber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalSerialNumber.strTag = xTerminalSerialNumber.strTag;
            xBaseTerminalSerialNumber.baValue = xTerminalSerialNumber.baValue;
            //xBaseTerminalSerialNumber.baValue = Converter.ToByteArray(dsEdipV2Dataset.PacketInfoV2[0].strDeviceSerialNumber);
            string strHexTerminalSerialNumberLength = Converter.IntToHexStr(xBaseTerminalSerialNumber.baValue.Length);
            xBaseTerminalSerialNumber.baLength = Converter.HexToByte(strHexTerminalSerialNumberLength);

            TLVObject xProcessNumber = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_NUMBER)];
            TLVBaseObject xBaseProcessNUmber = new TLVBaseObject();
            xBaseProcessNUmber.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseProcessNUmber.strTag = xProcessNumber.strTag;
            xBaseProcessNUmber.baValue = Converter.StrToBcd(xProcessNumber.strValue);
            string strHexProcessNUmberLength = Converter.IntToHexStr(xBaseProcessNUmber.baValue.Length);
            xBaseProcessNUmber.baLength = Converter.HexToByte(strHexProcessNUmberLength);

            TLVObject xTerminalDate = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE)];
            TLVBaseObject xBaseTerminalDate = new TLVBaseObject();
            xBaseTerminalDate.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalDate.strTag = xTerminalDate.strTag;
            xBaseTerminalDate.baValue = Converter.StrToBcd(xTerminalDate.strValue);
            string strHexTerminalDateLength = Converter.IntToHexStr(xBaseTerminalDate.baValue.Length);
            xBaseTerminalDate.baLength = Converter.HexToByte(strHexTerminalDateLength);

            TLVObject xTerminalTime = prm_xTlvParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME)];
            TLVBaseObject xBaseTerminalTime = new TLVBaseObject();
            xBaseTerminalTime.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseTerminalTime.strTag = xTerminalTime.strTag;
            xBaseTerminalTime.baValue = Converter.StrToBcd(xTerminalTime.strValue);
            string strHexTerminalTimeLength = Converter.IntToHexStr(xBaseTerminalTime.baValue.Length);
            xBaseTerminalTime.baLength = Converter.HexToByte(strHexTerminalTimeLength);

            TLVBaseObject xBaseProcessResponseCode = new TLVBaseObject();
            xBaseProcessResponseCode.strParentTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.PROCESS_GROUP);
            xBaseProcessResponseCode.strTag = StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.RESPONSE_PROCESS_CODE);
            EdipV2Dataset.ErrorDataDataTable dtErrorData = prm_dtEdipV2ErrorData;
            if (dtErrorData == null || dtErrorData.Rows.Count == 0)
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("00");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.EJ_IS_NOT_RECOGNIZED).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("03");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.INVALID_COMMAND).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("12");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SEQUENCE_NUMBER_ERROR).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("80");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SYSTEM_TEMPORARY_CLOSED).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("91");
            else if (dtErrorData[0].ERROR_CODE == ((int)Enums.GmpErrorCode.SYSTEM_ERROR).ToString())
                xBaseProcessResponseCode.baValue = Converter.StrToAscii("96");
            string strProcessResponseCodeLength = Converter.IntToHexStr(xBaseProcessResponseCode.baValue.Length);
            xBaseProcessResponseCode.baLength = Converter.HexToByte(strProcessResponseCodeLength);


            TLVBuilder xTlvBuilder = new TLVBuilder();
            return xTlvBuilder.baCreateParam(0, xBaseConnectionBlock, xBaseParameterBlock, xBaseExchangeBlock, xBaseTerminalID,
                 xBaseProcessResponseCode, xBaseTerminalTime, xBaseTerminalDate, xBaseProcessNUmber, xBaseTerminalSerialNumber);
        }


        private byte[] baParameterBlock(EdipV2Dataset.ParameterDeviceDataDataTable prm_dtDeviceParameters, EdipV2Dataset.ParameterVatDataDataTable prm_dtParameterVatData,
            EdipV2Dataset.ParameterDepartmentDataDataTable prm_dtParameterDepartmanData)
        {
            StringBuilder sbParameterBlock = new StringBuilder();
            EdipV2Dataset.ParameterDeviceDataRow drDeviceParametersData = (EdipV2Dataset.ParameterDeviceDataRow)prm_dtDeviceParameters[0];

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd("9999".PadLeft(4, '0'))));//drDeviceParametersData.OFFLINE_ACTIVE_DAY.PadLeft(4,'0'))));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.DAILY_Z_REPORT_UPLOADING_TIME)));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd("03")));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.GMP_Z_REPORT_SENDING_TRIAL_TIME.PadLeft(6, '0'))));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.DAILY_EVENT_LOGS_UPLOADING_HOUR)));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd("03")));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.GMP_EVENT_LOG_SENDING_TRIAL_TIME.PadLeft(6, '0'))));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.DAILY_RECEIPT_UPLOADING_TIME)));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd("03")));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.GMP_RECEIPT_SENDING_TRIAL_TIME.PadLeft(6, '0'))));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.EVENT_LOG_MINIMUM_CRITICAL_LEVEL.PadLeft(2, '0'))));

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.EVENT_LOG_MAXIMUM_CRITICAL_LEVEL.PadLeft(2, '0'))));

            if (drDeviceParametersData.SEND_Z_REPORT_WITHOUT_PASSWORD == "F")
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("H")));
            else
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("E")));

            if (drDeviceParametersData.SEND_RECEIPT_WITHOUT_PASSWORD == "F")
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("H")));
            else
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("E")));

            if (drDeviceParametersData.SEND_RECEIPT_CANCEL_WITHOUT_PASSWORD == "F")
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("H")));
            else
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("E")));

            if (drDeviceParametersData.SEND_EVENT_LOG_WITHOUT_PASSWORD == "F")
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("H")));
            else
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii("E")));


            for (int iVatRateIndex = 0; iVatRateIndex < Constants.EDIPv2_VAT_RATIO_COUNT; iVatRateIndex++)
            {
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(prm_dtParameterVatData[iVatRateIndex].VAT_RATE.Replace(",", "").PadLeft(4, '0'))));
            }

            sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(Constants.EDIPv2_DEPARTMENT_COUNT.ToString().PadLeft(4, '0'))));


            for (int iDepartmanIndex = 0; iDepartmanIndex < Constants.EDIPv2_DEPARTMENT_COUNT; iDepartmanIndex++)
            {
                string strDepartmentID = string.Empty;

                strDepartmentID = prm_dtParameterDepartmanData[iDepartmanIndex].DEPARTMENT_ID.PadLeft(4, '0');

                int iVatIndex = int.Parse(prm_dtParameterDepartmanData[iDepartmanIndex].DEPARTMENT_VAT_ID);
                string strVatRate = prm_dtParameterVatData[iVatIndex - 1].VAT_RATE.Replace(",", "");
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(strDepartmentID)));
                sbParameterBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(strVatRate)));
            }

            return Converter.HexToByte(Converter.StrToHexStr(sbParameterBlock.ToString()));
        }

        private byte[] baConnectionBlock(EdipV2Dataset.ParameterDeviceDataDataTable prm_dtDeviceParameters, TLVParser xTLVParser)
        {
            StringBuilder sbConnectionBlock = new StringBuilder();
            EdipV2Dataset.ParameterDeviceDataRow drDeviceParametersData = (EdipV2Dataset.ParameterDeviceDataRow)prm_dtDeviceParameters[0];

            TLVObject xTerminalDate = xTLVParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_DATE)];
            TLVObject xTerminalTime = xTLVParser.ObjectListMap[StringEnum.GetStringValue(Enums.GMP_PROCESS_GROUP.TERMINAL_TIME)];

            byte[] baDateTime = new byte[5];
            Array.Copy(xTerminalDate.baValue, 0, baDateTime, 0, 3);
            Array.Copy(xTerminalTime.baValue, 0, baDateTime, 3, 2);

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(baDateTime));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.FIRST_IP.PadLeft(12, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.FIRST_IP_GMP_PORT.PadLeft(6, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.SECOND_IP.PadLeft(12, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.SECOND_IP_GMP_PORT.PadLeft(6, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.FIRST_GMP_PHONE_NUMBER.PadLeft(10, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.SECOND_GMP_PHONE_NUMBER.PadLeft(10, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.FIRST_IP.PadLeft(12, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.FIRST_IP_GMP_PORT.PadLeft(6, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.SECOND_IP.PadLeft(12, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.SECOND_IP_GMP_PORT.PadLeft(6, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.FIRST_GMP_PHONE_NUMBER.PadLeft(10, '0'))));

            sbConnectionBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(drDeviceParametersData.SECOND_GMP_PHONE_NUMBER.PadLeft(10, '0'))));

            return Converter.HexToByte(Converter.StrToHexStr(sbConnectionBlock.ToString()));
        }

        private byte[] baExchangeBlock(EdipV2Dataset.ParameterCurrencyDataDataTable prm_dtCurrencyParameters)
        {
            StringBuilder sbExchangeBlock = new StringBuilder();

            sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(prm_dtCurrencyParameters.Rows.Count.ToString().PadLeft(2, '0'))));

            for (int iCurrecnyIndex = 0; iCurrecnyIndex < prm_dtCurrencyParameters.Rows.Count; iCurrecnyIndex++)
            {
                if (!prm_dtCurrencyParameters[iCurrecnyIndex].CURRENCY_NUMBER.Equals(""))
                    sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(prm_dtCurrencyParameters[iCurrecnyIndex].CURRENCY_NUMBER.ToString().PadLeft(4, '0'))));

                if (!prm_dtCurrencyParameters[iCurrecnyIndex].CURRENCY_CODE.Equals(""))
                    sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii(prm_dtCurrencyParameters[iCurrecnyIndex].CURRENCY_CODE)));


                if (!prm_dtCurrencyParameters[iCurrecnyIndex].CURRENCY_SIGN.Equals(""))
                    sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii(prm_dtCurrencyParameters[iCurrecnyIndex].CURRENCY_SIGN.PadLeft(3, '0'))));


                if (!prm_dtCurrencyParameters[iCurrecnyIndex].SIGN_DIRECTION.Equals(""))
                    sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii(prm_dtCurrencyParameters[iCurrecnyIndex].SIGN_DIRECTION)));

                sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii(prm_dtCurrencyParameters[iCurrecnyIndex].THOUSANDS_SEPERATOR)));

                sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToAscii(prm_dtCurrencyParameters[iCurrecnyIndex].PENNY_SEPERATOR)));

                sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.byteaIntegerToBcd(prm_dtCurrencyParameters[iCurrecnyIndex].NUMBER_OF_PENNY_DIGITS)));

                if (!prm_dtCurrencyParameters[iCurrecnyIndex].RATE_OF_CURRENCY.Equals(""))
                    sbExchangeBlock.Append(Converter.xTurkishEncoding.GetString(Converter.StrToBcd(prm_dtCurrencyParameters[iCurrecnyIndex].RATE_OF_CURRENCY.ToString().PadLeft(12, '0'))));
            }

            return Converter.HexToByte(Converter.StrToHexStr(sbExchangeBlock.ToString()));
        }





        
    }
}
