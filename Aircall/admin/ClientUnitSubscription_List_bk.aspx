<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ClientUnitSubscription_List_bk.aspx.cs" Inherits="Aircall.admin.ClientUnitSubscription_List_bk" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client Unit Subscriptions</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Client Unit Subscriptions</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>
                            Client Unit Subscription List
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

                                <label class="filter-label">Payment Due Date Range</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtStart" runat="server" class="input-small date-picker" size="16" type="text" />
                                    <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="FilterGroup" CssClass="error_required" ControlToValidate="txtStart" Enabled="false"></asp:RequiredFieldValidator>
                                </div>
                                <label>to</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtEnd" runat="server" class="input-small date-picker" size="16" type="text" />
                                    <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="FilterGroup" CssClass="error_required" ControlToValidate="txtEnd" Enabled="false"></asp:RequiredFieldValidator>
                                </div>
                                <label class="filter-label">Status</label>
                                <asp:DropDownList ID="drpStatus" runat="server" CssClass="input-small">
                                </asp:DropDownList>

                                <div style="clear: both;"></div>
                                <label class="filter-label">Payment Method</label>
                                <asp:DropDownList ID="drpPaymentMethod" ClientIDMode="Static" runat="server" CssClass="input-small">
                                </asp:DropDownList>
                                <%--<asp:RequiredFieldValidator ID="rqfvPaymentMethod" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="FilterGroup" CssClass="error_required" ControlToValidate="drpPaymentMethod" InitialValue="0"></asp:RequiredFieldValidator>--%>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" ValidationGroup="FilterGroup" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'ClientUnitSubscription_List.aspx'" />

                                <asp:LinkButton ID="lnkPaidUnpaid" runat="server" CssClass="btn btn-info add" Style="margin-left: 128px;" ValidationGroup="ChangeGroup" OnClick="lnkPaidUnpaid_Click">Mark Subscription as Paid
                                </asp:LinkButton>
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
                                        <th class="hidden-phone">Unit Name</th>
                                        <th class="hidden-phone">Due Date</th>
                                        <th class="tdPo">PO #</th>
                                        <th>Check #</th>
                                        <th class="hidden-phone">Status</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:ListView ID="lstUnitSubscription" runat="server">
                                        <ItemTemplate>
                                            <tr class='odd gradeX <%#Eval("ClsName") %>'>
                                                <td>
                                                    <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value='<%#Eval("Id") %>' />
                                                    <asp:HiddenField ID="hdnId" runat="server" Value='<%#Eval("Id") %>' />
                                                </td>
                                                <td>
                                                    <%#Container.DataItemIndex + 1 %>
                                                    <asp:HiddenField ID="ClientId" runat="server" Value='<%#Eval("ClientId") %>' />
                                                    <asp:HiddenField ID="DeviceToken" runat="server" Value='<%#Eval("DeviceToken") %>' />
                                                    <asp:HiddenField ID="DeviceType" runat="server" Value='<%#Eval("DeviceType") %>' />
                                                    <asp:HiddenField ID="PaymentDueDate" runat="server" Value='<%#Eval("PaymentDueDate","{0:MMMM yyyy}") %>' />
                                                </td>
                                                <td><%#Eval("ClientName") %></td>
                                                <td><%#Eval("Company") %></td>
                                                <td><%#Eval("UnitName") %><asp:HiddenField ID="hdnUnitName" runat="server" Value='<%#Eval("UnitName") %>' />
                                                </td>
                                                <%--<td><%#Eval("PaymentDueDate","{0:MM/dd/yyyy}") %></td>--%>
                                                 <td><%#Convert.ToDateTime(Eval("PaymentDueDate")).ToString("MM/dd/yyyy") %></td> <%--Code added on 13-07-2017--%>
                                                <td class="tdPo">
                                                    <asp:TextBox ID="txtPONumber" runat="server" CssClass="input-small" Text='<%#Eval("PONumber") %>'></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="regExpPONo" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPONumber" ValidationExpression="^[a-zA-Z0-9\-\/]*$"></asp:RegularExpressionValidator>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtCheckNumber" runat="server" CssClass="input-small" Text='<%#Eval("CheckNumber") %>'></asp:TextBox>
                                                    <asp:RegularExpressionValidator ID="regExpCheckNo" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCheckNumber" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                                </td>
                                                <td><%#Eval("Status") %></td>
                                                <td>
                                                    <a href="<%=Application["SiteAddress"]%>admin/ClientUnitSubscription_Edit.aspx?SubId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </tbody>
                            </table>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div id="dvCard" runat="server" visible="false">
                            <h4><i class="icon-credit-card"></i>
                                Payment Method: CC#
                            </h4>
                            <table class="table table-striped table-bordered" id="sample_13">
                                <thead>
                                    <tr>
                                        <th>Sr #</th>
                                        <th>Client Name</th>
                                        <th>Company</th>
                                        <th class="hidden-phone">Unit Name</th>
                                        <th class="hidden-phone">Due Date</th>
                                        <th>Card Number</th>
                                        <th class="hidden-phone">Status</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:ListView ID="lstUnitSubscriptionCard" runat="server">
                                        <ItemTemplate>
                                            <tr class='odd gradeX <%#Eval("ClsName") %>'>
                                                <td><%#Container.DataItemIndex + 1 %></td>
                                                <td><%#Eval("ClientName") %></td>
                                                <td><%#Eval("Company") %></td>
                                                <td><%#Eval("UnitName") %></td>
                                                <%--<td><%#Eval("PaymentDueDate","{0:MM/dd/yyyy}") %></td>--%>
                                                <td><%#Convert.ToDateTime(Eval("PaymentDueDate")).ToString("MM/dd/yyyy") %></td>
                                                <td><%#Eval("CardNumber") %></td>
                                                <td><%#Eval("Status") %></td>
                                                <td>
                                                    <a href="<%=Application["SiteAddress"]%>admin/ClientUnitSubscription_Edit.aspx?SubId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
            <div class="span12">
                <div class="widget">
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            var PageSize = '<%=ConfigurationManager.AppSettings["PageSize"]%>';
            //$('#sample_12').dataTable({
            //    "sDom": "<'row-fluid'<'span6'><'span6'f>r>t<'row-fluid'>",
            //    "aoColumnDefs": [{
            //        'bSortable': false,
            //        'aTargets': [0]
            //    }],
            //    "oSearch": { "bSmart": false, "bRegex": true },
            //    "iDisplayLength": PageSize
                
              
            //});
            $('#sample_12').dataTable({
                //  "sDom": "<'row-fluid'<'span6'><'span6'f>r>t<'row-fluid'>", Code commented on 11-07-2017
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
                         'bSortable': false,
                         'aTargets': [2]
                     },
                     {
                         'bSortable': false,
                         'aTargets': [3]
                     },
                    {
                    'bSortable': false,
                    'aTargets': [4]                   
                    },
                     {
                         'bSortable': false,
                         'aTargets': [6]
                     },
                     {
                         'bSortable': false,
                         'aTargets': [7]
                     },
                     {
                         'bSortable': false,
                         'aTargets': [8]
                     },
                     {
                         'bSortable': false,
                         'aTargets': [9]
                     }
                ],
                "oSearch": { "bSmart": false, "bRegex": true },
                //"iDisplayLength": PageSize,
                "fnDrawCallback": HidePO()
            });

            function HidePO() {
                if (jQuery('#drpPaymentMethod').val() == 'PO') {
                    jQuery('.tdPo').show();
                }
                else {
                    jQuery('.tdPo').hide();
                }
            }
            $('#sample_13').dataTable({
                //"sDom": "<'row-fluid'<'span6'><'span6'f>r>t<'row-fluid'>", Code commented on 11-07-2017
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
                        'bSortable': false,
                        'aTargets': [2]
                    },
                    {
                        'bSortable': false,
                        'aTargets': [3]
                    },
                    {
                        'bSortable': false,
                        'aTargets': [5]
                    },
                    {
                        'bSortable': false,
                        'aTargets': [6]
                    },
                    {
                        'bSortable': false,
                        'aTargets': [7]
                    },
                ],
                "oSearch": { "bSmart": false, "bRegex": true }
                //"iDisplayLength": PageSize
            });
            if (jQuery('#drpPaymentMethod').val() == 'PO') {
                jQuery('.tdPo').show();
            }
            else {
                jQuery('.tdPo').hide();
            }

            CheckAll();
        });
        function CheckAll() {
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

        function checkPaid(confirmationmessage, selectionmessage) {
            if (Page_ClientValidate("ChangeGroup")) {
                j = 0;
                var text = '';
                for (i = 0; i < document.getElementById('aspNetForm').length; i++) {
                    e = document.getElementById('aspNetForm').elements[i];
                    if (e.type == 'checkbox' && e.name != 'chkCheckAll' && e.checked) {
                        j++;
                    }
                }
                if (j > 0) {

                    if (text == '') {
                        rtn = confirm(confirmationmessage);
                    }
                    else {
                        rtn = confirm(text);
                    }
                    if (rtn == false) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
                else {
                    alert(selectionmessage);
                    return false;
                }
            }
        }
    </script>
</asp:Content>
