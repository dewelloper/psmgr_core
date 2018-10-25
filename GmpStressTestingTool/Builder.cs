using Edata.CommonLibrary;
using Edata.DataTypes.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmpStressTestingTool
{
    class Builder
    {
        //public byte[] baCreatePacketUsingDataset(Enums.MessageType prm_xMessageType, DateTime prm_xTransactionDateTime, string prm_strSerialNumber, EdipV2Dataset prm_xEdipV2Dataset, int prm_iCurrentPacketNumber, int prm_iTotalBodyLength, int prm_iPacketSize, int prm_iMessageSequenceNumber)
        //{
        //    switch (prm_xMessageType)
        //    {              
        //        case Enums.MessageType.:
        //            return baBuildLogoutDataReceived(prm_xTransactionDateTime, prm_strSerialNumber, prm_iMessageSequenceNumber, prm_xEdipV2Dataset.LogoutDataReceived, prm_iCurrentPacketNumber);
        //        default:
        //            return new byte[] { };
        //    }
        //}

        //public byte[] baBuildLogoutDataReceived(DateTime prm_xTransactionDateTime, string prm_strSerialNumber, int prm_iMessageSequenceNumber, EdipV2Dataset.LogoutDataReceivedDataTable prm_dtLogoutDataReceivedDataTable, int prm_iCurrentPacketNumber = 1)
        //{
        //    StringBuilder strBodyData = new StringBuilder();

        //    EdipV2Dataset.LogoutDataReceivedRow drLogoutDataReceivedRow = (EdipV2Dataset.LogoutDataReceivedRow)prm_dtLogoutDataReceivedDataTable[0];
        //    strBodyData.Append(FormatTypes.strFormatString(drLogoutDataReceivedRow.CASHIER_MESSAGE_LINE_1, 32, true));
        //    strBodyData.Append(Constants.Character.SEPERATOR_TAB);
        //    strBodyData.Append(FormatTypes.strFormatString(drLogoutDataReceivedRow.CASHIER_MESSAGE_LINE_2, 32, true));
        //    strBodyData.Append(Constants.Character.SEPERATOR_TAB);
        //    strBodyData.Append(FormatTypes.strFormatString(drLogoutDataReceivedRow.CASHIER_MESSAGE_LINE_3, 32, true));
        //    strBodyData.Append(Constants.Character.SEPERATOR_TAB);
        //    strBodyData.Append(FormatTypes.strFormatString(drLogoutDataReceivedRow.CASHIER_MESSAGE_LINE_4, 32, true));


        //    return baCreatePacket(Enums.MessageType.LOGOUT_DATA_RECEIVED, prm_xTransactionDateTime, prm_strSerialNumber, strBodyData.ToString(), prm_iCurrentPacketNumber, strBodyData.Length, m_iPacketSize, prm_iMessageSequenceNumber);
        //} // ok v2

    }
}
