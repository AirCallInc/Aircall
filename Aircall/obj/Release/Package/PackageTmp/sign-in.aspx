<%@ Page Title="Signin" Language="C#" MasterPageFile="~/Front.Master" AutoEventWireup="true" CodeBehind="sign-in.aspx.cs" Inherits="Aircall.sign_in" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- banner part -->
    <div class="banner" style="background-image: url('images/product-banner.jpg')">
        <div class="container">
            <h1>Login</h1>
        </div>
    </div>
    <!-- content area part -->
    <div id="content-area">
        <div class="common-content">
            <div class="container">
                <div class="border-block signin-block cf">
                    <div class="main-from login-block">
                        <div class="single-row cf error" id="dvError" clientidmode="static" runat="server" visible="false">
                        </div>
                        <%--<asp:ValidationSummary ID="vgSummary" runat="server" ValidationGroup="vgLogin" CssClass="error" DisplayMode="List" />--%>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Email</label>
                            </div>
                            <div class="right-side">
                                <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="vgLogin"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid Email." Display="Dynamic" ValidationGroup="vgLogin" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Password</label>
                            </div>
                            <div class="right-side">

                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="revPassword" CssClass="error" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." Display="Dynamic" ValidationGroup="vgLogin"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="right-side">
                                <div class="checkbox-outer">
                                    <asp:CheckBox ID="chkRememberMe" runat="server" ClientIDMode="Static" /><label for="chkRememberMe">Remember Me</label>
                                </div>
                            </div>
                        </div>
                        <div class="single-row button-bar cf">
                            <div class="right-side">
                                <asp:Button ID="btnLogin" OnClientClick="javascript:hidedvMessage();" runat="server" Text="Login" CssClass="main-btn" OnClick="btnLogin_Click" ValidationGroup="vgLogin" />
                            </div>
                        </div>
                    </div>
                    <div class="main-from forgot-block">
                        <h5>Forgot Password</h5>
                        <p>Enter your e-mail address below to reset your password.</p>
                        
                        <div class="success single-row button-bar cf" id="dvForgotPasswordSuccess" runat="server" visible="false">
                            Reset password link has been sent to your email address.
                        </div>
                        <div class="single-row cf error" id="dvError1" runat="server" visible="false">
                        </div>
                        <%--<asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="vgForgotPassword" CssClass="error" DisplayMode="List" />--%>
                        <div class="max380">
                            <asp:TextBox ID="txtForgotEmail" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvForgotEmail" CssClass="error" runat="server" ControlToValidate="txtForgotEmail" ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="vgForgotPassword"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="revForgotEmail" CssClass="error" runat="server" ControlToValidate="txtForgotEmail" ErrorMessage="Invalid Email." Display="Dynamic" ValidationGroup="vgForgotPassword" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                        </div>
                        <div class="single-row button-bar cf">
                            <asp:Button ID="btnForgotPassword" OnClientClick="javascript:hidedvMessage();" runat="server" Text="Submit" CssClass="main-btn dark-grey" OnClick="btnForgotPassword_Click" ValidationGroup="vgForgotPassword" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script>
        $(document).ready(function () {
        });
        function hidedvMessage()
        {
            $("#dvError").hide();
        }
    </script>
</asp:Content>
