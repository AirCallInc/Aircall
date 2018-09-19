<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeLeave_View.aspx.cs" Inherits="Aircall.admin.EmployeeLeave_View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Employee Leave List </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/EmployeeLeave_List.aspx">Employee Leave List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Employee Leave View</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-calendar"></i>&nbsp;Employee Leave Information
                        </h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Employee Name</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrEmpName" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Leave Date From</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrStart" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Leave Date To</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrEnd" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Reason</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrReason" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Approved By</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrApprovedBy" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Leave Added Date</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrAddedDate" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="form-actions">
                                <button type="button" onclick="window.location.href = '<%=Application["SiteAddress"]%>admin/EmployeeLeave_List.aspx'" class="btn">Back to list</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
