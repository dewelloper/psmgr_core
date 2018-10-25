using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBScheduler
{
    public enum SchedulerInterval
    {
        HOURLY,
        HOUR,
        DAY,
        WEEK,
        MONTH
    }

    public class MESSAGE_COUNT
    {
        public string _id { get; set; }
        public BsonDocument value { get; set; }
    }

    public class TableName
    {
        public static string GetTableName(string prm_strApplicationName)
        {
            switch (prm_strApplicationName)
            {
                case "EdipListener":
                    return "T_MESSAGE_LOG";
                case "ParametersManager":
                    return "T_PARAMETERS_MANAGER_MESSAGE_LOG";
                case "ProtelDispatcher":
                    return "T_PROTEL_DISPATCHER_MESSAGE_LOG";
                case "VectronDispatcher":
                    return "T_VECTRON_DISPATCHER_MESSAGE_LOG";
                case "TradesoftDispatcher":
                    return "T_TRADESOFT_DISPATCHER_MESSAGE_LOG";
                case "MigrosDispatcher":
                    return "T_MIGROS_DISPATCHER_MESSAGE_LOG";
                case "Indemo":
                    return "T_INDEMO_MESSAGE_LOG";
                case "EdipDispatcher":
                    return "T_EDIP_DISPATCHER_MESSAGE_LOG";
                case "CitadelgateDispatcher":
                    return "T_CITADELGATE_DISPATCHER_MESSAGE_LOG";
                case "EdipDispatcherTov1":
                    return "T_EDIP_DISPATCHER_TO_V1_MESSAGE_LOG";
                case "DeviceManager":
                    return "T_DEVICE_MANAGER_MESSAGE_LOG";
                case "DRDispatcher":
                    return "T_DR_DISPATCHER_MESSAGE_LOG";
                case "DominosDispatcher":
                    return "T_DOMINOS_DISPATCHER_MESSAGE_LOG";                
            }
 
            return string.Empty;
        }
    }
}
