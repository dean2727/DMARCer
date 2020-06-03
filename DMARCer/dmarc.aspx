<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="dmarc.aspx.cs" Inherits="Dean.dmarc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>dmarc</title>
    <link href="dmarcStyle.css?v=1" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="functionality">
            <div class="bigDiv">
                <div class="halfDiv left">
                    <asp:Button class="button" ID="getInfo" onClick="getInfo_Click" text="Deserialize an XML file" runat="server"/>
                </div>
                <div class="halfDiv right">
                    <asp:Button class="button" ID="listIPs" OnClick="listIPs_Click" Text="Get bad IPs from all XML files" runat="server"/>               
                </div>
            </div>
            <div class="outDiv">
                <div class="halfDiv left out">
                    <asp:Panel id="literal1" Visible="false" runat="server">  <!-- panel instead of div so that the properties are visible only when button is clicked -->
                        <asp:Literal ID="output1" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
                <div class="halfDiv right out">
                    <asp:Panel id="literal2" Visible="false" runat="server">
                        <asp:Literal ID="output2" runat="server"></asp:Literal>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
