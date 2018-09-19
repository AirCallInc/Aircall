<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="PartnerTicket_List.aspx.cs" Inherits="Aircall.admin.PartnerTicket_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Partner Ticket</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Partner Ticket Requests</a><span class="divider-last">&nbsp;</span></li>
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
                           Partner Ticket List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Partner Name</label>
                                <asp:TextBox ID="txtPartner" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Date Range</label>
                                <div class="input-append date left" data-date="02/13/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtStart" runat="server" class="input-small date-picker" size="16" type="text" />
                                    <%--<span class="add-on"><i class="icon-calendar"></i></span>--%>
                                </div>
                                <label>to</label>
                                <div class="input-append date left" data-date="02/13/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtEnd" runat="server" class="input-small date-picker" size="16" type="text" />
                                    <%--<span class="add-on"><i class="icon-calendar"></i></span>--%>
                                </div>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click"/>
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'PartnerTicket_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th>Sr #</th>
                                    <th>Partner Name</th>
                                    <th>Subject</th>
                                    <th>Type</th>
                                    <th>Added Date</th>
                                    <th>Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstPartnerTicket" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td><%# Container.DataItemIndex +1  %></td>
                                            <td><%#Eval("FirstName") %> <%#Eval("LastName") %></td>
                                            <td><%#Eval("Subject") %></td>
                                            <td><%#Eval("TicketType") %></td>
                                            <td><%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
                                            <td>
                                                <span class="label label-<%#Eval("Status").ToString().ToLower()=="true"?"active":"inactive"%>"><%#Eval("Status").ToString().ToLower()=="true"? "Open" :"Closed"%></span>
                                            </td>
                                            <td class=" ">
                                                <a href="<%=Application["SiteAddress"]%>admin/PartnerTicket_Edit.aspx?TicketId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>&nbsp;View</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="dataPagerPartnerTicket" runat="server" PagedControlID="lstPartnerTicket"
                            OnPreRender="dataPagerPartnerTicket_PreRender">
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
                "oSearch": { "bSmart": false, "bRegex": true },
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0]
                }],
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
