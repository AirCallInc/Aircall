<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceApproval.aspx.cs" Inherits="Aircall.ServiceApproval" %>

<%@ Register Src="~/controls/mobileheadernew.ascx" TagName="mobileheader" TagPrefix="uc1" %>
<%@ Register Src="~/controls/header.ascx" TagName="header" TagPrefix="uc2" %>
<%@ Register Src="~/controls/footer.ascx" TagName="footer" TagPrefix="uc3" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8">
    <title><%=Page.Title %></title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
    <!--<meta name="viewport" content="width=1024"/>-->
    <meta name="description" content="" />
    <meta name="keywords" content="">
    <meta name="format-detection" content="telephone=no">
    <meta name="SKYPE_TOOLBAR" content="SKYPE_TOOLBAR_PARSER_COMPATIBLE" />
    <link rel="icon" type="image/png" href="images/favicon.png" />
    <!--<link rel="icon" type="image/png" href="images/favicon.png" /> -->
    <!-- main css -->
    <link href="css/style.css" rel="stylesheet" />
    <!-- responsive css -->
    <link href="css/responsive.css" rel="stylesheet" />
    <!-- Font css -->
    <link href="fonts/fonts.css" rel="stylesheet" />
    <link href='http://fonts.googleapis.com/css?family=Lato:400,900italic,900,100italic,300italic,400italic,700italic,700,300,100' rel='stylesheet' type='text/css'>
    <link href='https://fonts.googleapis.com/css?family=Roboto:400,100,100italic,300,300italic,500italic,700,900italic,900,700italic,500,400italic' rel='stylesheet' type='text/css'>
    <!--[if IE]>
     	<script src="js/html5shiv.js"></script>
    <![endif]-->
    <!-- main script -->
    <script src="js/jquery-1.9.1.min.js"></script>
    <!-- placeholder script -->
    <script src="js/placeholder.js"></script>
    <!-- jquery-ui script -->
    <script src="js/jquery-ui.js"></script>
    <link href="css/jquery-ui.css" rel="stylesheet" />
    <!-- owl.carousel script -->
    <script src="js/owl.carousel.js"></script>
    <link href="css/owl.carousel.css" rel="stylesheet" />
    <!-- general script -->
    <script src="js/script.js"></script>
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
            vertical-align: middle;
            margin-left: 5px;
        }
    </style>
    <script>
        function pageLoad() {
            $(".checkbox-outer, .radio-outer, .radio-outer-dot").buttonset();
            var firstslotunits = $("#firstslotunits").val();
            var secondslotunits = $("#secondslotunits").val();
            if (parseInt($("#hdnUnitCnt").val()) > secondslotunits) {
                $('#rdslot2').attr("disabled", "disabled");
                $('#rdslot1').prop('checked', true);
                $('#rdslot2').prop('checked', false);
                $(".radio-outer").buttonset("refresh");
                //alert("More hours required to perform the service. Morning timeslot must be picked.");
                $("#dvMessage").html("<strong>More hours required to perform the service. Morning timeslot must be picked.</strong>");
                $("#dvMessage").addClass("error");
            } else {
                $('#rdslot2').removeAttr("disabled");
                $(".radio-outer").buttonset("refresh");
                $("#dvMessage").removeClass("error");
                $("#dvMessage").html("");
            }

            $(".datepicker1").datepicker("destroy");
            if ($("#drpPurposeOfVisit").val() == 'Continuing Previous Work' || $("#drpPurposeOfVisit").val() == 'Emergency' || $("#drpPurposeOfVisit").val() == 'Repairing') {
                $(".datepicker1").datepicker({
                    beforeShowDay: noSunday,
                    minDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>'
                });
            } else {
                $(".datepicker1").datepicker({
                    beforeShowDay: noSunday,
                    minDate: '+<%= Aircall.Common.General.GetSitesettingsValue("MaintenanceServicesWithinDays") %>'
                });
            }
            $(".datepicker1").datepicker("refresh");
            function noSunday(date) {
                return [(date.getDay() != 0 && date.getDay() != 6), ''];
            };
        }
        $(document).ready(function () {
            pageLoad();
        });
        function check24HoursForReschedule() {
            if ($("#is24Hrs").val().toLowerCase() == "true") {
                if (confirm($("#LateRescheduleDisplayMessage").val())) {
                    return true;
                }
            } else {
                return true;
            }
            return false;
        }
        function check24HoursForCancel() {
            if ($("#is24Hrs").val().toLowerCase() == "true") {
                if (confirm($("#LateCancelDisplayMessage").val())) {
                    return true;
                }
            } else {
                return true;
            }
            return false;
        }
    </script>
</head>
<body>
    <form runat="server">
        <asp:HiddenField ID="hdnEmployeeId" ClientIDMode="Static" runat="server" />
        <asp:HiddenField ID="hdnServiceId" ClientIDMode="Static" runat="server" />
        <asp:HiddenField ID="hdnUnitCnt" ClientIDMode="Static" runat="server" />
        <asp:HiddenField ID="hdnDate" ClientIDMode="Static" runat="server" />
        <asp:HiddenField ID="is24Hrs" ClientIDMode="Static" runat="server" />
        <asp:HiddenField ID="LateRescheduleDisplayMessage" ClientIDMode="Static" runat="server" />
        <asp:HiddenField ID="LateCancelDisplayMessage" ClientIDMode="Static" runat="server" />
        <asp:HiddenField ID="drpPurposeOfVisit" ClientIDMode="Static" runat="server" />

        <div id="wrapper">
            <uc2:header ID="Header" runat="server" />
            <!-- content area part -->
            <div id="content-area">
                <div class="common-section">
                    <div class="container">
                        <div class="title">
                            <h1>Service Schedule Detail</h1>
                        </div>
                        <div class="border-block">
                            <div class="main-from">
                                <div id="dvService" runat="server">
                                    <div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Service Case # :</label>
                                            </div>
                                            <div class="right-side">
                                                <p>
                                                    <asp:Literal ID="ltrServiceNo" runat="server"></asp:Literal>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Address :</label>
                                            </div>
                                            <div class="right-side">
                                                <p>
                                                    <asp:Literal ID="ltrAddress" runat="server"></asp:Literal>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Service Person :</label>
                                            </div>
                                            <div class="right-side">
                                                <p style="display: inline-block;">
                                                    <asp:Literal ID="ltrEmployee" runat="server"></asp:Literal>
                                                </p>
                                                <asp:Image ID="imgEmp" CssClass="empPhoto" runat="server" />
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Service Date :</label>
                                            </div>
                                            <div class="right-side">
                                                <p>
                                                    <asp:Literal ID="ltrDate" runat="server"></asp:Literal>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Service Time :</label>
                                            </div>
                                            <div class="right-side">
                                                <p>
                                                    <asp:Literal ID="ltrTime" runat="server"></asp:Literal>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="single-row cf" runat="server" id="dvUnit1">
                                            <div class="left-side">
                                                <label>Unit will be serviced :</label>
                                            </div>
                                            <div class="right-side">
                                                <p>
                                                    <asp:Literal ID="ltrUnits" runat="server"></asp:Literal>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Reschedule Service Date</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="datepicker-outer max290">
                                                    <asp:TextBox ID="txtReschedule" runat="server" CssClass="datepicker1"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvRescheduleDate" CssClass="error" runat="server" ControlToValidate="txtReschedule" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgReschedule"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row schedule-time cf">
                                            <div class="left-side">
                                                <label>Schedule Time</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="radio-outer max380">
                                                    <asp:RadioButton ID="rdslot1" ClientIDMode="Static" runat="server" GroupName="radio" /><label for="rdslot1"><asp:Literal ID="ltrSlot1" runat="server"></asp:Literal></label>
                                                    <asp:RadioButton ID="rdslot2" ClientIDMode="Static" runat="server" GroupName="radio" Checked="true" /><label for="rdslot2"><asp:Literal ID="ltrSlot2" runat="server"></asp:Literal></label>
                                                    <asp:HiddenField ID="hdnTimeSlot" runat="server" />
                                                    <asp:HiddenField ID="firstslotunits" ClientIDMode="Static" runat="server" />
                                                    <asp:HiddenField ID="secondslotunits" ClientIDMode="Static" runat="server" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label>Reason For Reschedule</label>
                                            </div>
                                            <div class="right-side">
                                                <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvReason" CssClass="error" runat="server" ControlToValidate="txtReason" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgReschedule"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="single-row button-bar cf">
                                            <asp:Button ID="btnSave" Text="Approve" CssClass="main-btn" runat="server" OnClick="btnSave_Click" />
                                            <asp:Button ID="btnReschedule" OnClientClick="return check24HoursForReschedule();" Text="Reschedule" CssClass="main-btn dark-grey" runat="server" ValidationGroup="vgReschedule" OnClick="btnReschedule_Click" />
                                            <asp:Button ID="btnCancel" runat="server" OnClientClick="return check24HoursForCancel();" CssClass="main-btn" Text="Cancel" OnClick="btnCancel_Click" />
                                        </div>
                                    </div>
                                </div>
                                <div id="dvExpire" runat="server" visible="false">
                                    <div class="container">
                                        <h3 style="text-align: center; line-height: 30px;"><%=Aircall.Common.General.GetSitesettingsValue("ServiceApproveLinkExpireMsg") %></h3>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="footer-push"></div>
        </div>
        <uc3:footer ID="Footer" runat="server" />
    </form>
</body>
</html>
