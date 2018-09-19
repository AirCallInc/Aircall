<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ClientUnitSubscription_List_UnSubmitted.aspx.cs" Inherits="Aircall.admin.ClientUnitSubscription_List_UnSubmitted" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client Unit UnSubmitted Subscriptions</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Client Unit UnSubmitted Subscriptions</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMessage" runat="server" visible="false">
                            <div class="clear">
                            </div>
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>
                            Client Unit UnSubmitted Subscription List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Client</label>
                                <asp:TextBox ID="txtClient" runat="server" CssClass="input-medium" Width="400px"></asp:TextBox>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" ValidationGroup="FilterGroup" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'ClientUnitSubscription_List_UnSubmitted.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div id="dvCheck" runat="server">

                            <table class="table table-striped table-bordered" id="sample_12">
                                <thead>
                                    <tr>
                                        <th style="width: 8px;">
                                            <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                        </th>
                                        <th>Sr #</th>
                                        <th>Client Name</th>
                                        <th>Company</th>
                                        <th>Total Units</th>
                                        <th>Price Per Month</th>
                                        <th>Total Amount</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:ListView ID="lstUnitSubscription" runat="server">
                                        <ItemTemplate>
                                            <tr class='odd gradeX'>
                                                <td>
                                                    <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value='<%#Eval("ClientId") %>' />
                                                    <asp:HiddenField ID="hdnId" runat="server" Value='<%#Eval("ClientId") %>' />
                                                </td>
                                                <td>
                                                    <%#Container.DataItemIndex + 1 %>
                                                    <asp:HiddenField ID="ClientId" runat="server" Value='<%#Eval("ClientId") %>' />
                                                </td>
                                                <td><%#Eval("ClientName") %></td>
                                                <td><%#Eval("Company") %></td>
                                                <td><%#Eval("TotalUnitCount") %></td>
                                                <td><%#Eval("PricePerMonth") %></td>
                                                <td><%#Eval("TotalAmount") %></td>
                                                <td>
                                                    <a href="<%=Application["SiteAddress"]%>admin/ClientUnitSubscription_Edit_ByClient.aspx?ClientId=<%#Eval("ClientId") %>" class="btn mini purple"><i class="icon-edit"></i>Pay</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </tbody>
                            </table>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                    </div>
                </div>
            </div>
            <div class="span12">
                <div class="widget">
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            var PageSize = '<%=ConfigurationManager.AppSettings["PageSize"]%>';
            $('#sample_12').dataTable({
                "aoColumnDefs": [
                    {
                        'bSortable': false,
                        'aTargets': [0]                   
                    },
                    {
                        'bSortable': false,
                         'aTargets': [1]
                     },
                    {
                        'bSortable': true,
                         'aTargets': [2]
                    },
                    {
                        'bSortable': true,
                         'aTargets': [3]
                    },
                    {
                        'bSortable': true,
                         'aTargets': [4]
                    },
                    {
                        'bSortable': true,
                         'aTargets': [5]
                    },
                    {
                        'bSortable': false,
                         'aTargets': [6]
                     },
                ],
                "oSearch": { "bSmart": false, "bRegex": true },
                "fnDrawCallback": fnDrawCallback_12()
            });

            function fnDrawCallback_12() {
                
            }
            assignCheckAllEvent();
        });
        function assignCheckAllEvent() {
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
        }
    </script>
</asp:Content>
