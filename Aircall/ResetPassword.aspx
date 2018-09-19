<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Aircall.ResetPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Password</title>
  <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />
    <link href="<%=Application["SiteAddress"] %>assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>assets/font-awesome/css/font-awesome.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>css/style.min.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>css/style_responsive.css" rel="stylesheet" />
    <link href="<%=Application["SiteAddress"] %>css/style_default.css" rel="stylesheet" id="style_color" />
</head>
<body id="login-body">
    <div class="login-header">
        <div id="logo-reset" class="center">
            <img src="<%=Application["SiteAddress"] %>img/logo.png" alt="logo" class="center"  style="max-width:175px;" />
        </div>
    </div>
    <div id="login">
        <form id="resetPassword" runat="server">
            <div id="resetPasswordform" class="form-vertical no-padding no-margin" runat="server">
                <div class="alert alert-error" id="dvMsg" style="display: none;" runat="server">
                    <asp:Label ID="lblMsg" runat="server" Text="Password and Re-type Password must be same."></asp:Label>
                </div>
                <div class="alert alert-success" id="dvSuccessMsg" runat="server">
                    <b>Process Successfully!</b>
                    <asp:Label ID="lblSuccessMsg" runat="server" Text=""></asp:Label>
                </div>
                <div class="lock">
                    <i class="icon-lock"></i>
                </div>
                <div class="control-wrap">
                    <h4>Reset Password</h4>
                    <div class="control-group" id="dvPassword">
                        <div class="controls">
                            <div class="input-prepend">
                                <span class="add-on"><i class="icon-key"></i></span>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rqfvPass" ForeColor="Red" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" Visible="false" CssClass="error_required" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPassword" CssClass="error" runat="server" ErrorMessage="Minimum 6 Characters Required!" ValidationGroup="ChangeGroup" Display="None" ValidationExpression="^.{6,}$"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                    </div>
                    <div class="control-group" id="dvRePassword">
                        <div class="controls">
                            <div class="input-prepend">
                                <span class="add-on"><i class="icon-key"></i></span>
                                <asp:TextBox ID="txtRetypePass" runat="server" TextMode="Password" placeholder="Re-Type Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rqfvRePass"  Visible="false"  ForeColor="Red" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtRetypePass"></asp:RequiredFieldValidator>
                                <asp:CompareValidator ID="cmpPass" runat="server" ErrorMessage="Password and Retype Password must be same." Font-Size="12px" Font-Bold="true" CssClass="error_required" ControlToValidate="txtRetypePass" ControlToCompare="txtPassword" ValidationGroup="ChangeGroup"></asp:CompareValidator>                                
                            </div>
                            <div class="clearfix space5"></div>
                        </div>
                    </div>
                </div>
                <asp:Button ID="resetPass" runat="server" Text="Reset Password" CssClass="btn btn-block login-btn" OnClientClick="return validatePassword()" OnClick="resetPass_Click" ValidationGroup="ChangeGroup" />
            </div>
            <div class="alert alert-error" id="dvInactive" runat="server">
                <b><asp:Label ID="lblInactive" runat="server" Text="Link Expired."></asp:Label></b>
            </div>
        </form>
    </div>
    <div id="login-copyright"><%= DateTime.Now.Year %> &copy; Admin Aircall. </div>
    <script src="<%=Application["SiteAddress"] %>admin/js/jquery-1.8.3.min.js"></script>
    <script src="<%=Application["SiteAddress"] %>admin/assets/bootstrap/js/bootstrap.min.js"></script>
    <script src="<%=Application["SiteAddress"] %>admin/js/jquery.blockui.js"></script>
    <script src="<%=Application["SiteAddress"] %>admin/js/scripts.js"></script>
    <script>jQuery(document).ready(function () { App.initLogin() });</script>
    <script>
        function validatePassword() {
            var error = 0;
            var password = jQuery.trim(jQuery('#txtPassword').val());
            var repassword = jQuery.trim(jQuery('#txtRetypePass').val());
            if (password.length < 1 || password == 'Password') {
                jQuery('#dvPassword').addClass('error');
                jQuery('#dvPassword').removeClass('success');

                error++;
            }
            else {
                jQuery('#dvPassword').removeClass('error');
                jQuery('#dvPassword').addClass('success');
            }

            if (repassword.length < 1 || repassword == 'Re-Type Password') {
                jQuery('#dvRePassword').addClass('error');
                jQuery('#dvRePassword').removeClass('success');

                error++;
            }
            else {
                jQuery('#dvRePassword').removeClass('error');
                jQuery('#dvRePassword').addClass('success');
            }

            if (password != repassword) {
                jQuery('#dvMsg').show();
                jQuery('#dvPassword').addClass('error');
                jQuery('#dvPassword').removeClass('success');
                jQuery('#dvRePassword').addClass('error');
                jQuery('#dvRePassword').removeClass('success');
                error++;
            }
            if (password == "")
            {
                jQuery('#txtPassword').val("");
            }
            if (repassword == "") {
                jQuery('#txtRetypePass').val("");
            }
            if (!error) {
                return true;
            }
            else {
                return false;
            }
        }
    </script>
</body>
</html>
