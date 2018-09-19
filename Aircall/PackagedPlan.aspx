<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PackagedPlan.aspx.cs" Inherits="Aircall.PackagedPlan" %>

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
                <asp:Literal ID="ltMiddle" runat="server"></asp:Literal>
                <asp:Literal ID="ltContent" runat="server"></asp:Literal>
                <asp:Literal ID="ltBottom" runat="server"></asp:Literal>
            </div>
            <%--<div class="footer-push"></div> --%>

            <!-- footer part --> 
            <asp:Literal ID="ltrFooter" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
