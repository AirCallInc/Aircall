<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ContactRequest_SendEmail.aspx.cs" Inherits="Aircall.admin.ContactRequest_SendEmail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Add/Edit Admin User</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/ContactRequest_List.aspx">Contact Request List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Contact Request Send Email</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-envelope"></i>Contact Request Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Customer Name</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrName" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrEmail" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Message</label>
                                <div class="controls">
                                    <label class="control-label span12">
                                        <asp:Literal ID="ltrMessage" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Request Date</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrReqDate" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Subject<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtSubject" runat="server" CssClass="span8 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvSubject" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtSubject" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Use Email Template</label>
                                <div class="controls">
                                    <label class="checkbox">
                                        <asp:CheckBox ID="chkUseTemplate" runat="server" AutoPostBack="true" OnCheckedChanged="chkUseTemplate_CheckedChanged"/>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email Body</label>
                                <div class="controls">
                                    <textarea class="span12 ckeditor" id="txtBody" runat="server" name="editor1" rows="10"></textarea>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSend" Text="Send Email" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSend_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'ContactRequest_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
