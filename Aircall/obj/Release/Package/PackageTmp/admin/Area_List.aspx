﻿<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Area_List.aspx.cs" Inherits="Aircall.admin.Area_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .zip{width:400px !important;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Areas</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Area List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-map-marker"></i>
                            Areas
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <asp:UpdatePanel runat="server" class="heading searchschedule" UpdateMode="Conditional" ID="upd1">
                                <ContentTemplate>
                                    <div class="mtop10">
                                        <div style="float: left; width: 25%;">
                                            <label>State</label>
                                            <asp:DropDownList ID="drpState" runat="server" CssClass="span9" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div style="float: left; width: 25%;">
                                            <label>City</label>
                                            <asp:DropDownList ID="drpCity" runat="server" CssClass="span9"></asp:DropDownList>
                                        </div>
                                        <div style="float: left; width: 30%;">
                                            <label class="filter-label">Area Name</label>
                                            <asp:TextBox ID="txtAreaName" runat="server" CssClass="input-medium"></asp:TextBox>
                                        </div>
                                        <div class="clear"></div>
                                    </div>
                                    <div class="clear"></div>
                                    <div class="mtop10">
                                        <div style="float: left; width: 25%;">
                                            <label class="filter-label">Zip Code</label>
                                            <asp:TextBox ID="txtZip" runat="server" CssClass="input-medium"></asp:TextBox>
                                        </div>
                                        <div style="float: left; width: 25%;">
                                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                            <input type="button" class="btn" value="Clear" onclick="location.href = 'Area_List.aspx'" />
                                        </div>
                                        <div class="clear"></div>
                                    </div>
                                    <div class="clear"></div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="drpState" EventName="SelectedIndexChanged" />
                                    <asp:PostBackTrigger ControlID="btnSearch" />
                                </Triggers>
                            </asp:UpdatePanel>
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
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/Area_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Area
                            </a>
                        </div>

                        <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="upd">
                            <ContentTemplate>
                                <script type="text/javascript">
                                    function jScriptmsg() {
                                        if (!jQuery().uniform) {
                                            return;
                                        }
                                        if (test = $("#sample_12 input[type=checkbox]:not(.toggle)")) {
                                            test.uniform();
                                        }
                                        CheckAll();
                                    }
                                    Sys.Application.add_load(jScriptmsg);
                                </script>
                                <asp:ListView ID="lstArea" runat="server" OnSorting="lstArea_Sorting">
                                    <LayoutTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">
                                                    <th style="width: 8px;">
                                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                                    </th>
                                                    <th>Sr #</th>
                                                    <th runat="server" class="sorting" id="th4" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="AreaName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="AreaName" OnClick="SortByServiceCase_Click">Area Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th2" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="StateName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="StateName" OnClick="SortByServiceCase_Click">State Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th1" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="CityName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="CityName" OnClick="SortByServiceCase_Click">City Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th3" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ZipCode" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ZipCode" OnClick="SortByServiceCase_Click">Zip Code</asp:LinkButton></th>
                                                    <th class="hidden-phone">Status</th>
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
                                                <asp:HiddenField ID="hdnAreaId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("Name") %></td>
                                            <td><%#Eval("StateName") %></td>
                                            <td><%#Eval("CityName") %></td>
                                            <td class="zip"><%#Eval("ZipCodes") %></td>
                                            <td>
                                                <span class="label label-<%#Eval("Status").ToString().ToLower()=="true"?"active":"inactive"%>"><%#Eval("Status").ToString().ToLower()=="true"? "Active" :"Inactive"%></span>
                                            </td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/Area_AddEdit.aspx?AreaId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">
                                                    <th style="width: 8px;">
                                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                                    </th>
                                                    <th>Sr #</th>
                                                    <th>Area Name</th>
                                                    <th>State Name</th>
                                                    <th>City Name</th>
                                                    <th class="hidden-phone zip">Zip Codes</th>
                                                    <th class="hidden-phone">Status</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td colspan="8">No Data Found</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </EmptyDataTemplate>
                                </asp:ListView>

                                <asp:DataPager ID="dataPagerArea" runat="server" PagedControlID="lstArea"
                                    OnPreRender="dataPagerArea_PreRender">
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
    </script>
</asp:Content>
