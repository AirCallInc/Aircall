<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ScheduledService_AddEdit.aspx.cs" Inherits="Aircall.admin.ScheduledService_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Service Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/ScheduledService_List.aspx">Schedule Services List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Service Add/Edit</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMessage" runat="server" visible="false">
                            <div class="clear">
                                <!-- -->
                            </div>
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>Service Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name<span class="required">*</span></label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLClient" runat="server" DefaultButton="lnkSearchClient" CssClass="controls">
                                    <asp:TextBox ID="txtClient" runat="server"></asp:TextBox>
                                    <asp:Literal ID="ltrClient" runat="server"></asp:Literal>
                                    <%--<asp:RequiredFieldValidator ID="rqfvClient" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="SearchGroup" CssClass="error_required" ControlToValidate="txtClient"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkSearchClient" runat="server" CssClass="btn btn-success" ValidationGroup="SearchGroup" OnClick="lnkSearchClient_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                    <asp:HiddenField ID="hdnClientName" runat="server" />
                                </asp:Panel>

                                <%--</div>--%>
                            </div>
                            <asp:UpdatePanel ID="UPClients" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPClients input[type=radio]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                            if (test = $("#UPClients input[type=checkbox]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label">Client</label>
                                        <div class="controls">
                                            <asp:Panel ID="UPClient" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblClient" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="rblClient_SelectedIndexChanged">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Client Addresses</label>
                                        <div class="controls">
                                            <asp:Panel ID="PAddress" runat="server" CssClass="span10 scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblAddress" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="rblAddress_SelectedIndexChanged">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Mobile</label>
                                        <div class="controls">
                                            <label class="control-label">
                                                <asp:Literal ID="ltrMobile" runat="server"></asp:Literal></label>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Home</label>
                                        <div class="controls">
                                            <label class="control-label">
                                                <asp:Literal ID="ltrHome" runat="server"></asp:Literal></label>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Office</label>
                                        <div class="controls">
                                            <label class="control-label">
                                                <asp:Literal ID="ltrOffice" runat="server"></asp:Literal></label>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Unit Names</label>
                                        <div class="controls">
                                            <asp:Panel ID="Panel1" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:CheckBoxList ID="chkUnits" runat="server" CssClass="checker" ClientIDMode="Static"></asp:CheckBoxList>
                                                <asp:HiddenField ID="hdnUnitCnt" ClientIDMode="Static" runat="server" />
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="control-group">
                                        <label class="control-label">Purpose Of Visit</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpPurpose" ClientIDMode="Static" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Work Area</label>
                                        <%--<div class="controls">--%>
                                        <asp:Panel ID="PNLWorkArea" runat="server" DefaultButton="lnkAreaSearch" CssClass="controls">
                                            <asp:TextBox ID="txtWorkArea" runat="server"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator ID="rqfvWorkArea" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="AreaSearchGroup" CssClass="error_required" ControlToValidate="txtWorkArea"></asp:RequiredFieldValidator>--%>
                                            <asp:LinkButton ID="lnkAreaSearch" runat="server" CssClass="btn btn-success" ValidationGroup="AreaSearchGroup" Visible="false" OnClick="lnkAreaSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                            <asp:HiddenField ID="hdnWorkAreaId" runat="server" />
                                        </asp:Panel>
                                        <%--</div>--%>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PWorkArea" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblWorkArea" runat="server" CssClass="checker" OnSelectedIndexChanged="rblWorkArea_SelectedIndexChanged" AutoPostBack="true">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Technician</label>
                                        <%--<div class="controls">--%>
                                        <asp:Panel ID="PNLEmployee" DefaultButton="lnkEmpSearch" runat="server" CssClass="controls">
                                            <asp:TextBox ID="txtEmployee" runat="server"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator ID="rqfvEmployee" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="EmployeeSearchGroup" CssClass="error_required" ControlToValidate="txtEmployee"></asp:RequiredFieldValidator>--%>
                                            <asp:LinkButton ID="lnkEmpSearch" runat="server" CssClass="btn btn-success" ValidationGroup="EmployeeSearchGroup" OnClick="lnkEmpSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                            <asp:HiddenField ID="hdnEmployeeId" runat="server" />
                                        </asp:Panel>
                                        <%--</div>--%>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PEmployee" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblEmployee" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdnDrivetime" runat="server" ClientIDMode="Static" />
                            		<asp:HiddenField ID="hdnServiceTimeForFirstUnit" runat="server" ClientIDMode="Static" />
                            		<asp:HiddenField ID="hdnServiceTimeForAdditionalUnits" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkSearchClient" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="control-group">
                                <label class="control-label">Service Schedule On</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtScheduleOn" runat="server" CssClass="date-picker"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvScheduleOn" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtScheduleOn" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:HiddenField ID="hdnScheduleOn" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Schedule Start Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtStart" runat="server" class="span6 timepicker-default" type="text" clientidmode="static" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtStart" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        <asp:HiddenField ID="hdnStart" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Schedule End Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtEnd" runat="server" class="span6 timepicker-default" type="text" clientidmode="static" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEnd" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        <asp:HiddenField ID="hdnEnd" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <%--<div class="control-group">
                                <label class="control-label">Service Status</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpServiceStatus" runat="server"></asp:DropDownList>
                                </div>
                            </div>--%>
                            <%--<div class="control-group">
                                <label class="control-label">Reschedule Reason</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtRescheduleReason" runat="server" CssClass="input-xxlarge" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReason" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtRescheduleReason" ValidationGroup="ReasonGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>--%>
                            <div class="control-group">
                                <label class="control-label">Customer Complaints</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCustomerNote" runat="server" CssClass="input-xxlarge" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Dispatcher Notes</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtDispatcherNote" runat="server" CssClass="input-xxlarge" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Technician Notes</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmpNote" runat="server" CssClass="input-xxlarge" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Part Lists</label>
                                <div class="controls">
                                    <asp:ListView ID="lstUnits" runat="server" OnItemDataBound="lstUnits_ItemDataBound">
                                        <ItemTemplate>
                                            <b>Unit Name: <%#Eval("UnitName") %></b>
                                            <asp:HiddenField ID="hdnUnitId" runat="server" Value='<%#Eval("UnitId") %>' />
                                            <br />
                                            <asp:ListView ID="lstUnitParts" runat="server">
                                                <ItemTemplate>
                                                    <%#Eval("Name") %> - <%#Eval("Size") %>
                                                    <br />
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Report History</label>
                                <div class="controls">
                                    <asp:ListView ID="lstServicereport" runat="server">
                                        <ItemTemplate>
                                            <a href="<%=Application["SiteAddress"]%>admin/ServiceReport_View.aspx?ReportId=<%#Eval("Id") %>"><%#Eval("ServiceReportNumber") %></a><br />
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSchedule" UseSubmitBehavior="false" Text="Schedule Service" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSchedule_Click" />
                                <asp:Button ID="btnAssignEmployee" Text="Assign Another Employee" CssClass="btn btn-success" runat="server" OnClick="btnAssignEmployee_Click" Visible="false" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'ScheduledService_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid" id="dvReschedule" runat="server">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMsg" style="display: none;"></div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>Reschedule Service</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Service Schedule Date</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrScheduleDate" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Schedule Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrScheduleTime" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Reschedule Date</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtReschedule" runat="server" CssClass="reschedule-date-picker"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReschedule" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtReschedule" ValidationGroup="RescheduleGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Preferred Reschedule Time</label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPTimeSlot" runat="server">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                function FillTimeSlot() {
                                                    var ServiceTimeSlot = $('#hdnServiceTime').val();
                                                    var SlotArr = ServiceTimeSlot.split("|");
                                                    $('.text-toggle-time').toggleButtons({
                                                        width: 300,
                                                        label: {
                                                            enabled: SlotArr[0],
                                                            disabled: SlotArr[1]
                                                        }
                                                    });
                                                    $(".toggle").change(function () {
                                                        checkRequestedUnits();
                                                    });
                                                }
                                                Sys.Application.add_load(FillTimeSlot);
                                            </script>
                                            <asp:HiddenField ID="hdnServiceTime" ClientIDMode="Static" runat="server" />
                                            <div class="text-toggle-time">
                                                <input type="checkbox" class="toggle" id="chkRequestedTime" runat="server" checked="checked" />
                                            </div>
                                            <asp:HiddenField ID="firstslotunits" ClientIDMode="Static" runat="server" />
                                            <asp:HiddenField ID="secondslotunits" ClientIDMode="Static" runat="server" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Reschedule Reason</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtReason" runat="server" CssClass="input-large" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReason" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtReason" ValidationGroup="RescheduleGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <asp:HiddenField ID="hdnPurposeOfVisit" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="hdnMaintenanceServicesWithinDays" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="hdnEmergencyAndOtherServiceWithinDays" runat="server" ClientIDMode="Static" />
                            <div class="form-actions">
                                <asp:Button ID="btnReschedule" Text="Reschedule Service" ClientIDMode="Static" CssClass="btn btn-success" runat="server" ValidationGroup="RescheduleGroup" OnClick="btnReschedule_Click" />
                                <asp:Button ID="btnCancel" Text="Cancel Request" CssClass="btn btn-primary" runat="server" OnClick="btnCancel_Click"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid" id="dvRescheduleAttempt" runat="server">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-warning-sign"></i>Reschedule Service Logs</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal">
                            <asp:ListView ID="lstReschedule" runat="server">
                                <ItemTemplate>
                                    <div class="control-group">
                                        <label>On:  <%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></label>
                                        <label>Added By:  <%#Eval("AddedByDesc") %> (<%# Eval("AddedByName") %>)</label>
                                        <label>Attempted to reschedule. This was attempt #: <%# Container.DataItemIndex + 1 %></label>
                                        <label>Reschedule was for: <%#Eval("RescheduleDate","{0:MM/dd/yyyy}") %></label>
                                        <label>Time:  <%#Eval("Rescheduletime") %></label>
                                        <label>Reason provided:  <%#Eval("Reason") %></label>                                        
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {

            //var MaintenanceServicesWithinDays = $("#hdnMaintenanceServicesWithinDays").val();
            //var EmergencyAndOtherServiceWithinDays = $("#hdnEmergencyAndOtherServiceWithinDays").val();
            //var PurposeOfVisit = $("#hdnPurposeOfVisit").val();
            //if (PurposeOfVisit == 'Continuing Previous Work' || PurposeOfVisit == 'Emergency' || PurposeOfVisit == 'Repairing') {
            //    $(".reschedule-date-picker").datepicker({
            //        startDate: '+' + EmergencyAndOtherServiceWithinDays + 'd',
            //        autoclose: true,
            //        orientation: "bottom right",
            //        daysOfWeekDisabled: [0, 6]
            //    });
            //} else {
            //    $(".reschedule-date-picker").datepicker({
            //        startDate: '+' + MaintenanceServicesWithinDays + 'd',
            //        autoclose: true,
            //        orientation: "bottom right",
            //        daysOfWeekDisabled: [0, 6]
            //    });
            //}
            checkRequestedUnits();

        });
        $(window).load(function () {
            // code here
            BindEvents();
        });
        function BindEvents() {
            $("#chkUnits input").on("change", function () {
                checkRequestedUnits();
                if ($("#txtStart").val() != '') {
                    getServiceEndTime();
                }
            });
            SetDateForService();
            $("#drpPurpose").on("change", function () {
                SetDateForService();
            });

            $("#txtStart").on("change", function () {
                if ($("#txtStart").val() != '') {
                    getServiceEndTime();
                }
            });
        }
        function SetDateForService() {
            $(".date-picker").datepicker("destroy");
            $("#txtServiceRequested").val('');
            if ($("#drpPurpose").val() == 'Continuing Previous Work' || $("#drpPurpose").val() == 'Repair') {
                $(".date-picker").datepicker({
                    startDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>d',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });

                $(".reschedule-date-picker").datepicker({
                    startDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>d',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });
            }
            else if ($("#drpPurpose").val() == 'Emergency') {
                $(".date-picker").datepicker({
                    startDate: '+0d',
                    autoclose: true,
                    orientation: "bottom right"
                });

                $(".reschedule-date-picker").datepicker({
                    startDate: '+0d',
                    autoclose: true,
                    orientation: "bottom right",
                });
            }
            else {
                $(".date-picker").datepicker({
                    startDate: '+<%= Aircall.Common.General.GetSitesettingsValue("MaintenanceServicesWithinDays") %>d',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });

                $(".reschedule-date-picker").datepicker({
                    startDate: '+<%= Aircall.Common.General.GetSitesettingsValue("MaintenanceServicesWithinDays") %>d',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });
            }
        }
        function checkRequestedUnits() {
            var firstslotunits = $("#firstslotunits").val();
            var secondslotunits = $("#secondslotunits").val();
            var morning = $(".toggle").attr("checked");
            if (morning) {
                $("#dvMsg").removeClass("alert alert-error");
                $("#dvMsg").html("");
                $("#dvMsg").hide();
                $("#btnReschedule").attr('disabled', false);

            }
            else {
                if (parseInt($("#hdnUnitCnt").val()) > parseInt(secondslotunits)) {
                    $("#dvMsg").html("<strong>More hours required to perform the service. Morning timeslot must be picked.</strong>");
                    $("#dvMsg").addClass("alert alert-error");
                    $("#dvMsg").show();
                    $("#btnReschedule").attr('disabled', true);
                } else {
                    $("#dvMsg").removeClass("alert alert-error");
                    $("#dvMsg").html("");
                    $("#dvMsg").hide();
                    $("#btnReschedule").attr('disabled', false);
                }
            }
        }
        function getServiceEndTime() {
            var unitCount = $("#chkUnits input:checked").length;
        if (unitCount > 0) {
            var Drivetime = $("#hdnDrivetime").val();
            var ServiceTimeForFirstUnit = $("#hdnServiceTimeForFirstUnit").val();
            var ServiceTimeForAdditionalUnits = $("#hdnServiceTimeForAdditionalUnits").val();
            var totalTime = 0;
            if (unitCount > 1) {
                totalTime = parseInt(Drivetime) + parseInt(ServiceTimeForFirstUnit) + parseInt((unitCount - 1) * parseInt(ServiceTimeForAdditionalUnits));
            }
            else {
                totalTime = parseInt(Drivetime) + parseInt(ServiceTimeForFirstUnit);
            }

            var d = Date.parseExact($("#txtStart").val(), "hh:mm tt");
            var newTime = d.addMinutes(totalTime)

            $("#txtEnd").val(newTime.toString('hh:mm tt'));
        }
    }
    </script>
</asp:Content>
