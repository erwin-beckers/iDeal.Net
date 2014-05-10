using iDeal.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iDeal.Example
{
  public partial class Default : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {

      EnableButtons();
    }
    protected void btnDirReq_Click(object sender, EventArgs e)
    {
      try
      {
        tableError.Visible = false;
        logResult.InnerHtml = "";

        var ideal = new iDealService();
        var response = ideal.SendDirectoryRequest();
        foreach (var issuer in response.Issuers)
        {
          logResult.InnerHtml += String.Format("Issuer: ID={0} Name={1}<br/>", issuer.Id, issuer.Name);
        }
        Session["issuers"] = response.Issuers;
        EnableButtons();
      }

      catch (iDealException ex)
      {
        tableError.Visible = true;
        lblErrorCode.Text = ex.ErrorCode;
        lblErrorDetail.Text = ex.ErrorDetail;
        lblErrorMessage.Text = ex.ErrorMessage;
        lblConsumerMessage.Text = ex.ConsumerMessage;
      }
    }
    protected void btnTransaction1_Click(object sender, EventArgs e)
    {
      try
      {
        tableError.Visible = false;
        logResult.InnerHtml = "";

        var ideal = new iDealService();

        var issuers = (IList<iDeal.Directory.Issuer>)Session["issuers"];
        int amount = Int32.Parse(txtAmount.Value);
        var response = ideal.SendTransactionRequest(issuers[0].Id, "http://www.your-url.com", "purchaseId", amount, TimeSpan.FromMinutes(5), "Buy something for " + txtAmount.Value + " euro", "myentrancecode");

        logResult.InnerHtml = String.Format("AcquirerId:{0}<br>", response.AcquirerId);
        logResult.InnerHtml += String.Format("TransactionId:{0}<br>", response.TransactionId);
        logResult.InnerHtml += String.Format("PurchaseId:{0}<br>", response.PurchaseId);
        logResult.InnerHtml += String.Format("<a href='{0}' target=='new'>complete transaction</a><br>", response.IssuerAuthenticationUrl);

        Session["transactionid"] = response.TransactionId;
        EnableButtons();
      }

      catch (iDealException ex)
      {
        tableError.Visible = true;
        lblErrorCode.Text = ex.ErrorCode;
        lblErrorDetail.Text = ex.ErrorDetail;
        lblErrorMessage.Text = ex.ErrorMessage;
        lblConsumerMessage.Text = ex.ConsumerMessage;
      }

    }

    private void EnableButtons()
    {
      btnTransaction1.Enabled = (Session["issuers"] != null);
      btnStatus1.Enabled = (Session["transactionid"] != null);
    }
    protected void btnStatus1_Click(object sender, EventArgs e)
    {
      try
      {
        tableError.Visible = false;
        logResult.InnerHtml = "";

        var ideal = new iDealService();
        var response = ideal.SendStatusRequest((string)Session["transactionid"]);

        logResult.InnerHtml += String.Format("Status:{0}<br>", response.Status);
        logResult.InnerHtml = String.Format("TransactionId:{0}<br>", response.TransactionId);
        logResult.InnerHtml += String.Format("AcquirerId:{0}<br>", response.AcquirerId);
        logResult.InnerHtml += String.Format("ConsumerName:{0}<br>", response.ConsumerName);
        logResult.InnerHtml += String.Format("ConsumerIBAN:{0}<br>", response.ConsumerIBAN);
        logResult.InnerHtml += String.Format("ConsumerBIC:{0}<br>", response.ConsumerBIC);

        Session["transactionid"] = null;
        EnableButtons();
      }

      catch (iDealException ex)
      {
        tableError.Visible = true;
        lblErrorCode.Text = ex.ErrorCode;
        lblErrorDetail.Text = ex.ErrorDetail;
        lblErrorMessage.Text = ex.ErrorMessage;
        lblConsumerMessage.Text = ex.ConsumerMessage;
      }

    }
  }
}