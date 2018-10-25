using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ValidateSignService
{

    public class ValidateSignObject
    {
        public string strPOkc { get; set; }
        public string strSignedData { get; set; }
        public string strSourcesData { get; set; }
        public string strTerminalId { get; set; }

    }

    public class ValidateSignObjectEncrypted 
    {
        public string strData { get; set; }
    }

}
