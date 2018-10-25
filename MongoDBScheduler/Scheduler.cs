using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edata.DataAccessLayer;
using Edata.CommonLibrary;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;
using MongoDB.Driver.Builders;

namespace MongoDBScheduler
{
    public class Scheduler
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ExecuteMessageCount();
                ApplicationStatisticsHourly();
                ApplicationStatisticsByMessageType();
            }
            else if (args.Length > 0 && args[0].ToLower() == "executeparametermessagecount")
            {
                ExecuteParameterManagerMessageCount();
            }
        }

        private static void ExecuteMessageCount()
        {
            MongoDBDAO.xGetInstance().ExecuteMessageCountScheduler();

            var connectionString = Parameters.xGetInstance().strGetParameter("ConnectionStringMongoDB", "");
            var mongoClient = new MongoClient(connectionString);
            var mongoServer = mongoClient.GetServer();
            var databaseName = Parameters.xGetInstance().strGetParameter("CommunicationLogsDatabaseName", "CYBERCOM_COMMUNICATION_LOGS");
            var db = mongoServer.GetDatabase(databaseName);

            var collection = db.GetCollection("T_MESSAGE_LOG_MESSAGE_COUNT");
            string strApplicationName = "EdipListener";
            int iMessageCount = 0;
            int iIsIncomingMessage = 1;
            int iErrorCode = -1;

            foreach (var xBsonDocument in collection.FindAllAs<MESSAGE_COUNT>())
            {
                BsonElement xBsonElement = xBsonDocument.value.GetElement(0);
                int.TryParse(xBsonElement.Value.ToString(), out iMessageCount);

                if (xBsonDocument._id.EndsWith("HOUR"))
                {
                    DbFunctionDAO.bUpdateMessageCount(strApplicationName, SchedulerInterval.HOUR.ToString(), iMessageCount, iIsIncomingMessage, null, ref iErrorCode);
                }
                //else if (xBsonDocument._id.EndsWith("DAY"))
                //{
                //    DbFunctionDAO.bUpdateMessageCount(strApplicationName, SchedulerInterval.DAY.ToString(), iMessageCount, iIsIncomingMessage, null, ref iErrorCode);
                //}
                //else if (xBsonDocument._id.EndsWith("WEEK"))
                //{
                //    DbFunctionDAO.bUpdateMessageCount(strApplicationName, SchedulerInterval.WEEK.ToString(), iMessageCount, iIsIncomingMessage, null, ref iErrorCode);
                //}
                //else if (xBsonDocument._id.EndsWith("MONTH"))
                //{
                //    DbFunctionDAO.bUpdateMessageCount(strApplicationName, SchedulerInterval.MONTH.ToString(), iMessageCount, iIsIncomingMessage, null, ref iErrorCode);
                //}
            }
        }

        private static void ApplicationStatisticsHourly()
        {
            int iErrorCode = -1;
            string strApplicationName = string.Empty;

            try
            {
                DataTable dtApplicationNames = DbFunctionDAO.GetApplicationNames(ref iErrorCode);

                if (dtApplicationNames != null && dtApplicationNames.Rows.Count > 0)
                {
                    for (int iIndex = 0; iIndex < dtApplicationNames.Rows.Count; iIndex++)
                    {
                        if (dtApplicationNames.Rows[iIndex]["HAVE_STATISTICS"].ToString() == "True")
                        {
                            strApplicationName = dtApplicationNames.Rows[iIndex]["APP_NAME"].ToString();
                            string strTableName = TableName.GetTableName(strApplicationName);

                            if (!string.IsNullOrEmpty(strTableName))
                                MongoDBDAO.xGetInstance().ApplicationStatisticsHourly(strTableName);
                        }
                    }
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }

            try
            {
                var connectionString = Parameters.xGetInstance().strGetParameter("ConnectionStringMongoDB", "");
                var mongoClient = new MongoClient(connectionString);
                var mongoServer = mongoClient.GetServer();
                var databaseName = Parameters.xGetInstance().strGetParameter("CommunicationLogsDatabaseName", "CYBERCOM_COMMUNICATION_LOGS");
                var db = mongoServer.GetDatabase(databaseName);

                var collection = db.GetCollection("T_APPLICATION_STATISTICS");

                int iMessageCount = 0;
                int iIsIncomingMessage = 0;
                DateTime dateTimeMessageDate = new DateTime();
                MongoCursor<BsonDocument> xList = collection.Find(Query.GT("value.MSG_DATE", DateTime.Now.AddHours(-2)));

                foreach (var xBsonDocument in xList)
                {
                    string[] strApplication = xBsonDocument["_id"].ToString().Split('|');
                    strApplicationName = strApplication[0];
                    iIsIncomingMessage = strApplication[1] == "true" ? 1 : 0;
                    dateTimeMessageDate = new DateTime(Convert.ToInt32(strApplication[2].Split('-')[0]), Convert.ToInt32(strApplication[2].Split('-')[1]), Convert.ToInt32(strApplication[2].Split('-')[2]), Convert.ToInt32(strApplication[2].Split('-')[3]), 0, 0);
                    int.TryParse(xBsonDocument["value"]["TOTAL_MESSAGE_COUNT"].ToString(), out iMessageCount);
                    DbFunctionDAO.bUpdateMessageCount(strApplicationName, SchedulerInterval.HOURLY.ToString(), iMessageCount, iIsIncomingMessage, dateTimeMessageDate, ref iErrorCode);
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        private static void ApplicationStatisticsByMessageType()
        {
            int iErrorCode = -1;
            string strApplicationName = string.Empty;

            try
            {
                DataTable dtApplicationNames = DbFunctionDAO.GetApplicationNames(ref iErrorCode);

                if (dtApplicationNames != null && dtApplicationNames.Rows.Count > 0)
                {
                    for (int iIndex = 0; iIndex < dtApplicationNames.Rows.Count; iIndex++)
                    {
                        if (dtApplicationNames.Rows[iIndex]["HAVE_STATISTICS"].ToString() == "True")
                        {
                            strApplicationName = dtApplicationNames.Rows[iIndex]["APP_NAME"].ToString();
                            string strTableName = TableName.GetTableName(strApplicationName);

                            if (!string.IsNullOrEmpty(strTableName))
                                MongoDBDAO.xGetInstance().ApplicationStatisticsByMessageType(strTableName);
                        }
                    }
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }

            try
            {
                var connectionString = Parameters.xGetInstance().strGetParameter("ConnectionStringMongoDB", "");
                var mongoClient = new MongoClient(connectionString);
                var mongoServer = mongoClient.GetServer();
                var databaseName = Parameters.xGetInstance().strGetParameter("CommunicationLogsDatabaseName", "CYBERCOM_COMMUNICATION_LOGS");
                var db = mongoServer.GetDatabase(databaseName);

                var collection = db.GetCollection("T_APPLICATION_STATISTICS_BY_MESSAGE_TYPE");

                int iMessageCount = 0;
                string strMessageType = string.Empty;
                DateTime dateTimeMessageDate = new DateTime();
                MongoCursor<BsonDocument> xList = collection.Find(Query.GT("value.MSG_DATE", DateTime.Now.AddHours(-2)));

                foreach (var xBsonDocument in xList)
                {
                    string[] strApplication = xBsonDocument["_id"].ToString().Split('|');
                    strApplicationName = strApplication[0];
                    strMessageType = strApplication[1];
                    dateTimeMessageDate = new DateTime(Convert.ToInt32(strApplication[2].Split('-')[0]), Convert.ToInt32(strApplication[2].Split('-')[1]), Convert.ToInt32(strApplication[2].Split('-')[2]), Convert.ToInt32(strApplication[2].Split('-')[3]), 0, 0);
                    int.TryParse(xBsonDocument["value"]["TOTAL_MESSAGE_COUNT"].ToString(), out iMessageCount);
                    DbFunctionDAO.bUpdateMessageCount(strApplicationName, strMessageType, iMessageCount, 1, dateTimeMessageDate, ref iErrorCode);
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        private static void ExecuteParameterManagerMessageCount()
        {
            int iMessageCount = MongoDBDAO.xGetInstance().ExecuteParameterManagerMessageCountScheduler();
            int iErrorCode = -1;
            DbFunctionDAO.bUpdateMessageCount("ParametersManager", SchedulerInterval.DAY.ToString(), iMessageCount, 1, null, ref iErrorCode);

            try
            {
                var connectionString = Parameters.xGetInstance().strGetParameter("ConnectionStringMongoDB", "");
                var mongoClient = new MongoClient(connectionString);
                var mongoServer = mongoClient.GetServer();
                var databaseName = Parameters.xGetInstance().strGetParameter("CommunicationLogsDatabaseName", "CYBERCOM_COMMUNICATION_LOGS");
                var db = mongoServer.GetDatabase(databaseName);

                var collection = db.GetCollection("T_PARAMETERS_MANAGER_MESSAGE_LOG_COUNT");

                MongoCursor<BsonDocument> xList = collection.FindAll();

                foreach (var xBsonDocument in xList)
                {
                    string strTerminalId = xBsonDocument["_id"].ToString();

                    DbFunctionDAO.bInsertParameterDownloadedDevice(strTerminalId, ref iErrorCode);
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
