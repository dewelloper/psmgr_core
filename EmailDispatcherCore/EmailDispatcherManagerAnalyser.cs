using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edata.DataTypes.DataSets;
using Edata.TcpIpClient;
using Edata.DataTypes;
using Edata.CommonLibrary;
using Edata.EdipV2;

namespace EmailDispatcherCore
{
    class EmailDispatcherManagerAnalyser
    {
        const int m_cnst_iProtocolBufferSize = 4096;
        Builder m_xEDIPBuilder = null;
        EdipV2Dataset m_xEdipV2Dataset = null;
        SocketClient m_xSocketClient = null;
        EdipV2Dataset.PacketInfoV2Row m_xPacketInfoWrapper = null;
        EdipCheckUpResponseMessage m_xEdipCheckUpResponseMessage = null;
        byte[] m_byteaIncommingMessage;


        public EmailDispatcherManagerAnalyser(byte[] prm_byteaIncommingMessage)
        {
            m_byteaIncommingMessage = prm_byteaIncommingMessage;
            m_xEdipV2Dataset = Parser.xGetInstance().ParseMessage(m_byteaIncommingMessage, m_byteaIncommingMessage.Length);
            m_xPacketInfoWrapper = m_xEdipV2Dataset.PacketInfoV2[0];
            m_xSocketClient = new SocketClient();
        }

        public int iAnalyseIncommingMessageAndPrepareResponse(byte[] prm_abIncommingMessage, int prm_iIncommingMessageLength, out byte[] prm_baOutgoingMessage, out int prm_iOutgoingMessageLength, ref byte[] prm_baPacketCollectorBuffer, ref int prm_iPacketCollectorBufferLength)
        {
            prm_baOutgoingMessage = null;
            prm_iOutgoingMessageLength = 0;
            string strThreadID = m_xSocketClient.m_strThreadId;

            try
            {
                m_xEDIPBuilder = new Builder();

                if (m_xPacketInfoWrapper.bPacketIsSuccessfullyParsed == true)//GELEN VERİ PAKETİ SAĞLAMSA
                {
                    if (m_xPacketInfoWrapper.iCommandType == (int)Enums.MessageType.CHECK_UP)
                    {
                        m_xEdipCheckUpResponseMessage = new EdipCheckUpResponseMessage();
                        prm_baOutgoingMessage = m_xEdipCheckUpResponseMessage.baPrepareCheckUpResponseData(m_xEdipV2Dataset);
                    }
                }

                return 0;
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
            return -1;
        }

    }
}
