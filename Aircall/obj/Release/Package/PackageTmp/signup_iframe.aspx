<%@ Page Title="SignUp" Language="C#" AutoEventWireup="true" CodeBehind="signup_iframe.aspx.cs" Inherits="Aircall.signup_iframe" %>

<!DOCTYPE HTML>
<html>
<head runat="server">
    <meta charset="UTF-8">
    <title><%=Page.Title %></title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <!--<meta name="viewport" content="width=1024"/>-->
    <meta name="description" content="" />
    <meta name="keywords" content="">
    <meta name="format-detection" content="telephone=no">
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link rel="icon" type="image/png" href="images/favicon.png" />
    <!--<link rel="icon" type="image/png" href="images/favicon.png" /> -->
    <!-- main css -->
    <link href="css/style.css" rel="stylesheet" />
    <!-- responsive css -->
    <link href="css/responsive.css" rel="stylesheet" />
    <!-- Font css -->
    <link href="fonts/fonts.css" rel="stylesheet" />
    <link href='https://fonts.googleapis.com/css?family=Lato:400,900italic,900,100italic,300italic,400italic,700italic,700,300,100' rel='stylesheet' type='text/css'>
    <link href='https://fonts.googleapis.com/css?family=Roboto:400,100,100italic,300,300italic,500italic,700,900italic,900,700italic,500,400italic' rel='stylesheet' type='text/css'>
    <!--[if IE]>
     	<script src="js/html5shiv.js"></script>
    <![endif]-->
    <!-- main script -->
    <script src="js/jquery-1.9.1.min.js"></script>
    <!-- placeholder script -->
    <script src="js/placeholder.js"></script>
    <!-- jquery-ui script -->
    <script src="js/jquery-ui.js"></script>
    <link href="css/jquery-ui.css" rel="stylesheet" />
    <!-- owl.carousel script -->
    <script src="js/owl.carousel.js"></script>
    <link href="css/owl.carousel.css" rel="stylesheet" />
    <!-- general script -->
    <script src="js/script.js"></script>
    <style>
        .left-side label {
            color: #fff !important;
        }

        span.ui-button-text {
            color: #ffffff;
        }

        .checkbox-outer label.ui-state-default:before {
            background-color: #fff;
        }

        .ui-state-default a, .ui-state-default a:link, .ui-state-default a:visited {
            color: #ffffff;
        }

        .single-row {
            border: none;
        }

        .main-from .submit-row {
            margin: 0;
            border-top: none;
            padding-top: 20px;
        }

        .main-from .left-side {
            width: 15% !important;
            float: left;
        }

        .main-from .right-side {
            width: 76%;
            float: left;
        }

        input[type="submit"], input[type="button"], input[type="reset"], button.main-btn {
            background-color: #1c6ad8;
            height: 45px;
            border-radius: 3px;
        }

            input[type="submit"]:hover, input[type="button"]:hover, button.main-btn:hover {
                background-color: #2089f9;
            }

        .max580 {
            max-width: 580px !important;
            width: 100%;
        }

        input[type="submit"], input[type="reset"], input[type="text"], input[type="button"], input[type="search"], input[type="url"], input[type="tel"], input[type="email"], input[type="password"], textarea {
            -webkit-appearance: none !important;
            border-radius: 0;
        }

        ::-webkit-input-placeholder {
            opacity: 1 !important;
            color: darkgray;
        }

        :-moz-placeholder {
            opacity: 1 !important;
            color: darkgray;
        }

        ::-moz-placeholder {
            opacity: 1 !important;
            color: darkgray;
        }

        :-ms-input-placeholder {
            opacity: 1 !important;
            color: darkgray;
        }

        input[type="text"], input[type="password"], input[type="email"], input[type="tel"], input[type="search"], textarea {
            font-size: 18px !important;
        }

        input[type="text"], input[type="password"], input[type="email"], input[type="tel"], input[type="search"], textarea {
            border: none;
            padding: 16px;
            color: #5b5b5b;
            font-size: 16px;
            height: auto;
            font-family: 'Lato', sans-serif;
            font-weight: 300;
            background-color: #ededed;
            border-radius: 0;
            width: 100%;
        }

        .signup-section {
            padding-bottom: 0px !important;
        }

        #loading {
            position: fixed;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background: rgba(255,255,255,0.8);
            z-index: 1000;
        }

        #loadingcontent {
            display: table;
            position: fixed;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
        }

        #loadingspinner {
            display: table-cell;
            vertical-align: middle;
            width: 100%;
            text-align: center;
            font-size: larger;
            padding-top: 80px;
        }
    </style>
    <script>
        function resize(extra) {
            var height = document.getElementsByTagName("html")[0].scrollHeight + extra;
            window.parent.postMessage(["setHeight", height], "*");
        }
    </script>
</head>

<body style="background-color: #154f85;" onload="resize(15);">
    <form runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <!-- content area part -->
        <div id="wrapper" style="background-color: #154f85;">
            <div id="content-area">
                <div class="signup-section">
                    <div class="container max580">
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
                            <ProgressTemplate>
                                <div id="loading">
                                    <div id="loadingcontent">
                                        <p id="loadingspinner">
                                            <img src="/images/ajax-loader.gif" style="height: 32px; width: 32px; vertical-align: bottom;" />
                                            processing...
                                        </p>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div id="dvMessage" runat="server" visible="false">
                                </div>
                                <asp:HiddenField ID="hdnExtra" ClientIDMode="Static" runat="server" Value="5"></asp:HiddenField>
                                <script>
                                    function resize1() {
                                        var height = document.getElementsByTagName("html")[0].scrollHeight + parseInt($("#hdnExtra").val());
                                        window.parent.postMessage(["setHeight", height], "*");
                                        populateCaptchaQuestion();
                                    }
                                    function populateCaptchaQuestion() {
                                        var n1 = Math.floor((Math.random() * 10) + 1);
                                        var n2 = Math.floor((Math.random() * 10) + 1);
                                        $("#num1").html(n1);
                                        $("#num2").html(n2);
                                        $("#hdnNum1").val(n1);
                                        $("#hdnNum2").val(n2);
                                    }
                                    Sys.Application.add_load(resize1);
                                </script>
                                <div class="main-from">
                                    <asp:Panel ID="pnlPreReg" runat="server">
                                        <div class="single-row cf">
                                            <asp:TextBox ID="txtPreEmail" runat="server" placeholder="Email"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" CssClass="error" runat="server" ControlToValidate="txtPreEmail" ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="vgPreSignup"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="error" runat="server" ControlToValidate="txtPreEmail" ErrorMessage="Invalid Email." Display="Dynamic" ValidationGroup="vgPreSignup" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                        </div>
                                        <div class="single-row submit-row cf" style="text-align: center;">
                                            <asp:Button ID="btnPreReg" runat="server" ValidationGroup="vgPreSignup" Text="Sign Up" OnClick="btnPreReg_Click" />
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlReg" runat="server" Visible="false">
                                        <div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtFirstName" runat="server" placeholder="First Name"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvFName" CssClass="error" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                            </div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtLastName" runat="server" placeholder="Last Name"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvLName" CssClass="error" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                            </div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtCompany" runat="server" placeholder="Company"></asp:TextBox>
                                            </div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtPhone" runat="server" placeholder="Mobile"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Mobile Number is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regExpPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Invalid Mobile Number." Display="Dynamic" ValidationGroup="vgSignup" ValidationExpression="\d{8,}$"></asp:RegularExpressionValidator>
                                            </div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtEmail" runat="server" placeholder="Email"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid Email." Display="Dynamic" ValidationGroup="vgSignup" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                            </div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvPassword" CssClass="error" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPassword" CssClass="error" runat="server" ErrorMessage="Minimum 6 Characters Required!" ValidationGroup="vgSignup" Display="Dynamic" ValidationExpression="^.{6,}$"></asp:RegularExpressionValidator>
                                            </div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtRePassword" runat="server" TextMode="Password" placeholder="Re-type Password"></asp:TextBox>
                                                <asp:CompareValidator ID="cmpRePassword" CssClass="error" runat="server" ErrorMessage="Password and Re-type Password must be same." Display="Dynamic" ValidationGroup="vgSignup" ControlToCompare="txtPassword" ControlToValidate="txtRePassword"></asp:CompareValidator>
                                            </div>
                                            <div class="single-row cf">
                                                <asp:TextBox ID="txtPartner" runat="server" placeholder="Partner Code (optional)"></asp:TextBox>
                                            </div>
                                            <div class="single-row cf" style="color: #ffffff; font-size: 16px;">
                                                <div id="num1" style="font-size: 35px !important; float: left;"></div>
                                                <div style="font-size: 35px !important; float: left;">+ </div>
                                                <div id="num2" style="font-size: 35px !important; float: left;"></div>
                                                <div style="font-size: 35px !important; float: left;">= </div>
                                                <asp:HiddenField ID="hdnNum1" ClientIDMode="Static" runat="server" />
                                                <asp:HiddenField ID="hdnNum2" ClientIDMode="Static" runat="server" />
                                                &nbsp;
                                                <asp:TextBox ID="txtCaptcha" CssClass="max290" runat="server" Style="float: left; width: 55px; margin-left: 5px;" MaxLength="2"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="error" runat="server" ControlToValidate="txtCaptcha" ErrorMessage="Captcha is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                                <div style="clear: both;"></div>
                                            </div>
                                            <div class="single-row  cf">
                                                <div class="checkbox-outer">
                                                    <asp:CheckBox ID="chkTerms" runat="server" ClientIDMode="Static" />
                                                    <label for="chkTerms" style="color: #ffffff;">Agree to <a href="disclosure-agreement.aspx" style="text-decoration: underline; color: #ffffff;" target="_blank">Terms & Conditions</a></label>
                                                </div>
                                            </div>
                                            <div class="single-row submit-row cf" style="text-align: center;">
                                                <asp:Button ID="btnSubmit" runat="server" ValidationGroup="vgSignup" Text="Sign Up" OnClick="btnSubmit_Click" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnSubmit" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div class="footer-push"></div>
        </div>
    </form>
</body>
</html>
