using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace TerminalManager
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstallerTerminalManager_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstallerTerminalManager_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
