using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

using Edata.CommonLibrary;
using Edata.DataAccessLayer;

namespace SystemInformationServiceCore
{

    /// <summary>
    /// The class provide to get system informations. Update time interval of information set in "Regedit.exe" via get instance method. 
    /// </summary>
    public class SystemInformationTasks
    {
        SystemInformation xSystemInformation = new SystemInformation();
        Thread m_xUpdateInformationThread = null;
        int m_iUpdateInformationTime = Parameters.xGetInstance().iGetParameter("TimeInterval", "30000");
        DateTime xDateTime = DateTime.MinValue;

        /// <summary>
        /// The method provide to start many tasks as simultaneously with threads. In time period, call to run "bUpdateSystemInformation" method.
        /// </summary>
        public void vStartThread()
        {
            Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "vStartThread() Called.");
            m_xUpdateInformationThread = new Thread(new ThreadStart(bUpdateSystemInformation));
            m_xUpdateInformationThread.Start();
        }

        /// <summary>
        /// It's constructor of the class. The constructor provide to kill threads when project close.
        /// </summary>
        ~SystemInformationTasks()
        {
            bKillThreads();
        }

        /// <summary>
        /// The method kill threads for stop tasks these are working as simultaneously.
        /// If the thread equal to null, thread will abort.
        /// </summary>
        /// <returns>Return a boolean value.</returns>
        public bool bKillThreads()
        {
            Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "bKillThreads() Called.");

            if (m_xUpdateInformationThread != null)
                m_xUpdateInformationThread.Abort();

            m_xUpdateInformationThread = null;
            return true;
        }

        /// <summary>
        /// The method private to get system information and then update the information in database for each self system that in running service. 
        /// The method calls in each specified time period. CPU and RAM of system is calculated by "PerformanceCounter" class. 
        /// Also, network information of system find via its specified classes NetworkInterface, IPHostEntry,AddressFamily etc.
        /// If has any problem to find and update information of system, will catch error for has shown trace. Task running in infinite loop.
        /// </summary>
        public void bUpdateSystemInformation()
        {
            Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "bUpdateSystemInformation() Thread STARTED.");

            while (true)
            {
                if (xDateTime.AddMilliseconds(m_iUpdateInformationTime) < DateTime.Now)
                {
                    try
                    {
                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Start to Update system informations to DB at {0}", DateTime.Now);

                        xSystemInformation.strSystemName = Environment.MachineName;

                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "xSystemInformation.strSystemName : \"{0}\"", xSystemInformation.strSystemName);

                        PerformanceCounter xCpuPerformanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);

                        float fCpuCounter = xCpuPerformanceCounter.NextValue();
                        Thread.Sleep(1000);
                        fCpuCounter = xCpuPerformanceCounter.NextValue();

                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "CpuPerformanceCounter : \"{0}\"", fCpuCounter);

                        PerformanceCounter xRamPerformanceCounter = new PerformanceCounter("Memory", "Available MBytes", true);

                        xRamPerformanceCounter.NextValue();
                        Thread.Sleep(1000);
                        float fGetAvailableRAM = xRamPerformanceCounter.NextValue();
                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "RamPerformanceCounter : \"{0}\"", fGetAvailableRAM);

                        xSystemInformation.iUsingCpu = Convert.ToInt32(fCpuCounter);

                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "xSystemInformation.iUsingCpu : \"{0}\"", xSystemInformation.iUsingCpu);

                        ManagementClass xManagementClass = new ManagementClass("Win32_ComputerSystem");
                        ManagementObjectCollection xManagementObjectCollection = xManagementClass.GetInstances();

                        foreach (ManagementObject xManagementObject in xManagementObjectCollection)
                        {
                            xSystemInformation.iAvailableRam = Convert.ToInt32((Convert.ToInt64(xManagementObject.Properties["TotalPhysicalMemory"].Value) * 1024 / 1073741824));
                            break;
                        }

                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "xSystemInformation.iAvailableRam : \"{0}\"", xSystemInformation.iAvailableRam);

                        xSystemInformation.iUsingRam = (xSystemInformation.iAvailableRam - Convert.ToInt32(fGetAvailableRAM));

                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "xSystemInformation.iUsingRam : \"{0}\"", xSystemInformation.iUsingRam);

                        NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                        IPHostEntry xIPHostEntry;
                        xIPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                        StringBuilder xStringBuilder = new StringBuilder();
                        foreach (NetworkInterface xNetworkInterfaceAdapter in interfaces)
                        {
                            var varIpProperties = xNetworkInterfaceAdapter.GetIPProperties();

                            foreach (var varIp in varIpProperties.UnicastAddresses)
                            {
                                if ((xNetworkInterfaceAdapter.OperationalStatus == OperationalStatus.Up) && (varIp.Address.AddressFamily == AddressFamily.InterNetwork))
                                {
                                    xStringBuilder.Append(varIp.Address.ToString() + ";" + xNetworkInterfaceAdapter.Name.ToString() + ";");
                                }
                            }
                        }
                        xDateTime = DateTime.Now;
                        xSystemInformation.strDescription = xStringBuilder.ToString();

                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.LowLevel, "xSystemInformation.strDescription : \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"", xSystemInformation.strDescription);

                        int iErrorCode = -1;
                        DbFunctionDAO.bUpdateSystemInformation(xSystemInformation.strSystemName, xSystemInformation.iAvailableRam, xSystemInformation.iUsingRam, xSystemInformation.iUsingCpu, xSystemInformation.strDescription, ref iErrorCode);

                        Edata.CommonLibrary.Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "Updating system informations to DB finished at {0}", DateTime.Now);
                    }
                    catch (Exception xException)
                    {
                        xDateTime = DateTime.Now;
                        xException.TraceError();
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
