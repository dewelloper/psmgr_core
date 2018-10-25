using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Edata.CommonLibrary;
using EmailDispatcherCore;


namespace EmailDispatcherForm
{

    /// <summary>
    /// The class is a main form class. It host all properties of form application properties.
    /// </summary>
    public partial class FormMain : Form
    {
        EmailDispatcherManager m_xEmailDispatcherManager = null;

        /// <summary>
        /// It is a constructor of class.
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
        private void Form1_Load(object sender, EventArgs e)
        {
            Trace.vInsertMethodTrace(enumTraceLevel.UsefulInformation, "Email Dispatcher Loaded");
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
        /// The trigger is using for start service.  It is creating "EmailDispatcherManager" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_xEmailDispatcherManager = new EmailDispatcherManager();
                m_xEmailDispatcherManager.bStart();
                buttonStop.Enabled = true;
                buttonStart.Enabled = false;
                lblInfoMessage.Text = "Service Started...";
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
        /// The method is using to stop task manager. Was created "EmailDispatcherManager" class whenever used trigger for stop service tasks.
        /// </summary>
        private void vCloseConnection()
        {
            try
            {
                if (m_xEmailDispatcherManager != null && m_xEmailDispatcherManager.bStop() == true)
                {
                    m_xEmailDispatcherManager.Dispose();
                    m_xEmailDispatcherManager = null;
                    buttonStop.Enabled = false;
                    buttonStart.Enabled = true;
                    lblInfoMessage.Text = "Service Stoped...";
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
              
    }
}
