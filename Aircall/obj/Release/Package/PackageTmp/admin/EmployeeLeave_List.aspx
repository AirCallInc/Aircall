<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeLeave_List.aspx.cs" Inherits="Aircall.admin.EmployeeLeave_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Employee Leave List</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Employee Leave List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-calendar"></i>
                            Employee Leave List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Employee</label>
                                <asp:TextBox ID="txtEmployee" runat="server" CssClass="input-medium"></asp:TextBox>
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
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click"/>
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'EmployeeLeave_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger" OnClick="lnkDelete_Click">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/EmployeeLeave_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Employee Leave
                            </a>
                        </div>
                        <table class="table table-striped table-bordered" id="sample_1">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_1 .checkboxes" />
                                    </th>
                                    <th>Sr #</th>
                                    <th>Employee Name</th>
                                    <th class="hidden-phone">Leave Date From</th>
                                    <th class="hidden-phone">Leave Date To</th>
                                    <th>Leave Added Date</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstEmpLeave" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnLeaveId" runat="server" Value='<%#Eval("Id") %>' />
                                                <asp:HiddenField ID="hdnAllowDelete" runat="server" Value='<%#Eval("AllowDelete") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("FirstName") %>  <%#Eval("LastName") %></td>
                                            <td><%#Eval("StartDate","{0:MM/dd/yyyy}") %></td>
                                            <td><%#Eval("EndDate","{0:MM/dd/yyyy}") %></td>
                                            <td><%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/EmployeeLeave_AddEdit.aspx?LeaveId=<%#Eval("Id") %>" style='display:<%#Eval("AllowEdit") %>;' 
                                                    class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                                <a href="<%=Application["SiteAddress"]%>admin/EmployeeLeave_View.aspx?LeaveId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-eye-open"></i>View</a>
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
