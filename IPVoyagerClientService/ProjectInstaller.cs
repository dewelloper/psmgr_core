using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace IPVoyagerClient
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstallerIPVoyagerClient_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void IPVoyagerClientService_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
