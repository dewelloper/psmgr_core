using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

namespace InDemo
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceProcessInstallerInDemo_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceInstallerInDemo_AfterInstall(object sender, InstallEventArgs e)
        {
             
        }
    }
}
