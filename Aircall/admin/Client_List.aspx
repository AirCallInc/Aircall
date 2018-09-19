﻿<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Client_List.aspx.cs" Inherits="Aircall.admin.Client_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Clients</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Client List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-group"></i>
                            Clients
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
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'Client_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkActive" runat="server" CssClass="btn btn-success hidden-phone" OnClick="lnkActive_Click">
                                <i class="icon-ok icon-white"></i>Active
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkInactive" runat="server" CssClass="btn btninactive hidden-phone" OnClick="lnkInactive_Click">
                                <i class="icon-off icon-white"></i>Inactive
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger" OnClick="lnkDelete_Click">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/Client_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Client
                            </a>
                        </div>
                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>
                                <script type="text/javascript">
                                    function jScriptmsg() {
                                        if (!jQuery().uniform) {
                                            return;
                                        }
                                        if (test = $("#sample_12 input[type=checkbox]:not(.toggle)")) {
                                            test.uniform();
                                        }
                                        checkall();
                                    }
                                    Sys.Application.add_load(jScriptmsg);
                                </script>
			<asp:ListView ID="lstClients" runat="server" OnSorting="lstClients_Sorting">
			<LayoutTemplate>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                               <tr runat="server" id="tr">
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                    </th>
                                    <th>Sr #</th>
                                    <th runat="server" class="sorting" id="th6" style="padding: 0;">
                                        <asp:LinkButton runat="server" ID="ClientName" CommandName="Sort" Style="display: block; padding: 8px;"
                                        CommandArgument="ClientName" OnClick="SortByServiceCase_Click">Client Name</asp:LinkButton></th>                                    
                                   <th runat="server" class="sorting" id="th8" style="padding: 0;">
                                        <asp:LinkButton runat="server" ID="Email" CommandName="Sort" Style="display: block; padding: 8px;"
                                        CommandArgument="Email" OnClick="SortByServiceCase_Click">Email</asp:LinkButton></th>
                                    <th class="hidden-phone">Mobile Number</th>
                                    <th runat="server" class="sorting" id="th7" style="padding: 0;">
                                        <asp:LinkButton runat="server" ID="AddedDate" CommandName="Sort" Style="display: block; padding: 8px;"
                                            CommandArgument="AddedDate" OnClick="SortByServiceCase_Click">Added Date</asp:LinkButton></th>                                      
                                   <th runat="server" class="sorting hidden-phone" id="th1" style="padding: 0;">
                                        <asp:LinkButton runat="server" ID="IsActive" CommandName="Sort" Style="display: block; padding: 8px;"
                                            CommandArgument="IsActive" OnClick="SortByServiceCase_Click">Status</asp:LinkButton></th>  
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
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnClientId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("FirstName") %>  <%#Eval("LastName") %></td>
                                            <td><%#Eval("Email") %></td>
                                            <td><%#Eval("MobileNumber") %></td>
                                            <td>
                                                <%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %>

                                            </td>
                                            <td>
                                                <span class="label label-<%#Eval("IsActive").ToString().ToLower()=="true"?"active":"inactive"%>"><%#Eval("IsActive").ToString().ToLower()=="true"? "Active" :"Inactive"%></span>
                                            </td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/Client_AddEdit.aspx?ClientId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                                <a href="<%=Application["SiteAddress"]%>admin/ClientAcUnit_List.aspx?ClientId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>Units</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th style="width: 8px;">
                                                        <input type="checkbox" class="group-checkable" data-set="#sample_1 .checkboxes" />
                                                    </th>
                                                    <th>Sr #</th>
                                                    <th>Employee Name</th>
                                                    <th class="hidden-phone">Email</th>
                                                    <th class="hidden-phone">Phone Number</th>
                                                    <th>Added Date</th>
                                                    <th>Is Sales Employee</th>
                                                    <th class="hidden-phone">Status</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="9">No Data Found</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                        <asp:DataPager ID="dataPagerClients" runat="server" PagedControlID="lstClients"
                            OnPreRender="dataPagerClients_PreRender">
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
            checkall();
        });
        function checkall() {
            jQuery('#sample_12 .group-checkable').on("change", function () {
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
