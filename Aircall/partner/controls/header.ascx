<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="header.ascx.cs" Inherits="Aircall.partner.controls.header1" %>
<div id="header" class="navbar navbar-inverse navbar-fixed-top">
    <div class="navbar-inner">
        <div class="container-fluid">
            <a class="brand" href="">
                <img src="<%=Application["SiteAddress"] %>partner/img/logo.png" alt="Aircall Partner" style="height:50px;" /></a><a class="btn btn-navbar collapsed" id="main_menu_trigger" data-toggle="collapse" data-target=".nav-collapse"><span class="icon-bar"></span><span class="icon-bar"></span><span class="icon-bar"></span><span class="arrow"></span></a><div id="top_menu" class="nav notify-row">
                </div>
            <div class="top-nav ">
                <ul class="nav pull-right top-menu">
                    <li class="dropdown mtop5">&nbsp;</li>
                    <li class="dropdown">
                        <a href="#" class="dropdown-toggle" data-toggle="dropdown">
                            <img src="" id="imgUser" width="29" runat="server" alt="" />
                            <span class="username">
                                <asp:Literal ID="ltrFullname" runat="server"></asp:Literal>
                            </span>
                            <b class="caret"></b></a>

                        <ul class="dropdown-menu">
                            <li><a href="<%=Application["SiteAddress"] %>partner/Profile.aspx"><i class="icon-user"></i>My Profile</a></li>
                            <li><a href="<%=Application["SiteAddress"] %>partner/ChangePassword.aspx"><i class="icon-key"></i>Change Password</a></li>
                            <li class="divider"></li>
                            <%--<li><a href="<%=Application["SiteAddress"] %>partner/Logout.aspx"><i class="icon-key"></i>Log Out</a></li>--%>
                            <li>
                                <asp:LinkButton ID="lnkLogout" runat="server" Text='<i class="icon-key"></i>Logout' OnClick="lnkLogout_Click"></asp:LinkButton></li>
                        </ul>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</div>

