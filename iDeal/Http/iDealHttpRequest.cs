using System.IO;
using System.Net;
using System.Text;
using iDeal.Base;
using iDeal.SignatureProviders;

namespace iDeal.Http
{
  public class iDealHttpRequest : IiDealHttpRequest
  {
    public iDealResponse SendRequest(iDealRequest idealRequest, ISignatureProvider signatureProvider, string url, IiDealHttpResponseHandler iDealHttpResponseHandler)
    {
      System.Net.ServicePointManager.ServerCertificateValidationCallback =((sender, certificate, chain, sslPolicyErrors) => true);
      

      // Create request
      var request = (HttpWebRequest)WebRequest.Create(url);
      request.ProtocolVersion = HttpVersion.Version11;
      request.ContentType = "text/xml";
      request.Method = "POST";
//      request.Proxy = new WebProxy("192.168.1.8", 8080);
      // Set content

      string xml = idealRequest.ToXml(signatureProvider);
      var postBytes = Encoding.ASCII.GetBytes(xml);

      // Send
      var requestStream = request.GetRequestStream();
      requestStream.Write(postBytes, 0, postBytes.Length);
      requestStream.Close();

      // Return result
      var response = (HttpWebResponse)request.GetResponse();
      return iDealHttpResponseHandler.HandleResponse(new StreamReader(response.GetResponseStream()).ReadToEnd(), signatureProvider);
    }
  }
}
