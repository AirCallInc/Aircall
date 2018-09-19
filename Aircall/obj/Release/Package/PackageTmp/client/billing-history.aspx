<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="billing-history.aspx.cs" Inherits="Aircall.client.billing_history" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }
        a.btn.btn-default {
            float: left;
            padding: 4px 12px;
            line-height: 20px;
            text-decoration: none;
            background-color: #fff;
            border: 1px solid #ddd;
        }

        span.btn.btn-primary.disabled {
            float: left;
            padding: 4px 12px;
            line-height: 20px;
            text-decoration: none;
            background-color: #f5f5f5;
            border: 1px solid #ddd;
        }

        .pagination {
            display: inline-block;
            margin-bottom: 0;
            margin-left: 0;
            margin-top: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Billing History</h1>
                </div>                
                <div class="table-outer">
                    <table cellspacing="0" cellpadding="0" border="none" class="common-table billing-history-table">
                        <thead>
                            <tr>
                                <th style="display:none">Package Name</th>
                                <th>Amount</th>
                                <th>Transaction Time</th>
                                <th>Status</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:ListView ID="lstBilling" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td style="display:none"><%#Eval("PackageName") %></td>
                                        <td>$  <%#Eval("PurchasedAmount") %></td>
                                        <td><%# (DateTime.Parse(Eval("TransactionDate").ToString()).ToString("MMMM dd,yyyy hh:mm tt")) %></td>
                                        <td><%#Eval("Reason") %></td>
                                        <td class="edit-remove-btn">                                            
                                            <a class="main-btn" href='BillingDetails.aspx?bid=<%#Eval("Id") %>'>View Detail</a>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <tr>
                                        <td colspan="4" style="padding-left: 0;text-align: center;">No records found.</td>
                                    </tr>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </tbody>
                    </table>
                    <%--<asp:DataPager ID="dataPagerBilling" runat="server" PagedControlID="lstBilling"
                        OnPreRender="dataPagerBilling_PreRender">
                        <Fields>
                            <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                                ShowNextPageButton="false" />
                            <asp:NumericPagerField ButtonType="Link" />
                            <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false"
                                ShowPreviousPageButton="false" />
                        </Fields>
                    </asp:DataPager>--%>
                    <asp:DataPager ID="dataPagerBilling" runat="server" class="btn-group btn-group-sm pagination" PagedControlID="lstBilling" OnPreRender="dataPagerBilling_PreRender">
                        <Fields>
                            <asp:NextPreviousPagerField PreviousPageText="<" FirstPageText="|<" ShowPreviousPageButton="true"
                                ShowFirstPageButton="true" ShowNextPageButton="false" ShowLastPageButton="false"
                                ButtonCssClass="btn btn-default" RenderNonBreakingSpacesBetweenControls="false" RenderDisabledButtonsAsLabels="false" />
                            <asp:NumericPagerField ButtonType="Link" CurrentPageLabelCssClass="btn btn-primary disabled" RenderNonBreakingSpacesBetweenControls="false"
                                NumericButtonCssClass="btn btn-default" ButtonCount="10" NextPageText="..." NextPreviousButtonCssClass="btn btn-default" />
                            <asp:NextPreviousPagerField NextPageText=">" LastPageText=">|" ShowNextPageButton="true"
                                ShowLastPageButton="true" ShowPreviousPageButton="false" ShowFirstPageButton="false"
                                ButtonCssClass="btn btn-default" RenderNonBreakingSpacesBetweenControls="false" RenderDisabledButtonsAsLabels="false" />
                        </Fields>
                    </asp:DataPager>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
