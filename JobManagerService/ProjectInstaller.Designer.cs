namespace JobManagerService
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
            this.serviceProcessInstallerJob = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerJob = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerJob
            // 
            this.serviceProcessInstallerJob.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerJob.Password = null;
            this.serviceProcessInstallerJob.Username = null;
            // 
            // serviceInstallerJob
            // 
            this.serviceInstallerJob.Description = "The aim of the service is controls many jobs(Deleting unneccesary folders. Get ve" +
    "rsion of applications  etc.).  ";
            this.serviceInstallerJob.DisplayName = "JobManagerService";
            this.serviceInstallerJob.ServiceName = "JobManagerService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerJob,
            this.serviceInstallerJob});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerJob;
        private System.ServiceProcess.ServiceInstaller serviceInstallerJob;
    }
}