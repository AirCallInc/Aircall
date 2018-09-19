<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="CompletedService_List.aspx.cs" Inherits="Aircall.admin.CompletedService_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/jstarbox.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Completed Services</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Completed Services</a><span class="divider-last">&nbsp;</span></li>
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
                            Completed Services List
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
                                <asp:TextBox ID="txtemployee" runat="server" CssClass="input-medium"></asp:TextBox>
                                <div style="clear:both;margin-top: 45px;"></div>
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
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'CompletedService_List.aspx'" />
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
                                <asp:ListView ID="lstCompleted" runat="server" OnItemDataBound="lstCompleted_ItemDataBound" OnSorting="lstCompleted_Sorting">
                                    <LayoutTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">
                                                    <th>Service Case #</th>
                                                    <th runat="server" class="sorting" id="th2" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ClientName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ClientName" OnClick="SortByServiceCase_Click">Client Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th6" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="Technician" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Technician" OnClick="SortByServiceCase_Click">Technician</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th4" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ServicedOn" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ServicedOn" OnClick="SortByServiceCase_Click">Serviced On</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th1" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="Ratings" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Ratings" OnClick="SortByServiceCase_Click">Ratings</asp:LinkButton></th>
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
                                            <td><%#Eval("ServiceCaseNumber") %></td>
                                            <td><%#Eval("ClientName") %></td>
                                            <td><%#Eval("EmployeeName") %></td>
                                            <td><%#Eval("ScheduleDate","{0:MM/dd/yyyy}") %></td>
                                            <%--<td><div><span class="stars-container stars-<%#Eval("Ratings") %>">★★★★★</span></div></td>--%>
                                            <td>
                                                <div class="starbox" data-rate="" id="dvRating" runat="server"></div>
                                            </td>
                                            <td class=" ">
                                                <%--<a href="<%=Application["SiteAddress"]%>admin/Past_Service_View.aspx?Id=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>&nbsp;View</a>--%>
                                                <a href="<%=Application["SiteAddress"]%>admin/CompletedService_View.aspx?ServiceId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>&nbsp;View</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th>Service Order No</th>
                                                    <th>Client Name</th>
                                                    <th>Technician</th>
                                                    <th>Serviced On</th>
                                                    <th>Ratings</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="odd gradeX">
                                                    <td colspan="6">No Data Found </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                                <asp:DataPager ID="dataPagerCompleted" runat="server" PagedControlID="lstCompleted"
                                    OnPreRender="dataPagerCompleted_PreRender">
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
                    buttons: 5, //false will allow any value between 0 and 1 to be set
                    ghosting: true,
                    changeable: false, // true, false, or "once"
                    autoUpdateAverage: true
                });
            });

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
        });
    </script>
</asp:Content>
