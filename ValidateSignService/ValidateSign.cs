using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Net;
using Edata.CommonLibrary;
using System.Runtime.Serialization.Json;


namespace ValidateSignService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ValidateSign : IValidateSign
    {

        public string strValidateSign(ValidateSignObjectEncrypted xValidateSignObjectEncrypted)
        {
            DateTime dCertificateStartTime;
            DateTime dCertificateEndTime;

            ValidateSignObject xValidateSignObject = xDecryptValidateSignObject(xValidateSignObjectEncrypted);

            byte[] baPOkc = Converter.HexToByteArray(xValidateSignObject.strPOkc);
            byte[] baSignedData = Converter.HexToByteArray(xValidateSignObject.strSignedData);
            byte[] baSourcesData = Converter.HexToByteArray(xValidateSignObject.strSourcesData);
            string strTerminalId = xValidateSignObject.strTerminalId;
            string strCertificateDeviceSerialNumber = "";
            string strCertificateSubjects = "";

            try
            {
                X509Certificate2 xX509Certificate2 = new X509Certificate2();
                xX509Certificate2.Import(baPOkc);

                strCertificateSubjects = xX509Certificate2.Subject;
                dCertificateStartTime = xX509Certificate2.NotBefore;
                dCertificateEndTime = xX509Certificate2.NotAfter;

                string[] astrCertificateSubjects = strCertificateSubjects.Split(',');
                foreach (string subjects in astrCertificateSubjects)
                {
                    if (subjects.Contains("SERIALNUMBER"))
                    {
                        strCertificateDeviceSerialNumber = subjects.Substring(13, subjects.Length - 13);
                        break;
                    }
                }

                if (strCertificateDeviceSerialNumber == strTerminalId && dCertificateEndTime >= DateTime.Now && dCertificateStartTime <= DateTime.Now)
                    return strVerifySignature_2048_Bit_PKCS1_v1_5(baSourcesData, baSignedData, xX509Certificate2);
                else
                {
                    if (dCertificateEndTime >= DateTime.Now && dCertificateStartTime <= DateTime.Now)
                        Trace.vInsertMethodTrace(enumTraceLevel.Important, string.Format("Certificate has expired on {0}", dCertificateEndTime));
                    else
                        Trace.vInsertMethodTrace(enumTraceLevel.Important, "DeviceSerialNumber does not match");
                    return strEncryptReturnString("false|" + DateTime.Now.ToString("yyMMddHHmm"));
                }
            }
            catch (Exception xException)
            {
                Trace.vInsertMethodTrace(enumTraceLevel.Important, xException.ToString());
                return strEncryptReturnString("false|" + DateTime.Now.ToString("yyMMddHHmm"));
            }

        }

        private string strVerifySignature_2048_Bit_PKCS1_v1_5(byte[] baSourceData, byte[] ba_SignedData, X509Certificate2 x509Certificate2)
        {
            SHA256 sha1 = SHA256.Create();
            byte[] hash = sha1.ComputeHash(baSourceData);

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509Certificate2.PublicKey.Key;
            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");
            if (rsaDeformatter.VerifySignature(hash, ba_SignedData))
            {
                return strEncryptReturnString("true|" + DateTime.Now.ToString("yyMMddHHmm"));
            }
            else
            {
                return strEncryptReturnString("false|" + DateTime.Now.ToString("yyMMddHHmm"));
            }
        }

        private ValidateSignObject xDecryptValidateSignObject(ValidateSignObjectEncrypted prm_xValidateSignObjectEncrypted)
        {
            byte[] baAESKey = Converter.HexToByteArray("393DDC99AB2350EC43651D2A76499932EBEDF828873581E1EF081C2521ACF8E2");
            byte[] baData = Converter.HexToByteArray(prm_xValidateSignObjectEncrypted.strData);
            var xAesDecryption = new AesManaged
            {
                KeySize = 256,
                Key = baAESKey,
                BlockSize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            ICryptoTransform decryptor = xAesDecryption.CreateDecryptor(xAesDecryption.Key, xAesDecryption.IV);
            byte[] decryptedOutput = decryptor.TransformFinalBlock(baData, 0, baData.Length);
            Console.Write(Converter.ToString(decryptedOutput));

            ValidateSignObject xValidateSignObject = new ValidateSignObject();
            MemoryStream stream = new MemoryStream(decryptedOutput);
            DataContractJsonSerializer js = new DataContractJsonSerializer(xValidateSignObject.GetType());

            xValidateSignObject = (ValidateSignObject)js.ReadObject(stream);

            return xValidateSignObject;
        }

        private string strEncryptReturnString(string prm_strReturnString)
        {
            ValidateSignObject xValidateSignObject = new ValidateSignObject();
            byte[] baAESKey = Converter.HexToByteArray("393DDC99AB2350EC43651D2A76499932EBEDF828873581E1EF081C2521ACF8E2");
            byte[] baData = Converter.ToByteArray(prm_strReturnString);
            var xAesEncryption = new AesManaged
            {
                KeySize = 256,
                Key = baAESKey,
                BlockSize = 128,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            ICryptoTransform encryptor = xAesEncryption.CreateEncryptor(xAesEncryption.Key, xAesEncryption.IV);
            byte[] encryptedOutput = encryptor.TransformFinalBlock(baData, 0, baData.Length);

            return encryptedOutput.ToByteString();
        }

    }

}
