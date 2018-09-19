<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="HistoryServiceDeleted_View.aspx.cs" Inherits="Aircall.admin.HistoryServiceDeleted_View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Request Service History Detail</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/HistoryService_List.aspx">Request Service History List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Request Service History Detail</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>Deleted Services Detail Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Service Case #</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblServiceCaseNo" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Client Name</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblClientName" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Address</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblAddress" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Mobile</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblMobile" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Home</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblHome" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Office</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblOffice" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Requested</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblUnitRequested" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Requested On</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblServiceReq" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Requested Time</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblServiceReqTime" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Purpose Of Visit</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblPurposeofVisit" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Notes</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblNotes" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Deleted By Client</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblDeleted" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-actions">
                                <input type="button" class="btn" value="Back To List" onclick="location.href = 'HistoryService_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
