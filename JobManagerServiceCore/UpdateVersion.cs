using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.Xml;

using Edata.DataAccessLayer;
using Edata.CommonLibrary;
using System.IO;
using System.Threading;

namespace JobManagerServiceCore
{
    public class UpdateVersion
    {
        private static UpdateVersion m_xGlobalsInstance = null;
        private VerifoneFS.VerifoneFieldService m_xWSverifone;

        public UpdateVersion()
        {
        }

        ~UpdateVersion()
        {
        }

        public static UpdateVersion xGetInstance()
        {
            if (m_xGlobalsInstance == null)
                m_xGlobalsInstance = new UpdateVersion();
            return m_xGlobalsInstance;
        }

        public DataTable dtGetApplicationVersion(string prm_strTerminalNumber, VerifoneAccountDataModel prm_xVerifoneAccountDataModel)
        {
            try
            {
                m_xWSverifone = new VerifoneFS.VerifoneFieldService();
                m_xWSverifone.CookieContainer = new CookieContainer();
                DataTable dtDataTable = null;

                bLoginService(prm_xVerifoneAccountDataModel);

                if (m_xWSverifone != null)
                {
                    dtDataTable = new DataTable();
                    
                    VerifoneFS.WSRESULT xWSresult = m_xWSverifone.GetTerminalInfoWithID(prm_strTerminalNumber.Trim());

                    if (xWSresult != null && xWSresult.results[0].ToString() != string.Empty && xWSresult.errCode == 0 && xWSresult.results.Length > 0)
                    {

                        XmlDocument xXmlDocument = new XmlDocument();

                        string strXmlString = xWSresult.results[0].ToString();

                        int iIndexOfTerminalWord = 0;
                        int iLastIndexOfString = 0;

                        while ((iIndexOfTerminalWord = strXmlString.IndexOf("<terminalinfo ", iIndexOfTerminalWord)) != -1)
                        {
                            if (iIndexOfTerminalWord != -1)
                            {
                                iLastIndexOfString = iIndexOfTerminalWord++;
                            }
                        }
                        string strLastElementOfXML = strXmlString.Substring(iLastIndexOfString);

                        xXmlDocument.LoadXml(strLastElementOfXML);

                        dtDataTable.Columns.Add("ApplicationName");
                        dtDataTable.Columns.Add("Value");

                        foreach (XmlLinkedNode xXmlLinkedNode in xXmlDocument.ChildNodes)
                        {
                            foreach (XmlAttribute xXmlAttribute in xXmlLinkedNode.Attributes)
                            {
                                DataRow xDataRow = dtDataTable.Rows.Add();
                                try
                                {
                                    xDataRow["ApplicationName"] = xXmlAttribute.Name;
                                    xDataRow["Value"] = xXmlAttribute.Value.ToString();
                                 
                                }
                                catch
                                {
                                }
                            }

                            foreach (XmlElement xXmlElement in xXmlLinkedNode.ChildNodes)
                            {
                                foreach (XmlElement xXmlElementInner in xXmlElement.ChildNodes)
                                {
                                    string strName = string.Empty;
                                    string strVersion = string.Empty;
                                    string strGroup = string.Empty;

                                    foreach (XmlAttribute xXmlAttribute in xXmlElementInner.Attributes)
                                    {
                                        if (xXmlAttribute.Name == "appname")
                                            strName = xXmlAttribute.Value.ToString();
                                        else if (xXmlAttribute.Name == "appver")
                                            strVersion = xXmlAttribute.Value.ToString();
                                        else if (xXmlAttribute.Name == "appgroup")
                                            strGroup = xXmlAttribute.Value.ToString();
                                    }

                                    DataRow drDataRow = dtDataTable.Rows.Add();
                                    try
                                    {
                                        drDataRow["ApplicationName"] = strName;
                                        drDataRow["Value"] = string.Format("{0} (Group:{1})", strVersion, strGroup);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }

                return dtDataTable;
            
            }
            catch (Exception xException)
            {
                xException.TraceError();
            }

            return null;
        }



        public bool bLoginService(VerifoneAccountDataModel prm_xVerifoneAccountDataModel)
        {
            m_xWSverifone = new VerifoneFS.VerifoneFieldService();
            m_xWSverifone.CookieContainer = new CookieContainer();

            m_xWSverifone.Login(Int32.Parse(prm_xVerifoneAccountDataModel.strCustomerName), prm_xVerifoneAccountDataModel.strUserName, prm_xVerifoneAccountDataModel.strPassword);
         
            return true;
        }
    }
    public class VerifoneAccountDataModel
    {
        public VerifoneAccountDataModel(string prm_strCustomerName, string prm_strUsername, string prm_strPassword)
        {
            m_strCustomerName = prm_strCustomerName;
            m_strUserName = prm_strUsername;
            m_strPassword = prm_strPassword;
        }
        private string m_strCustomerName;
        private string m_strUserName;
        private string m_strPassword;

        public string strCustomerName
        {
            get
            {
                return m_strCustomerName;
            }
            set
            {
                m_strCustomerName = value;
            }
        }
        public string strUserName
        {
            get
            {
                return m_strUserName;
            }
            set
            {
                m_strUserName = value;
            }
        }
        public string strPassword
        {
            get
            {
                return m_strPassword;
            }
            set
            {
                m_strPassword = value;
            }
        }
    }
}
