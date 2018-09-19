<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="NoShowService_List.aspx.cs" Inherits="Aircall.admin.NoShowService_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">No Show Services</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">No Show Services</a><span class="divider-last">&nbsp;</span></li>
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
                            No Show Services List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Service Case #</label>
                                <asp:TextBox ID="txtCaseNo" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Client</label>
                                <asp:TextBox ID="txtClient" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Employee</label>
                                <asp:TextBox ID="txtEmployee" runat="server" CssClass="input-medium"></asp:TextBox>
                                <div style="clear: both; margin-top: 45px;"></div>
                                <label class="filter-label">Work Area</label>
                                <asp:TextBox ID="txtWorkArea" runat="server" CssClass="input-medium"></asp:TextBox>
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
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'NoShowService_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger" OnClick="lnkDelete_Click">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>
                        </div>
                        <asp:UpdatePanel runat="server" ID="UPDService" ClientIDMode="Static">
                            <ContentTemplate>
                                <script type="text/javascript">
                                    function jScriptmsg() {
                                        if (!jQuery().uniform) {
                                            return;
                                        }
                                        if (test = $("#UPDService input[type=checkbox]:not(.toggle)")) {
                                            test.uniform();
                                        }
                                    }
                                    Sys.Application.add_load(jScriptmsg);
                                </script>
                                <asp:ListView ID="lstNoShowServices" runat="server" OnSorting="lstNoShowServices_Sorting" OnItemDataBound="lstNoShowServices_ItemDataBound">
                                    <LayoutTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">
                                                    <th></th>
                                                    <%--<th runat="server" class="sorting" id="th1" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ServiceCase" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ServiceCase" OnClick="ServiceCase_Click">Service Case #</asp:LinkButton></th>--%>
                                                    <th>Service Case #</th>
                                                    <th runat="server" class="sorting" id="th2" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ClientName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ClientName" OnClick="ServiceCase_Click">Client Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th3" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ScheduledDate" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ScheduledDate" OnClick="ServiceCase_Click">Scheduled Date</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th4" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ScheduleTime" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ScheduleTime" OnClick="ServiceCase_Click">Schedule Time</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th5" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="RequestFor" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="RequestFor" OnClick="ServiceCase_Click">Request For</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th6" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="Technician" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Technician" OnClick="ServiceCase_Click">Technician</asp:LinkButton></th>
                                                    <th>Status</th>
                                                    <th runat="server" class="sorting" id="th7" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="IsNotify" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="IsNotify" OnClick="ServiceCase_Click">Is Notify Client</asp:LinkButton></th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr class="odd gradeX <%#Eval("IsNotify").ToString() == "0"?"waiting-approval":"" %>">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnServiceId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%#Eval("ServiceCaseNumber") %></td>
                                            <td><%#Eval("FirstName") %> <%#Eval("LastName") %></td>
                                            <td><%#Eval("ScheduleDate","{0:MM/dd/yyyy}") %></td>
                                            <td><%#Eval("ScheduleStartTime") %> - <%#Eval("ScheduleEndTime") %></td>
                                            <td><%#Eval("PurposeOfVisit") %></td>
                                            <td><%#Eval("EmployeeName") %></td>
                                            <td><%#Convert.ToInt32(Eval("ReportCount").ToString())>0? Eval("Status"): Eval("Status").ToString().ToLower() == "no show"?"Late Reschedule":Eval("Status") %></td>
                                            <td>
                                                <%#Eval("IsNotify").ToString()=="0"?"No":"Yes" %>
                                            </td>
                                            <td>
                                                <a href='#' runat="server" id="lnkEdit" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th class="srno">Service Order No</th>
                                                    <th>Client Name</th>
                                                    <th>Schedule Date</th>
                                                    <th>Schedule Time</th>
                                                    <th>Request For</th>
                                                    <th>Technician</th>
                                                    <th>Status</th>
                                                    <th>Is Notify Client</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="odd gradeX">
                                                    <td colspan="9">No Data Found </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </EmptyDataTemplate>
                                </asp:ListView>

                                <asp:DataPager ID="dataPagerNoShowService" runat="server" PagedControlID="lstNoShowServices"
                                    OnPreRender="dataPagerNoShowService_PreRender">
                                    <Fields>
                                        <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                                            ShowNextPageButton="false" />
                                        <asp:NumericPagerField ButtonType="Link" />
                                        <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false"
                                            ShowPreviousPageButton="false" />
                                    </Fields>
                                </asp:DataPager>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            <%--var PageSize = '<%=ConfigurationManager.AppSettings["PageSize"]%>';
            $('#sample_12').dataTable({
                "sDom": "<'row-fluid'<'span12'f>r>t<'row-fluid'>",
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0]
                }],
                "oSearch": { "bSmart": false, "bRegex": true },
                "iDisplayLength": PageSize
            });

            jQuery('#sample_12_wrapper .dataTables_filter input').addClass("input-medium"); // modify table search input--%>
        })
    </script>
</asp:Content>
