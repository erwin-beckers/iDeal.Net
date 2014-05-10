namespace iDeal.SignatureProviders
{
    public interface ISignatureProvider
    {
        /// <summary>
        /// Verifies the digital signature used in status responses from the ideal api (stored in xml field signature value)
        /// </summary>
        /// <param name="signature">Signature provided by ideal api, stored in signature value xml field</param>
        /// <param name="messageDigest">Concatenation of designated fields from the status response</param>
        bool VerifySignature(string xml);

        /// <summary>

        string SignXml(System.Xml.Linq.XDocument directoryRequestXmlMessage);
    }
}