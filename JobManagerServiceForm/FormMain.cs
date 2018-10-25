using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using JobManagerServiceCore;

namespace JobManagerServiceForm
{

    public partial class FormMain : Form
    {
        JobManagement m_xJobManager = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            bStop();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            m_xJobManager = new JobManagement();
            m_xJobManager.bStart();
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            bStop();

            buttonStart.Enabled = true;
            buttonStop.Enabled = false;
        }

        bool bStop()
        {
            m_xJobManager.bStop();

            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            return true;
        }

    }
}
