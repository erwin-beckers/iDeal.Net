using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using iDeal.Base;

namespace iDeal.Directory
{
    public class DirectoryResponse : iDealResponse
    {
        private readonly IList<Issuer> _issuers = new List<Issuer>();

        public string DirectoryDateTimeStamp { get; private set; }

        public DateTime DirectoryDateTimeStampLocalTime { get { return DateTime.Parse(DirectoryDateTimeStamp); } }

        public IList<Issuer> Issuers
        {
            get { return new ReadOnlyCollection<Issuer>(_issuers); }
        }

        public DirectoryResponse(string xmlDirectoryResponse)
        {
            // Parse document
            var xDocument = XElement.Parse(xmlDirectoryResponse);
            XNamespace xmlNamespace = "http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1";

            // Create datetimestamp
            createDateTimestamp = xDocument.Element(xmlNamespace + "createDateTimestamp").Value;
            
            // Acquirer id
            AcquirerId = (int)xDocument.Element(xmlNamespace + "Acquirer").Element(xmlNamespace + "acquirerID");

            // Directory datetimestamp
            DirectoryDateTimeStamp = xDocument.Element(xmlNamespace + "Directory").Element(xmlNamespace + "directoryDateTimestamp").Value;
          
            // Get list of countries
            foreach (var country in xDocument.Element(xmlNamespace + "Directory").Elements(xmlNamespace + "Country"))
            {
              // Get list of issuers
              foreach (var issuer in country.Elements(xmlNamespace + "Issuer"))
              {
                  _issuers.Add(
                          new Issuer(
                                  issuer.Element(xmlNamespace + "issuerID").Value,
                                  issuer.Element(xmlNamespace + "issuerName").Value
                              )
                      );
              }
            }
        }
    }
}