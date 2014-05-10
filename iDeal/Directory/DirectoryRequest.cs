using System;
using System.Xml.Linq;
using iDeal.Base;
using iDeal.SignatureProviders;

namespace iDeal.Directory
{
    public class DirectoryRequest : iDealRequest
    {
        public DirectoryRequest(string merchantId, int? subId)
        {
            MerchantId = merchantId;
            MerchantSubId = subId ?? 0; // If no sub id is specified, sub id should be 0
        }
        
        public override string MessageDigest
        {
            get { return createDateTimestamp + MerchantId + MerchantSubId; }
        }

        /// <summary>
        /// Creates xml representation of directory request
        /// </summary>
        public override string ToXml(ISignatureProvider signatureProvider)
        {
            XNamespace xmlNamespace = "http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1";

            var directoryRequestXmlMessage =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    new XElement(xmlNamespace + "DirectoryReq",
                        new XAttribute("version", "3.3.1"),
                        new XElement(xmlNamespace + "createDateTimestamp", createDateTimestamp),
                        new XElement(xmlNamespace + "Merchant",
                            new XElement(xmlNamespace + "merchantID", MerchantId.PadLeft(9, '0')),
                            new XElement(xmlNamespace + "subID", "0")
                        )
                    )
                );

            return signatureProvider.SignXml(directoryRequestXmlMessage);
        }
    }
}