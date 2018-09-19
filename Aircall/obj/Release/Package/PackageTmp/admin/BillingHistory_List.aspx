<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="BillingHistory_List.aspx.cs" Inherits="Aircall.admin.BillingHistory_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function checkActive1(confirmationmessage) {
            rtn = confirm(confirmationmessage);
            if (rtn == false) {
                return false;
            }
            else {
                return true;
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Billing History</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Billing History</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-money"></i>
                            Billing History List
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
                                <label class="filter-label">Date Range</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtStart" runat="server" class="input-small date-picker" size="16" type="text" />
                                    <%--<span class="add-on"><i class="icon-calendar"></i></span>--%>
                                </div>
                                <label>to</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtEnd" runat="server" class="input-small date-picker" size="16" type="text" />
                                    <%--<span class="add-on"><i class="icon-calendar"></i></span>--%>
                                </div>
                                <label>Status</label>
                                <asp:DropDownList runat="server" ID="drpStatus" CssClass="input-medium">
                                    <asp:ListItem Text="All" Value="All"></asp:ListItem>
                                    <asp:ListItem Text="Success" Value="Success"></asp:ListItem>
                                    <asp:ListItem Text="Failed" Value="Failed"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'BillingHistory_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th>Sr. No.</th>
                                    <th>Client Name</th>
                                    <th>Billing Name</th>
                                    <th>Company</th>
                                    <th>Amount</th>
                                    <th>Transaction Id</th>
                                    <th>Transaction time</th>
                                    <th>IsPaid</th>
                                    <th>Message</th>
                                    <th>Charge By</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstBilling" runat="server" OnItemDataBound="lstBilling_ItemDataBound" OnItemCommand="lstBilling_ItemCommand">
                                    <ItemTemplate>
                                        <tr class="odd gradeX" runat="server" id="trTd">
                                            <td><%# Container.DataItemIndex + 1 %>
                                                <asp:HiddenField ID="hdnId" Value='<%#Eval("Id") %>' runat="server" />
                                            </td>
                                            <td><%#Eval("ClientName") %></td>
                                            <td><%#Eval("BillingFirstName") %> <%#Eval("BillingLastName") %></td>
                                            <td><%#Eval("Company") %></td>
                                            <td>$ <%#Eval("PurchasedAmount") %></td>
                                            <td><%#Eval("TransactionId") %></td>
                                            <td><%#Convert.ToDateTime(Eval("TransactionDate")).ToString() %></td>
                                            <td><%#(Eval("IsPaid").ToString()=="True"?"Yes":"No") %></td>
                                            <td><%#Eval("faildesc") %></td>
                                            <td><%#Eval("BillingType") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/BillingHistory_View.aspx?bid=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>&nbsp;View</a>
                                                <asp:Button ID="btnPay" runat="server" Text="Pay" CommandName="PayInvoice" CommandArgument='<%#Eval("Id")%>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <tr>
                                            <td colspan="8">No Data Found</td>
                                        </tr>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="dataPagerBilling" runat="server" PagedControlID="lstBilling"
                            OnPreRender="dataPagerBilling_PreRender">
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
