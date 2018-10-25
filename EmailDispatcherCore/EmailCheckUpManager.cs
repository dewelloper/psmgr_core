using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Edata.Protocol;
using Edata.Edip;
using Edata.DataTypes.DataSets;
using Edata.DataTypes;
using Edata.TcpIpClient;
using Edata.CommonLibrary;

namespace EmailDispatcherCore
{
    public class EmailCheckUpManager : IProtocol
    {
        private const int m_cnst_iProtocolBufferSize = Constants.COMMUNICATION_BUFFER_SIZE;

        public EmailCheckUpManager()
        {
        }

        public int iAnalyseIncommingMessageAndPrepareResponse(byte[] prm_abIncommingMessage, int prm_iIncommingMessageLength, out byte[] prm_abOutgoingMessage, out int prm_iOutgoingMessageLength, ref byte[] prm_baPacketCollectorBuffer, ref int prm_iPacketCollectorBufferLength, EndPoint prm_xRemoteEndPoint)
        {
            
            prm_abOutgoingMessage = null;
            prm_iOutgoingMessageLength = 0;

            EmailDispatcherManagerAnalyser xEmailDispatcherManagerAnalyser = new EmailDispatcherManagerAnalyser(prm_abIncommingMessage);

            int iReturnValue = xEmailDispatcherManagerAnalyser.iAnalyseIncommingMessageAndPrepareResponse(prm_abIncommingMessage, prm_iIncommingMessageLength, out prm_abOutgoingMessage,
                out prm_iOutgoingMessageLength, ref prm_baPacketCollectorBuffer, ref  prm_iPacketCollectorBufferLength);

            prm_iOutgoingMessageLength = prm_abOutgoingMessage.Length;
            prm_iPacketCollectorBufferLength = prm_baPacketCollectorBuffer.Length;

            return iReturnValue;
        }

      
    }
}

