namespace EmailDispatcher
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstallerEdipDispatcher = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerEdipDispatcher = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerEdipDispatcher
            // 
            this.serviceProcessInstallerEdipDispatcher.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerEdipDispatcher.Password = null;
            this.serviceProcessInstallerEdipDispatcher.Username = null;
            this.serviceProcessInstallerEdipDispatcher.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerEdipDispatcher_AfterInstall);
            // 
            // serviceInstallerEdipDispatcher
            // 
            this.serviceInstallerEdipDispatcher.Description = "Edata Email Dispatcher";
            this.serviceInstallerEdipDispatcher.DisplayName = "Email Dispatcher";
            this.serviceInstallerEdipDispatcher.ServiceName = "Email Dispatcher";
            this.serviceInstallerEdipDispatcher.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerEdipDispatcher_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerEdipDispatcher,
            this.serviceInstallerEdipDispatcher});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerEdipDispatcher;
        private System.ServiceProcess.ServiceInstaller serviceInstallerEdipDispatcher;
    }
}