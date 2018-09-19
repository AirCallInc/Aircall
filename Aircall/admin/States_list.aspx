<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="States_list.aspx.cs" Inherits="Aircall.admin.States_list" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">States</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">State List</a><span class="divider-last">&nbsp;</span></li>
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
                            States
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkActive" runat="server" CssClass="btn btn-success hidden-phone" OnClick="lnkActive_Click">
                                <i class="icon-ok icon-white"></i>Active
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkInactive" runat="server" CssClass="btn btninactive hidden-phone" OnClick="lnkInactive_Click">
                                <i class="icon-off icon-white"></i>Inactive
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" runat="server" Visible="false" CssClass="btn btn-danger" OnClick="lnkDelete_Click">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/States_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add State
                            </a>
                        </div>
                        <table class="table table-striped table-bordered" id="sample_1">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_1 .checkboxes" />
                                    </th>
                                    <th><a href="#">Sr #</a></th>
                                    <th><a href="#">State Name</a></th>
                                    <th class="hidden-phone"><a href="#">Status</a></th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstState" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnStateId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("Name") %></td>
                                            <td>
                                                <span class="label label-<%#Eval("DisplayStatus").ToString()=="1"?"active":"inactive"%>"><%#Eval("DisplayStatus").ToString()=="-1"? "Pending InActive" :Eval("DisplayStatus").ToString()=="1"?"Active":"Inactive"%></span>
                                            </td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/States_AddEdit.aspx?StateId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
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
