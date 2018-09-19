<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeRatingReview_List.aspx.cs" Inherits="Aircall.admin.EmployeeRatingReview_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/jstarbox.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Rating & Review List</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Rating & Review List</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-group"></i>
                            Rating & Review List
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
                                <asp:TextBox ID="txtemployee" runat="server" CssClass="input-medium"></asp:TextBox>
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
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'EmployeeRatingReview_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>
                                <script type="text/javascript">
                                    function jScriptmsg() {
                                        $('.starbox').each(function () {
                                            var starbox = jQuery(this);
                                            starbox.starbox({
                                                average: parseFloat(starbox.attr("data-rate")),
                                                stars: 5,
                                                buttons: 5, //false will allow any value between 0 and 1 to be set
                                                ghosting: true,
                                                changeable: false, // true, false, or "once"
                                                autoUpdateAverage: true
                                            });
                                        });
                                    }
                                    Sys.Application.add_load(jScriptmsg);
                                </script>
                                <asp:ListView ID="lstEmpRatings" runat="server" OnItemDataBound="lstEmpRatings_ItemDataBound" OnSorting="lstEmpRatings_Sorting">
                                    <LayoutTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">
                                                    <th>Sr #</th>
                                                    <th>Service Case #</th>
                                                    <th runat="server" class="sorting" id="th2" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ClientName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ClientName" OnClick="SortByServiceCase_Click">Client Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th6" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="Technician" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Technician" OnClick="SortByServiceCase_Click">Employee Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th1" style="padding: 0;width:14%;">
                                                        <asp:LinkButton runat="server" ID="Ratings" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Ratings" OnClick="SortByServiceCase_Click">Service Rating</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th3" style="padding: 0;width:14%;">
                                                        <asp:LinkButton runat="server" ID="ServicedOn" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ServicedOn" OnClick="SortByServiceCase_Click">Service Date</asp:LinkButton></th>
                                                    <th>Rate Added Date</th>
                                                    <th runat="server" class="sorting" id="th4" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="EmployeeAverageRating" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="EmployeeAverageRating" OnClick="SortByServiceCase_Click">Employee Average Rating</asp:LinkButton></th>
                                                    <th class="hidden-phone">Note Added Date</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("ServiceCaseNumber") %></td>
                                            <td><%#Eval("ClientName") %></td>
                                            <td><%#Eval("EmployeeName") %></td>
                                            <td>
                                                <div class="starbox" data-rate="" id="dvRating" runat="server"></div>
                                            </td>
                                            <td><%#DateTime.Parse(Eval("ScheduleDate").ToString()).ToString("MM/dd/yyyy") %></td>
                                            <td><%#Eval("RatingDate").ToString()=="1/1/1900 12:00:00 AM"?"-":Eval("RatingDate") %></td>
                                            <td>
                                                <div class="starbox" data-rate="" id="dvEmpRating" runat="server"></div>
                                            </td>
                                            <td><%#Eval("NotesAddedDate").ToString()=="1/1/1900 12:00:00 AM"?"-":Eval("NotesAddedDate") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/EmployeeRatingReview_View.aspx?ServiceId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>View</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th>Service Order No</th>
                                                    <th>Client Name</th>
                                                    <th>Employee Name</th>
                                                    <th>Service Rating</th>
                                                    <th>Rate Added Date</th>
                                                    <th>Note Added Date</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="odd gradeX">
                                                    <td colspan="7">No Data Found </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </EmptyDataTemplate>
                                </asp:ListView>

                                <asp:DataPager ID="dataPagerEmpRatings" runat="server" PagedControlID="lstEmpRatings"
                                    OnPreRender="dataPagerEmpRatings_PreRender">
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
    <script type="text/javascript">
        $(document).ready(function () {
            /* star-rating script */
            $('.starbox').each(function () {
                var starbox = jQuery(this);
                starbox.starbox({
                    average: parseFloat(starbox.attr("data-rate")),
                    stars: 5,
                    buttons: 10, //false will allow any value between 0 and 1 to be set
                    ghosting: true,
                    changeable: false, // true, false, or "once"
                    autoUpdateAverage: true
                });
            });

            <%--var PageSize = '<%=ConfigurationManager.AppSettings["PageSize"]%>';
            $('#sample_12').dataTable({
                "sDom": "<'row-fluid'<'span6'><'span6'f>r>t<'row-fluid'>",
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0]
                }],
                "oSearch": { "bSmart": false, "bRegex": true },
                "iDisplayLength": PageSize
            });--%>

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

            //jQuery('#sample_12_wrapper .dataTables_filter input').addClass("input-medium"); // modify table search input
        });
    </script>
</asp:Content>
