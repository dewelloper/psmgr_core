namespace SystemInformationService
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
            this.serviceProcessInstallerSystemInfo = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerSystemInfo = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerSystemInfo
            // 
            this.serviceProcessInstallerSystemInfo.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerSystemInfo.Password = null;
            this.serviceProcessInstallerSystemInfo.Username = null;
            // 
            // serviceInstallerSystemInfo
            // 
            this.serviceInstallerSystemInfo.Description = "Display system information service.";
            this.serviceInstallerSystemInfo.DisplayName = "System Information Service";
            this.serviceInstallerSystemInfo.ServiceName = "SystemInformationService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerSystemInfo,
            this.serviceInstallerSystemInfo});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerSystemInfo;
        private System.ServiceProcess.ServiceInstaller serviceInstallerSystemInfo;
    }
}