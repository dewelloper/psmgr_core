namespace IPVoyagerClient
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
            this.serviceProcessInstallerIPVoyagerClient = new System.ServiceProcess.ServiceProcessInstaller();
            this.IPVoyagerClientService = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerIPVoyagerClient
            // 
            this.serviceProcessInstallerIPVoyagerClient.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerIPVoyagerClient.Password = null;
            this.serviceProcessInstallerIPVoyagerClient.Username = null;
            this.serviceProcessInstallerIPVoyagerClient.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerIPVoyagerClient_AfterInstall);
            // 
            // IPVoyagerClientService
            // 
            this.IPVoyagerClientService.Description = "IPVoyagerClientService";
            this.IPVoyagerClientService.DisplayName = "IPVoyagerClientService";
            this.IPVoyagerClientService.ServiceName = "IPVoyagerClientService";
            this.IPVoyagerClientService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.IPVoyagerClientService.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.IPVoyagerClientService_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerIPVoyagerClient,
            this.IPVoyagerClientService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerIPVoyagerClient;
        private System.ServiceProcess.ServiceInstaller IPVoyagerClientService;
    }
}