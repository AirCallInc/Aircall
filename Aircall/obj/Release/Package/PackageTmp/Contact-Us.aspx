<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Contact-Us.aspx.cs" Inherits="Aircall.Contact_Us" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <title><%=Page.Title %></title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <!--<meta name="viewport" content="width=1024"/>-->
    <meta name="description" content="" />
    <meta name="keywords" content="">
    <meta name="format-detection" content="telephone=no">
    <asp:Literal ID="ltrAdditionalMeta" runat="server"></asp:Literal>
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link rel="icon" type="image/png" href="images/favicon.png" />
    <!--<link rel="icon" type="image/png" href="images/favicon.png" /> -->
    <!-- main css -->
    <link href="css/style.css" rel="stylesheet" />
    <!-- responsive css -->
    <link href="css/responsive.css" rel="stylesheet" />
    <!-- Font css -->
    <link href="fonts/fonts.css" rel="stylesheet" />
    <link href='http://fonts.googleapis.com/css?family=Lato:400,900italic,900,100italic,300italic,400italic,700italic,700,300,100' rel='stylesheet' type='text/css'>
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
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltrMobileMenu" runat="server"></asp:Literal>
        <div id="wrapper">
            <asp:Literal ID="ltrHeader" runat="server"></asp:Literal>

            <!-- banner part -->
            <div class="banner-product" id="BanngerImg" runat="server" src="">
                <div class="container">
                    <h1>
                        <asp:Literal ID="ltBannerText" runat="server"></asp:Literal></h1>
                </div>
            </div>

            <!-- content area part -->
            <div id="content-area">
                <div class="common-content">
                    <div class="container">
                        <div id="dvMessage" runat="server" visible="false"></div>
                        <div class="contactus-row cf">
                            <div class="contact-left">
                                <asp:Literal ID="ltMiddle" runat="server"></asp:Literal>
                                <asp:Literal ID="ltContent" runat="server"></asp:Literal>
                            </div>
                            <div class="contact-right">
                                <div class="border-block">
                                    <h3>Contact Information:</h3>
                                    <div class="main-from">

                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Your name: </label>
                                            </div>
                                            <div class="right-side">
                                                <asp:TextBox ID="txtYourName" runat="server" CssClass="input-large"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvName" CssClass="error" runat="server" ControlToValidate="txtYourName" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>E-mail address:</label>
                                            </div>
                                            <div class="right-side">
                                                <asp:TextBox ID="txtEmail" runat="server" CssClass="input-large"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid Email." Display="Dynamic" ValidationGroup="vgContact" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Phone Number: </label>
                                            </div>
                                            <div class="right-side">
                                                <asp:TextBox ID="txtphone" runat="server" CssClass="input-large"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Phone Number is required." Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regExpPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Invalid Phone Number." Display="Dynamic" ValidationGroup="vgContact" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Message: </label>
                                            </div>
                                            <div class="right-side">
                                                <asp:TextBox ID="txtmsg" runat="server" CssClass="input-large" TextMode="MultiLine"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvMessage" CssClass="error" runat="server" ControlToValidate="txtmsg" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="single-row submit-row cf">
                                            <div class="right-side">
                                                <asp:Button ID="btnSave" Text="Submit" CssClass="btn btn-primary" runat="server" ValidationGroup="vgContact" OnClick="btnSave_Click" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                            <asp:Literal ID="ltBottom" runat="server"></asp:Literal>

                        </div>
                    </div>
                </div>
            </div>
            <%--<div class="footer-push"></div> --%>

            <!-- footer part -->
            <asp:Literal ID="ltrFooter" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>

