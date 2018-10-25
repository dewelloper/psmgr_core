using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace LogManager
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstallerLogManager_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstallerLogManager_AfterInstall(object sender, InstallEventArgs e)
        {
             
        }
    }
}
