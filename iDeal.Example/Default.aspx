<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="iDeal.Example.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1> Commands:</h1> 
            <asp:Button runat="server" ID="btnDirReq" Text="DirectoryRequest" OnClick="btnDirReq_Click" /> <br />
            <asp:Button runat="server" ID="btnTransaction1" Text="Transaction" OnClick="btnTransaction1_Click" /><br />
            <asp:Button runat="server" ID="btnStatus1" Text="Status" OnClick="btnStatus1_Click" /><br />
            Amount:<input runat="server" id="txtAmount" value="1" />euro<br />
        </div>
        <hr />
        <h2>Results:</h2>

         <div runat="server" id="logResult"></div>

        <table runat="server" ID="tableError">
            <tr>
                <td>Error Code</td>
                <td><asp:Label runat="server" ID="lblErrorCode"></asp:Label></td>
            </tr>
            <tr>
                <td>Error Message</td>
                <td><asp:Label runat="server" ID="lblErrorMessage"></asp:Label></td>
            </tr>
            <tr>
                <td>Error Detail</td>
                <td><asp:Label runat="server" ID="lblErrorDetail"></asp:Label></td>
            </tr>
            <tr>
                <td>Consumer Message</td>
                <td><asp:Label runat="server" ID="lblConsumerMessage"></asp:Label></td>
            </tr>
        </table>
    </form>
</body>
</html>
