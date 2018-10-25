using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using Edata.CommonLibrary;



namespace ValidateSignTestForm
{
    public partial class Form1 : Form
    {
        string m_strCertificateData = "3082053C30820424A0030201020208008F938F3F0743EC300D06092A864886F70D01010B05003058310B300906035504061302545231493047060355040313404F64656D65204B617964656469636920436968617A20456C656B74726F6E696B20536572746966696B612048697A6D6574205361676C617969636973692D5331301E170D3135303631353130353132365A170D3135313231343131353132365A3039310B300906035504061302545231133011060355040A0C0A4B414D55534D54455354311530130603550405130C54455354303030303137323830820122300D06092A864886F70D01010105000382010F003082010A028201010097F6BF1AC0512E2EAAED233FA97C7AB4627D6694F0E6F9F966FB0439247263C379D20C1CF3C4E73DE40A8AAAD2F160465B69E9B53F8A706E897A8F30C706188501A5F6AB068E6C132477052E8F53FE600D5D76C34AADA1C2B657B3343076A050EA20F00073F41E36B025AB4332ADD4534A679BDE28BF593F871895C1C28E791E062BF65881819BBEBA7B4A6128F42500EEFD2FE00686A4F872CDD1C47956344EB0665CF216EE20BDF14A386C39662CF00419A531802B22203629DF45E45001F4FAC04A59ADB74DAD3F47ED94B0E15F8A9CB457377D2772910754E2899E33C5ABD663D05F70892A97BBCAD1CB9586DB3172DC9674A3E8DA8BA1A012584B0736A30203010001A382022730820223301F0603551D23041830168014E3B42006CF63731EA93CACC89989E15661730838301D0603551D0E041604142DEA4BADF71D18A1EADC868AFD56E7284BD2BBBA300E0603551D0F0101FF0404030205A0308201200603551D2004820117308201133082010F060B60861801020101050701023081FF302A06082B06010505070201161E687474703A2F2F6465706F2E6B616D75736D2E676F762E74722F696C6B653081D006082B060105050702023081C31E81C00042007500200073006500720074006900660069006B006100200069006C006500200069006C00670069006C006900200073006500720074006900660069006B006100200069006C006B0065006C006500720069006E00690020006F006B0075006D0061006B0020006900E70069006E002000620065006C0069007200740069006C0065006E00200077006500620020007300690074006500730069006E00690020007A0069007900610072006500740020006500640069006E0069007A002E30090603551D1304023000301D0603551D250416301406082B0601050507030106082B06010505070302303D0603551D1F043630343032A030A02E862C687474703A2F2F6465706F2E6B616D75736D2E676F762E74722F6F6B632F4F4B432D53312D53494C2E63726C304406082B0601050507010104383036303406082B060105050730028628687474703A2F2F6465706F2E6B616D75736D2E676F762E74722F6F6B632F4F4B432D53312E636572300D06092A864886F70D01010B050003820101009F7F7AB092A7FD0E6EF8D5BB7B4DD9E8BC64CEF127B8E0C392E19C051559654ED5B29C9DD89233E1F18150879B2DE9569BA8C5C33FA2E78B6B463CB44AF342519A0C3505378D43D5088CC8A0B850D99FC8FB8B9BCD8CADCD869D3A37434C6FE5D85D6C43797CF031905E11D44827D614CB482E23BE7AE0BA5F4D51F25CDCC61ADB434F3B777E45E133FA0F8BA5D57160E26EF29BF373E147D7A7D8A3E682DF6D3DDC23218101B6AE642BA447C5B5D3F4950C8F2E1FDAE173DDF88673569827B4C3A302F05CBDBA7DB1A4AA1C183EA50BA60619114C7026EA0CB8DA248BABEE2D2601DCCF55DCDD340D59659E60D79898A5E1159889E085EE89EB956A0A23F421";
        string m_strSignature = "6DD255C0077F0F737D408E510D527260F31096AA2D92FD50E08F9794B274712511270F54934E9A05303E9C37AF31BA8D785FF869F48ADDDE8E62BF49CD9955FB636FBB2CC86FEBF5E3FCB3A78AF2460A6F6299CC326CBAD37DCBA0B262FB473CB8123F7327D3C5D6F046DAE3276F33BAA792BC9A498A90F0222EAFEF5269A515D8DC0F05146D4A8265033A9B3EDB20D23F96313DF65D55F3824F20DB11ADE2483E16D6BF161587D8C73EE1A59E6A464E034B95986D296B4894908AC114DE1C8FCA378CD5E94A050210DAE2BDF4DD9D1DA8D0289D4BCE2AFF5107A1A45F68CE603B420CDE93BAA002DF706A8FC75200AA877E6BB39A311B0840985AC75A88D6A6";
        string m_strOriginal = "0969398B5587E5A9027DA11E25790A4F9BF7267ECF7812F8D34AEF07B5EC2669E15EC2CC6661F622C39DD0AC39B6BEFEE2A30FC5BEBE94673BF18BD10F35F913FA855D8D07179A0789EC841B51073CFE88CB3A6577211A94785968FF3AB7FC60E9EF49D6F5DF6705001F7AD17A06BB120064059CB462B067E88CE49F6F6CE5E5B89882ACFB71DFB396993DB68636C444DD0D79E36EE6526A44E79860813506D7494D041EB4F1DA6304C9FA30ABC6A0CEBC5BE92BC8FBD3213C1F0837DD00295DC6D0354314B4AF611886176C0A2747DB8ACC26DFB883314B1C167416BCEB73B9AFDCEB7B6D36499029999A072A4C69C7ECCF5C932EB2C5FFFEDF7CEFA5FA00A8";

        public class ValidateSignObject
        {
            public string strPOkc { get; set; }
            public string strSignedData { get; set; }
            public string strSourcesData { get; set; }
            public string strTerminalId { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {



            ValidateSignObject xValidateSignObject = new ValidateSignObject();
            xValidateSignObject.strPOkc = m_strCertificateData;
            xValidateSignObject.strSignedData = m_strSignature;
            xValidateSignObject.strSourcesData = m_strOriginal;
            xValidateSignObject.strTerminalId = "TEST00001728";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string strOutput = serializer.Serialize(xValidateSignObject);

            string strUri = "http://kasa.edata.com.tr/services/ValidateSign";
            Uri uri = new Uri(strUri);
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string strOut = jsonSerializer.Serialize(xValidateSignObject);

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(strOut);
            }

            WebResponse responce = request.GetResponse();
            Stream reader = responce.GetResponseStream();

            StreamReader sReader = new StreamReader(reader);
            string strOutResult = sReader.ReadToEnd();
            sReader.Close();
            label1.Text = string.Format("Sonuç : {0}", strOutResult);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            try
            {
                ValidateSignObject xValidateSignObject = new ValidateSignObject();
                xValidateSignObject.strPOkc = m_strCertificateData;
                xValidateSignObject.strSignedData = m_strSignature;
                xValidateSignObject.strSourcesData = m_strOriginal;
                xValidateSignObject.strTerminalId = "TEST00001728";

                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                string strJsonOut = jsonSerializer.Serialize(xValidateSignObject);

                string strBody = strJsonOut;
                string strHostname = "kasa.edata.com.tr";
                int iPort = 80;
                string strMessage = @"POST /services/ValidateSign HTTP/1.0
Host: " + strHostname + @"
Accept: application/json
Accept-encoding: gzip;q=0,deflate,sdch
User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)
Content-Type: application/json; charset=utf-8
Content-Length: " + strBody.Length.ToString() + @"

" + strBody + @"
";

                string strResponseMessage = String.Empty;
                using (TcpClient clientHandler = new TcpClient(strHostname, iPort))
                {
                    clientHandler.ReceiveTimeout = 1000;
                    if (clientHandler.Connected)
                    {
                        using (NetworkStream clientStream = clientHandler.GetStream())
                        {
                            if (!clientStream.CanRead || !clientStream.CanWrite)
                            {
                                throw new Exception("TCP Socket: Stream cannont preform read/write");
                            }
                            Byte[] bytes2send = Encoding.ASCII.GetBytes(strMessage);
                            clientStream.Write(bytes2send, 0, bytes2send.Length);
                            do
                            {
                                Byte[] bytesReceived = new Byte[clientHandler.ReceiveBufferSize];

                               
                                int iStreamReceived = clientStream.Read(bytesReceived, 0, bytesReceived.Length);
                                strResponseMessage = Encoding.ASCII.GetString(bytesReceived, 0, iStreamReceived);
                                
                                
                            } while (clientStream.DataAvailable);
                        }

                    }
                    else
                    {
                        throw new Exception("TCP Socket: Could not connect to server");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ValidateSignObject xValidateSignObject = new ValidateSignObject();
            //xValidateSignObject.strPOkc = m_strCertificateData;
            //xValidateSignObject.strSignedData = m_strSignature;
            //xValidateSignObject.strSourcesData = m_strOriginal;
            //xValidateSignObject.strTerminalId = "TEST00001728";


            //ValidateSignObjectEncrypted xVSOEncrypted = new ValidateSignObjectEncrypted(){
            //    strData = Converter.StrToHexStr(strEncryptReturnString(JsonConvert.SerializeObject(xValidateSignObject)))
            //};

            //label1.Text = strValidateSign(xVSOEncrypted);
        }
       

    }
}

