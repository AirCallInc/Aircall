<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="change-password.aspx.cs" Inherits="Aircall.client.change_password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Change Password</h1>
                </div>
                <div class="border-block change-password-block max640 cf">
                    <%--<asp:ValidationSummary ID="vgSummary" runat="server" ValidationGroup="vgChangePassword" CssClass="error" DisplayMode="List" />--%>
                    <div id="dvMessage" runat="server" visible="false"></div>
                    <div class="main-from">
                        <div>
                            <div class="password-row cf">
                                <div class="left-side">
                                    <label>Old Passwod</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtOldPassword" runat="server" TextMode="Password" autocomplete="off"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvOldPassword" CssClass="error" runat="server" ControlToValidate="txtOldPassword" ErrorMessage="Old Password is required." Display="Dynamic" ValidationGroup="vgChangePassword"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtOldPassword" CssClass="error" runat="server" ErrorMessage="Minimum 6 Characters Required!" ValidationGroup="vgChangePassword" Display="Dynamic" ValidationExpression="^.{6,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="password-row cf">
                                <div class="left-side">
                                    <label>New Password</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" autocomplete="off"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvNewPassword" CssClass="error" runat="server" ControlToValidate="txtNewPassword" ErrorMessage="New Password is required." Display="Dynamic" ValidationGroup="vgChangePassword"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtNewPassword" CssClass="error" runat="server" ErrorMessage="Minimum 6 Characters Required!" ValidationGroup="vgChangePassword" Display="Dynamic" ValidationExpression="^.{6,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="password-row cf">
                                <div class="left-side">
                                    <label>Confirm Password</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" onfocus="this.removeAttribute('readonly');"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" CssClass="error" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Confirm Password is required." Display="Dynamic" ValidationGroup="vgChangePassword"></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="cmpConfPassword" CssClass="error" runat="server" ErrorMessage="Password and Confirm Password must be same." Display="Dynamic" ValidationGroup="vgChangePassword" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword"></asp:CompareValidator>                                    
                                </div>
                            </div>
                            <div class="password-row button-bar cf">
                                <asp:Button ID="btnChangePassword" runat="server" CssClass="main-btn" Text="Change password" ValidationGroup="vgChangePassword" OnClick="btnChangePassword_Click" />
                                <input type="button" class="main-btn dark-grey" value="Cancel" onclick="location.href = 'account-setting.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
