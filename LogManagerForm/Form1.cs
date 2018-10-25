using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Edata.CommonLibrary;
using LogManagerCore;


namespace LogManagerForm
{
    public partial class Form1 : Form
    {
        LogManager m_xLogManager = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_xLogManager = new LogManager();
                m_xLogManager.bStart();
                buttonStop.Enabled = true;
                buttonStart.Enabled = false;
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

        /// <summary>
        /// The method provide to use for stop manager service. Was created "LogManager" class whenever used trigger for stop service tasks. 
        /// </summary>
        private void vStop()
        {
            try
            {
                if (m_xLogManager != null && m_xLogManager.bStop() == true)
                {
                    m_xLogManager.Dispose();
                    m_xLogManager = null;
                    buttonStop.Enabled = false;
                    buttonStart.Enabled = true;
                 
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

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

