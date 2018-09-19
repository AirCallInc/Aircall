<%@ Page Title="Notification" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="notification.aspx.cs" Inherits="Aircall.client.notification" %>

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
                    <h1>Notifications</h1>

                </div>
                <div class="table-outer">
                    <table cellspacing="0" cellpadding="0" border="none" class="common-table notifications-table">
                        <asp:ListView ID="lstNotification" runat="server" OnItemDataBound="lstNotification_ItemDataBound" OnItemCommand="lstNotification_ItemCommand">
                            <LayoutTemplate>
                                <tbody>
                                    <tr runat="server" id="itemPlaceholder" />
                                </tbody>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr runat="server">
                                    <td class="timing"><strong>
                                        <asp:Literal ID="ltDisplayDate" runat="server"></asp:Literal></strong></td>
                                    <td style="width: 5%;">
                                        <img class="notiicon" src="images/blue-notification.png" alt="" title=""></td>
                                    <td>
                                        <asp:HiddenField ID="hfCommonId" Value='<%#Eval("CommonId") %>' runat="server" />
                                        <asp:LinkButton ID="lnkMsg" runat="server"></asp:LinkButton>
                                        <asp:Literal ID="ltMessage" runat="server"></asp:Literal></td>
                                    <td>
                                        <asp:Image ID="imgPersonSRC" runat="server" CssClass="empPhoto" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="4" style="padding-left: 0; text-align: center;">No notification found.
                                    </td>
                                </tr>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </table>
                    <asp:DataPager ID="it" runat="server" class="btn-group btn-group-sm pagination" PagedControlID="lstNotification" OnPreRender="dataPagerRequest_PreRender">
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
                <a class="main-btn" href="dashboard.aspx">Go to Dashboard</a>
            </div>
        </div>
    </div>
</asp:Content>
