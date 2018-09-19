<%@ Page Title="Reschedule" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="reschedule.aspx.cs" Inherits="Aircall.client.reschedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }
    </style>
    <script>
        function pageLoad() {
            $(".checkbox-outer, .radio-outer, .radio-outer-dot").buttonset();
            var firstslotunits = $("#firstslotunits").val();
            var secondslotunits = $("#secondslotunits").val();
            if (parseInt($("#hdnUnitCnt").val()) > secondslotunits) {
                if ($("#drpPurposeOfVisit").val() == 'Emergency') {
                    $('#rdslot2E').attr("disabled", "disabled");
                    $('#rdslot1E').prop('checked', true);
                    $('#rdslot2E').prop('checked', false);
                    $("#dvSTE .radio-outer").buttonset("refresh");
                }
                else {
                    $('#rdslot2').attr("disabled", "disabled");
                    $('#rdslot1').prop('checked', true);
                    $('#rdslot2').prop('checked', false);
                    $("#dvST .radio-outer").buttonset("refresh");
                    //alert("More hours required to perform the service. Morning timeslot must be picked.");
                    $("#dvMessage").html("<strong>More hours required to perform the service. Morning timeslot must be picked.</strong>");
                    $("#dvMessage").addClass("error");
                }
            } else {
                if ($("#drpPurposeOfVisit").val() == 'Emergency') {
                    $('#rdslot2E').removeAttr("disabled");
                    $("#dvSTE .radio-outer").buttonset("refresh");
                }
                else {
                    $('#rdslot2').removeAttr("disabled");
                    $("#dvST .radio-outer").buttonset("refresh");
                }
                //$('#rdslot2').removeAttr("disabled");
                //$(".radio-outer").buttonset("refresh");
                $("#dvMessage").removeClass("error");
                $("#dvMessage").html("");
            }


            $(".datepicker1").datepicker("destroy");
            if ($("#drpPurposeOfVisit").val() == 'Continuing Previous Work' || $("#drpPurposeOfVisit").val() == 'Repair') {
                $(".datepicker1").datepicker({
                    beforeShowDay: noSunday,
                    minDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>'
                });
                $("#dvST").hide();
                $("#dvSTE").show();
            } else if ($("#drpPurposeOfVisit").val() == 'Emergency') {
                $("#dvST").hide();
                $("#dvSTE").show();
                $(".datepicker1").datepicker({
                    minDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>',
                    onSelect: function (dateText) {
                        console.log("Selected date: " + dateText + "; input's current value: " + this.value);
                        var date3 = $(this).datepicker('getDate');
                        if (date3.getDay() == 0 || date3.getDay() == 6) {
                            $("#dvST").show();
                            $("#dvSTE").hide();
                        }
                        else {
                            $("#dvST").hide();
                            $("#dvSTE").show();
                        }
                    }
                });
            } else {
                $("#dvST").hide();
                $("#dvSTE").show();
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
    function CheckHourDiff() {
        $("#dvMessage").removeClass("error");
        $("#dvMessage").html("");
        var screenWidth, screenHeight, dialogWidth, dialogHeight, isDesktop;

        screenWidth = window.screen.width;
        screenHeight = window.screen.height;

        if (screenWidth < 500) {
            dialogWidth = screenWidth * .95;
            dialogHeight = screenHeight * .95;
        } else {
            dialogWidth = 500;
            dialogHeight = 500;
            isDesktop = true;
        }

        var LateRescheduleHours = parseInt(<%= Aircall.Common.General.GetSitesettingsValue("LateRescheduleHours") %>);
        var HourDiff = parseInt($("#hdnHourDiff").val());
        if (HourDiff <= LateRescheduleHours) {
            $("#spnAlert").text("<%= Aircall.Common.General.GetSitesettingsValue("LateRescheduleDisplayMessage") %>");
            $("#dialog-confirm").dialog({
                resizable: false,
                height: "auto",
                width: dialogWidth,
                modal: true,
                buttons: {
                    "Yes": function () {
                        $(this).dialog("close");
                        $("#btnReschedule").click();
                        return true;
                    },
                    "No": function () {
                        $(this).dialog("close");
                        return false;
                    }
                }
            });
        } else {
            $("#btnReschedule").click();
        }
    }

    $(document).ready(function () {
        pageLoad();
    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dialog-confirm" title="Aircall System" style="display: none;">
        <p>
            <span id="spnAlert"></span>
        </p>
    </div>
    <asp:HiddenField ID="hdnUnitCnt" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdnHourDiff" ClientIDMode="Static" runat="server" />

    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>
                        <asp:Literal ID="ltrHeading" runat="server"></asp:Literal></h1>
                </div>
                <div class="border-block">
                    <div id="dvMessage" runat="server" clientidmode="static"></div>
                    <div id="dvCancel" runat="server" clientidmode="static"></div>
                    <div class="main-from">
                        <div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Service Case # : </label>
                                </div>
                                <div class="right-side">
                                    <p>
                                        <asp:Literal ID="ltrCaseNo" runat="server"></asp:Literal>
                                    </p>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Unit Name :</label>
                                </div>
                                <div class="right-side">
                                    <p>
                                        <asp:Literal ID="ltrUnits" runat="server"></asp:Literal>
                                    </p>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Service Type: </label>
                                </div>
                                <div class="right-side">
                                    <p>
                                        <asp:HiddenField ID="drpPurposeOfVisit" ClientIDMode="Static" runat="server" />
                                        <asp:Literal ID="ltrServiceType" runat="server"></asp:Literal>
                                    </p>
                                </div>
                            </div>
                            <div class="single-row technician cf">
                                <div class="left-side">
                                    <label>Technician: </label>
                                </div>
                                <div class="right-side">
                                    <asp:Literal ID="ltrEmployee" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hdnEmployeeId" runat="server" />
                                    <img title="" alt="" class="empPhoto" id="imgEmployee" runat="server" src="">
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Service Date: </label>
                                </div>
                                <div class="right-side">
                                    <p>
                                        <asp:Literal ID="ltrServiceDate" runat="server"></asp:Literal>
                                    </p>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Customer Complaint: </label>
                                </div>
                                <div class="right-side">
                                    <p>
                                        <asp:Literal ID="ltrCustomerComplaint" runat="server"></asp:Literal>
                                    </p>
                                </div>
                            </div>
                            <div class="single-row cf" runat="server" id="dvRSD">
                                <div class="left-side">
                                    <label>Reschedule Service Date</label>
                                </div>
                                <div class="right-side">
                                    <div class="datepicker-outer max290">
                                        <input type="text" id="txtReschedule" runat="server" readonly="readonly" autocomplete="off" class="datepicker1">
                                        <%--<asp:TextBox ID="txtReschedule" runat="server" CssClass="datepicker1" ReadOnly="true"></asp:TextBox>--%>
                                        <asp:RequiredFieldValidator ID="rfvRescheduleDate" CssClass="error" runat="server" ControlToValidate="txtReschedule" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgReschedule"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row schedule-time cf" runat="server" clientidmode="static" id="dvST">
                                <div class="left-side">
                                    <label>Schedule Time</label>
                                </div>
                                <div class="right-side">
                                    <div class="radio-outer max380">
                                        <asp:RadioButton ID="rdslot1" ClientIDMode="Static" runat="server" GroupName="radio" Checked="true" /><label for="rdslot1"><asp:Literal ID="ltrSlot1" runat="server"></asp:Literal></label>
                                        <asp:RadioButton ID="rdslot2" ClientIDMode="Static" runat="server" GroupName="radio" /><label for="rdslot2"><asp:Literal ID="ltrSlot2" runat="server"></asp:Literal></label>
                                        <asp:HiddenField ID="hdnTimeSlot" runat="server" />
                                        <asp:HiddenField ID="hdnTimeSlotE" runat="server" />
                                        <asp:HiddenField ID="firstslotunits" ClientIDMode="Static" runat="server" />
                                        <asp:HiddenField ID="secondslotunits" ClientIDMode="Static" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="single-row schedule-time cf" runat="server" clientidmode="static" id="dvSTE">
                                <div class="left-side">
                                    <label>Schedule Time</label>
                                </div>
                                <div class="right-side">
                                    <div class="radio-outer max380">
                                        <asp:RadioButton ID="rdslot1E" ClientIDMode="Static" runat="server" GroupName="radioe" /><label for="rdslot1E"><asp:Literal ID="ltrSlot1E" runat="server"></asp:Literal></label>
                                        <asp:RadioButton ID="rdslot2E" ClientIDMode="Static" runat="server" GroupName="radioe" Checked="true" /><label for="rdslot2E"><asp:Literal ID="ltrSlot2E" runat="server"></asp:Literal></label>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row cf" runat="server" id="dvR">
                                <div class="left-side">
                                    <label>
                                        <asp:Literal ID="ltrReson" runat="server"></asp:Literal></label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvReason" CssClass="error" runat="server" ControlToValidate="txtReason" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgReschedule"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="single-row button-bar no-border cf">
                                <a class="main-btn" href="javascript:;" runat="server" id="btnCan" onclick="CheckHourDiff();">Reschedule</a>
                                <asp:Button ID="btnReschedule" ClientIDMode="Static" runat="server" Style="display: none;" CssClass="main-btn" ValidationGroup="vgReschedule" Text="Reschedule" OnClick="btnReschedule_Click" />
                                <input type="button" class="main-btn dark-grey" value="Back To List" onclick="location.href = 'schedule.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
