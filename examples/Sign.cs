using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace myApp
{
    using System.Globalization;

    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;

    public class Signature
    {
        public Signature(string partnerId, string privateKey, string requestUrl, string requestMethod, string payload)
        {
            this.PartnerId = partnerId;
            this.PrivateKey = privateKey;
            this.RequestUrl = requestUrl;
            this.RequestMethod = requestMethod;
            this.Payload = payload;
        }

        public string PartnerId { get; set; }

        public string PrivateKey { get; set; }

        public string RequestUrl { get; set; }

        public string RequestMethod { get; set; }

        public string TimeStamp { get; private set; }

        public string Payload { get; private set; }

        public string GetSignature(string timestamp)
        {
            this.TimeStamp = timestamp;

            var message = this.PartnerId + "\\n" + this.RequestUrl + "\\n" +
                this.RequestMethod.ToLower() + "\\n" + this.TimeStamp + "\\n" + this.Payload;

            RsaKeyParameters rsaKeyParameter;
            try
            {
                byte[] keyBytes = File.ReadAllBytes(@"/Users/ankit/handydev/repos/service-partners-backend/private.der");
                var asymmetricKeyParameter = PrivateKeyFactory.CreateKey(keyBytes);
                rsaKeyParameter = (RsaKeyParameters)asymmetricKeyParameter;
            }
            catch (System.Exception)
            {
                throw new Exception("Unable to load private key");
            }

            var signer = SignerUtilities.GetSigner("SHA256withRSA");
            signer.Init(true, rsaKeyParameter);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            signer.BlockUpdate(messageBytes, 0, messageBytes.Length);
            var signed = signer.GenerateSignature();
            var hashed = Convert.ToBase64String(signed);
            return hashed;
        }

        private static string GetTimestampInMillis()
        {
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime localDateTime, univDateTime;
            localDateTime = DateTime.Now;          
            univDateTime = localDateTime.ToUniversalTime();
            return ((long)(univDateTime - UnixEpoch).TotalMilliseconds/1000).ToString();
        }

        static void Main(string[] args)
        {
            var partnerId = "partner_name";
            var payload = "{\"user\":{\"first_name\":\"John\",\"last_name\":\"Doe\",\"email\":\"johndoe@example.com\",\"phone_number\":\"2125555555\"},\"address\":{\"address1\":\"10 Main Street\",\"address2\":\"Apt 2\",\"city\":\"New York\",\"state\":\"NY\",\"zipcode\":10012,\"country\":\"US\"},\"order\":{\"partner_order_id\":\"12345\",\"products\":[{\"sku\":\"STSP1001\",\"product_info\":{\"product_name\":\"Arm chair photo\"},\"order_details\":{\"quantity\":2,\"delivery_date\":\"2018-04-21T03:50:02.000Z\"}}]}}";
    	      var url = "http://https://partners.services.handy.com/api/v1/orders";
    	      var httpMethod = "post";

            /* You will need to convert your private key to PKCS8 format using following command:
            * openssl pkcs8 -topk8 -inform PEM -outform DER -in private.pem -out private.der -nocrypt
            */
            var privateKeyPath = "test_private_key.der";

            var timestamp = GetTimestampInMillis();
            var signature = new Signature(partnerId, privateKeyPath, url, httpMethod, payload).GetSignature(timestamp);

            Console.WriteLine("Content-Type: application/json");
            Console.WriteLine("HDY-PARTNER-ID: " + partnerId);
            Console.WriteLine("HDY-TIMESTAMP: " + timestamp);
            Console.WriteLine("HDY-SIGNATURE: " + signature);
        }
    }
}
