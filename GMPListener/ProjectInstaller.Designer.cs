namespace GMPListener
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
            this.serviceInstallerGMPListener = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstallerGMPListener = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // serviceInstallerGMPListener
            // 
            this.serviceInstallerGMPListener.Description = "Edata GMP Listener";
            this.serviceInstallerGMPListener.DisplayName = "GMP Listener";
            this.serviceInstallerGMPListener.ServiceName = "GMP Listener";
            // 
            // serviceProcessInstallerGMPListener
            // 
            this.serviceProcessInstallerGMPListener.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerGMPListener.Password = null;
            this.serviceProcessInstallerGMPListener.Username = null;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceInstallerGMPListener,
            this.serviceProcessInstallerGMPListener});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller serviceInstallerGMPListener;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerGMPListener;
    }
}