<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="sitesetting_list.aspx.cs" Inherits="Aircall.admin.sitesetting_list" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Site Setting List </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Site Setting</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMessage" runat="server" visible="false">
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4>
                            <i class="icon-wrench"></i>&nbsp;Site Setting List
                        </h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <table class="table table-striped table-bordered" id="sample_1">
                            <thead>
                                <tr>
                                    <th style="width: 44px;" class="hidden-phone srno">Sr. No.</th>
                                    <th>Site Setting Name
                                    </th>
                                    <th class="hidden-phone wordwrapformobile">Site Setting Value
                                    </th>
                                    <th>Action
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstSiteSettings" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("DisplayName") %></td>
                                            <td><%#Eval("Value") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/sitesettingEdit.aspx?id=<%#Eval("Id") %>"
                                                    class="btn mini purple"><i class="icon-edit"></i>&nbsp;Edit</a>
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
    </div>
</asp:Content>
