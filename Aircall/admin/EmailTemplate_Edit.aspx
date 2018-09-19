<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmailTemplate_Edit.aspx.cs" Inherits="Aircall.admin.EmailTemplate_Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Email Template Edit </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="<%=Application["SiteAddress"]%>admin/EmailTemplate_List.aspx">Email Templates</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Email Template Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-envelope"></i>&nbsp;Email Template Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Email Template Name&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtTemplateName" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Subject&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtSubject" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvSubject" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtSubject"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">From Email&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmail" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regEmail" runat="server" ErrorMessage="Invalid Email." Font-Size="12px" Font-Bold="true" ControlToValidate="txtEmail" CssClass="error_required"  Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">CC Emails</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCCEmails" runat="server" CssClass="span8"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regExpCCEmail" runat="server" ErrorMessage="Invalid Email" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCCEmails" ValidationExpression="^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[;]{0,1}\s*)+$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email Body&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <textarea id="CKEditor1" clientidmode="static" style="resize: none" runat="server" class="span5 ckeditor" rows="5" cols="5" maxlength="250" placeholder="Description"></textarea>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Available Tags</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrAvailableTag" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" runat="server" ValidationGroup="ChangeGroup" OnClick="btnSave_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'EmailTemplate_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
