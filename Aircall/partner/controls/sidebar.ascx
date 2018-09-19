<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="sidebar.ascx.cs" Inherits="Aircall.partner.controls.sidebar1" %>
<ul class="sidebar-menu">
    <li id="liDashboard"><a class="" href="<%=Application["SiteAddress"] %>partner/dashboard.aspx"><span class="icon-box"><i class="icon-dashboard"></i></span>Dashboard</a></li>
    <li id="liCommission"><a class="" href="<%=Application["SiteAddress"] %>partner/Commission_Report.aspx"><span class="icon-box"><i class="icon-user"></i></span>Commission Report</a></li>
    <li id="liTickets"><a class="" href="<%=Application["SiteAddress"] %>partner/Ticket_List.aspx"><span class="icon-box"><i class="icon-magic"></i></span>Tickets </a></li>
    <li id="liClients"><a class="" href="<%=Application["SiteAddress"] %>partner/Client_List.aspx"><span class="icon-box"><i class="icon-list"></i></span>Client List</a></li>
    <%--<li><a class="" href="<%=Application["SiteAddress"] %>partner/Logout.aspx"><span class="icon-box"><i class="icon-user"></i></span>Logout</a></li>--%>
    <li><asp:LinkButton ID="lnkLogout" runat="server" Text='<span class="icon-box"><i class="icon-user"></i></span>Logout' OnClick="lnkLogout_Click"></asp:LinkButton></li>
</ul>