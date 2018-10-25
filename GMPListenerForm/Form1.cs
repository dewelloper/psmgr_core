/****************************************************************************************
 * Class		: FormMain
 * Purpose		: Main form of GMP Listener debuging application
 * Create Date  :  08/04/2014 By Eylem Artut (BeterBocek)
 * Modify Date  : 

 Copyright(c) 2013 By Edata Electronics
 All Rights Reserved
 
 * Modifiying History
 *  08/04/2014 By Eylem Artut (BeterBocek) - Create Initial Class
 *****************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Edata.CommonLibrary;
using Edata.DataTypes;
using GMPListenerCore;


namespace GMPListenerForm
{
    public partial class Form1 : Form
    {
        GMPListenerManager m_xGMPListenerManager = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Trace.vInsertMethodTrace(enumTraceLevel.UsefulInformation, "GMP Listener Loaded");
            buttonStop.Enabled = false;
            buttonStart.Enabled = true;

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
                m_xGMPListenerManager = new GMPListenerManager();
                m_xGMPListenerManager.bStart();
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
            vCloseConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int errorCode = 0;
            Edata.DataTypes.DataSets.SqlSelectDataset.DispatcherSelectRow dr_row = Edata.DataAccessLayer.DbFunctionDAO.xGetDispatcher("TEST00000250", ref errorCode);
        }

        private void vCloseConnection()
        {
            try
            {
                if (m_xGMPListenerManager != null && m_xGMPListenerManager.bStop() == true)
                {
                    m_xGMPListenerManager.Dispose();
                    m_xGMPListenerManager = null;
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

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            vCloseConnection();
        }

    }
}
