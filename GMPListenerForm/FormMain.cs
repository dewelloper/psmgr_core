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
    /// <summary>
    /// The class is a main form class. It host all properties of form application properties.
    /// </summary>
    public partial class FormMain : Form
    {
        GMPListenerManager m_xGMPListenerManager = null;

        /// <summary>
        ///  It is a constructor of class.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The trigger provide to load form and to create initial components. Also it provide to start service from outside without start trigger, when form load. 
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void FormMain_Load(object sender, EventArgs e)
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

        /// <summary>
        /// The trigger is using for start service.  It is creating "GMPListenerManager" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_xGMPListenerManager = new GMPListenerManager();
                m_xGMPListenerManager.bStart();
                buttonStop.Enabled = true;
                buttonStart.Enabled = false;
                lblInfoMessage.Text = "Server Started... ";
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        /// <summary>
        /// The trigger is using to stop service.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            vCloseConnection();
        }

        /// <summary>
        /// The method is using to stop task manager. Was created "GMPListenerManager" class whenever used trigger for stop service tasks.
        /// </summary>
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

        /// <summary>
        ///  The trigger provide to close form. When close form application, automaticly stop all tasks.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            vCloseConnection();
        }

        /// <summary>
        ///  The trigger is using to get station of a terminal that was specified. 
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            //int errorCode = 0;
            //Edata.DataTypes.DataSets.SqlSelectDataset.DispatcherSelectRow dr_row = Edata.DataAccessLayer.DbFunctionDAO.xGetDispatcher("TEST00000250", ref errorCode);
        }
       
    }
}
