<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HistoryService_List.aspx.cs" Inherits="Aircall.admin.HistoryService_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Request Service History</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Request Service History List</a><span class="divider-last">&nbsp;</span></li>
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
                            Request Service History List
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
                                <label class="filter-label">Client Name</label>
                                <asp:TextBox ID="txtClient" runat="server" CssClass="input-medium"></asp:TextBox>
                                <div style="clear: both; margin-top: 45px;"></div>
                                <label class="filter-label">Date Range</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtStart" runat="server" class="input-small date-picker" size="16" type="text" />
                                </div>
                                <label>to</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtEnd" runat="server" class="input-small date-picker" size="16" type="text" />
                                </div>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'HistoryService_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>
                                <asp:ListView ID="lstHistoryService" runat="server" OnSorting="lstHistoryService_Sorting">
                                    <LayoutTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">
                                                    <th>Service Case #</th>
                                                    <th runat="server" class="sorting" id="th2" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ClientName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ClientName" OnClick="SortByServiceCase_Click">Client Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th3" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ScheduledDate" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ScheduledDate" OnClick="SortByServiceCase_Click">Requested On</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th5" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="RequestFor" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="RequestFor" OnClick="SortByServiceCase_Click">Request For</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th6" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="Technician" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Technician" OnClick="SortByServiceCase_Click">Technician</asp:LinkButton></th>
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
                                            <td><%#Eval("ServiceCaseNumber")%></td>
                                            <td><%#Eval("ClientName") %></td>
                                            <td><%#Eval("ScheduleDate","{0:MM/dd/yyyy}") %></td>
                                            <td><%#Eval("PurposeOfVisit") %></td>
                                            <td><%#Eval("EmployeeName") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/<%#Eval("RedirectedPage") %>.aspx?ServiceId=<%#Eval("Id") %>"
                                                    class="btn mini purple"><i class="icon-eye-open"></i>&nbsp;View</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th>Service Order No</th>
                                                    <th>Client Name</th>                                                    
                                                    <th>Requested On</th>
                                                    <th>Request For</th>
                                                    <th>Technician</th>
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
                                <asp:DataPager ID="dataPagerHistory" runat="server" PagedControlID="lstHistoryService"
                                    OnPreRender="dataPagerHistory_PreRender">
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
</asp:Content>
