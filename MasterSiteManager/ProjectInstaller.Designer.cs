namespace MasterSiteManager
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
            this.serviceProcessInstallerSiteManager = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerSiteManager = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerProtelDispatcher
            // 
            this.serviceProcessInstallerSiteManager.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerSiteManager.Password = null;
            this.serviceProcessInstallerSiteManager.Username = null;
            this.serviceProcessInstallerSiteManager.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerSiteManager_AfterInstall);
            // 
            // serviceInstallerProtelDispatcher
            // 
            this.serviceInstallerSiteManager.Description = "Edata Site Manager";
            this.serviceInstallerSiteManager.DisplayName = "Site Manager";
            this.serviceInstallerSiteManager.ServiceName = "Site Manager";
            this.serviceInstallerSiteManager.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerSiteManager_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerSiteManager,
            this.serviceInstallerSiteManager});
                                
        }                       
                               
        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerSiteManager;
        private System.ServiceProcess.ServiceInstaller serviceInstallerSiteManager;
    }
}