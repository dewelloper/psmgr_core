using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Edata.CommonLibrary;
using MasterSiteManagerCore;

namespace MasterSiteManagerForm
{
    /// <summary>
    /// The class is a main form class. It host all properties of form application properties.
    /// </summary>
    public partial class Form1 : Form
    {
        MasterSiteManager m_xSiteManager = null;
        /// <summary>
        /// The method is a constructor.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The trigger is using for start dispatcher service. It is creating "DominosDispatcherManager" class whenever used trigger for start service tasks. 
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_xSiteManager = new MasterSiteManager();
                m_xSiteManager.bStart();
                buttonStop.Enabled = true;
                buttonStart.Enabled = false;
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        /// <summary>
        /// The trigger is using to stop dispatcher service. 
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            vStop();
        }

        /// <summary>
        /// The method provide to use for stop dispatcher service. Was created "DominosDispatcherManager" class whenever used trigger for stop service tasks. 
        /// </summary>
        private void vStop()
        {
            try
            {
                if (m_xSiteManager != null && m_xSiteManager.bStop() == true)
                {
                    m_xSiteManager.Dispose();
                    m_xSiteManager = null;
                    buttonStop.Enabled = false;
                    buttonStart.Enabled = true;
                 
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        /// <summary>
        /// The trigger provide to close form. When close form application, automaticly stop all tasks.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            vStop();
        }

        /// <summary>
        /// The trigger provide to load form and to create initial components. Also it provide to start service from outside without start trigger, when form load. 
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            bool bStartNeeded = false;
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.Equals("-start")) bStartNeeded = true;
            }
            if (bStartNeeded) buttonStart.PerformClick();
        }
    }
}



