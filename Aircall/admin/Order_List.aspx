<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Order_List.aspx.cs" Inherits="Aircall.admin.Order_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Orders</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Order List</a><span class="divider-last">&nbsp;</span></li>
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
                            Orders
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
                                <label class="filter-label">Employee</label>
                                <asp:TextBox ID="txtEmployee" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Date Range</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtStart" runat="server" class="input-small date-picker" size="16" type="text" />
                                </div>
                                <label>to</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtEnd" runat="server" class="input-small date-picker" size="16" type="text" />
                                </div>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click"/>
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'Order_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>

                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger" OnClick="lnkDelete_Click">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/Order_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Order
                            </a>
                        </div>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                    </th>
                                    <th>Sr #</th>
                                    <th>Order #</th>
                                    <th>Client Name</th>
                                    <th class="hidden-phone">Subtotal Amount</th>
                                    <th>Charge By</th>
                                    <th>Technician</th>
                                    <th>Added Date</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstOrders" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdOrderId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("OrderNumber") %></td>
                                            <td><%#Eval("ClientName") %></td>
                                            <td>$ <%#Eval("OrderAmount") %></td>
                                            <td><%#Eval("ChargeBy") %></td>
                                            <td><%#Eval("Employee") %></td>
                                            <td><%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/Order_View.aspx?OrderId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>View</a>
                                                <%--<a href="<%=Application["SiteAddress"]%>admin/Order_AddEdit.aspx?OrderId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>--%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="dataPagerOrder" runat="server" PagedControlID="lstOrders"
                            OnPreRender="dataPagerOrder_PreRender">
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
    <script>
        $(document).ready(function () {
            var PageSize = '<%=ConfigurationManager.AppSettings["PageSize"]%>';
            $('#sample_12').dataTable({
                "sDom": "<'row-fluid'<'span6'><'span6'f>r>t<'row-fluid'>",
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0]
                }],
                "oSearch": { "bSmart": false, "bRegex": true },
                "iDisplayLength": PageSize
            });

            jQuery('#sample_12 .group-checkable').change(function () {
                var set = jQuery(this).attr("data-set");
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    if (checked) {
                        $(this).attr("checked", true);
                    } else {
                        $(this).attr("checked", false);
                    }
                });
                jQuery.uniform.update(set);
            });

            jQuery('#sample_12_wrapper .dataTables_filter input').addClass("input-medium"); // modify table search input
        })
    </script>
</asp:Content>
