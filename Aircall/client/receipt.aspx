<%@ Page Title="Receipt" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="receipt.aspx.cs" Inherits="Aircall.client.receipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<!-- Google Code for Successful Plan Purchase Conversion Page -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 851483616;
var google_conversion_language = "en";
var google_conversion_format = "3";
var google_conversion_color = "ffffff";
var google_conversion_label = "Qb4LCMCq7XEQ4LeClgM";
var google_remarketing_only = false;
/* ]]> */
</script>
<script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="//www.googleadservices.com/pagead/conversion/851483616/?label=Qb4LCMCq7XEQ4LeClgM&amp;guid=ON&amp;script=0"/>
</div>
</noscript>
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Payment Receipt</h1>
                </div>
                <div class="receipt-name">
                    <div class="name">
                        <asp:Literal ID="litName" runat="server"></asp:Literal>
                    </div>
                    <span>
                        <asp:Literal ID="litEmail" runat="server"></asp:Literal></span>
                </div>
                <div class="table-outer">
                    <div id="dvMessage" runat="server" visible="false"></div>
                    <table class="common-table receipt-table">
                        <asp:ListView ID="lstSummary" runat="server" OnItemDataBound="lstSummary_ItemDataBound" OnDataBound="lstSummary_DataBound">
                            <LayoutTemplate>
                                <thead>
                                    <tr>
                                        <th>Unit Name</th>
                                        <th>Package/Plan</th>
                                        <th>Payment Type</th>
                                        <th>Price</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr runat="server" id="itemPlaceholder" />
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td class="total-amount">Total Amount:
                                            <br>
                                            <big><strong><asp:Literal ID="litTotal" runat="server"></asp:Literal></strong></big>
                                        </td>
                                    </tr>
                                </tfoot>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr runat="server">
                                    <td>
                                        <asp:Literal ID="litUnitName" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="litPlan" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="litPlanType" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="litAmount" runat="server"></asp:Literal>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </table>
                </div>
                <a class="main-btn" href="dashboard.aspx">Go to Dashboard</a>
                <asp:LinkButton ID="btnRetry" runat="server" CssClass="main-btn" Text="Try Again" Visible="false" PostBackUrl="/client/PaymentMethod.aspx"/>
            </div>
        </div>
    </div>
</asp:Content>
