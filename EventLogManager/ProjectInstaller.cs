using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace EventLogManager
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstallerEventLogManager_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstallerEventLogManager_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
