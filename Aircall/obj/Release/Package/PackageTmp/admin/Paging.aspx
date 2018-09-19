<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Paging.aspx.cs" Inherits="Aircall.admin.Paging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Admin Users</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Admin Users List</a><span class="divider-last">&nbsp;</span></li>
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
                            Admin Users
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                    </th>
                                    <th>Username</th>
                                    <th class="hidden-phone">Name</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstUsers" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnUserId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%#Eval("UserName") %></td>
                                            <td><%#Eval("Name") %></td>
                                            <td></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lstUsers" PageSize="10"
                            OnPreRender="DataPager1_PreRender">
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
    <%--<script>
        jQuery(document).ready(function () {
            $(".checkboxes").change(function () {
                var checked = jQuery(this).is(":checked");
                if (checked == false) {
                    jQuery('#sample_12 .group-checkable').attr("checked", false);
                    jQuery.uniform.update(jQuery('#sample_12 .group-checkable'));
                }
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
        });
    </script>--%>
</asp:Content>
