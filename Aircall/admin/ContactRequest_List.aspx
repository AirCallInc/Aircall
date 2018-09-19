<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ContactRequest_List.aspx.cs" Inherits="Aircall.admin.ContactRequest_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Contact Request</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Contact Request List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-envelope"></i>
                            Contact Requests
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Name</label>
                                <asp:TextBox ID="txtName" runat="server" CssClass="input-medium"></asp:TextBox>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'ContactRequest_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger" OnClick="lnkDelete_Click">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>
                        </div>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                    </th>
                                    <th>Name</th>
                                    <th class="hidden-phone">Email</th>
                                    <th class="hidden-phone">RequestDate</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstContactRequest" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX <%#Eval("Status").ToString().ToLower()=="true"?"":"background-green"%>">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%#Eval("Name") %></td>
                                            <td><%#Eval("Email") %></td>
                                            <td><%#DateTime.Parse(Eval("RequestDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss tt") %></td>
                                            <td>
                                                <%--<a href="<%=Application["SiteAddress"]%>admin/ContactRequest_SendEmail.aspx?RequestId=<%#Eval("Id") %>" class="btn btn-primary" visible='<%# Eval("Status").ToString().ToLower()=="true"?true:false %>'><i class="icon-envelope-alt"></i>Send Email</a>--%>
                                                <a href="<%=Application["SiteAddress"]%>admin/ContactRequest_SendEmail.aspx?RequestId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>View</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="dataPagerContactReq" runat="server" PagedControlID="lstContactRequest"
                            OnPreRender="dataPagerContactReq_PreRender">
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
