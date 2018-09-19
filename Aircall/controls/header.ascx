<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="header.ascx.cs" Inherits="Aircall.controls.header" %>

<header class="main-header">
    <div class="header-top">
        <div class="container cf">
            <a class="logo" href="<%=Application["SiteAddress"] %>index.aspx">
                <img src="<%=Application["SiteAddress"] %>images/logo.png" alt="" title=""></a>
            <%--<div class="responsive-icon">
                <a class="btn-m-nav" href="#">
                    <span></span>
                    <span></span>
                    <span></span>
                </a>
            </div>--%>
            <div class="signup-right">
                <div class="login-signup cf">
                    <a href="<%=Application["SiteAddress"] %>sign-in.aspx" class="login-link">LOG IN</a>
                    <a href="<%=Application["SiteAddress"] %>signup.aspx" class="signup-link">SIGN UP</a>
                </div>
                <nav class="signup-menu">
                    <ul>
                        <%--<li><a href="<%=Application["SiteAddress"] %>our-story.aspx">Our story</a></li>
                        <li><a href="<%=Application["SiteAddress"] %>partner/">Partners</a></li>
                        <li><a href="<%=Application["SiteAddress"] %>News.aspx">NEWS</a></li>
                        <li><a href="<%=Application["SiteAddress"] %>Contact-Us.aspx">ContaCt us</a></li>--%>
                    </ul>
                </nav>
            </div>

        </div>
    </div>
    <%--<div class="header-bottom">
        <div class="container cf">
            <nav class="main-menu">
                <ul>
                    <li><a href="<%=Application["SiteAddress"] %>index.aspx" title="">Home</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>industrial.aspx" title="">Industrial</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>commercial.aspx" title="">Commercial</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>multi_family.aspx" title="">Multi-family</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>residential.aspx" title="">Residential</a></li>
                </ul>
            </nav>
        </div>
    </div>--%>
</header>