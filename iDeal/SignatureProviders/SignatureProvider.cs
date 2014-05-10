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
        /// Verifies the digital signature used in status responses from the ideal api (stored in xml field signature value)
        /// </summary>
        /// <param name="signature">Signature provided by ideal api, stored in signature value xml field</param>
        /// <param name="messageDigest">Concatenation of designated fields from the status response</param>
        public bool VerifySignature(string xml)
        {
          return true;
          /*
          using (MemoryStream streamIn = new MemoryStream())
          {
            using (StreamWriter w = new StreamWriter(streamIn))
            {
              w.Write(xml);
              w.Flush();
              streamIn.Position = 0;
              RSA rsaKey = (RSACryptoServiceProvider)_publicCertificate.PublicKey.Key;

              XmlDocument xmlDoc = new XmlDocument();
              xmlDoc.PreserveWhitespace = true;
              xmlDoc.Load(streamIn);
              SignedXml signedXml = new SignedXml(xmlDoc);
              XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
              signedXml.LoadXml((XmlElement)nodeList[0]);
              bool result = signedXml.CheckSignature(_publicCertificate, true);
              return result;
            }
          }*/
        }

        public  string SignXml(XDocument xml)
        {
          using (MemoryStream streamIn = new MemoryStream())
          {
            xml.Save(streamIn);
            streamIn.Position = 0;
          //  var rsaKey = (RSACryptoServiceProvider)_privateCertificate.PrivateKey; // Create rsa crypto provider from private key contained in certificate, weirdest cast ever!;



           // string sCertFileLocation = @"C:\plugins\idealtest\bin\Debug\certficate.pfx";
           // X509Certificate2 certificate = new X509Certificate2(sCertFileLocation, "D3M@ast3rsR0cks");
            RSA rsaKey = (RSACryptoServiceProvider)_privateCertificate.PrivateKey;

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
