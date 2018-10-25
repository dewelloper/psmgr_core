using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Edata.CommonLibrary;
using Edata.DataAccessLayer;
using Edata.DataTypes.DataSets;
using Edata.TcpIpClient;
using Edata.TcpIpSocket;
using Edata.SystemInformation;

namespace EmailDispatcherCore
{
    /// <summary>
    /// The class provide to manage email dispatcher.  Purpose of the class is take control for start or stop tasks.
    /// </summary>
    public class EmailDispatcherManager : IDisposable
    {
        bool m_bDisposed;
        Timer m_tmLastTime = new Timer();

        private SocketServer<EmailCheckUpManager> m_xSocketServer;
        private EmailCheckUpManager m_xEmailCheckUpManager;
        SmtpAccountDataModel m_xSmtpAccountDataModel;
        string m_strSmtpSender = Parameters.xGetInstance().strGetParameter("SmtpSender", "notification@edata.com.tr");
        string m_strPassword = Parameters.xGetInstance().strGetParameter("Password", "Notification!34");
        string m_strHost = Parameters.xGetInstance().strGetParameter("Host", "mail.akfaholding.com");
        string m_strFromAddress = Parameters.xGetInstance().strGetParameter("FromAddress", "notification@edata.com.tr");
        string m_strFromDisplayName = Parameters.xGetInstance().strGetParameter("FromDisplayName", "Günlük Z Raporu");
        private SystemInformationController m_xSystemInformationController = null;

        /// <summary>
        /// It's constructor of service main. Make creating initial stiuation.
        /// </summary>
        public EmailDispatcherManager()
        {
            m_xSystemInformationController = new SystemInformationController();
            m_xEmailCheckUpManager = new EmailCheckUpManager();
            m_xSocketServer = new SocketServer<EmailCheckUpManager>(m_xEmailCheckUpManager, Parameters.xGetInstance().iGetParameter("DispatcherPort", "10021")); // Veri tabanından port numarası alınması gerekiyor.
            m_bDisposed = false;
        }

        /// <summary>
        ///The method is using to dispose tasks.It will be killed all objects these don't have referance via Garbage Collector(GC).
        /// </summary>
        public void Dispose()
        {
            bStop();
            Dispose(true);
         
            // Call SupressFinalize in case a subclass implements a finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The method manage to dispose tasks with conditions.
        /// </summary>
        /// <param name="prm_bDisposing">Parameter using to decide for dispose tasks.</param>
        protected virtual void Dispose(bool prm_bDisposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (m_bDisposed == false)
            {
                if (prm_bDisposing == true)
                {
                    // Dispose disposable objects
                    m_xSystemInformationController.Dispose();
                }
                m_xEmailCheckUpManager = null;
                m_xSocketServer = null;

                // null the disposable objects
                //m_xEdipDispatcher = null;
                //m_xSocketServer = null;
                // Indicate that the instance has been disposed.
                m_bDisposed = true;
            }
        }

        /// <summary>
        /// The method is using for start email dispatcher service. It also provides to start tasks. And also the method provide to service remind itself to system for is live or isn't live in time period.
        /// </summary>
        /// <returns>Return boolean value. If call method, will return 'True'.</returns>
        public bool bStart()
        {
            m_xSystemInformationController.bStart();
            m_tmLastTime.Elapsed += new ElapsedEventHandler(vLastTimeElapsed);
            m_tmLastTime.AutoReset = true;
            m_tmLastTime.Interval = Parameters.xGetInstance().iGetParameter("PingUpdateInterval", "30000");
            m_tmLastTime.Start();
            m_xSmtpAccountDataModel = new SmtpAccountDataModel(m_strSmtpSender, m_strPassword, m_strHost,
                         m_strFromAddress, m_strFromDisplayName);

            m_tmLastTime.Elapsed += new ElapsedEventHandler(vEmailSend);
            m_tmLastTime.AutoReset = true;
            m_tmLastTime.Interval = Parameters.xGetInstance().iGetParameter("SendEmailInterval", "30000");
            m_tmLastTime.Start();

            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Email Dispatcher will be started...");
            m_xSocketServer.bStartServer();
            return true;
        }

        /// <summary>
        /// The method is using for update email logs when send each email. 
        /// Send an email with attachment file. Content of attachment file is Z report of dispatchers and sales information. 
        ///  If has any problem, will catch error for has shown trace.
        /// </summary>
        /// <param name="sender">The parameter is an object.</param>
        /// <param name="e">The parameter is an event arguments.</param>
        void vLastTimeElapsed(object sender, ElapsedEventArgs e)
        {
            int iErrorCode = 0;
            Edata.DataAccessLayer.DbFunctionDAO.bUpdateAppInformationPingTime(ref iErrorCode);
        }

        void vEmailSend(object prm_objSender, ElapsedEventArgs e)
        {
            int iErrorCode = 0;
            SqlSelectDataset.EmailLogsSelectDataTable xEmailLogsSelectDataTable = EmailDispatcherDAO.xGetEmailLogs(Enums.EMAIL_STATUS.PENDING, ref iErrorCode);

            if (iErrorCode != -1)//HATA OLUŞTU İSE
            {
                Enums.ErrorCode eCode = (Enums.ErrorCode)iErrorCode;
                Trace.vInsertError(null, eCode.ToString());
            }
            else
                if (xEmailLogsSelectDataTable != null && xEmailLogsSelectDataTable.Rows.Count > 0)
                {
                    List<byte[]> bListFileByteArray;

                    foreach (SqlSelectDataset.EmailLogsSelectRow xEmailLogsSelectRow in xEmailLogsSelectDataTable)
                    {
                        bListFileByteArray = new List<byte[]>();
                        if (xEmailLogsSelectRow.ATTACHMENT_FILE_CONTENT != null)
                            bListFileByteArray.Add(xEmailLogsSelectRow.ATTACHMENT_FILE_CONTENT);

                        Boolean bMailReturnValue = SendEmail.xGetInstance().bSendAnEmail(new StringBuilder(xEmailLogsSelectRow.EMAIL_ADDRESSES), xEmailLogsSelectRow.ATTACHMENT_FILE_NAME + " Z Raporu Dosyası",
                           xEmailLogsSelectRow.EMAIL_CONTENT, m_xSmtpAccountDataModel, bListFileByteArray, xEmailLogsSelectRow.ATTACHMENT_FILE_NAME);

                        if (bMailReturnValue)
                            EmailDispatcherDAO.bUpdateEmailLog(xEmailLogsSelectRow.ID, Enums.EMAIL_STATUS.FORWARDED, ref iErrorCode);

                        System.Threading.Thread.Sleep(2000);
                    }
                }
        }

        /// <summary>
        /// The method is using for stop to email dispatcher service. It also provide to stop tasks for dont send any mail. If stop service, also can't make to remind itself to system.
        /// </summary>
        /// <returns>Return boolean value. If call method, will return 'True'.</returns>
        public bool bStop()
        {
            m_xSystemInformationController.bStop();
            m_tmLastTime.Stop();
            Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "Email Dispatcher will be stopped...");
            m_xSocketServer.bStopServer();
            return true;
        }

    }
}
