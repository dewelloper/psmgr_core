using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Edata.CommonLibrary;
using XmlExporterCore;



namespace XmlExporter
{
    public partial class ServiceMain : ServiceBase
    {
        XmlExporterManager m_xXmlExporterManager = null;

        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                m_xXmlExporterManager = new XmlExporterManager();
                m_xXmlExporterManager.bStart();
            } 
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (m_xXmlExporterManager != null && m_xXmlExporterManager.bStop() == true)
                {
                    m_xXmlExporterManager.Dispose();
                    m_xXmlExporterManager = null;
                }
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }
        }
    }
}
