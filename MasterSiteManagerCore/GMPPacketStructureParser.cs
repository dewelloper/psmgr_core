using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Edata.DataTypes;
using Edata.DataTypes.DataSets;
using Edata.CommonLibrary;
using Edata.DataValidity;

namespace MasterSiteManagerCore
{
    class GMPPacketStructureParser
    {
        private  const int MESSAGE_TYPE_END = 22;
        private  const int MESSAGE_TYPE_START = 19;
        private  const int TPDU_END = 7;
        private  const int TPDU_START = 2;
        private  const int TERMINAL_SERIAL_NUMBER_END = 19;
        private  const int TERMINAL_SERIAL_NUMBER_START = 7;
        private  const int MESSAGE_DATA_INDEX = 19;
        
        private const int m_iGMPHeaderSize = 19;
        private const int m_iGMPFooterSize = 1;

        private bool m_bIsGMPPacketSuccessfullyParsed = false;
        public bool bIsGMPPacketSuccessfullyParsed { get { return m_bIsGMPPacketSuccessfullyParsed; } }
  
        private int m_iGMPPacketLength = 0;
        public int iGMPPacketLength { get { return m_iGMPPacketLength; } }
  
        private byte[] m_baGMPTPDU = new byte[5]; 
        public byte[] baGMPTPDU { get { return m_baGMPTPDU; } }

        private byte[] m_baSerialNumber = new byte[12];
        public byte[] baSerialNumber { get { return m_baSerialNumber; } }

        private byte[] m_baGMPMessageData;
        public byte[] baGMPMessageData { get { return m_baGMPMessageData; } }


        public GMPPacketStructureParser(byte[] prm_baIncomingMessage, int prm_iIncomingMessageLength)
        {
            try
            {
                byte[] baDataTemp;
                byte[] baDataIncoming = new byte[prm_iIncomingMessageLength];

                Array.Copy(prm_baIncomingMessage, baDataIncoming, prm_iIncomingMessageLength);

                byte byteDataIncomingLrc = baDataIncoming[baDataIncoming.Length - 1];
                Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Incomming LRC:{0:X}", byteDataIncomingLrc);

                baDataTemp = new byte[1];

                byte byteCalculatedLrc = Lrc8.byteCalculateLrc(baDataIncoming, 2, baDataIncoming.Length - 3);
                Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Calculated LRC:{0:X}", byteCalculatedLrc);


                if (byteDataIncomingLrc == byteCalculatedLrc)
                {
                    baDataTemp = new byte[2];
                    Array.Copy(baDataIncoming, 0, baDataTemp, 0, 2);
                    m_iGMPPacketLength = (baDataIncoming[0] * (0xFF + 1)) + baDataIncoming[1];

                    m_baGMPTPDU = new byte[5];
                    Array.Copy(baDataIncoming, 2, m_baGMPTPDU, 0, 5);

                    m_baSerialNumber = new byte[12];
                    Array.Copy(baDataIncoming, 7, m_baSerialNumber, 0, 12);

                  
                    // Body Data
                    m_baGMPMessageData = new byte[prm_baIncomingMessage.Length - 20];
                    Array.Copy(baDataIncoming, 19, this.m_baGMPMessageData, 0, m_baGMPMessageData.Length);

                    baDataTemp = new byte[1];

                    m_bIsGMPPacketSuccessfullyParsed = true;

                    Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Incomming LRC is  equal to Calculated LRC. {0}={1}", byteDataIncomingLrc, byteCalculatedLrc);
                }
                else
                {
                    Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Incomming LRC is not equal to Calculated LRC. {0:X}!={1:X}", byteDataIncomingLrc, byteCalculatedLrc);
                    return;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
                return;
            }
        }

    }
}