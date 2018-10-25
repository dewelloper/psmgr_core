namespace InDemo
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
            this.serviceProcessInstallerInDemo = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerInDemo = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerInDemo
            // 
            this.serviceProcessInstallerInDemo.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerInDemo.Password = null;
            this.serviceProcessInstallerInDemo.Username = null;
            this.serviceProcessInstallerInDemo.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerInDemo_AfterInstall);
            // 
            // serviceInstallerInDemo
            // 
            this.serviceInstallerInDemo.Description = "Edata InDemo Dispatcher";
            this.serviceInstallerInDemo.DisplayName = "InDemo";
            this.serviceInstallerInDemo.ServiceName = "InDemo";
            this.serviceInstallerInDemo.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerInDemo_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerInDemo,
            this.serviceInstallerInDemo});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerInDemo;
        private System.ServiceProcess.ServiceInstaller serviceInstallerInDemo;
    }
}