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
            this.serviceProcessInstallerXmlExporter = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerXmlExporter = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerXmlExporter
            // 
            this.serviceProcessInstallerXmlExporter.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerXmlExporter.Password = null;
            this.serviceProcessInstallerXmlExporter.Username = null;
            this.serviceProcessInstallerXmlExporter.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstallerInDemo_AfterInstall);
            // 
            // serviceInstallerXmlExporter
            // 
            this.serviceInstallerXmlExporter.Description = "Edata XmlExporter";
            this.serviceInstallerXmlExporter.DisplayName = "XmlExporter";
            this.serviceInstallerXmlExporter.ServiceName = "XmlExporter";
            this.serviceInstallerXmlExporter.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceInstallerInDemo_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerXmlExporter,
            this.serviceInstallerXmlExporter});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerXmlExporter;
        private System.ServiceProcess.ServiceInstaller serviceInstallerXmlExporter;
    }
}