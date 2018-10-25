using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Edata.CommonLibrary;

namespace SystemInformationServiceCore
{
    /// <summary>
    /// The class provide to manage system information service. Purpose of the class is take control for start or stop tasks.
    /// </summary>
    public class SystemInformationManagement : IDisposable
    {
        private bool m_bDisposed;
        SystemInformationTasks m_xSystemInformationTasks = new SystemInformationTasks();

        /// <summary>
        /// It's constructor of service main. Make creating initial stiuation.
        /// </summary>
        public SystemInformationManagement()
        {
            m_bDisposed = false;
        }

        /// <summary>
        /// The method is using for start to system information service. It also provide to start tasks.
        /// </summary>
        /// <returns>Return boolean value. If call method, will return 'True'.</returns>
        public bool bStart()
        {
            Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "bStart() Called.");
            m_xSystemInformationTasks.vStartThread();
            return true;
        }

        /// <summary>
        /// The method is using for stop to system information service. It also provide to stop tasks.
        /// </summary>
        /// <returns>Return boolean value. If call method, will return 'True'.</returns>
        public bool bStop()
        {
            Trace.vInsertMethodTrace(enumTraceLevel.Unnecessary, "bStop() Called.");
            m_xSystemInformationTasks.bKillThreads();

            return true;
        }

        /// <summary>
        /// The method is using to dispose tasks. Will be killed all objects these don't have referance via Garbage Collector(GC).
        /// </summary>
        public void Dispose()
        {
            bStop();
         
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The method manage to dispose tasks with conditions.
        /// </summary>
        /// <param name="prm_bDisposing">Parameter using to decide for dispose tasks or not.</param>
        protected virtual void Dispose(bool prm_bDisposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (m_bDisposed == false)
            {
                if (prm_bDisposing == true)
                {
                    // Dispose disposable objects
                }

                m_xSystemInformationTasks = null;

                m_bDisposed = true;
            }
        }
    }
}
