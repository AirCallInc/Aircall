<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="RenewCancel_List_bk.aspx.cs" Inherits="Aircall.admin.RenewCancel_List_bk" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Renew / Cancel Plan</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Subscription List</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMessage" runat="server" visible="false">
                            <div class="clear">
                                <!-- -->
                            </div>
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-map-marker"></i>
                            Unit Subscriptions
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Client</label>
                                <asp:TextBox ID="txtClient" runat="server" CssClass="input-medium"></asp:TextBox>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'RenewCancel_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th>Sr #</th>
                                    <th>Client Name</th>
                                    <th>Unit Name</th>
                                    <th class="hidden-phone">Address</th>
                                    <th>Remaining Subscriptions</th>
                                    <th>AutoRenewals</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstSubscriptions" runat="server" OnItemDataBound="lstSubscriptions_ItemDataBound">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("ClientName") %></td>
                                            <td><%#Eval("UnitName") %></td>
                                            <td><%#Eval("Address") %></td>
                                            <td>
                                                <asp:Literal ID="ltrSubscription" runat="server"></asp:Literal></td>
                                            <td><%#Eval("AutoRenewal").ToString().ToLower()=="true"?"Yes":"No" %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/RenewCancel_UnitSubscription.aspx?UnitId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Renew/Cancel</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <tr>
                                            <td colspan="7">No Data Found</td>
                                        </tr>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="dataPagerUnitSubscription" runat="server" PagedControlID="lstSubscriptions"
                            OnPreRender="dataPagerUnitSubscription_PreRender">
                            <Fields>
                                <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                                    ShowNextPageButton="false" />
                                <asp:NumericPagerField ButtonType="Link" />
                                <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false"
                                    ShowPreviousPageButton="false" />
                            </Fields>
                        </asp:DataPager>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
