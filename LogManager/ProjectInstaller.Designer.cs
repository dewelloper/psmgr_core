namespace LogManager
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
            this.serviceProcessInstallerLogManager = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerLogManager = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerInDemo
            // 
            this.serviceProcessInstallerLogManager.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerLogManager.Password = null;
            this.serviceProcessInstallerLogManager.Username = null;
            this.serviceProcessInstallerLogManager.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerLogManager_AfterInstall);
            // 
            // serviceInstallerInDemo
            // 
            this.serviceInstallerLogManager.Description = "Edata Log Manager";
            this.serviceInstallerLogManager.DisplayName = "LogManager";
            this.serviceInstallerLogManager.ServiceName = "LogManager";
            this.serviceInstallerLogManager.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerLogManager_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerLogManager,
            this.serviceInstallerLogManager});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerLogManager;
        private System.ServiceProcess.ServiceInstaller serviceInstallerLogManager;
    }
}