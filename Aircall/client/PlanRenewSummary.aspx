<%@ Page Title="Summary" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="PlanRenewSummary.aspx.cs" Inherits="Aircall.client.PlanRenewSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Summary</h1>
                </div>
                <div class="receipt-name">
                    <div class="name">
                        <asp:Literal ID="litName" runat="server"></asp:Literal></div>
                    <span>
                        <asp:Literal ID="litEmail" runat="server"></asp:Literal></span>
                </div>
                <div class="table-outer">
                    <table class="common-table receipt-table">
                        <asp:ListView ID="lstSummary" runat="server" OnItemDataBound="lstSummary_ItemDataBound" OnDataBound="lstSummary_DataBound">
                            <LayoutTemplate>
                                <thead>
                                    <tr>
                                        <th>Unit Name</th>
                                        <th>Package/Plan</th>
                                        <th>Description</th>
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
                                        <asp:Literal ID="litDesc" runat="server"></asp:Literal>
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
                <a class="main-btn" href="OtherPayment.aspx">Checkout</a>
            </div>
        </div>
    </div>
</asp:Content>
