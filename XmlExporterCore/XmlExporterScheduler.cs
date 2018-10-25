using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Edata.DataAccessLayer;
using Edata.CommonLibrary;
using Edata.DataTypes;
using Edata.DataTypes.DataSets;
using MongoDB.Bson;
using System.Data;

namespace XmlExporterCore
{
    class XmlExporterScheduler
    {
        public Thread m_xCheckZReportThread { get; set; }
        public Thread m_xCheckZReportExcelThread { get; set; }
        public Thread m_xBuildAndSendZReportThread { get; set; }
        private int m_iSleepTime = 15000;
          
        private class ThreadObject
        {
            public string m_strTerminalId  { get; set; }
            public string m_strZNo { get; set; }
        }

        public XmlExporterScheduler()
        {
            int.TryParse(Parameters.xGetInstance().strGetParameter(Parameters.xGetInstance().strGetModulName(), "SleepLength", "15000"), out m_iSleepTime);
        }

        public void vCheckZReport()
        {
            IEnumerable<BsonDocument> xBsonDocument;
            string strTerminalId = string.Empty;
            string strZNo = string.Empty;

            while (true)
            {
                try
                {
                    xBsonDocument = null;
                    MongoDBDAO.xGetInstance().GetZReportData(ref xBsonDocument);

                    foreach (BsonDocument item in xBsonDocument)
                    {
                        try
                        {
                            ThreadObject xThreadObject = new ThreadObject();
                            xThreadObject.m_strTerminalId = item.GetValue("_id")["TERMINAL_ID"].ToString();
                            xThreadObject.m_strZNo = item.GetValue("_id")["Z_NO"].ToString();
                            m_xBuildAndSendZReportThread = new Thread(new ParameterizedThreadStart(vBuildAndSendZReport));
                            m_xBuildAndSendZReportThread.Start(xThreadObject);
                        }
                        catch (Exception xException)
                        {
                            xException.TraceError();
                            continue;
                        }

                        Thread.Sleep(m_iSleepTime);
                    }
                }
                catch (Exception xException)
                {
                    Trace.vInsertError(xException);
                    continue;
                }

                Thread.Sleep(m_iSleepTime * 5);
            }
        }

        public void vCheckZReportExcel()
        {
            IEnumerable<BsonDocument> xBsonDocument;
            string strTerminalId = string.Empty;
            string strZNo = string.Empty;
            int iErrorCode = -1;

            while (true)
            {                 
                if (DateTime.Now.Hour != 15)
                {
                    Thread.Sleep(1000 * 60 * 60);
                    continue;
                }
                else if (DateTime.Now.Hour ==15)
                {
                    Thread.Sleep(1000 * 60 * 60);
                }

                try
                {
                    Trace.vInsertMethodTrace(enumTraceLevel.Normal, "vCheckZReportExcel THREAD");
                    xBsonDocument = null;
                    MongoDBDAO.xGetInstance().GetZReportData(ref xBsonDocument);
                  
                    foreach (BsonDocument item in xBsonDocument)
                    {      
                        try
                        {
                            EdipLogsDataset xEdiplogDataSet = null;
                            Edata.EdipXmlExporter.EdipXmlManager xEdipXmlManager = new Edata.EdipXmlExporter.EdipXmlManager();
                            ThreadObject xThreadObject = new ThreadObject();
                            xThreadObject.m_strTerminalId =item.GetValue("_id")["TERMINAL_ID"].ToString();
                            xThreadObject.m_strZNo = item.GetValue("_id")["Z_NO"].ToString();
                            strTerminalId = xThreadObject.m_strTerminalId;
                            strZNo = xThreadObject.m_strZNo;
                           

                            EdipV2Dataset xEdipV2DatasetParameters = DbFunctionDAO.xGetParametersV2(strTerminalId, false, false, ref iErrorCode);

                            if (xEdipV2DatasetParameters.ParameterTsmData[0].PARAMETER_ID != 54)
                                continue;

                            xEdiplogDataSet = DbFunctionDAO.xGetZReportDetailsMongoDB(strTerminalId, strZNo, ref xEdipV2DatasetParameters, ref iErrorCode);
                            if (iErrorCode != -1 || xEdiplogDataSet.T_Z_REPORT_DATA.Rows.Count <= 0)
                            {
                                DbFunctionDAO.bUpdateSendedZReportMongoDB(strTerminalId, strZNo, ref iErrorCode);
                                Trace.vInsertMethodTrace(enumTraceLevel.Normal, "vCheckZReport112");
                                return;
                            }

                            var bResult = xEdipXmlManager.bBuildAndSendZReportV3Xml(strTerminalId, xEdiplogDataSet);
                            if (bResult) DbFunctionDAO.bUpdateSendedZReportMongoDB(strTerminalId, strZNo, ref iErrorCode);
                        }
                    
                        catch (Exception xException)
                        {
                            xException.TraceError();
                            continue;
                        }

                        Thread.Sleep(m_iSleepTime);
                    }
                }
               
                catch (Exception xException)
                {
                    Trace.vInsertError(xException);
                    continue;
                }
            }
        }

        private void vBuildAndSendZReport(object prm_objParameters)
        {
            ThreadObject prm_xThreadObject = (ThreadObject)prm_objParameters;
            int prm_iErrorCode = -1;
            EdipLogsDataset xEdiplogDataSet = null;
            Edata.EdipXmlExporter.EdipXmlManager xEdipXmlManager = new Edata.EdipXmlExporter.EdipXmlManager();
            string strTerminalId = prm_xThreadObject.m_strTerminalId;
            string strZNo = prm_xThreadObject.m_strZNo;

            int iErrorCode = -1;

            EdipV2Dataset xEdipV2DatasetParameters = DbFunctionDAO.xGetParametersV2(strTerminalId, false, false, ref iErrorCode);

            if (xEdipV2DatasetParameters.ParameterTsmData[0].PARAMETER_ID == 54)
                return;

            if (xEdipV2DatasetParameters.ParameterTsmData[0].COLLECT_Z_REPORTS == false && xEdipV2DatasetParameters.ParameterTsmData[0].COLLECT_SALES_RECEIPTS == false)
            {
                DbFunctionDAO.bUpdateSendedZReportMongoDB(strTerminalId, strZNo, ref prm_iErrorCode);
                Trace.vInsertMethodTrace(enumTraceLevel.Normal, "vCheckZReport111"); 
                return;
            }

            xEdiplogDataSet = DbFunctionDAO.xGetZReportDetailsMongoDB(strTerminalId, strZNo, ref xEdipV2DatasetParameters, ref prm_iErrorCode);
            if (prm_iErrorCode != -1 || xEdiplogDataSet.T_Z_REPORT_DATA.Rows.Count <= 0)
            {
                DbFunctionDAO.bUpdateSendedZReportMongoDB(strTerminalId, strZNo, ref prm_iErrorCode);
                Trace.vInsertMethodTrace(enumTraceLevel.Normal, "vCheckZReport112");
                return;
            }

            var bResult = xEdipXmlManager.bBuildAndSendZReportV3Xml(strTerminalId, xEdiplogDataSet);
            if (bResult) DbFunctionDAO.bUpdateSendedZReportMongoDB(strTerminalId, strZNo, ref prm_iErrorCode);
        }

        private void vDone(EdipLogsDataset.T_Z_REPORT_DATARow xZreportItem)
        {
            int prm_iErrorCode = -1;
            Edata.EdipXmlExporter.EdipXmlManager xEdipXmlManager = new Edata.EdipXmlExporter.EdipXmlManager();

           // EdipLogsDataset xEdiplogDataSet = DbFunctionDAO.xGetZReportDetails(xZreportItem.TERMINAL_ID, xZreportItem.Z_NO, ref prm_iErrorCode);
            /*
            if (prm_iErrorCode != -1 || xEdiplogDataSet.T_Z_REPORT_DATA.Rows.Count <= 0) return;

            var bResult = xEdipXmlManager.bBuildAndSendZReportV3Xml(xZreportItem.TERMINAL_ID, xEdiplogDataSet);

            if (bResult) DbFunctionDAO.bUpdateSendedZReport(xZreportItem.TERMINAL_ID, xZreportItem.Z_NO, ref prm_iErrorCode);
            else return;*/

            EdipLogsDataset xEdiplogDataSet = null;
             xEdiplogDataSet = DbFunctionDAO.xGetZReportDetails(xZreportItem.TERMINAL_ID, xZreportItem.Z_NO, ref prm_iErrorCode);
            if (prm_iErrorCode != -1 || xEdiplogDataSet.T_Z_REPORT_DATA.Rows.Count <= 0)
            {
                return;
            }

            var bResult = xEdipXmlManager.bBuildAndSendZReportV3Xml(xZreportItem.TERMINAL_ID, xEdiplogDataSet);

            if (bResult) DbFunctionDAO.bUpdateSendedZReport(xZreportItem.TERMINAL_ID, xZreportItem.Z_NO, ref prm_iErrorCode);
            else return;
        }

        public void vStartThread()
        {
            m_xCheckZReportThread = new Thread(new ThreadStart(vCheckZReport));
            m_xCheckZReportThread.Start();

            m_xCheckZReportExcelThread = new Thread(new ThreadStart(vCheckZReportExcel));
            m_xCheckZReportExcelThread.Start();
        }

        public void vStopThread() 
        {
            if (m_xCheckZReportThread != null) m_xCheckZReportThread.Abort();

            m_xCheckZReportThread = null;

            if (m_xCheckZReportExcelThread != null) m_xCheckZReportExcelThread.Abort();            
           
            m_xCheckZReportExcelThread = null;
        }
    }
}
