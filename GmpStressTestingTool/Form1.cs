using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Edata.CommonLibrary;
using Edata.GMP;

namespace GmpStressTestingTool
{
    public partial class Form1 : Form
    {
        Thread[] m_xaThread = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            m_xaThread = new Thread[Convert.ToInt32(txtConnectionCount.Value)];

            SendCommandThreadParameterType xSendCommandThreadParameterType;

            for (int iIndex = 0; iIndex < txtConnectionCount.Value; iIndex++)
            {                
                xSendCommandThreadParameterType = new SendCommandThreadParameterType();
                xSendCommandThreadParameterType.iThreadIndex = iIndex;
                xSendCommandThreadParameterType.iPacketSize = Convert.ToInt16(maskedTextBoxPacketSize.Text);
                xSendCommandThreadParameterType.strFiscalCode = txtFiscalcode.Text;
                xSendCommandThreadParameterType.iFiscalNumberStart = Convert.ToInt32(textBoxFiscalNumberStart.Text);
                xSendCommandThreadParameterType.iFiscalNumberEnd = Convert.ToInt32(textBoxFiscalNumberEnd.Text);
                xSendCommandThreadParameterType.strDestinationIPAddress = cbDestinationIPAddress.Text;
                xSendCommandThreadParameterType.iDestinationPort = int.Parse(txtDestinationPort.Text);

                m_xaThread[iIndex] = new Thread(new ParameterizedThreadStart(vSendCommandThreadRoutine));
                m_xaThread[iIndex].Start(xSendCommandThreadParameterType);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            vStop();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            vStop();
        }

        private void vStop()
        {
            btnStop.Enabled = false;
            btnStart.Enabled = true;

            for (int iIndex = 0; iIndex < txtConnectionCount.Value; iIndex++)
            {
                if (m_xaThread[iIndex].IsAlive) m_xaThread[iIndex].Abort();
            }
        }

        private void vSendCommandThreadRoutine(object prm_objParameters)
        {
            try
            {
                SendCommandThreadParameterType xSendCommandThreadParameterType = (SendCommandThreadParameterType)prm_objParameters;

                GmpEcrSimulator xEdip2EcrSimulator = new GmpEcrSimulator(xSendCommandThreadParameterType.iThreadIndex, xSendCommandThreadParameterType.strDestinationIPAddress, xSendCommandThreadParameterType.iDestinationPort, xSendCommandThreadParameterType.iPacketSize);

                int iResponseValue = 0;
                int iFiscalNumber = xSendCommandThreadParameterType.iFiscalNumberStart;

                while (true)
                {
                    string strFiscalCodeAndNumber = string.Format("{0}{1:00000000}", xSendCommandThreadParameterType.strFiscalCode, iFiscalNumber);

                    //iResponseValue = xEdip2EcrSimulator.iSendDeviceCertificate(strFiscalCodeAndNumber);
                    //vUpdateCounters(Enums.GMP_MESSAGE_TYPES.REQUEST_CERTIFICATE_MESSAGE, iResponseValue);
                    //Thread.Sleep(100);

                    //iResponseValue = xEdip2EcrSimulator.iSendTDKRequest(strFiscalCodeAndNumber);
                    //vUpdateCounters(Enums.GMP_MESSAGE_TYPES.REQUEST_TDK_MESSAGE, iResponseValue);
                    //Thread.Sleep(100);

                    //iResponseValue = xEdip2EcrSimulator.iSendTrekTrakRequest(strFiscalCodeAndNumber);
                    //vUpdateCounters(Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, iResponseValue);
                    //Thread.Sleep(100);

                    iResponseValue = xEdip2EcrSimulator.iSendCheckDevice(strFiscalCodeAndNumber);
                    vUpdateCounters(Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, iResponseValue);
                    Thread.Sleep(100);

                    iResponseValue = xEdip2EcrSimulator.iRequestParameters(strFiscalCodeAndNumber);
                    vUpdateCounters(Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, iResponseValue);
                    Thread.Sleep(100);

                    iResponseValue = xEdip2EcrSimulator.iRequestFullParameters(strFiscalCodeAndNumber);
                    vUpdateCounters(Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE, iResponseValue);
                    Thread.Sleep(100);

                    if (iFiscalNumber < xSendCommandThreadParameterType.iFiscalNumberEnd)
                        iFiscalNumber++;
                    else
                        iFiscalNumber = xSendCommandThreadParameterType.iFiscalNumberStart;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        delegate void vUpdateCountersDelegate(Enums.GMP_MESSAGE_TYPES prm_enumMessageType, int prm_iResponseValue);
        void vUpdateCounters(Enums.GMP_MESSAGE_TYPES prm_enumMessageType, int prm_iResponseValue)
        {          
            if (this.InvokeRequired == true)
            {
                this.Invoke(new vUpdateCountersDelegate(vUpdateCounters), new object[] { prm_enumMessageType, prm_iResponseValue });
                return;
            }

            try
            {
                switch (prm_enumMessageType)
                {
                    case Enums.GMP_MESSAGE_TYPES.REQUEST_CERTIFICATE_MESSAGE:
                        lblCacheList.Text = string.Format("{0}", Caching.xListCache().Count);
                        if (prm_iResponseValue == 0)
                            lblReceiveSuccessREQUEST_CERTIFICATE_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveSuccessREQUEST_CERTIFICATE_MESSAGE.Text) + 1);
                        else if (prm_iResponseValue == 1)
                            lblReceiveErrorREQUEST_CERTIFICATE_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveErrorREQUEST_CERTIFICATE_MESSAGE.Text) + 1);
                        else
                            lblReceiveNullREQUEST_CERTIFICATE_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveNullREQUEST_CERTIFICATE_MESSAGE.Text) + 1);
                        break;
                    case Enums.GMP_MESSAGE_TYPES.REQUEST_TDK_MESSAGE:
                        lblCacheList.Text = string.Format("{0}", Caching.xListCache().Count);
                        if (prm_iResponseValue == 0)
                            lblReceiveSuccessREQUEST_TDK_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveSuccessREQUEST_TDK_MESSAGE.Text) + 1);
                        else if (prm_iResponseValue == 1)
                            lblReceiveErrorREQUEST_TDK_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveErrorREQUEST_TDK_MESSAGE.Text) + 1);
                        else
                            lblReceiveNullREQUEST_TDK_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveNullREQUEST_TDK_MESSAGE.Text) + 1);
                        break;
                    case Enums.GMP_MESSAGE_TYPES.REQUEST_EDIP_MESSAGE:
                        lblCacheList.Text = string.Format("{0}", Caching.xListCache().Count);
                        if (prm_iResponseValue == 0)
                            lblReceiveSuccessREQUEST_EDIP_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveSuccessREQUEST_EDIP_MESSAGE.Text) + 1);
                        else if (prm_iResponseValue == 1)
                            lblReceiveErrorREQUEST_EDIP_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveErrorREQUEST_EDIP_MESSAGE.Text) + 1);
                        else
                            lblReceiveNullREQUEST_EDIP_MESSAGE.Text = string.Format("{0}", int.Parse(lblReceiveNullREQUEST_EDIP_MESSAGE.Text) + 1);
                        break;
                }
            }
            catch
            {
            }
        }

        private struct SendCommandThreadParameterType
        {
            public int iThreadIndex;
            public int iPacketSize;
            public string strFiscalCode;
            public int iFiscalNumberStart;
            public int iFiscalNumberEnd;
            public string strDestinationIPAddress;
            public int iDestinationPort;
        }
    }
}
