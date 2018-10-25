using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Edata.CommonLibrary;
using XmlExporterCore;

namespace XmlExporterForm
{
    public partial class FormMain : Form
    {
        XmlExporterManager m_xXmlExporterManager = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            bool bStartNeeded = false;
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.Equals("-start")) bStartNeeded = true;
            }
            if (bStartNeeded) buttonStart.PerformClick();
        }


        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_xXmlExporterManager = new XmlExporterManager();
                m_xXmlExporterManager.bStart();
                buttonStop.Enabled = true;  
                buttonStart.Enabled = false;
                lblInfoMessage.Text = "Server Started...";
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            vStop();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            vStop();
        }

        private void vStop()
        {
            try
            {
                if (m_xXmlExporterManager != null && m_xXmlExporterManager.bStop() == true)
                {
                    m_xXmlExporterManager.Dispose();
                    m_xXmlExporterManager = null;
                    buttonStop.Enabled = false;
                    buttonStart.Enabled = true;
                    lblInfoMessage.Text = "Server Stoped...";
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
