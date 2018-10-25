namespace TerminalManager
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
            this.serviceInstallerTerminalManager = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerEdipDispatcher
            // 
            this.serviceProcessInstallerEdipDispatcher.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerEdipDispatcher.Password = null;
            this.serviceProcessInstallerEdipDispatcher.Username = null;
            this.serviceProcessInstallerEdipDispatcher.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerTerminalManager_AfterInstall);
            // 
            // serviceInstallerTerminalManager
            // 
            this.serviceInstallerTerminalManager.Description = "Edata Terminal Manager";
            this.serviceInstallerTerminalManager.DisplayName = "Terminal Manager";
            this.serviceInstallerTerminalManager.ServiceName = "Terminal Manager";
            this.serviceInstallerTerminalManager.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerTerminalManager_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerEdipDispatcher,
            this.serviceInstallerTerminalManager});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerEdipDispatcher;
        private System.ServiceProcess.ServiceInstaller serviceInstallerTerminalManager;
    }
}