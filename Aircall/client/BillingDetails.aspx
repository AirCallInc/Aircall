<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="BillingDetails.aspx.cs" Inherits="Aircall.client.BillingDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Billing History Detail</h1>
                </div>
                <div class="border-block">
                    <div class="main-from">
                        <div class="single-row cf" runat="server" id="dvNoShow">
                            <div class="left-side">
                                <label>Service No :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceNo" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvPart">
                            <div class="left-side">
                                <label>Order No :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrOrderNo" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvUnit" style="display:none">
                            <div class="left-side">
                                <label>Plan :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrPlan" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvUnit1" style="display:none">
                            <div class="left-side">
                                <label>Unit :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrUnit" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Transaction Id :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrTransactionId" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Date :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrDate" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Time :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Amount :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrAmount" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label><asp:Literal ID="ltrPaymentMethod" runat="server"></asp:Literal>:</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrPaymentByNumber" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvPart1">
                            <div class="left-side">
                                <label>Parts :</label>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvPartGrid">
                            <div class="table-outer">
                                <table cellspacing="0" cellpadding="0" border="none" class="common-table billing-history-table">
                                    <thead>
                                        <tr>
                                            <th>Part</th>
                                            <th>Quantity</th>
                                            <th>Size</th>
                                            <th>Rate</th>
                                            <th>Total</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstParts" runat="server">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%#Eval("PartName") %></td>
                                                    <td><%#Eval("Quantity") %></td>
                                                    <td><%#Eval("PartSize") %></td>
                                                    <td>$ <%#Eval("Amount") %></td>
                                                    <td>$ <%#decimal.Parse(Eval("Quantity").ToString()) * decimal.Parse(Eval("Amount").ToString()) %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </tbody>
                                </table>
                            </div>
                        </div>

                        <div class="single-row button-bar cf">
                            <%--<button type="submit" class="main-btn dark-grey">Back to list</button>--%>
                            <input type="button" class="main-btn dark-grey" value="Back To List" onclick="location.href = 'billing-history.aspx'" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
