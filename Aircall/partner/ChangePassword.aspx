<%@ Page Title="" Language="C#" MasterPageFile="~/partner/PartnerMaster.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="Aircall.partner.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Change Password</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>partner/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Change Password</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-key"></i>Change Password</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Old Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtOldPassword" runat="server" CssClass="input-large" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvOldPass" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtOldPassword"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">New Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="input-large"  TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvNewPassword" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtNewPassword" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Confirm Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtConfirmPass" runat="server" CssClass="input-large"  TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvConfirmPass" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtConfirmPass" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="CompareValidator" Font-Size="12px" Font-Bold="true" CssClass="error_required" ControlToValidate="txtConfirmPass" ControlToCompare="txtNewPassword" ValidationGroup="ChangeGroup">Password entries do not match</asp:CompareValidator>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click"/>
                                <%--<button type="submit" class="btn btn-primary" runat="server" ValidationGroup="ChangeGroup"><i class="icon-ok"></i>Save</button>--%>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Profile.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
