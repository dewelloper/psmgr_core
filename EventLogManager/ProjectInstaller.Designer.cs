namespace EventLogManager
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
            this.serviceInstallerEventLogManager = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerEdipDispatcher
            // 
            this.serviceProcessInstallerEdipDispatcher.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerEdipDispatcher.Password = null;
            this.serviceProcessInstallerEdipDispatcher.Username = null;
            this.serviceProcessInstallerEdipDispatcher.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerEventLogManager_AfterInstall);
            // 
            // serviceInstallerEventLogManager
            // 
            this.serviceInstallerEventLogManager.Description = "Edata Event Log Manager";
            this.serviceInstallerEventLogManager.DisplayName = "Event Log Manager";
            this.serviceInstallerEventLogManager.ServiceName = "Event Log Manager";
            this.serviceInstallerEventLogManager.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerEventLogManager_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerEdipDispatcher,
            this.serviceInstallerEventLogManager});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerEdipDispatcher;
        private System.ServiceProcess.ServiceInstaller serviceInstallerEventLogManager;
    }
}