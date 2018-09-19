<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BeforeSignUp.ascx.cs" Inherits="Aircall.controls.BeforeSignUp" %>
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
            <li><a href="https://aircallservices.com/contact-us/">ContaCt us</a></li>--%>
        </ul>
    </nav>
</div>