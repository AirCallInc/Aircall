<%@ Page Title="" Language="C#" MasterPageFile="~/Front.Master" AutoEventWireup="true" CodeBehind="reset-password.aspx.cs" Inherits="Aircall.reset_password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- banner part -->
    <div class="banner" style="background-image: url('images/product-banner.jpg')">
        <div class="container">
            <h1>Reset Password</h1>
        </div>
    </div>
    <!-- content area part -->
    <div id="content-area">
        <div class="common-content">
            <div class="container">
                <div class="urlnotexist single-row button-bar cf" id="dvLinkExpired" runat="server" visible="false">
                    <h5>This URL is no longer valid!</h5>
                </div>
                <div class="urlnotexist single-row button-bar cf" id="dvResetPasswordSuccess" runat="server" visible="false">
                    <h5>Your password has been reset successfully.</h5>
                </div>
                <div class="urlnotexist single-row button-bar cf" id="dvResetError" runat="server" visible="false">
                    <h5>Oops! Some error occured. Please try again later.</h5>
                </div>
                <div class="border-block resetpassword-block cf" id="dvResetPassword" runat="server" visible="false">
                    <div class="main-from login-block">
                        <h5>Reset Password</h5>
                        <p>Choose new password to reset your account password.</p>

                        <div class="error single-row cf" id="dvError" runat="server" visible="false">
                        </div>
                        <asp:ValidationSummary ID="vgSummary" runat="server" ValidationGroup="vgLogin" CssClass="error" DisplayMode="List" />
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Email</label>
                            </div>
                            <div class="right-side">
                                <label id="lblEmail" runat="server"></label>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Password</label>
                            </div>
                            <div class="right-side">
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="revPassword" CssClass="error" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." Display="None" ValidationGroup="vgLogin"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPassword" CssClass="error" runat="server" ErrorMessage="Minimum 6 Characters Required!" ValidationGroup="vgLogin" Display="None" ValidationExpression="^.{6,}$"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Confirm Password</label>
                            </div>
                            <div class="right-side">
                                <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvConfirmPassword" CssClass="error" runat="server" ControlToValidate="txtConfirmPassword" ErrorMessage="Password is required." Display="None" ValidationGroup="vgLogin"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" ErrorMessage="Password & confirm password must be same." Display="None" ValidationGroup="vgLogin"></asp:CompareValidator>                                
                            </div>
                        </div>
                        <div class="single-row button-bar cf">
                            <div class="left-side">
                                &nbsp;
                            </div>
                            <div class="right-side">
                                <asp:Button ID="btnResetPassword" runat="server" Text="Change Password" CssClass="main-btn" OnClick="btnResetPassword_Click" ValidationGroup="vgLogin" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
