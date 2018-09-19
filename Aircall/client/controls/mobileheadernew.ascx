<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="mobileheadernew.ascx.cs" Inherits="Aircall.client.controls.mobileheadernew" %>
<div class="mobile-nav-block">
    <div class="nav-content">
        <div class="mobile-account-toggle">
            <div class="name">
                <a href="#" id="lnkUsername" runat="server"></a>
            </div>
            <div class="account-setting">
                <a href="account-setting.aspx">Account settings</a>
            </div>
        </div>
        <nav class="mobile-main-nav">
            <ul>
                <li class=""><a href="dashboard.aspx" title="">Dashboard</a></li>
                <li><a href="schedule.aspx" title="">Upcoming schedule</a></li>
                <li><a href="request-service-list.aspx" title="">request for service</a></li>
                <li><a href="past_services.aspx" title="">Service history</a></li>
                <li><a href="my_units.aspx" title="">my units</a></li>
                <li><a href="PlanCoverage.aspx" title="">Plan Options</a></li>
                <%--<li><a href="our-story.aspx" title="">About us</a></li>--%>
                <li><a href="Contact-Us.aspx" title="">Contact us</a></li>
                <li><a href="terms-condition.aspx" title="">Terms & conditions</a></li>
                <li><asp:LinkButton ID="lnkLogout" runat="server" Text="logout" OnClick="lnkLogout_Click"></asp:LinkButton></li>
            </ul>
        </nav>
    </div>
</div>
