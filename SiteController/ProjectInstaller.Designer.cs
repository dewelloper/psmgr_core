namespace SiteController
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
            this.serviceProcessInstallerSiteController = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerSiteController = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerProtelDispatcher
            // 
            this.serviceProcessInstallerSiteController.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerSiteController.Password = null;
            this.serviceProcessInstallerSiteController.Username = null;
            this.serviceProcessInstallerSiteController.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerSiteController_AfterInstall);
            // 
            // serviceInstallerProtelDispatcher
            // 
            this.serviceInstallerSiteController.Description = "Edata Site Manager";
            this.serviceInstallerSiteController.DisplayName = "Site Manager";
            this.serviceInstallerSiteController.ServiceName = "Site Manager";
            this.serviceInstallerSiteController.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerSiteController_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerSiteController,
            this.serviceInstallerSiteController});
                                
        }                       
                               
        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerSiteController;
        private System.ServiceProcess.ServiceInstaller serviceInstallerSiteController;
    }
}