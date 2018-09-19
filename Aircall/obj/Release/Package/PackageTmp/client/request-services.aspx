<%@ Page Title="Add/Edit Request Service" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="request-services.aspx.cs" Inherits="Aircall.client.request_services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .ui-dialog .ui-dialog-content {
        }
    </style>
    <link href="<%=Application["SiteAddress"]%>client/css/jquery.timepicker.css" rel="stylesheet" />
    <script>
        $(document).ready(function () {
            pageLoad();
        });
        function emergencyConfirmation() {
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

            if ($("#drpPurposeOfVisit").val() == '0') {
                $("#spnAlert").text("<%= HttpUtility.HtmlDecode(Aircall.Common.General.GetSitesettingsValue("RepairServiceSubmitMessage")) %>");
                $("#dialog-confirm").dialog({
                    resizable: false,
                    height: "auto",
                    width: dialogWidth,
                    modal: true,
                    buttons: {
                        "Yes": function () {
                            $(this).dialog("close");
                            $("#btnSubmit").click();
                            return true;
                        },
                        "No": function () {
                            $(this).dialog("close");
                            return false;
                        }
                    }
                });
            } else if ($("#drpPurposeOfVisit").val() == '1') {
                $("#spnAlert").text("<%= HttpUtility.HtmlDecode(Aircall.Common.General.GetSitesettingsValue("EmergencyServiceSubmitMessage")) %>");
                $("#dialog-confirm").dialog({
                    resizable: false,
                    height: "auto",
                    width: dialogWidth,
                    modal: true,
                    buttons: {
                        "Agree": function () {
                            $(this).dialog("close");
                            $("#btnSubmit").click();
                            return true;
                        },
                        "Do not agree": function () {
                            $(this).dialog("close");
                            return false;
                        }
                    }
                });
            }
            else if ($("#drpPurposeOfVisit").val() == '2') {
                $("#spnAlert").text("<%= HttpUtility.HtmlDecode(Aircall.Common.General.GetSitesettingsValue("ContinuingPreviousWorkServiceSubmitMessage")) %>");
                $("#dialog-confirm").dialog({
                    resizable: false,
                    height: "auto",
                    width: dialogWidth,
                    modal: true,
                    buttons: {
                        "Yes": function () {
                            $(this).dialog("close");
                            $("#btnSubmit").click();
                            return true;
                        },
                        "No": function () {
                            $(this).dialog("close");
                            return false;
                        }
                    }
                });
            }
            else if ($("#drpPurposeOfVisit").val() == '3') {
                $("#spnAlert").text("<%= HttpUtility.HtmlDecode(Aircall.Common.General.GetSitesettingsValue("MaintenanceServiceSubmitMessage")) %>");
                    $("#dialog-confirm").dialog({
                        resizable: false,
                        height: "auto",
                        width: dialogWidth,
                        modal: true,
                        buttons: {
                            "Yes": function () {
                                $(this).dialog("close");
                                $("#btnSubmit").click();
                                return true;
                            },
                            "No": function () {
                                $(this).dialog("close");
                                return false;
                            }
                        }
                    });
                }
    return false;
}

function pageLoad() {
    $("#dvEmergency").hide();
    $(".select-outer select").selectmenu({
        select: function (event, ui) {
            $(this).trigger("change");
        }
    });
    $(".datepicker1").datepicker({
        beforeShowDay: noSunday,
        minDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>'
    });
    $("#drpPurposeOfVisit").on("change", function () {
        if ($("#drpPurposeOfVisit").val() == '0' || $("#drpPurposeOfVisit").val() == '2') {
            $(".datepicker1").val("");
            $(".datepicker1").datepicker("destroy");
            $(".datepicker1").datepicker({
                beforeShowDay: noSunday,
                minDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>'
            });
            $("#dvRequestedTime").show();
            $("#dvEmergency").hide();
        } else if ($("#drpPurposeOfVisit").val() == '1') {
            //$(".datepicker1").val("");
            $(".datepicker1").datepicker("destroy");
            $("#dvRequestedTime").hide();
            $("#dvEmergency").show();
            $(".datepicker1").datepicker({
                minDate: '0',
                onSelect: function (dateText) {
                    console.log("Selected date: " + dateText + "; input's current value: " + this.value);
                    var date3 = $(this).datepicker('getDate');
                    if (date3.getDay() == 0 || date3.getDay() == 6) {
                        $("#dvRequestedTime").show();
                        $("#dvEmergency").hide();
                    }
                    else {
                        $("#dvRequestedTime").hide();
                        $("#dvEmergency").show();
                    }
                }
            });
        } else {
            $(".datepicker1").val("");
            $(".datepicker1").datepicker("destroy");
            $("#dvRequestedTime").show();
            $("#dvEmergency").hide();
            $(".datepicker1").datepicker({
                beforeShowDay: noSunday,
                minDate: '+<%= Aircall.Common.General.GetSitesettingsValue("MaintenanceServicesWithinDays") %>'
            });
        }
        $(".datepicker1").datepicker("refresh");
    });
    function noSunday(date) {
        return [(date.getDay() != 0 && date.getDay() != 6), ''];
    };
    $(".checkbox-outer, .radio-outer, .radio-outer-dot").buttonset();
    $("#chkUnits input").on("click", function () {
        var firstslotunits = $("#firstslotunits").val();
        var secondslotunits = $("#secondslotunits").val();
        if ($("#chkUnits input:checked").length > secondslotunits) {
            if ($("#drpPurposeOfVisit").val() == '1') {
                $('#rdslot2E').attr("disabled", "disabled");
                $('#rdslot1E').prop('checked', true);
                $('#rdslot2E').prop('checked', false);
                $("#dvEmergency .radio-outer").buttonset("refresh");
            }
            else {
                $('#rdslot2').attr("disabled", "disabled");
                $('#rdslot1').prop('checked', true);
                $('#rdslot2').prop('checked', false);
                $("#dvRequestedTime .radio-outer").buttonset("refresh");
            }

            //alert("More hours required to perform the service. Morning timeslot must be picked.");
            $("#dvMessage").html("<strong>More hours required to perform the service. Morning timeslot must be picked.</strong>");
            $("#dvMessage").addClass("error");
        } else {
            if ($("#drpPurposeOfVisit").val() == '1') {
                $('#rdslot2E').removeAttr("disabled");
                $("#dvEmergency .radio-outer").buttonset("refresh");
            }
            else {
                $('#rdslot2').removeAttr("disabled");
                $("#dvRequestedTime .radio-outer").buttonset("refresh");
            }

            $("#dvMessage").removeClass("error");
            $("#dvMessage").html("");
        }
    });
}
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dialog-confirm" title="Aircall System" style="display: none;">
        <p>
            <span id="spnAlert"></span>
        </p>
    </div>
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Request Service Add / Edit</h1>
                </div>
                <div class="border-block">
                    <div id="dvMessage" runat="server" clientidmode="static">
                    </div>
                    <div class="main-from">
                        <div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Select Address</label>
                                </div>
                                <div class="right-side">
                                    <div class="select-outer">
                                        <asp:DropDownList ID="drpClientAddress" CssClass="selectaddress" runat="server" AutoPostBack="true" ClientIDMode="Static" OnSelectedIndexChanged="drpClientAddress_SelectedIndexChanged"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvAddress" CssClass="error" runat="server" ControlToValidate="drpClientAddress" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSubmit" InitialValue="0"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row schedule-time cf">
                                <div class="left-side">
                                    <label>Plan</label>
                                </div>
                                <div class="right-side">
                                    <div class="max290">
                                        <asp:UpdatePanel ID="UpdatePanel2" ClientIDMode="Static" runat="server">
                                            <ContentTemplate>
                                                <asp:RadioButtonList ID="rblPlan" runat="server" OnSelectedIndexChanged="rblPlan_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:RadioButtonList>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="drpClientAddress" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Select Unit</label>
                                </div>
                                <div class="right-side">
                                    <div class="checkbox-outer">
                                        <asp:UpdatePanel ID="UPUnits" ClientIDMode="Static" runat="server">
                                            <ContentTemplate>
                                                <script type="text/javascript">
                                                    function jScriptmsg() {
                                                        $(".checkbox-outer").buttonset();
                                                    }
                                                    Sys.Application.add_load(jScriptmsg);
                                                </script>
                                                <asp:CheckBoxList ID="chkUnits" ClientIDMode="Static" RepeatColumns="2" runat="server"></asp:CheckBoxList>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="drpClientAddress" EventName="SelectedIndexChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="rblPlan" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Request For</label>
                                </div>
                                <div class="right-side">
                                    <div class="select-outer max290">
                                        <asp:DropDownList ID="drpPurposeOfVisit" ClientIDMode="Static" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Schedule Date</label>
                                </div>
                                <div class="right-side">
                                    <div class="datepicker-outer max290">
                                        <input type="text" id="txtDate" runat="server" readonly="readonly" autocomplete="off" class="datepicker1">
                                        <asp:RequiredFieldValidator ID="rfvScheduleDate" CssClass="error" runat="server" ControlToValidate="txtDate" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSubmit"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel1" class="single-row schedule-time cf" ClientIDMode="Static" runat="server">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            $("#dvRequestedTime .radio-outer").buttonset();
                                            $("#dvEmergencyTime .radio-outer").buttonset();
                                            setTimeout(function () { $("#drpPurposeOfVisit").change(); }, 100);
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div id="dvRequestedTime" runat="server" clientidmode="Static" visible="false">
                                        <div class="left-side">
                                            <label>Schedule Time</label>
                                        </div>
                                        <div class="right-side">
                                            <div class="radio-outer max360">

                                                <asp:RadioButton ID="rdslot1" ClientIDMode="Static" runat="server" GroupName="radio" /><label for="rdslot1"><asp:Literal ID="ltrSlot1" runat="server"></asp:Literal></label>
                                                <asp:RadioButton ID="rdslot2" ClientIDMode="Static" runat="server" GroupName="radio" Checked="true" /><label for="rdslot2"><asp:Literal ID="ltrSlot2" runat="server"></asp:Literal></label>
                                                <asp:HiddenField ID="hdnTimeSlot" runat="server" />
                                                <asp:HiddenField ID="hdnTimeSlotE" runat="server" />
                                                <asp:HiddenField ID="firstslotunits" ClientIDMode="Static" runat="server" />
                                                <asp:HiddenField ID="secondslotunits" ClientIDMode="Static" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                    <div id="dvEmergency" runat="server" clientidmode="Static">
                                        <div class="left-side">
                                            <label>Schedule Time</label>
                                        </div>
                                        <div class="right-side">
                                            <div class="radio-outer max360">
                                                <asp:RadioButton ID="rdslot1E" ClientIDMode="Static" runat="server" GroupName="radioe" /><label for="rdslot1E"><asp:Literal ID="ltrSlot1E" runat="server"></asp:Literal></label>
                                                <asp:RadioButton ID="rdslot2E" ClientIDMode="Static" runat="server" GroupName="radioe" Checked="true" /><label for="rdslot2E"><asp:Literal ID="ltrSlot2E" runat="server"></asp:Literal></label>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="rblPlan" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Notes</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtNotes" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="single-row button-bar no-border cf">
                                <a class="main-btn" href="javascript:;" onclick="emergencyConfirmation();">Submit request</a>
                                <asp:Button CssClass="main-btn" runat="server" ID="btnSubmit" ClientIDMode="Static" ValidationGroup="vgSubmit" Style="display: none;" Text="Submit request" OnClick="btnSubmit_Click"></asp:Button>
                                <a class="main-btn dark-grey" href="request-service-list.aspx">View past requests</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="HiddenField3" runat="server" />
    <script src="<%=Application["SiteAddress"]%>client/js/jquery.timepicker.js"></script>
</asp:Content>
