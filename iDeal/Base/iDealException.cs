using System;
using System.Xml.Linq;

namespace iDeal.Base
{
    public class iDealException : Exception
    {
        public DateTime createDateTimestamp { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ErrorDetail { get; private set; }
        public string ConsumerMessage { get; set; }
        
        public iDealException(XElement xDocument)
        {
            XNamespace xmlNamespace = "http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1";

            createDateTimestamp = DateTime.Parse(xDocument.Element(xmlNamespace + "createDateTimestamp").Value);

            ErrorCode = xDocument.Element(xmlNamespace + "Error").Element(xmlNamespace + "errorCode").Value;
            ErrorMessage = xDocument.Element(xmlNamespace + "Error").Element(xmlNamespace + "errorMessage").Value;
            ErrorDetail = xDocument.Element(xmlNamespace + "Error").Element(xmlNamespace + "errorDetail").Value;
            ConsumerMessage = xDocument.Element(xmlNamespace + "Error").Element(xmlNamespace + "consumerMessage").Value;
        }
    }
}
