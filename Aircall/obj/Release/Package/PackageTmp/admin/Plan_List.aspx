<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Plan_List.aspx.cs" Inherits="Aircall.admin.Plan_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Plans</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Plan List</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-magic"></i>
                            Plans
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <table class="table table-striped table-bordered" id="sample_1">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_1 .checkboxes" />
                                    </th>
                                    <th>Sr #</th>
                                    <th>Plan Name</th>
                                    <th class="hidden-phone">Basic Fee</th>
                                    <th class="hidden-phone">Increment</th>
                                    <th class="hidden-phone">Terms</th>
                                    <th style="display:none">Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstPlan" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnPlanTypeId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("PlanName") %></td>
                                            <td><%#Eval("BasicFee") %></td>
                                            <td><%#Eval("FeeIncrement") %></td>
                                            <td><%#Eval("Terms") %></td>
                                            <td style="display:none">
                                                <a href="<%=Application["SiteAddress"]%>admin/Plan_AddEdit.aspx?PlanTypeId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
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
