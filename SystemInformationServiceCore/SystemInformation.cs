using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemInformationServiceCore
{
    /// <summary>
    /// The class is using for get and set information of system. Properties will be used when need each system information. 
    /// </summary>
    public class SystemInformation
    {
        private string m_strSystemName;
        private int m_iAvailableRam;
        private int m_iUsingRam;
        private int m_iUsingCpu;
        private string m_strDescription;

        /// <summary>
        /// The object defines name of the system. That property of application is detail information.
        /// </summary>
        public string strSystemName
        {
            get
            {
                return m_strSystemName;
            }
            set
            {
                m_strSystemName = value;
            }
        }

        /// <summary>
        /// The object defines available RAM of system. 
        /// </summary>
        public int iAvailableRam
        {
            get
            {
                return m_iAvailableRam;
            }
            set
            {
                m_iAvailableRam = value;
            }
        }

        /// <summary>
        /// The object defines usage of RAM of system.
        /// </summary>
        public int iUsingRam
        {
            get
            {
                return m_iUsingRam;
            }
            set
            {
                m_iUsingRam = value;
            }
        }

        /// <summary>
        /// The object defines usage of CPU of system.
        /// </summary>
        public int iUsingCpu
        {
            get
            {
                return m_iUsingCpu;
            }
            set
            {
                m_iUsingCpu = value;
            }
        }

        /// <summary>
        ///The object defines more details information about system. Describe IP addresses and adapter information.
        /// </summary>
        public string strDescription
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }
    }
}
