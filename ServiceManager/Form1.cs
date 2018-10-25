using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.ServiceProcess;
using System.Management;

namespace ServiceManager
{
public partial class Form1 : Form
{
    TableLayoutPanel m_xFirstTableBuffer = null;
    public Form1()
    {
        InitializeComponent();
        m_xFirstTableBuffer = tableLayoutPanel1;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        try
        {
            RegistryKey rkDefaultUser = Registry.Users.OpenSubKey(".DEFAULT");
            RegistryKey rkSoftwareKey = rkDefaultUser.OpenSubKey("Software");
            RegistryKey rkCyberComKey = rkSoftwareKey.OpenSubKey("Edata CYBERCOM");
            string[] straSubKeys = rkCyberComKey.GetSubKeyNames();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute,30));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.Visible = false;
            tableLayoutPanel1.SuspendLayout();
            foreach (string strSubKey in straSubKeys)
            {
                bool bIsServiceInstalled = false;
                bool bIsServiceRunning = false;
                RegistryKey rkDispatcherKey = rkCyberComKey.OpenSubKey(strSubKey);
                
                if (rkDispatcherKey.GetValue("DispatcherPort", "-1").ToString() != "-1" ||
                        rkDispatcherKey.GetValue("ListenerPort", "-1").ToString() != "-1" ||
                        rkDispatcherKey.GetValue("NtpPort", "-1").ToString() != "-1" ||
                    rkDispatcherKey.GetValue("SendEmailInterval", "-1").ToString() != "-1" 
                        )
                {
                    string strServiceTitle = strSubKey;
                    tableLayoutPanel1.RowCount++;
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute,30));

                    PictureBox pcbServiceStatus = new PictureBox();
                    pcbServiceStatus.Anchor = AnchorStyles.None;
                    pcbServiceStatus.Name = "pcbServiceStatus";


                    string strSubKeyServiceName = strGetServiceName(strSubKey);
                    ServiceController ctl = ServiceController.GetServices()
                                            .FirstOrDefault(s => s.ServiceName == strSubKeyServiceName);

                    Label lblStatus = new Label();
                    lblStatus.Name = "lblStatus";
                    lblStatus.Anchor = AnchorStyles.Left;
                    lblStatus.AutoSize = true;

                    if (ctl == null)
                    {
                        pcbServiceStatus.Image = ServiceManager.Properties.Resources.Hopstarter_Scrap_Aqua_Ball_Red.ToBitmap();
                        lblStatus.Text = "Not Installed";
                    }
                    else
                    {
                        bIsServiceInstalled = true;
                        strServiceTitle = ctl.DisplayName;
                        if (ctl.Status == ServiceControllerStatus.Running)
                        {
                            bIsServiceRunning = true;
                            pcbServiceStatus.Image = ServiceManager.Properties.Resources.Hopstarter_Scrap_Aqua_Ball_Green.ToBitmap();
                        }
                        else
                        {
                            pcbServiceStatus.Image = ServiceManager.Properties.Resources.Hopstarter_Scrap_Aqua_Ball_Red.ToBitmap();
                        }

                        lblStatus.Text = ctl.Status.ToString();
                    }
                    pcbServiceStatus.Tag = strServiceTitle;
                    lblStatus.Tag = strServiceTitle;
                    pcbServiceStatus.Size = new Size(20, 20);
                    pcbServiceStatus.SizeMode = PictureBoxSizeMode.Zoom;
                    pcbServiceStatus.Anchor = AnchorStyles.Top;
                    tableLayoutPanel1.Controls.Add(pcbServiceStatus, 0, tableLayoutPanel1.RowCount - 1);
                    tableLayoutPanel1.Controls.Add(lblStatus, 1, tableLayoutPanel1.RowCount - 1);

                    Label lblCaption = new Label();
                    lblCaption.Text = strServiceTitle;
                    lblCaption.Name = "lblCaption";
                    lblCaption.AutoSize = true;
                    lblCaption.Anchor = AnchorStyles.Left;
                    tableLayoutPanel1.Controls.Add(lblCaption, 2, tableLayoutPanel1.RowCount - 1);

                    Button btnStartService = new Button();
                    btnStartService.Text = "Start";
                    btnStartService.Name = "btnStartService";
                    btnStartService.Tag = strServiceTitle;
                    btnStartService.Click += new EventHandler(btnStartService_Click);
                    if (!bIsServiceInstalled || bIsServiceRunning) btnStartService.Enabled = false;
                    btnStartService.Anchor = AnchorStyles.None;
                    tableLayoutPanel1.Controls.Add(btnStartService, 3, tableLayoutPanel1.RowCount - 1);

                    Button btnStopService = new Button();
                    btnStopService.Text = "Stop";
                    btnStopService.Name = "btnStopService";
                    btnStopService.Tag = strServiceTitle;
                    if (!bIsServiceInstalled || !bIsServiceRunning) btnStopService.Enabled = false;
                    btnStopService.Click += new EventHandler(btnStopService_Click);
                    btnStopService.Anchor = AnchorStyles.None;
                    tableLayoutPanel1.Controls.Add(btnStopService, 4, tableLayoutPanel1.RowCount - 1);

                    Label lblPort = new Label();
                    lblPort.Name = "lblPort";
                    if (rkDispatcherKey.GetValue("DispatcherPort", "-1").ToString() != "-1")
                        lblPort.Text = rkDispatcherKey.GetValue("DispatcherPort", "-1").ToString();
                    else if (rkDispatcherKey.GetValue("ListenerPort", "-1").ToString() != "-1")
                        lblPort.Text = rkDispatcherKey.GetValue("ListenerPort", "-1").ToString();
                    else if (rkDispatcherKey.GetValue("NtpPort", "-1").ToString() != "-1")
                        lblPort.Text = rkDispatcherKey.GetValue("NtpPort", "-1").ToString();                                                                                                    

                    lblPort.AutoSize = true;
                    lblPort.Anchor = AnchorStyles.Left;
                    tableLayoutPanel1.Controls.Add(lblPort, 5, tableLayoutPanel1.RowCount - 1);

                }
            }

            // add stop all/start all buttons
            tableLayoutPanel1.RowCount++;

            Button btnStartAll = new Button();
            btnStartAll.Text = "Start All";
            btnStartAll.Click += new EventHandler(btnStartAll_Click);
            btnStartAll.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(btnStartAll, 0, tableLayoutPanel1.RowCount - 1);
            tableLayoutPanel1.SetColumnSpan(btnStartAll, tableLayoutPanel1.ColumnCount/2);

            Button btnStopAll = new Button();
            btnStopAll.Text = "Stop All";
            btnStopAll.Click += new EventHandler(btnStopAll_Click);
            btnStopAll.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(btnStopAll, 1, tableLayoutPanel1.RowCount - 1);
            tableLayoutPanel1.SetColumnSpan(btnStopAll, tableLayoutPanel1.ColumnCount / 2);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles[tableLayoutPanel1.RowCount - 1].Height = 45F;
            tableLayoutPanel1.RowStyles[tableLayoutPanel1.RowCount - 1].SizeType = SizeType.Absolute;


            // add refresh button

            tableLayoutPanel1.RowCount++;

            Button btnRefresh = new Button();
            btnRefresh.Text = "Refresh Services";
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnRefresh.Dock = DockStyle.Fill;
            tableLayoutPanel1.Controls.Add(btnRefresh, 0, tableLayoutPanel1.RowCount - 1);
            tableLayoutPanel1.SetColumnSpan(btnRefresh, tableLayoutPanel1.ColumnCount);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles[tableLayoutPanel1.RowCount - 1].Height = 45F;
            tableLayoutPanel1.RowStyles[tableLayoutPanel1.RowCount - 1].SizeType = SizeType.Absolute;

            // Resume Layout
            tableLayoutPanel1.Visible = true;
            tableLayoutPanel1.ResumeLayout();
            this.Height = tableLayoutPanel1.Height + 30;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    void btnStopAll_Click(object sender, EventArgs e)
    {
        try
        {
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if (c is Button)
                {
                    if(((Button)c).Name == "btnStopService")
                    {
                        ((Button)c).PerformClick();
                    }
                }
            }
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
        }
    }

    void btnStartAll_Click(object sender, EventArgs e)
    {

        try
        {
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if (c is Button)
                {
                    if (((Button)c).Name == "btnStartService")
                    {
                        ((Button)c).PerformClick();
                    }
                }
            }
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
        }
    }

    void btnRefresh_Click(object sender, EventArgs e)
    {
        tableLayoutPanel1.SuspendLayout();
        for(int iRow = 1; iRow < tableLayoutPanel1.RowCount; iRow++)
        {
            for(int iCol = 0; iCol < tableLayoutPanel1.ColumnCount; iCol++)
            {
                tableLayoutPanel1.Controls.Remove(tableLayoutPanel1.GetControlFromPosition(iCol, iRow));
            }
        }
        tableLayoutPanel1.ResumeLayout();
        tableLayoutPanel1.RowCount = 1;
        Form1_Load(sender, e);
    }

    private string strGetServiceName(string prm_strServiceRegistryName)
    {
        WqlObjectQuery wqlObjectQuery = new WqlObjectQuery("SELECT * FROM Win32_Service WHERE PathName LIKE '%" + prm_strServiceRegistryName + ".exe%'");
        ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(wqlObjectQuery);
        ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();

        foreach (ManagementObject managementObject in managementObjectCollection)
        {
            return managementObject.GetPropertyValue("DisplayName").ToString();
        }

        return null;
    }

    void btnStopService_Click(object sender, EventArgs e)
    {
        ServiceController _service = new ServiceController(((Button)sender).Tag.ToString());
        try
        {
            _service.Stop();
            _service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 0, 15));
        }
        catch (System.ServiceProcess.TimeoutException)
        {
            MessageBox.Show(this, "Servis durdurulamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }
        finally
        {
            _service.Refresh();
            if (_service.Status == ServiceControllerStatus.Stopped)
            {
                ((Button)sender).Enabled = false;
                ((Button)tableLayoutPanel1.Controls.Find("btnStartService",true).Where(s => s.Tag.ToString() == ((Button)sender).Tag.ToString()).First()).Enabled = true;
                ((PictureBox)tableLayoutPanel1.Controls.Find("pcbServiceStatus", true).Where(s => s.Tag.ToString() == ((Button)sender).Tag.ToString()).First()).Image = ServiceManager.Properties.Resources.Hopstarter_Scrap_Aqua_Ball_Red.ToBitmap();
                ((Label)tableLayoutPanel1.Controls.Find("lblStatus", true).Where(s => s.Tag.ToString() == ((Button)sender).Tag.ToString()).First()).Text = _service.Status.ToString();
            }
        }
    }

    void btnStartService_Click(object sender, EventArgs e)
    {
        ServiceController _service = new ServiceController(((Button)sender).Tag.ToString());
        try
        {
            if (_service.Status != ServiceControllerStatus.Running ||
                    _service.Status != ServiceControllerStatus.StartPending)
                _service.Start();

            _service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 0, 15));
        }
        catch (System.ServiceProcess.TimeoutException)
        {
            MessageBox.Show(this, "Servis çalıştırılamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
        }
        finally
        {
            _service.Refresh();
            if (_service.Status == ServiceControllerStatus.Running)
            {
                ((Button)sender).Enabled = false;
                ((Button)tableLayoutPanel1.Controls.Find("btnStopService", true).Where(s => s.Tag.ToString() == ((Button)sender).Tag.ToString()).First()).Enabled = true;
                ((PictureBox)tableLayoutPanel1.Controls.Find("pcbServiceStatus", true).Where(s => s.Tag.ToString() == ((Button)sender).Tag.ToString()).First()).Image = ServiceManager.Properties.Resources.Hopstarter_Scrap_Aqua_Ball_Green.ToBitmap();
                ((Label)tableLayoutPanel1.Controls.Find("lblStatus", true).Where(s => s.Tag.ToString() == ((Button)sender).Tag.ToString()).First()).Text = _service.Status.ToString();
            }
        }
    }
}
}
