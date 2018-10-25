using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SystemInformationServiceCore;
namespace SystemInformationServiceForm
{

    /// <summary>
    /// The class is a main form class. It host all properties of form application properties.
    /// </summary>
    public partial class FormMain : Form
    {
        SystemInformationManagement m_xSystemInformationManagement = null;

        /// <summary>
        /// It is a constructor of class.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        ///The trigger provide to load form and to create initial components.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        /// <summary>
        ///The trigger is using for start service. It is creating "SystemInformationManagement" class whenever used trigger for start service tasks.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            m_xSystemInformationManagement = new SystemInformationManagement();
            m_xSystemInformationManagement.bStart();
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        /// <summary>
        /// The trigger is using to stop service.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event argument.</param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            bStop();
        }

        /// <summary>
        /// The method is using to stop task manager. Was created "SystemInformationManagement" class whenever used trigger for stop service tasks.
        /// </summary>
        /// <returns>Return a boolean value.</returns>
        bool bStop()
        {
            m_xSystemInformationManagement.bStop();

            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            return true;
        }
    }
}
