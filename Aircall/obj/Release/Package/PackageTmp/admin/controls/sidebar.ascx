<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="sidebar.ascx.cs" Inherits="Aircall.admin.controls.sidebar" %>
<ul class="sidebar-menu">
    <li id="liDashboard"><a class="" href="<%=Application["SiteAddress"] %>admin/dashboard.aspx"><span class="icon-box"><i class="icon-dashboard"></i></span>Dashboard</a></li>
    
    <li id="liServices" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-wrench"></i></span>Services<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liPending"><a href="<%=Application["SiteAddress"]%>admin/PendingService_List.aspx">Pending</a></li>
            <li id="liWaiting"><a href="<%=Application["SiteAddress"]%>admin/WaitingService_List.aspx">Waiting For Approval</a></li>
            <li id="liScheduled"><a href="<%=Application["SiteAddress"]%>admin/ScheduledService_List.aspx">Scheduled</a></li>
            <li id="liNoShow"><a href="<%=Application["SiteAddress"]%>admin/NoShowService_List.aspx">No Show Services</a></li>
            <li id="liCompleted"><a href="<%=Application["SiteAddress"]%>admin/CompletedService_List.aspx">Completed</a></li>
            <li id="liHistory"><a href="<%=Application["SiteAddress"]%>admin/HistoryService_List.aspx">History</a></li>
            <li id="liReports"><a href="<%=Application["SiteAddress"]%>admin/ServiceReport_List.aspx">Service Reports</a></li>
        </ul>
    </li>

    <li id="liOrders" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"] %>admin/Order_List.aspx"><span class="icon-box"><i class="icon-money"></i></span>Orders</a></li>

    <li id="liClient" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-group"></i></span>Client MGMT<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liClientProfile"><a href="<%=Application["SiteAddress"]%>admin/Client_List.aspx">Client Profiles</a></li>
            <li id="liClientUnit"><a href="<%=Application["SiteAddress"]%>admin/ClientAcUnit_List.aspx">Client AC Unit</a></li>
            <li id="liSubscription"><a href="<%=Application["SiteAddress"]%>admin/ClientUnitSubscription_List.aspx">AC Unit Subscriptions</a></li>
            <li id="liSubscription_UnSubmitted"><a href="<%=Application["SiteAddress"]%>admin/ClientUnitSubscription_List_UnSubmitted.aspx">AC Unit UnSubmitted Subscriptions</a></li>
            <li id="liBilling"><a href="<%=Application["SiteAddress"]%>admin/BillingHistory_List.aspx">Billing History</a></li>
            <li id="liClientNotification"><a href="<%=Application["SiteAddress"]%>admin/ClientSendNotification.aspx">Send Notification</a></li>
            <li id="liRenewCancel"><a href="<%=Application["SiteAddress"]%>admin/RenewCancel_List.aspx">Renew/Cancel Subscription</a></li>
        </ul>
    </li>

    <li id="liEmployee" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-group"></i></span>Employee MGMT<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liEmpProfile"><a href="<%=Application["SiteAddress"]%>admin/Employee_List.aspx">Employee Profiles</a></li>
            <li id="liEmpWorkArea"><a href="<%=Application["SiteAddress"]%>admin/EmployeeWorkArea_List.aspx">Employee WorkAreas</a></li>
            <li id="liLeave"><a href="<%=Application["SiteAddress"]%>admin/EmployeeLeave_List.aspx">Leave MGMT</a></li>
            <li id="liSendNotification"><a href="<%=Application["SiteAddress"]%>admin/SendNotification.aspx">Send Notification</a></li>
            <li id="liEmpRating"><a href="<%=Application["SiteAddress"]%>admin/EmployeeRatingReview_List.aspx">Rating and Review</a></li>
        </ul>
    </li>

    <li id="liPartner" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"] %>admin/Partner_List.aspx"><span class="icon-box"><i class="icon-user"></i></span>Partner MGMT</a></li>
    <li id="liPlan" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"] %>admin/Plan_List.aspx"><span class="icon-box"><i class="icon-magic"></i></span>Plan MGMT</a></li>
    <li id="liUser" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"] %>admin/AdminUser_List.aspx"><span class="icon-box"><i class="icon-user"></i></span>Admin Users</a></li>

    <li id="liLocation" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-map-marker"></i></span>Location MGMT<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liState"><a href="<%=Application["SiteAddress"]%>admin/States_list.aspx">States</a></li>
            <li id="liCity"><a href="<%=Application["SiteAddress"]%>admin/City_List.aspx">Cities</a></li>
            <li id="liZip"><a href="<%=Application["SiteAddress"]%>admin/ZipCode_List.aspx">Zip Codes</a></li>
            <li id="liArea"><a href="<%=Application["SiteAddress"]%>admin/Area_List.aspx">Areas</a></li>
        </ul>
    </li>
    <li id="liInventory" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-wrench"></i></span>Inventory MGMT<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liDailyPart"><a href="<%=Application["SiteAddress"]%>admin/DailyPart_List.aspx">Daily Part List</a></li>
            <li id="liParts"><a href="<%=Application["SiteAddress"]%>admin/Part_List.aspx">Part List</a></li>
            <li id="liUnitParts"><a href="<%=Application["SiteAddress"]%>admin/UnitType_List.aspx">Unit Type List</a></li>
            <li id="liEmpPartReq"><a href="<%=Application["SiteAddress"]%>admin/EmployeePartRequest_List.aspx">Emp. Part Request</a></li>
            <li id="liEmpPartList"><a href="<%=Application["SiteAddress"]%>admin/EmployeePart_List.aspx">Emp. Parts List</a></li>
        </ul>
    </li>
    <li id="liRequest" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-envelope-alt"></i></span>Request MGMT<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liContact"><a href="<%=Application["SiteAddress"]%>admin/ContactRequest_List.aspx">Contact</a></li>
            <li id="liSales"><a href="<%=Application["SiteAddress"]%>admin/SalesRequest_List.aspx">Sales Visit</a></li>
            <li id="liPartnerTicket"><a href="<%=Application["SiteAddress"]%>admin/PartnerTicket_List.aspx">Partner Ticket</a></li>
        </ul>
    </li>
    <li id="liCMS" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-file-alt"></i></span>CMS<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liBlock"><a href="<%=Application["SiteAddress"]%>admin/Block_List.aspx">Blocks</a></li>
            <li id="liPages"><a href="<%=Application["SiteAddress"]%>admin/CMSPages_List.aspx">Pages</a></li>
            <li id="liMobileScreen"><a href="<%=Application["SiteAddress"]%>admin/CMSPagesMobile_List.aspx">Mobile Screen</a></li>
            <li id="liNews" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"] %>admin/News_List.aspx">News</a></li>
        </ul>
    </li>
    <li id="liReports1" runat="server" clientidmode="static" class="has-sub"><a class="" href="javascript:;">
        <span class="icon-box"><i class="icon-bar-chart"></i></span>Reports<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liSalesReport" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"]%>admin/sales_report.aspx">Sales</a></li>
            <li id="liUnitReport" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"]%>admin/unit_report.aspx">Units</a></li>
            <li id="liRatingReport" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"]%>admin/rating_report.aspx">Ratings</a></li>
            <li id="liLowStockReport" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"]%>admin/lowstock_report.aspx">Low Stock</a></li>
            <li id="liInventoryReport" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"]%>admin/inventory_report.aspx">Missing Inventory</a></li>
            <li id="liPartnerReport" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"]%>admin/partner_report.aspx">Partners Sales</a></li>
            <li id="liRecurringReport" runat="server" clientidmode="static"><a class="" href="<%=Application["SiteAddress"]%>admin/recurring_report.aspx">Recurring Billing</a></li>
        </ul>
    </li>
    <li id="liOthers" runat="server" clientidmode="static" class="has-sub">
        <a class="" href="javascript:;"><span class="icon-box"><i class="icon-list"></i></span>Settings<span class="arrow"></span></a>
        <ul class="sub">
            <li id="liEmailTemplate"><a href="<%=Application["SiteAddress"]%>admin/EmailTemplate_List.aspx">Email Template</a></li>
            <li id="liSendMail"><a href="<%=Application["SiteAddress"]%>admin/send_mail.aspx">Send Mail</a></li>
            <li id="liNotification"><a href="<%=Application["SiteAddress"]%>admin/Notification_List.aspx">Notifications</a></li>
            <li id="liSitesettingList"><a href="<%=Application["SiteAddress"]%>admin/sitesetting_list.aspx">Site Settings</a></li>
        </ul>
    </li>
    <li id="liHelp" runat="server" clientidmode="static" class="has-sub">
        <a class="" href="javascript:;"><span class="icon-box"><i class="icon-list"></i></span>Help<span class="arrow"></span></a>
        <ul class="sub">
            <li><a class="" href="<%=Application["SiteAddress"]%>admin/help/AirCall Documentation.html" target="_blank">Help</a></li>
            <li><a target="_blank" href="https://docs.google.com/gview?url=<%=Application["SiteAddress"]%>uploads/pdfs/UserManualOfEmployeeAppPDF.pdf">Employee App Manual</a></li>
            <li><a target="_blank" href="https://docs.google.com/gview?url=<%=Application["SiteAddress"]%>uploads/pdfs/UserManualOfClientAppPDF.pdf">Client App Manual</a></li>
        </ul>
        
    </li>    
    <%--<li><a class="" href="<%=Application["SiteAddress"] %>admin/Logout.aspx"><span class="icon-box"><i class="icon-user"></i></span>Logout</a></li>--%>
    <li><asp:LinkButton ID="lnkLogout" runat="server" Text='<span class="icon-box"><i class="icon-user"></i></span>Log Out' OnClick="lnkLogout_Click"></asp:LinkButton></li>
</ul>
