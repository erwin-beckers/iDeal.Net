using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace iDeal.SignatureProviders
{
    public class SignatureProvider : ISignatureProvider
    {
        private readonly X509Certificate2 _privateCertificate;
        private readonly X509Certificate2 _publicCertificate;

        public SignatureProvider(X509Certificate2 privateCertificate, X509Certificate2 publicCertificate)
        {
            _privateCertificate = privateCertificate;
            _publicCertificate = publicCertificate;
        }
        
        /// <summary>
        /// Gets the digital signature used in each request send to the ideal api (stored in xml field tokenCode)
        /// </summary>
        /// <param name="messageDigest">Concatenation of designated fields from the request. Varies between types of request, consult iDeal Merchant Integratie Gids</param>
        public string GetSignature(string messageDigest)
        {
            // Step 1: Create a 160 bit message digest
            var hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(messageDigest));

            //Step 2: Sign with 1024 bits private key (RSA)
            var rsaCryptoServiceProvider = (RSACryptoServiceProvider)_privateCertificate.PrivateKey; // Create rsa crypto provider from private key contained in certificate, weirdest cast ever!
            var encryptedMessage = rsaCryptoServiceProvider.SignHash(hash, "SHA1");

            // Step 3: Base64 encode string for storage in xml request
            return Convert.ToBase64String(encryptedMessage);
        }

        /// <summary>
        /// Verifies the digital signature used in status responses from the ideal api (stored in xml field signature value)
        /// </summary>
        /// <param name="signature">Signature provided by ideal api, stored in signature value xml field</param>
        /// <param name="messageDigest">Concatenation of designated fields from the status response</param>
        public bool VerifySignature(string signature, string messageDigest)
        {
            // Step 1: Create a 160 bit message digest to compare with the one provided in the signature
            var hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(messageDigest));
            
            // Step 2: Base 64 deocde signature
            var decodedSignature = System.Convert.FromBase64String(signature);

            // Step 3: Verify signature with public key
            var rsaCryptoServiceProvider = (RSACryptoServiceProvider)_publicCertificate.PublicKey.Key;
            return rsaCryptoServiceProvider.VerifyHash(hash, "SHA1", decodedSignature);
        }

        /// <summary>
        /// Gets thumbprint of acceptant's certificate, used in each request to the ideal api (stored in field token)
        /// </summary>
        public string GetThumbprintAcceptantCertificate()
        {
            return _privateCertificate.Thumbprint;
        }

        /// <summary>
        /// Gets thumbprint of the acquirer's certificate, used in status response from ideal api
        /// </summary>
        public string GetThumbprintAcquirerCertificate()
        {
            return _publicCertificate.Thumbprint;
        }

        public  string SignXml(XDocument xml)
        {
          using (MemoryStream streamIn = new MemoryStream())
          {
            xml.Save(streamIn);
            streamIn.Position = 0;
          //  var rsaKey = (RSACryptoServiceProvider)_privateCertificate.PrivateKey; // Create rsa crypto provider from private key contained in certificate, weirdest cast ever!;



            string sCertFileLocation = @"C:\plugins\idealtest\bin\Debug\certficate.pfx";
            X509Certificate2 certificate = new X509Certificate2(sCertFileLocation, "D3M@ast3rsR0cks");
            RSA rsaKey = (RSACryptoServiceProvider)certificate.PrivateKey;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(streamIn);

            SignedXml signedXml = new SignedXml(xmlDoc);
            signedXml.SigningKey = rsaKey;

            Reference reference = new Reference();
            reference.Uri = "";
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);
            signedXml.AddReference(reference);


            KeyInfo keyInfo = new KeyInfo();
            KeyInfoName kin = new KeyInfoName();
            kin.Value = _privateCertificate.Thumbprint;
            keyInfo.AddClause(kin);
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));


            using (MemoryStream sout = new MemoryStream())
            {
              xmlDoc.Save(sout);
              sout.Position = 0;
              using (StreamReader reader = new StreamReader(sout))
              {
                string xmlOut = reader.ReadToEnd();
                return xmlOut;
              }
            }
          }

        }
    }
}
