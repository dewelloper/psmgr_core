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

namespace JobManagerServiceCore
{
    public class JobManager : IProtocol
    {
        private const int m_cnst_iProtocolBufferSize = Constants.COMMUNICATION_BUFFER_SIZE;

        public JobManager()
        {
        }

        public int iAnalyseIncommingMessageAndPrepareResponse(byte[] prm_baIncommingMessage, int prm_iIncommingMessageLength, out byte[] prm_baOutgoingMessage, out int prm_iOutgoingMessageLength, ref byte[] prm_baPacketCollectorBuffer, ref int prm_iPacketCollectorBufferLength, EndPoint prm_xRemoteEndPoint)
        {

            prm_baOutgoingMessage = null;
            prm_iOutgoingMessageLength = 0;

            JobManagerServiceAnalyser xJobManagerServiceAnalyser = new JobManagerServiceAnalyser(prm_baIncommingMessage);

            int iReturnValue = xJobManagerServiceAnalyser.iAnalyseIncommingMessageAndPrepareResponse(prm_baIncommingMessage, prm_iIncommingMessageLength, out prm_baOutgoingMessage,
                out prm_iOutgoingMessageLength, ref prm_baPacketCollectorBuffer, ref  prm_iPacketCollectorBufferLength);

            prm_iOutgoingMessageLength = prm_baOutgoingMessage.Length;
            prm_iPacketCollectorBufferLength = prm_baPacketCollectorBuffer.Length;

            return iReturnValue;
        }


    }
}