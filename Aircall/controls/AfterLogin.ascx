<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AfterLogin.ascx.cs" Inherits="Aircall.controls.AfterLogin" %>
<div class="account-right">
    <div class="account-toggle">
        <div class="name">
            <a href="/client/dashboard.aspx" id="lnkUsername" runat="server"></a>
        </div>
        <div class="account-setting">
            <a href="/client/account-setting.aspx">Account settings</a>
        </div>
    </div>
    <a class="notification-icon" href="/client/notification.aspx">
        <asp:Label ID="ltrCnt" CssClass="notification-number" runat="server" Text=""></asp:Label>        
    </a>
    <a class="home-icon" href="/client/address-list.aspx"></a>    
</div>


