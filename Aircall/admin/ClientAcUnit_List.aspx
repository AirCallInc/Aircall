<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ClientAcUnit_List.aspx.cs" Inherits="Aircall.admin.ClientAcUnit_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client's AC Unit List</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Client's AC Unit List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-list"></i>
                            Client's AC Unit List
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
                                <label class="filter-label">Unit Status</label>
                                <asp:DropDownList ID="drpStatus" runat="server" CssClass="input-medium">
                                    <asp:ListItem Value="0">Select Status</asp:ListItem>
                                    <asp:ListItem Value="3">Need Repair</asp:ListItem>
                                    <asp:ListItem Value="2">Service Soon</asp:ListItem>
                                    <asp:ListItem Value="1">Serviced</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'ClientAcUnit_List.aspx'" />
                                
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons">
                            <%--<asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>--%>
                            <asp:Button ID="btnAddSubscription" runat="server" CssClass="btn btn-info add" Text="Add Subscription" OnClick="btnAddSubscription_Click" style="text-align:right"/>
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/ClientAcUnit_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add AC Unit
                            </a>
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
                                <asp:ListView ID="lstUnits" runat="server" OnSorting="lstUnits_Sorting">
                                    <LayoutTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">

                                                    <th>Sr #</th>
                                                    <th runat="server" class="sorting" id="th1" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ClientName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ClientName" OnClick="SortByServiceCase_Click">Client Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th2" style="padding: 0;display:none">
                                                        <asp:LinkButton runat="server" ID="PlanType" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="PlanType" OnClick="SortByServiceCase_Click">Plan Type</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th3" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="UnitName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="UnitName" OnClick="SortByServiceCase_Click">Unit Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th4" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="IsSpecialApplied" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="IsSpecialApplied" OnClick="SortByServiceCase_Click">Special</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th5" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="Status" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Status" OnClick="SortByServiceCase_Click">Status</asp:LinkButton></th>
                                                    <th class="hidden-phone">Address</th>
                                                    <th>AuthorizeNet Subscription Id</th>
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
                                            <td><%#Eval("ClientName") %></td>
                                            <td style="display:none"><%#Eval("PlanTypeName") %></td>
                                            <td><%#Eval("UnitName") %></td>
                                            <td>
                                                <%#Eval("IsSpecialApplied").ToString()=="True"?"Yes":"No"%>
                                            </td>
                                            <td>
                                                <span class="label label-<%#Enum.GetName(typeof(Aircall.Common.General.UnitStatus),Eval("Status")) %>">
                                                    <%--<%#Enum.GetName(typeof(Aircall.Common.General.UnitStatus),Eval("Status")) %>--%>
                                                    <%# Aircall.Common.DurationExtensions.GetUnitStatus((int)Eval("Status")) %>
                                                </span>
                                            </td>
                                            <td><%#Eval("Address") %></td>
                                            <td><%#Eval("AuthorizeNetSubscriptionId") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/ClientAcUnit_AddEdit.aspx?CUnitId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th></th>
                                                    <th class="srno">Service Order No</th>
                                                    <th>Client Name</th>
                                                    <th>Schedule Date</th>
                                                    <th>Schedule Time</th>
                                                    <th>Request For</th>
                                                    <th>Technician</th>
                                                    <th>Special Offer Added</th>
                                                    <th>Status</th>
                                                    <th>Action
                                                    </th>
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
                                <asp:DataPager ID="dataPagerClientUnit" runat="server" PagedControlID="lstUnits"
                                    OnPreRender="dataPagerClientUnit_PreRender">
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
