using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;



namespace ValidateSignService
{
    [ServiceContract]
    public interface IValidateSign
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
           ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json,
           UriTemplate = "sign")]
        string strValidateSign(ValidateSignObjectEncrypted xValidateSignObjectEncrypted);

    }
}
