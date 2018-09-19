<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Aircall.Admin.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login page</title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
    <link href="<%=Application["SiteAddress"] %>admin/assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>admin/assets/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>admin/css/style.min.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>admin/css/style_responsive.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>admin/css/style_default.css" rel="stylesheet" id="style_color" />
</head>
<body id="login-body">
    <div class="login-header">
        <div id="logo" class="center">
            <img src="<%=Application["SiteAddress"] %>admin/img/logo.png" alt="logo" class="center"  style="max-width:175px;" />
        </div>
    </div>
    <div id="login">
        <form id="loginfrm" runat="server">
            <div class="alert alert-error" id="dvMsg" runat="server">
                <b>Failed!</b>
                <asp:Label ID="lblMsg" runat="server" Text=""></asp:Label>
            </div>
            <div class="alert alert-success" id="dvSuccessMsg" runat="server">
                <b>Process Successfully!</b>
                <asp:Label ID="lblSuccessMsg" runat="server" Text=""></asp:Label>
            </div>
            <div id="loginform" class="form-vertical no-padding no-margin">
                <div class="lock">
                    <i class="icon-lock"></i>
                </div>
                <div class="control-wrap">
                    <h4>Admin Login</h4>
                    <div class="control-group" id="dvUserName">
                        <div class="controls">
                            <div class="input-prepend">
                                <span class="add-on"><i class="icon-user"></i></span>
                                <asp:TextBox ID="txtUsername" runat="server" placeholder="Username" ValidationGroup="Login"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="control-group" id="dvPassword">
                        <div class="controls">
                            <div class="input-prepend">
                                <span class="add-on"><i class="icon-key"></i></span>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password" ValidationGroup="Login"></asp:TextBox>
                            </div>
                            <div class="mtop10">
                                <div class="block-hint pull-left small">
                                    <asp:CheckBox ID="chkRemember" runat="server" />
                                    Remember Me
                                </div>
                                <div class="block-hint pull-right">
                                    <a href="javascript:;" class="" id="forget-password">Forgot Password?</a>
                                </div>
                            </div>
                            <div class="clearfix space5"></div>
                        </div>
                    </div>
                </div>
                <asp:Button ID="loginbtn" runat="server" Text="Login" CssClass="btn btn-block login-btn" ValidationGroup="Login" OnClientClick="return validateLogin()" OnClick="loginbtn_Click"/>
                <%--<input type="submit" id="login-btn" class="btn btn-block login-btn" value="Login"/>--%>
            </div>

            <div id="forgotform" class="form-vertical no-padding no-margin hide">
                <p class="center">Enter your e-mail address below to reset your password.</p>
                <div class="control-group" id="dvEmail">
                    <div class="controls">
                        <div class="input-prepend">
                            <span class="add-on"><i class="icon-envelope"></i></span>
                            <asp:TextBox ID="txtEmail" runat="server" placeholder="Email" TextMode="Email" ValidationGroup="ForgotPassword"></asp:TextBox>
                            <%--<input id="input-email" type="text" placeholder="Email" />--%>
                        </div>
                    </div>
                    <div class="space20"></div>
                </div>
                <asp:Button ID="btnForgot" runat="server" CssClass="btn btn-block login-btn" Text="Submit"  ValidationGroup="ForgotPassword" OnClientClick="return validateForgotPassword()" OnClick="btnForgot_Click"/>
                <%--<input type="button" id="forget-btn" class="btn btn-block login-btn" value="Submit" />--%>
            </div>
        </form>
    </div>
    <div id="login-copyright"><%= DateTime.Now.Year %> &copy; Admin Aircall. </div>
    <script src="<%=Application["SiteAddress"] %>admin/js/jquery-1.8.3.min.js"></script>
    <script src="<%=Application["SiteAddress"] %>admin/assets/bootstrap/js/bootstrap.min.js"></script>
    <script src="<%=Application["SiteAddress"] %>admin/js/jquery.blockui.js"></script>
    <script src="<%=Application["SiteAddress"] %>admin/js/scripts.js"></script>
    <script src="<%=Application["SiteAddress"] %>admin/js/common.js"></script>
    <script>jQuery(document).ready(function () { App.initLogin() });</script>
</body>
</html>
