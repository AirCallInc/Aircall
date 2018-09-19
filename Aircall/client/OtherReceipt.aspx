<%@ Page Title="Receipt" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="OtherReceipt.aspx.cs" Inherits="Aircall.client.OtherReceipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                                        <th>Client Name</th>
                                        <th>Service No</th>
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
                                        <asp:Literal ID="litClientName" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:Literal ID="litService" runat="server"></asp:Literal>
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
            </div>
        </div>
    </div>
</asp:Content>
