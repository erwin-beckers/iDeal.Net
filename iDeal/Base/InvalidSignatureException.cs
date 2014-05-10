using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace iDeal.Base
{
  public class InvalidSignatureException:iDealException
  {
    public InvalidSignatureException()
    {

      ErrorCode = "SIG0001";
      ErrorMessage = "Invalid signature";
      ErrorDetail = "Unable to verify signature from iDeal provider";
      ConsumerMessage = "Betaling met iDeal is momenteel niet mogelijk, probeer het later nog eens";
    }
    public InvalidSignatureException(XElement xDocument)
    {

    }

  }
}
