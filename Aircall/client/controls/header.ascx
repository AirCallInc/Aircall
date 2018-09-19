<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="header.ascx.cs" Inherits="Aircall.client.controls.header" %>
<header class="main-header">
    <div class="header-top">
        <div class="container cf">
            <a class="logo" href="dashboard.aspx">
                <img src="images/logo.png" alt="" title=""></a>
            <div class="account-right">
                <div class="account-toggle">
                    <div class="name">
                        <a href="/client/account-setting.aspx" id="lnkUsername" runat="server"></a>
                    </div>
                    <div class="account-setting">
                        <a href="/client/account-setting.aspx">Account settings</a>
                    </div>
                </div>
                <a class="notification-icon" href="notification.aspx">
                    <span class="notification-number">
                        <asp:Literal ID="ltrCnt" runat="server"></asp:Literal></span>
                </a>
                <a class="home-icon" href="address-list.aspx"></a>
                <div class="responsive-icon">
                    <a class="btn-m-nav" href="#">
                        <span></span>
                        <span></span>
                        <span></span>
                    </a>
                </div>
            </div>
        </div>
    </div>
    <div class="header-bottom">
        <div class="container cf">
            <nav class="main-menu">
                <ul>
                    <li class=""><a href="dashboard.aspx" title="">Dashboard</a></li>
                    <li><a href="schedule.aspx" title="">Upcoming schedule</a></li>
                    <li><a href="request-service-list.aspx" title="">request for service</a></li>
                    <li><a href="past_services.aspx" title="">Service history</a></li>
                    <li><a href="my_units.aspx" title="">my units</a></li>
                    <li><a href="summary.aspx" title="">Unsubmitted units</a></li>
                    <li><a href="PlanCoverage.aspx" title="">Plan Options</a></li>
                    <%--<li><a href="our-story.aspx" title="">About us</a></li>--%>
                    <li><a href="Contact-Us.aspx" title="">Contact us</a></li>
                    <li><a href="terms-condition.aspx" title="">Terms & conditions</a></li>
                    <li><asp:LinkButton ID="lnkLogout" runat="server" Text="logout" OnClick="lnkLogout_Click"></asp:LinkButton></li>
                </ul>
            </nav>
        </div>
    </div>
</header>
