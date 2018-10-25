using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace EmailDispatcher
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstallerEdipDispatcher_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstallerEdipDispatcher_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
