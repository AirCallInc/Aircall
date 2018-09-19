<%@ Page Title="News" Language="C#" AutoEventWireup="true" CodeBehind="News.aspx.cs" Inherits="Aircall.News" %>

<!DOCTYPE HTML>

<html>
<head runat="server">
    <meta charset="UTF-8">
    <title><%=Page.Title %></title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">

    <meta name="description" content="" />
    <meta name="keywords" content="">
    <meta name="format-detection" content="telephone=no">
    <asp:Literal ID="ltrAdditionalMeta" runat="server"></asp:Literal>
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link rel="icon" type="image/png" href="images/favicon.png" />

    <!-- main css -->
    <link href="<%=Application["SiteAddress"] %>css/style.css" rel="stylesheet" />

    <!-- responsive css -->
    <link href="<%=Application["SiteAddress"] %>css/responsive.css" rel="stylesheet" />

    <!-- Font css -->
    <link href="<%=Application["SiteAddress"] %>fonts/fonts.css" rel="stylesheet" />
    <link href='http://fonts.googleapis.com/css?family=Lato:400,900italic,900,100italic,300italic,400italic,700italic,700,300,100' rel='stylesheet' type='text/css'>
    <link href='https://fonts.googleapis.com/css?family=Roboto:400,100,100italic,300,300italic,500italic,700,900italic,900,700italic,500,400italic' rel='stylesheet' type='text/css'>

    <!--[if IE]>
     	<script src="js/html5shiv.js"></script>
    <![endif]-->

    <!-- main script -->
    <script src="<%=Application["SiteAddress"] %>js/jquery-1.9.1.min.js"></script>

    <!-- placeholder script -->
    <script src="<%=Application["SiteAddress"] %>js/placeholder.js"></script>

    <!-- jquery-ui script -->
    <script src="<%=Application["SiteAddress"] %>js/jquery-ui.js"></script>
    <link href="<%=Application["SiteAddress"] %>css/jquery-ui.css" rel="stylesheet" />

    <!-- owl.carousel script -->
    <script src="<%=Application["SiteAddress"] %>js/owl.carousel.js"></script>
    <link href="<%=Application["SiteAddress"] %>css/owl.carousel.css" rel="stylesheet" />

    <!-- general script -->
    <script src="<%=Application["SiteAddress"] %>js/script.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltrMobileMenu" runat="server"></asp:Literal>
        <div id="wrapper">
            <asp:Literal ID="ltrHeader" runat="server"></asp:Literal>
            <asp:Literal ID="ltBanner" runat="server"></asp:Literal>

            <!-- content area part -->
            <div id="content-area">
                <div class="common-content">
                    <div class="container">
                        <asp:Literal ID="ltMiddle" runat="server"></asp:Literal>
                        <asp:Literal ID="ltContent" runat="server"></asp:Literal>
                        <div class="news-listing">
                            <asp:ListView ID="lstNews" runat="server">
                                <ItemTemplate>
                                    <div class="single-news-row cf">
                                        <div class="news-left">
                                            <div class="posted-date">
                                                <div class="date"><%# Eval("PublishDate","{0:MMM}")%> <%# Eval("Day")%> </div>
                                                <div class="year"><%# Eval("Year")%></div>
                                            </div>
                                        </div>
                                        <div class="news-right">
                                            <div class="title"><a href="News/<%#Eval("NewsUrl") %>"><%#Eval("NewsHeading") %></a></div>
                                            <div class="post-content"><%#Eval("ShortDescription") %></div>
                                            <a class="main-btn" href="News/<%#Eval("NewsUrl") %>">Read more</a>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="single-news-row cf">
                                        No News Available
                                    </div>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>
                        <asp:Literal ID="ltBottom" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
            <asp:Literal ID="ltrFooter" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
