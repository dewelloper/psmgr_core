using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using System.IO;
using System.Globalization;

using Edata.CommonLibrary;
using Edata.DataAccessLayer;
using Edata.DataTypes.DataSets;
using System.Reflection;
using System.Net;
using System.Xml;

namespace JobManagerServiceCore
{
    public class ScheduledTasks
    {
        Thread m_xUpdateVersionThread = null;
        Thread m_xDeleteUnnecassaryFolderThread = null;
        Thread m_xParseAndLogMessageThread = null;
        Thread m_xUpdateCurrencyRateThread = null;
        VerifoneAccountDataModel m_xVerifoneAccountDataModel;

        public ScheduledTasks()
        { 
        }

        ~ScheduledTasks()
        {
            bKillThreads();
        }

        public void vStartThread()
        {
            m_xUpdateVersionThread = new Thread(new ThreadStart(vUpdateVersion));
            m_xDeleteUnnecassaryFolderThread = new Thread(new ThreadStart(vDeleteUnnecassaryFolders));
            m_xParseAndLogMessageThread = new Thread(new ThreadStart(vParseAndLogMessage));
            m_xUpdateCurrencyRateThread = new Thread(new ThreadStart(vUpdateCurrencyRate));


            m_xUpdateVersionThread.Start();
            m_xDeleteUnnecassaryFolderThread.Start();
            m_xParseAndLogMessageThread.Start();
            m_xUpdateCurrencyRateThread.Start();
        }

        public bool bKillThreads()
        {
            if (m_xUpdateVersionThread != null)
                m_xUpdateVersionThread.Abort();
            m_xUpdateVersionThread = null;


            if (m_xDeleteUnnecassaryFolderThread != null)
                m_xDeleteUnnecassaryFolderThread.Abort();
            m_xDeleteUnnecassaryFolderThread = null;


            if (m_xParseAndLogMessageThread != null)
                m_xParseAndLogMessageThread.Abort();
            m_xParseAndLogMessageThread = null;


            if (m_xUpdateCurrencyRateThread != null)
                m_xUpdateCurrencyRateThread.Abort();
            m_xUpdateCurrencyRateThread = null;


            return true;
        }

        public void vUpdateVersion()
        {
            m_xVerifoneAccountDataModel = new VerifoneAccountDataModel(Convert.ToString(2), "edatauser1", "q1av53Kj");
            int iErrorCode = 0;
            int iIndexOfDevice = 0;
            DataTable dtDevices = null;
            DataTable dtApplicationVersion = null;
            string strBaseApplicationVersion = string.Empty;
            Convert.ToInt32(Parameters.xGetInstance().bSetParameter("LastIndexOfDevice", "0"));
            bool bFlagUpdateVersion = Convert.ToBoolean(Parameters.xGetInstance().strGetParameter("FlagUpdateVersion", "true"));

            List<string> strListApplicationNames = new List<string>();
            strListApplicationNames.Add("os");
            strListApplicationNames.Add("vmac");
            strListApplicationNames.Add("eos");
            strListApplicationNames.Add("ce");
            strListApplicationNames.Add("ceif");
            strListApplicationNames.Add("tcpip");
            strListApplicationNames.Add("pppdial");
            strListApplicationNames.Add("dialup");
            strListApplicationNames.Add("gprs");
            strListApplicationNames.Add("pppgsm");
            strListApplicationNames.Add("gsm");
            strListApplicationNames.Add("cla");
            strListApplicationNames.Add("midle");
            strListApplicationNames.Add("edipbase");
            strListApplicationNames.Add("mevtlog");
            strListApplicationNames.Add("sqlite");
            strListApplicationNames.Add("applmngr");
            strListApplicationNames.Add("EDATAECR");

            try
            {
                while (true)
                {
                    if (bFlagUpdateVersion)
                    {
                        string strStartUpdateVersionTime = Parameters.xGetInstance().strGetParameter("StartUpdateVersionTime", "10:00:00");
                        DateTime xDateTimeStart = new DateTime();
                        xDateTimeStart = DateTime.ParseExact(strStartUpdateVersionTime, "HH:mm:ss", CultureInfo.InvariantCulture);

                        string strStopUpdateVersionTime = Parameters.xGetInstance().strGetParameter("StopUpdateVersionTime", "23:00:00");
                        DateTime xDateTimeStop = new DateTime();
                        xDateTimeStop = DateTime.ParseExact(strStopUpdateVersionTime, "HH:mm:ss", CultureInfo.InvariantCulture);


                        if ((xDateTimeStart < DateTime.Now) && (DateTime.Now < xDateTimeStop))
                        {
                            dtDevices = new DataTable();
                            dtApplicationVersion = new DataTable();
                            dtDevices = DbFunctionDAO.dtGetDeviceListBCA(ref iErrorCode);
                           
                            for (int iIndex = Convert.ToInt32(Parameters.xGetInstance().iGetParameter("LastIndexOfDevice", "0")); iIndex < dtDevices.Rows.Count; iIndex++)
                            {
                                iIndexOfDevice = iIndex;

                                if (DateTime.Now < xDateTimeStop)
                                {
                                    Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Start to update versions.");
                                    string strTerminalNumber = string.Format("{0}", dtDevices.Rows[iIndex]["FISCAL_CODE_AND_NUMBER"]);
                                    dtApplicationVersion = UpdateVersion.xGetInstance().dtGetApplicationVersion(strTerminalNumber, m_xVerifoneAccountDataModel);

                                    if (dtApplicationVersion != null)
                                    {
                                        for (int iCountAppVersion = 0; iCountAppVersion < dtApplicationVersion.Rows.Count; iCountAppVersion++)
                                        {
                                            for (int iCountAppList = 0; iCountAppList < strListApplicationNames.Count; iCountAppList++)
                                            {
                                                if (strListApplicationNames[iCountAppList].ToString() == dtApplicationVersion.Rows[iCountAppVersion][0].ToString())
                                                {
                                                    string strApplicationName = (dtApplicationVersion.Rows[iCountAppVersion][0]).ToString().ToUpper();
                                                    strApplicationName = strApplicationName.Replace("ı", "i");
                                                    strApplicationName = strApplicationName.Replace("ü", "u");
                                                    strApplicationName = strApplicationName.Replace("ö", "o");
                                                    strApplicationName = strApplicationName.Replace("ş", "s");
                                                    strApplicationName = strApplicationName.Replace("ğ", "g");
                                                    strApplicationName = strApplicationName.Replace("İ", "I");
                                                    strApplicationName = strApplicationName.Replace("Ü", "U");
                                                    strApplicationName = strApplicationName.Replace("Ö", "O");
                                                    strApplicationName = strApplicationName.Replace("Ş", "S");
                                                    strApplicationName = strApplicationName.Replace("Ğ", "G");

                                                    string strApplicationVersion = (dtApplicationVersion.Rows[iCountAppVersion][1]).ToString().ToUpper();
                                                    strApplicationVersion = strApplicationVersion.Replace("ı", "i");
                                                    strApplicationVersion = strApplicationVersion.Replace("ü", "u");
                                                    strApplicationVersion = strApplicationVersion.Replace("ö", "o");
                                                    strApplicationVersion = strApplicationVersion.Replace("ş", "s");
                                                    strApplicationVersion = strApplicationVersion.Replace("ğ", "g");
                                                    strApplicationVersion = strApplicationVersion.Replace("İ", "I");
                                                    strApplicationVersion = strApplicationVersion.Replace("Ü", "U");
                                                    strApplicationVersion = strApplicationVersion.Replace("Ö", "O");
                                                    strApplicationVersion = strApplicationVersion.Replace("Ş", "S");
                                                    strApplicationVersion = strApplicationVersion.Replace("Ğ", "G");

                                                    int iIndexCharacter = strApplicationVersion.IndexOf("(");
                                                    if (iIndexCharacter > 0)
                                                    {
                                                        strBaseApplicationVersion = strApplicationVersion.Substring(0, iIndexCharacter);
                                                    }
                                                    else
                                                    {
                                                        strBaseApplicationVersion = strApplicationVersion;
                                                    }
                                                   
                                                    DbFunctionDAO.bUpdateApplicationVersion(Convert.ToString(dtDevices.Rows[iIndex]["FISCAL_CODE_AND_NUMBER"]), strApplicationName, strBaseApplicationVersion, ref iErrorCode);
                                                }
                                            }
                                        }

                                        if ((iIndexOfDevice + 1) == dtDevices.Rows.Count)
                                        {
                                            Convert.ToInt32(Parameters.xGetInstance().bSetParameter("LastIndexOfDevice", "0"));
                                        }
                                        else
                                        {
                                            Convert.ToInt32(Parameters.xGetInstance().bSetParameter("LastIndexOfDevice", (iIndexOfDevice + 1).ToString()));
                                        }
                                    }
                                }
                                System.Threading.Thread.Sleep(10);
                            }
                        }
                    }
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        public void vDeleteUnnecassaryFolders()
        {
            try
            {

                DataTable dtGetReportInformation = new DataTable();
                int iErrorCode = 0;

                bool bFlagDeleteFolders = Convert.ToBoolean(Parameters.xGetInstance().strGetParameter("FlagDeleteFolders", "true"));

                int iDaysBeforeDeleteXML = Int32.Parse(Parameters.xGetInstance().strGetParameter("DaysBeforeDeleteXML", "15"));
                int iDaysBeforeDeleteReport = Int32.Parse(Parameters.xGetInstance().strGetParameter("DaysBeforeDeleteReport", "15"));
                int iDaysBeforeDeleteTraces = Int32.Parse(Parameters.xGetInstance().strGetParameter("DaysBeforeDeleteTraces", "15"));

                string strPathXmlFolders = Parameters.xGetInstance().strGetParameter("PathXmlFolders", @"c:\\a");
                string strPathReportFolders = Parameters.xGetInstance().strGetParameter("PathReportFolders", @"c:\\a");
                string strPathTraceFolders = Parameters.xGetInstance().strGetParameter("PathTraceFolders", @"c:\\a");

                string strStartDeleteTime = Parameters.xGetInstance().strGetParameter("StartDeleteFoldersTime", "01:00:00");
                string strStopDeleteTime = Parameters.xGetInstance().strGetParameter("StopDeleteFoldersTime", "23:00:00");


                DateTime xDateTimeStopDeleteFolder = new DateTime();
                DateTime xDateTimeStartDeleteFolder = new DateTime();

                xDateTimeStopDeleteFolder = DateTime.ParseExact(strStopDeleteTime, "HH:mm:ss", CultureInfo.InvariantCulture);
                xDateTimeStartDeleteFolder = DateTime.ParseExact(strStartDeleteTime, "HH:mm:ss", CultureInfo.InvariantCulture);


                while (true)
                {

                    if (bFlagDeleteFolders)
                    {
                        dtGetReportInformation = DbFunctionDAO.xGetReportRequestInformation(ref iErrorCode);
                        for (int iCount = 0; iCount < dtGetReportInformation.Rows.Count; iCount++)
                        {
                            int iIndexChar = 0;
                            int iLastIndexOfString = 0;
                            string strParsOfPath = string.Empty;
                            strParsOfPath = dtGetReportInformation.Rows[iCount]["DOWNLOAD_LINK"].ToString();
                            strParsOfPath = strParsOfPath.Replace("/", @"\");

                            int iFirstIndexString = strParsOfPath.IndexOf(@"\Content");

                            while ((iIndexChar = strParsOfPath.IndexOf(@"\", iIndexChar)) != -1)
                            {
                                if (iIndexChar != -1)
                                {
                                    iLastIndexOfString = iIndexChar++;
                                }
                            }

                            strParsOfPath = strParsOfPath.Substring(iFirstIndexString, iLastIndexOfString - iFirstIndexString);

                            strPathReportFolders = (@"c:\" + strParsOfPath);

                            DeleteUnnecesarryFolders.xGetInstance().vDeleteFoldersWithExtension(strPathReportFolders, "xls", iDaysBeforeDeleteReport, xDateTimeStartDeleteFolder, xDateTimeStopDeleteFolder);
                        }

                        DeleteUnnecesarryFolders.xGetInstance().vDeleteFoldersWithExtension(strPathXmlFolders, "xml", iDaysBeforeDeleteXML, xDateTimeStartDeleteFolder, xDateTimeStopDeleteFolder);
                        DeleteUnnecesarryFolders.xGetInstance().vDeleteAllFolder(strPathTraceFolders, iDaysBeforeDeleteTraces, xDateTimeStartDeleteFolder, xDateTimeStopDeleteFolder);
                    }

                }

            }

            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        public void vParseAndLogMessage()
        {
            try
            {
                bool bFlagParseAndLogMessage = Convert.ToBoolean(Parameters.xGetInstance().strGetParameter("FlagParseAndLogMessage", "true"));
               
                if (bFlagParseAndLogMessage)
                {
                    string strStartParseAndLogMessageTime = Parameters.xGetInstance().strGetParameter("StartParseAndLogMessageTime", "10:00:00");
                    DateTime xDateTimeStart = new DateTime();
                    xDateTimeStart = DateTime.ParseExact(strStartParseAndLogMessageTime, "HH:mm:ss", CultureInfo.InvariantCulture);

                    string strStopParseAndLogMessageTime = Parameters.xGetInstance().strGetParameter("StopParseAndLogMessageTime", "23:45:00");
                    DateTime xDateTimeStop = new DateTime();
                    xDateTimeStop = DateTime.ParseExact(strStopParseAndLogMessageTime, "HH:mm:ss", CultureInfo.InvariantCulture);


                    if ((xDateTimeStart < DateTime.Now) && (DateTime.Now < xDateTimeStop))
                    {
                        Edata.Edip.Parser xEdipV1Parser = new Edata.Edip.Parser();
                        Edata.EdipV2.Parser xEdipV2Parser = new Edata.EdipV2.Parser();
                        EdipDataset xEdipDataset = new EdipDataset();
                        EdipV2Dataset xEdipV2Dataset = new EdipV2Dataset();
                        DataTable dtGetMessageInfo = new DataTable();
                        string prm_strInputText = string.Empty;
                        int iErrorCode = 0;
                        Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Start parse and log messages.");
                       

                        while (true)
                        { 
                            dtGetMessageInfo = DbFunctionDAO.xGetMessageInformation(ref iErrorCode);
                            if (dtGetMessageInfo==null)
                            {
                                Thread.Sleep(600000);
                                continue;
                            }
                            if (dtGetMessageInfo.Rows.Count > 0)
                            {
                                for (int iCount = 0; iCount < dtGetMessageInfo.Rows.Count; iCount++)
                                {
                                    prm_strInputText = dtGetMessageInfo.Rows[iCount]["MESSAGE"].ToString();
                                    string strTerminalID = dtGetMessageInfo.Rows[iCount]["TERMINAL_ID"].ToString().Trim();

                                    if (prm_strInputText.Trim() != string.Empty)
                                    {
                                                    DateTime xMessageDateTime = xMessageDateTime = Convert.ToDateTime(dtGetMessageInfo.Rows[iCount]["MSG_DATE"]);
                                                    if (prm_strInputText.Trim().StartsWith("0201"))
                                                    {
                                                        xEdipDataset = xEdipV1Parser.ParseMessage(prm_strInputText.Trim().GetByteArray(), prm_strInputText.Trim().GetByteArray().Length);
                                                         Edata.Edip.EdipDataConverterToversion2 xty = new Edata.Edip.EdipDataConverterToversion2();

                                                         xEdipV2Dataset = xty.ToEdipV2Dataset(xEdipDataset, false);

                                                        if (xEdipDataset.PacketInfo[0].bPacketIsSuccessfullyParsed == true)
                                                        {
                                                        }
                                                        else
                                                        {

                                                        }
                                                    }
                                                    else if (prm_strInputText.Trim().StartsWith("0202"))
                                                    {

                                            

                                                        xEdipV2Dataset = xEdipV2Parser.ParseMessage(prm_strInputText.Trim().GetByteArray(), prm_strInputText.Trim().GetByteArray().Length);
                                                    }
                                            if (xEdipV2Dataset.PacketInfoV2[0].bPacketIsSuccessfullyParsed == true)
                                            {

                                                switch ((Enums.MessageType)xEdipV2Dataset.PacketInfoV2[0].iCommandType)
                                                {
                                                    case Enums.MessageType.SEND_Z_REPORT:

                                                        EdipV2Dataset.ZReportDataDataTable xZReportDataDataTable = xEdipV2Dataset.ZReportData;
                                                        if (xZReportDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertZReportData(strTerminalID, xMessageDateTime, xZReportDataDataTable, ref iErrorCode);
                                                        xZReportDataDataTable.Clear();

                                                        EdipV2Dataset.ZReportCashierDataDataTable xZReportCashierDataDataTable = xEdipV2Dataset.ZReportCashierData;
                                                        if (xZReportCashierDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertZreportCashierData(strTerminalID,String.Empty, xMessageDateTime, xZReportCashierDataDataTable, ref iErrorCode);
                                                        xZReportCashierDataDataTable.Clear();

                                                        EdipV2Dataset.ZReportCurrencyDataDataTable xZReportCurrencyDataDataTable = xEdipV2Dataset.ZReportCurrencyData;
                                                        if (xZReportCurrencyDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertZreportCurrencyData(strTerminalID, String.Empty, xMessageDateTime, xZReportCurrencyDataDataTable, ref iErrorCode);
                                                        xZReportCurrencyDataDataTable.Clear();

                                                        EdipV2Dataset.ZReportDepartmentDataDataTable xZReportDepartmentDataDataTable = xEdipV2Dataset.ZReportDepartmentData;
                                                        if (xZReportDepartmentDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertZreportDepartmentData(strTerminalID, String.Empty, xMessageDateTime, xZReportDepartmentDataDataTable, ref iErrorCode);
                                                        xZReportDepartmentDataDataTable.Clear();

                                                        EdipV2Dataset.ZReportVatDataDataTable xZReportVatDataDataTable = xEdipV2Dataset.ZReportVatData;
                                                        if (xZReportVatDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertZreportVatData(strTerminalID, String.Empty, xMessageDateTime, xZReportVatDataDataTable, ref iErrorCode);
                                                        xZReportVatDataDataTable.Clear();

                                                        DbFunctionDAO.bUpdateMessageLog(dtGetMessageInfo.Rows[iCount]["ID"].ToString(), ref iErrorCode);
                                                        break;

                                                    case Enums.MessageType.SEND_CANCEL_RECEIPT:

                                                        EdipV2Dataset.CancelReceiptDataDataTable xCancelReceiptDataDataTable = xEdipV2Dataset.CancelReceiptData;
                                                        if (xCancelReceiptDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertCancelReceiptData(strTerminalID, xMessageDateTime, xCancelReceiptDataDataTable, ref iErrorCode);
                                                        xCancelReceiptDataDataTable.Clear();

                                                        DbFunctionDAO.bUpdateMessageLog(dtGetMessageInfo.Rows[iCount]["ID"].ToString(), ref iErrorCode);
                                                        break;

                                                    case Enums.MessageType.SEND_PAYMENT_DETAIL:

                                                        EdipV2Dataset.PaymentItemDataDataTable xPaymentItemDataDataTable = xEdipV2Dataset.PaymentItemData;
                                                        if (xPaymentItemDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertPaymentItemData(strTerminalID, String.Empty,String.Empty, xMessageDateTime, xPaymentItemDataDataTable, ref iErrorCode);
                                                        xPaymentItemDataDataTable.Clear();

                                                        EdipV2Dataset.SoldItemDataDataTable xSoldItemDataDataTable = xEdipV2Dataset.SoldItemData;
                                                        if (xSoldItemDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertSoldItemData(strTerminalID, String.Empty,String.Empty, xMessageDateTime, xSoldItemDataDataTable, ref iErrorCode);
                                                        xSoldItemDataDataTable.Clear();

                                                        EdipV2Dataset.ReceiptDataDataTable xReceiptDataDataTable = xEdipV2Dataset.ReceiptData;
                                                        EdipV2Dataset.T_RECEIPT_DATA_MESSAGE_LINESDataTable xReceiptMessageLineDataDataTable = xEdipV2Dataset.T_RECEIPT_DATA_MESSAGE_LINES;
                                                        if (xReceiptDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertReceiptData(strTerminalID, xMessageDateTime, xReceiptDataDataTable, ref iErrorCode);
                                                        xReceiptDataDataTable.Clear();

                                                        DbFunctionDAO.bUpdateMessageLog(dtGetMessageInfo.Rows[iCount]["ID"].ToString(), ref iErrorCode);
                                                        break;

                                                    case Enums.MessageType.SEND_INVOICE_PAYMENT_DETAIL:

                                                        xPaymentItemDataDataTable = xEdipV2Dataset.PaymentItemData;
                                                        if (xPaymentItemDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertPaymentItemData(strTerminalID, String.Empty, String.Empty, xMessageDateTime, xPaymentItemDataDataTable, ref iErrorCode);
                                                        xPaymentItemDataDataTable.Clear();

                                                        xReceiptDataDataTable = xEdipV2Dataset.ReceiptData;
                                                        xReceiptMessageLineDataDataTable = xEdipV2Dataset.T_RECEIPT_DATA_MESSAGE_LINES;
                                                        if (xReceiptDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertReceiptData(strTerminalID, xMessageDateTime, xReceiptDataDataTable, ref iErrorCode);
                                                        xReceiptDataDataTable.Clear();


                                                        DbFunctionDAO.bUpdateMessageLog(dtGetMessageInfo.Rows[iCount]["ID"].ToString(), ref iErrorCode);
                                                       
                                                        break;

                                                    case Enums.MessageType.SEND_TABLE_PAYMENT_DETAIL:

                                                        xPaymentItemDataDataTable = xEdipV2Dataset.PaymentItemData;
                                                        if (xPaymentItemDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertPaymentItemData(strTerminalID, String.Empty, String.Empty, xMessageDateTime, xPaymentItemDataDataTable, ref iErrorCode);
                                                        
                                                        xPaymentItemDataDataTable.Clear();

                                                        xSoldItemDataDataTable = xEdipV2Dataset.SoldItemData;
                                                        if (xSoldItemDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertSoldItemData(strTerminalID, String.Empty, String.Empty, xMessageDateTime, xSoldItemDataDataTable, ref iErrorCode);
                                                        
                                                        xSoldItemDataDataTable.Clear();

                                                        xReceiptDataDataTable = xEdipV2Dataset.ReceiptData;
                                                        xReceiptMessageLineDataDataTable = xEdipV2Dataset.T_RECEIPT_DATA_MESSAGE_LINES;
                                                        if (xReceiptDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertReceiptData(strTerminalID, xMessageDateTime, xReceiptDataDataTable, ref iErrorCode);
                                                       
                                                        xReceiptDataDataTable.Clear();

                                                        DbFunctionDAO.bUpdateMessageLog(dtGetMessageInfo.Rows[iCount]["ID"].ToString(), ref iErrorCode);
                                                       
                                                        break;

                                                    case Enums.MessageType.SEND_ORDER_PAYMENT_DETAIL:

                                                        xPaymentItemDataDataTable = xEdipV2Dataset.PaymentItemData;
                                                        if (xPaymentItemDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertPaymentItemData(strTerminalID, String.Empty, String.Empty, xMessageDateTime, xPaymentItemDataDataTable, ref iErrorCode);
                                                       
                                                        xPaymentItemDataDataTable.Clear();

                                                        xSoldItemDataDataTable = xEdipV2Dataset.SoldItemData;
                                                        if (xSoldItemDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertSoldItemData(strTerminalID, String.Empty, String.Empty, xMessageDateTime, xSoldItemDataDataTable, ref iErrorCode);
                                                      
                                                        xSoldItemDataDataTable.Clear();

                                                        xReceiptDataDataTable = xEdipV2Dataset.ReceiptData;
                                                        xReceiptMessageLineDataDataTable = xEdipV2Dataset.T_RECEIPT_DATA_MESSAGE_LINES;
                                                        if (xReceiptDataDataTable.Rows.Count > 0)
                                                            DbFunctionDAO.bInsertReceiptData(strTerminalID, xMessageDateTime, xReceiptDataDataTable, ref iErrorCode);
                                                       
                                                        xReceiptDataDataTable.Clear();

                                                        DbFunctionDAO.bUpdateMessageLog(dtGetMessageInfo.Rows[iCount]["ID"].ToString(), ref iErrorCode);
                                                        break;

                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                    //}
                                    else
                                    {
                                        continue;
                                    }
                                    Thread.Sleep(10);
                                    xEdipDataset.Clear();

                                }
                            }
                        }
                    }
                }
            }

            catch (Exception xException)
            {
                xException.TraceError();
            }

        }

        public void vUpdateCurrencyRate()
        {
            int iErrorCode = 0;
            DataSet ds = new DataSet();
            DataTable dtUpdateCurrencyRate = null;
            DataRow drUpdateCurrencyRate = null;
            CultureInfo[] cultures = { new CultureInfo("en-US") };

            try
            {
                while (true)
                {

                    string strStartUpdateVersionTime = Parameters.xGetInstance().strGetParameter("StartUpdateCurrencyRate", "05:00:00");
                    DateTime xDateTimeStart = new DateTime();
                    xDateTimeStart = DateTime.ParseExact(strStartUpdateVersionTime, "HH:mm:ss", CultureInfo.InvariantCulture);

                    string strStopUpdateVersionTime = Parameters.xGetInstance().strGetParameter("StopUpdateCurrencyRate", "23:00:00");
                    DateTime xDateTimeStop = new DateTime();
                    xDateTimeStop = DateTime.ParseExact(strStopUpdateVersionTime, "HH:mm:ss", CultureInfo.InvariantCulture);


                    if ((xDateTimeStart < DateTime.Now) && (DateTime.Now < xDateTimeStop))
                    {
                        dtUpdateCurrencyRate = new DataTable();

                        HttpWebRequest xWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://www.tcmb.gov.tr/kurlar/today.xml");
                        HttpWebResponse xWebResponse = (HttpWebResponse)xWebRequest.GetResponse();
                        Stream xStream = xWebResponse.GetResponseStream();
                        XmlTextReader xTextReader = new XmlTextReader(xStream);
                        ds.ReadXml(xTextReader); 
                        dtUpdateCurrencyRate = ds.Tables[1];


                        if (DateTime.Now < xDateTimeStop)
                        {
                            for (int i = 0; i <= dtUpdateCurrencyRate.Rows.Count; i++)
                            {
                                drUpdateCurrencyRate = ds.Tables[1].Rows[i];

                                foreach (CultureInfo culture in cultures)
                                {
                                    bool bReturn = DbFunctionDAO.bUpdateCurrencyRate(ref iErrorCode, drUpdateCurrencyRate[11].ToString(), drUpdateCurrencyRate[2].ToString(),
                                      Convert.ToDecimal(drUpdateCurrencyRate[3].ToString(), culture), Convert.ToDecimal(drUpdateCurrencyRate[4].ToString(), culture), DateTime.Now);  

                                    if (bReturn)
                                        Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Currency Rate guncellendi");
                                    else
                                        Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Update gerceklestirilemedi");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

    }
}

