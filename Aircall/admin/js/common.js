$(document).ready(function () {
    menuselect();
    sidemenuheight();
});

function menuselect() {
    if (document.URL.indexOf("dashboard.aspx") != -1) {
        $("#liDashboard").addClass("active");
    }
    else if (document.URL.indexOf("PendingService_List.aspx") != -1 || document.URL.indexOf("RequestService_AddEdit.aspx") != -1 || document.URL.indexOf("WaitingService_List.aspx") != -1 || document.URL.indexOf("WaitingService_Edit.aspx") != -1 || document.URL.indexOf("ScheduledService_List.aspx") != -1 || document.URL.indexOf("ScheduledService_AddEdit.aspx") != -1 || document.URL.indexOf("HistoryService_List.aspx") != -1 || document.URL.indexOf("HistoryService_View.aspx") != -1 || document.URL.indexOf("CompletedService_List.aspx") != -1 || document.URL.indexOf("CompletedService_View.aspx") != -1 || document.URL.indexOf("NoShowService_List.aspx") != -1 || document.URL.indexOf("NoShowService_Edit.aspx") != -1
        || document.URL.indexOf("ServiceReport_List.aspx") != -1 || document.URL.indexOf("ServiceReport_View.aspx") != -1 || document.URL.indexOf("ServiceReport_Edit.aspx") != -1 || document.URL.indexOf("PendingService_Schedule.aspx") != -1 || document.URL.indexOf("HistoryServiceDeleted_View.aspx") != -1) {
        $("#liServices").addClass("active");
        if (document.URL.indexOf("PendingService_List.aspx") != -1 || document.URL.indexOf("RequestService_AddEdit.aspx") != -1 || document.URL.indexOf("PendingService_Schedule.aspx") != -1) {
            $("#liPending").addClass("active");
        }
        else if (document.URL.indexOf("WaitingService_List.aspx") != -1 || document.URL.indexOf("WaitingService_Edit.aspx") != -1) {
            $("#liWaiting").addClass("active");
        }
        else if (document.URL.indexOf("ScheduledService_List.aspx") != -1 || document.URL.indexOf("ScheduledService_AddEdit.aspx") != -1) {
            $("#liScheduled").addClass("active");
        }
        else if (document.URL.indexOf("NoShowService_List.aspx") != -1 || document.URL.indexOf("NoShowService_Edit.aspx") != -1) {
            $("#liNoShow").addClass("active");
        }
        else if (document.URL.indexOf("CompletedService_List.aspx") != -1 || document.URL.indexOf("CompletedService_View.aspx") != -1) {
            $("#liCompleted").addClass("active");
        }
        else if (document.URL.indexOf("HistoryService_List.aspx") != -1 || document.URL.indexOf("HistoryService_View.aspx") != -1 || document.URL.indexOf("HistoryServiceDeleted_View.aspx") != -1) {
            $("#liHistory").addClass("active");
        }
        else if (document.URL.indexOf("ServiceReport_List.aspx") != -1 || document.URL.indexOf("ServiceReport_View.aspx") != -1 || document.URL.indexOf("ServiceReport_Edit.aspx") != -1) {
            $("#liReports").addClass("active");
        }
    }
    else if (document.URL.indexOf("Order_List.aspx") != -1 || document.URL.indexOf("Order_AddEdit.aspx") != -1 || document.URL.indexOf("Order_View.aspx") != -1) {
        $("#liOrders").addClass("active");
    }
    else if (document.URL.indexOf("Client_List.aspx") != -1 || document.URL.indexOf("Client_AddEdit.aspx") != -1 || document.URL.indexOf("ClientAcUnit_List.aspx") != -1
        || document.URL.indexOf("ClientAcUnit_AddEdit.aspx") != -1 || document.URL.indexOf("BillingHistory_List.aspx") != -1
        || document.URL.indexOf("ClientSendNotification.aspx") != -1 || document.URL.indexOf("RenewCancel_List.aspx") != -1
        || document.URL.indexOf("RenewCancel_UnitSubscription.aspx") != -1 || document.URL.indexOf("ClientUnitSubscription_List.aspx") != -1
        || document.URL.indexOf("ClientUnitSubscription_Edit.aspx") != -1 || document.URL.indexOf("ClientUnitSubscription_List_UnSubmitted.aspx") != -1) {
        $("#liClient").addClass("active");
        if (document.URL.indexOf("Client_List.aspx") != -1 || document.URL.indexOf("Client_AddEdit.aspx") != -1) {
            $("#liClientProfile").addClass("active");
        }
        else if (document.URL.indexOf("ClientAcUnit_List.aspx") != -1 || document.URL.indexOf("ClientAcUnit_AddEdit.aspx") != -1) {
            $("#liClientUnit").addClass("active");
        }
        else if (document.URL.indexOf("ClientUnitSubscription_List.aspx") != -1 || document.URL.indexOf("ClientUnitSubscription_Edit.aspx") != -1) {
            $("#liSubscription").addClass("active");
        }
        else if (document.URL.indexOf("ClientUnitSubscription_List_UnSubmitted.aspx") != -1) {
            $("#liSubscription_UnSubmitted").addClass("active");
        }
        else if (document.URL.indexOf("BillingHistory_List.aspx") != -1) {
            $("#liBilling").addClass("active");
        }
        else if (document.URL.indexOf("ClientSendNotification.aspx") != -1) {
            $("#liClientNotification").addClass("active");
        }
        else if (document.URL.indexOf("RenewCancel_List.aspx") != -1 || document.URL.indexOf("RenewCancel_UnitSubscription.aspx") != -1) {
            $("#liRenewCancel").addClass("active");
        }
    }
    else if (document.URL.indexOf("Employee_List.aspx") != -1 || document.URL.indexOf("Employee_AddEdit.aspx") != -1 || document.URL.indexOf("EmployeeWorkArea_List.aspx") != -1 || document.URL.indexOf("EmployeeWorkArea_AddEdit.aspx") != -1 || document.URL.indexOf("EmployeeLeave_List.aspx") != -1 ||
        document.URL.indexOf("EmployeeLeave_AddEdit.aspx") != -1 || document.URL.indexOf("EmployeeLeave_View.aspx") != -1 || document.URL.indexOf("SendNotification.aspx") != -1 || document.URL.indexOf("EmployeeRatingReview_List.aspx") != -1 || document.URL.indexOf("EmployeeRatingReview_View.aspx") != -1) {
        $("#liEmployee").addClass("active");
        if (document.URL.indexOf("Employee_List.aspx") != -1 || document.URL.indexOf("Employee_AddEdit.aspx") != -1) {
            $("#liEmpProfile").addClass("active");
        }
        else if (document.URL.indexOf("EmployeeWorkArea_List.aspx") != -1 || document.URL.indexOf("EmployeeWorkArea_AddEdit.aspx") != -1) {
            $("#liEmpWorkArea").addClass("active");
        }
        else if (document.URL.indexOf("EmployeeLeave_List.aspx") != -1 || document.URL.indexOf("EmployeeLeave_AddEdit.aspx") != -1 || document.URL.indexOf("EmployeeLeave_View.aspx") != -1) {
            $("#liLeave").addClass("active");
        }
        else if (document.URL.indexOf("SendNotification.aspx") != -1) {
            $("#liSendNotification").addClass("active");
        }
        else if (document.URL.indexOf("EmployeeRatingReview_List.aspx") != -1 || document.URL.indexOf("EmployeeRatingReview_View.aspx") != -1) {
            $("#liEmpRating").addClass("active");
        }
    }
    else if (document.URL.indexOf("Partner_List.aspx") != -1 || document.URL.indexOf("Partner_AddEdit.aspx") != -1) {
        $("#liPartner").addClass("active");
    }
    else if (document.URL.indexOf("Plan_List.aspx") != -1 || document.URL.indexOf("Plan_AddEdit.aspx") != -1) {
        $("#liPlan").addClass("active");
    }
    else if (document.URL.indexOf("AdminUser_List.aspx") != -1 || document.URL.indexOf("AdminUser_AddEdit.aspx") != -1) {
        $("#liUser").addClass("active");
    }
    else if (document.URL.indexOf("States_list.aspx") != -1 || document.URL.indexOf("States_AddEdit.aspx") != -1 || document.URL.indexOf("City_List.aspx") != -1 || document.URL.indexOf("City_AddEdit.aspx") != -1 || document.URL.indexOf("ZipCode_List.aspx") != -1 || document.URL.indexOf("ZipCode_AddEdit.aspx") != -1 || document.URL.indexOf("Area_List.aspx") != -1 || document.URL.indexOf("Area_AddEdit.aspx") != -1) {
        $("#liLocation").addClass("active");
        if (document.URL.indexOf("States_list.aspx") != -1 || document.URL.indexOf("States_AddEdit.aspx") != -1) {
            $("#liState").addClass("active");
        }
        else if (document.URL.indexOf("City_List.aspx") != -1 || document.URL.indexOf("City_AddEdit.aspx") != -1) {
            $("#liCity").addClass("active");
        }
        else if (document.URL.indexOf("ZipCode_List.aspx") != -1 || document.URL.indexOf("ZipCode_AddEdit.aspx") != -1) {
            $("#liZip").addClass("active");
        }
        else if (document.URL.indexOf("Area_List.aspx") != -1 || document.URL.indexOf("Area_AddEdit.aspx") != -1) {
            $("#liArea").addClass("active");
        }
    }
    else if (document.URL.indexOf("ContactRequest_List.aspx") != -1 || document.URL.indexOf("ContactRequest_SendEmail.aspx") != -1 || document.URL.indexOf("SalesRequest_List.aspx") != -1 || document.URL.indexOf("SalesRequest_Detail.aspx") != -1 || document.URL.indexOf("PartnerTicket_List.aspx") != -1 || document.URL.indexOf("PartnerTicket_Edit.aspx") != -1) {
        $("#liRequest").addClass("active");
        if (document.URL.indexOf("ContactRequest_List.aspx") != -1 || document.URL.indexOf("ContactRequest_SendEmail.aspx") != -1) {
            $("#liContact").addClass("active");
        }
        else if (document.URL.indexOf("SalesRequest_List.aspx") != -1 || document.URL.indexOf("SalesRequest_Detail.aspx") != -1) {
            $("#liSales").addClass("active");
        }
        else if (document.URL.indexOf("PartnerTicket_List.aspx") != -1 || document.URL.indexOf("PartnerTicket_Edit.aspx") != -1) {
            $("#liPartnerTicket").addClass("active");
        }
    }
    else if (document.URL.indexOf("DailyPart_List.aspx") != -1 || document.URL.indexOf("Part_List.aspx") != -1 || document.URL.indexOf("Part_AddEdit.aspx") != -1 || document.URL.indexOf("UnitType_List.aspx") != -1 || document.URL.indexOf("UnitType_AddEdit.aspx") != -1 || document.URL.indexOf("EmployeePartRequest_List.aspx") != -1 || document.URL.indexOf("EmployeePartRequest_AddEdit.aspx") != -1 || document.URL.indexOf("EmployeePart_List.aspx") != -1) {
        $("#liInventory").addClass("active");
        if (document.URL.indexOf("DailyPart_List.aspx") != -1) {
            $("#liDailyPart").addClass("active");
        }
        else if (document.URL.indexOf("EmployeePart_List.aspx") != -1) {
            $("#liEmpPartList").addClass("active");
        }
        else if (document.URL.indexOf("Part_List.aspx") != -1 || document.URL.indexOf("Part_AddEdit.aspx") != -1) {
            $("#liParts").addClass("active");
        }
        else if (document.URL.indexOf("UnitType_List.aspx") != -1 || document.URL.indexOf("UnitType_AddEdit.aspx") != -1) {
            $("#liUnitParts").addClass("active");
        }
        else if (document.URL.indexOf("EmployeePartRequest_List.aspx") != -1 || document.URL.indexOf("EmployeePartRequest_AddEdit.aspx") != -1) {
            $("#liEmpPartReq").addClass("active");
        }
    }
    else if (document.URL.indexOf("Block_List.aspx") != -1 || document.URL.indexOf("Block_AddEdit.aspx") != -1 || document.URL.indexOf("CMSPages_List.aspx") != -1 || document.URL.indexOf("CMSPages_AddEdit.aspx") != -1 || document.URL.indexOf("CMSPagesMobile_List.aspx") != -1 || document.URL.indexOf("CMSPagesMobile_Edit.aspx") != -1) {
        $("#liCMS").addClass("active");
        if (document.URL.indexOf("Block_List.aspx") != -1 || document.URL.indexOf("Block_AddEdit.aspx") != -1) {
            $("#liBlock").addClass("active");
        }
        else if (document.URL.indexOf("CMSPages_List.aspx") != -1 || document.URL.indexOf("CMSPages_AddEdit.aspx") != -1) {
            $("#liPages").addClass("active");
        }
        else if (document.URL.indexOf("CMSPagesMobile_List.aspx") != -1 || document.URL.indexOf("CMSPagesMobile_Edit.aspx") != -1) {
            $("#liMobileScreen").addClass("active");
        }
    }
    else if (document.URL.indexOf("sales_report.aspx") != -1 || document.URL.indexOf("SalesPerDay.aspx") != -1 || document.URL.indexOf("unit_report.aspx") != -1 || document.URL.indexOf("rating_report.aspx") != -1 || document.URL.indexOf("lowstock_report.aspx") != -1 || document.URL.indexOf("inventory_report.aspx") != -1 || document.URL.indexOf("partner_report.aspx") != -1 || document.URL.indexOf("recurring_report.aspx") != -1) {
        $("#liReports1").addClass("active");
        if (document.URL.indexOf("sales_report.aspx") != -1 || document.URL.indexOf("SalesPerDay.aspx") != -1) {
            $("#liSalesReport").addClass("active");
        }
        else if (document.URL.indexOf("unit_report.aspx") != -1) {
            $("#liUnitReport").addClass("active");
        }
        else if (document.URL.indexOf("rating_report.aspx") != -1) {
            $("#liRatingReport").addClass("active");
        }
        else if (document.URL.indexOf("lowstock_report.aspx") != -1) {
            $("#liLowStockReport").addClass("active");
        }
        else if (document.URL.indexOf("inventory_report.aspx") != -1) {
            $("#liInventoryReport").addClass("active");
        }
        else if (document.URL.indexOf("partner_report.aspx") != -1) {
            $("#liPartnerReport").addClass("active");
        }
        else if (document.URL.indexOf("recurring_report.aspx") != -1) {
            $("#liRecurringReport").addClass("active");
        }
    }
    else if (document.URL.indexOf("sales_report.aspx") != -1 || document.URL.indexOf("lowstock_report.aspx") != -1 || document.URL.indexOf("recurring_report.aspx") != -1 || document.URL.indexOf("partner_report.aspx") != -1 || document.URL.indexOf("inventory_report.aspx") != -1 || document.URL.indexOf("unit_report.aspx") != -1 || document.URL.indexOf("rating_report.aspx") != -1) {
        $("#liReports1").addClass("active"); 
        if (document.URL.indexOf("sales_report.aspx") != -1) {
            $("#liSalesReport").addClass("active");
        }
        else if (document.URL.indexOf("unit_report.aspx") != -1) {
            $("#liUnitReport").addClass("active");
        }
        else if (document.URL.indexOf("rating_report.aspx") != -1) {
            $("#liRatingReport").addClass("active");
        }
        else if (document.URL.indexOf("lowstock_report.aspx") != -1) {
            $("#liLowStockReport").addClass("active");
        }
        else if (document.URL.indexOf("inventory_report.aspx") != -1) {
            $("#liInventoryReport").addClass("active");
        }
        else if (document.URL.indexOf("partner_report.aspx") != -1) {
            $("#liPartnerReport").addClass("active");
        }
        else if (document.URL.indexOf("recurring_report.aspx") != -1) {
            $("#liRecurringReport").addClass("active");
        }
        
    }
    else if (document.URL.indexOf("EmailTemplate_List.aspx") != -1 || document.URL.indexOf("EmailTemplate_Edit.aspx") != -1 ||
                document.URL.indexOf("Notification_List.aspx") != -1 || document.URL.indexOf("Notification_Edit.aspx") != -1 ||
                document.URL.indexOf("sitesetting_list.aspx") != -1 || document.URL.indexOf("sitesettingEdit.aspx") != -1 ||
                document.URL.indexOf("send_mail.aspx") != -1) {
        $("#liOthers").addClass("active");
        if (document.URL.indexOf("EmailTemplate_List.aspx") != -1 || document.URL.indexOf("EmailTemplate_Edit.aspx") != -1) {
            $("#liEmailTemplate").addClass("active");
        }
        else if (document.URL.indexOf("Notification_List.aspx") != -1 || document.URL.indexOf("Notification_Edit.aspx") != -1) {
            $("#liNotification").addClass("active");
        }
        else if (document.URL.indexOf("sitesetting_list.aspx") != -1 || document.URL.indexOf("sitesettingEdit.aspx") != -1) {
            $("#liSitesettingList").addClass("active");
        }
        else if (document.URL.indexOf("send_mail.aspx") != -1) {
            $("#liSendMail").addClass("active");
        }
    }
    else if (document.URL.indexOf("News_List.aspx") != -1 || document.URL.indexOf("News_AddEdit.aspx") != -1) {
        $("#liCMS").addClass("active");
        $("#liNews").addClass("active");
    }
}
function sidemenuheight() {
    var max = 1;
    $("#container .equal-height").each(function () {
        var i = $(this).index(i);
        var height1 = $(this).height();
        max = height1 > max ? height1 : max;
        //alert(max);
    })
    //$("#container .equal-height").css("min-height", "100px");
    $("#container .equal-height").css("min-height", max);
}
function validateLogin() {
    var error = 0;
    //ckeck if inputs aren't empty        

    var value = jQuery('#txtUsername').val();
    var password = jQuery('#txtPassword').val();
    if (value.length < 1 || value == 'Username') {
        jQuery('#dvUserName').addClass('error');
        jQuery('#dvUserName').removeClass('success');
        error++;
    }
    else {
        jQuery('#dvUserName').removeClass('error');
        jQuery('#dvUserName').addClass('success');
    }

    if (password.length < 4 || password == 'Password') {
        jQuery('#dvPassword').addClass('error');
        jQuery('#dvPassword').removeClass('success');
        error++;
    }
    else {
        jQuery('#dvPassword').removeClass('error');
        jQuery('#dvPassword').addClass('success');
    }

    if (!error) {
        return true;
    }
    else {
        return false;
    }
}
function validateForgotPassword() {
    var error = 0;
    var email = jQuery('#txtEmail').val();
    if (email.length < 1 || email == 'Email') {
        jQuery('#dvEmail').addClass('error');
        jQuery('#dvEmail').removeClass('success');
        error++;
    }
    else {
        jQuery('#dvEmail').removeClass('error');
        jQuery('#dvEmail').addClass('success');
    }
    if (!error) {
        return true;
    }
    else {
        return false;
    }
}
function checkDelete(confirmationmessage, selectionmessage) {
    j = 0;
    var text = '';
    for (i = 0; i < document.getElementById('aspNetForm').length; i++) {
        e = document.getElementById('aspNetForm').elements[i];
        if (e.type == 'checkbox' && e.name != 'chkCheckAll' && e.checked) {
            j++;
        }
    }
    if (j > 0) {
        if (text == '') {
            rtn = confirm(confirmationmessage);
        }
        else {
            rtn = confirm(text);
        }
        if (rtn == false) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        alert(selectionmessage);
        return false;
    }
}
function checkActive(confirmationmessage, selectionmessage) {
    j = 0;
    var text = '';
    for (i = 0; i < document.getElementById('aspNetForm').length; i++) {
        e = document.getElementById('aspNetForm').elements[i];
        if (e.type == 'checkbox' && e.name != 'chkCheckAll' && e.checked) {
            j++;
        }
    }
    if (j > 0) {

        if (text == '') {
            rtn = confirm(confirmationmessage);
        }
        else {
            rtn = confirm(text);
        }
        if (rtn == false) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        alert(selectionmessage);
        return false;
    }
}
function checkInactive(confirmationmessage, selectionmessage) {
    j = 0;
    var text = '';
    for (i = 0; i < document.getElementById('aspNetForm').length; i++) {
        e = document.getElementById('aspNetForm').elements[i];
        if (e.type == 'checkbox' && e.name != 'chkCheckAll' && e.checked) {
            j++;
        }
    }
    if (j > 0) {
        if (text == '') {
            rtn = confirm(confirmationmessage);
        }
        else {
            rtn = confirm(text);
        }
        if (rtn == false) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        alert(selectionmessage);
        return false;
    }
}