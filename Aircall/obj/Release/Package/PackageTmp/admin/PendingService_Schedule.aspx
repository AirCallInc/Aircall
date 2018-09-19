<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="PendingService_Schedule.aspx.cs" Inherits="Aircall.admin.PendingService_Schedule" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Pending Service Schedule</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/PendingService_List.aspx">Pending Service List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Edit Pending Service</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>Pending Service Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtClient" runat="server"></asp:TextBox>
                                    <asp:HiddenField ID="hdnClient" runat="server" />
                                    <asp:HiddenField ID="hdnServiceDayGap" runat="server" />
                                    <%--<asp:HiddenField ID="hdnServiceCaseNo" runat="server" />--%>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                    <asp:Panel ID="PAddress" runat="server" CssClass="span10 scrollingControlContainer checkboxPanel">
                                        <asp:RadioButtonList ID="rblAddress" runat="server" CssClass="checker">
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
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Requested On</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtServiceRequested" runat="server" CssClass="date-picker"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Purpose Of Visit</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPurpose" ClientIDMode="Static" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <asp:UpdatePanel ID="UPWorkArea" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPWorkArea input[type=radio]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label">Work Area</label>
                                        <asp:Panel ID="pnlWorkArea" CssClass="controls" DefaultButton="lnkSearch" runat="server">
                                            <asp:TextBox ID="txtWorkArea" runat="server"></asp:TextBox>
                                            <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" Visible="false" ValidationGroup="AreaSearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                        </asp:Panel>
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
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel ID="UPEmployee" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPEmployee input[type=radio]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label">Technician</label>
                                        <%--<div class="controls">--%>
                                        <asp:Panel ID="PNLEmployee" DefaultButton="lnkEmpSearch" runat="server" CssClass="controls">
                                            <asp:TextBox ID="txtEmployee" runat="server"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator ID="rqfvEmployee" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="EmployeeSearchGroup" CssClass="error_required" ControlToValidate="txtEmployee"></asp:RequiredFieldValidator>--%>
                                            <asp:LinkButton ID="lnkEmpSearch" runat="server" CssClass="btn btn-success" ValidationGroup="EmployeeSearchGroup" OnClick="lnkEmpSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                        </asp:Panel>

                                        <%--<asp:HiddenField ID="hdnEmployeeId" runat="server" />--%>
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
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkEmpSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="control-group">
                                <label class="control-label">Service Schedule On</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtScheduleOn" runat="server" CssClass="date-picker1" ClientIDMode="Static"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvScheduleOn" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtScheduleOn" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Schedule Start Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtStart" runat="server" class="span7 timepicker-default" type="text" clientidmode="static" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtStart" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Schedule End Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtEnd" runat="server" class="span7 timepicker-default" type="text" clientidmode="static" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEnd" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
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
                            <asp:HiddenField ID="hdnDrivetime" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="hdnServiceTimeForFirstUnit" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="hdnServiceTimeForAdditionalUnits" runat="server" ClientIDMode="Static" />
                            <div class="form-actions">
                                <asp:Button ID="btnSchedule" UseSubmitBehavior="false" Text="Schedule Service" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSchedule_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'PendingService_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid" id="dvAttempt" runat="server">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-warning-sign"></i>Schedule Attempt Logs</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body" style="background-color: #eed3d7 !important; color: #b94a48 !important;">
                        <div class="form-horizontal">
                            <asp:ListView ID="lstAttempt" runat="server">
                                <ItemTemplate>
                                    <div class="control-group">
                                        <label class="control-label span2">Attempt <%# Container.DataItemIndex + 1 %></label>
                                        <label class="control-label span6"><%#Eval("AttemtFailReason") %></label>
                                        <label class="control-label span4"><%#DateTime.Parse(Eval("AttemptDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></label>
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid" id="dvReschedule" runat="server">
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
            $('.date-picker').datepicker({
                startDate: new Date(),
                autoclose: true,
                orientation: "bottom right",
                daysOfWeekDisabled: [0, 6]
            });

            $("#txtStart").on("change", function () {
                if ($("#txtStart").val() != '') {
                    getServiceEndTime();
                }
            });

            $("#chkUnits input").on("click", function () {
                if ($("#txtStart").val() != '') {
                    getServiceEndTime();
                }
            });

            SetDateForService();
            $("#drpPurpose").on("change", function () {
                $("#txtScheduleOn").val('');
                SetDateForService();
            });
        });
        function SetDateForService() {            
            $(".date-picker1").datepicker("destroy");
            if ($("#drpPurpose").val() == 'Continuing Previous Work' || $("#drpPurpose").val() == 'Repair') {
                $(".date-picker1").datepicker({
                    startDate: '+<%= Aircall.Common.General.GetSitesettingsValue("EmergencyAndOtherServiceWithinDays") %>d',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });
            }
            else if ($("#drpPurpose").val() == 'Emergency') {
                $(".date-picker1").datepicker({
                    startDate: '+0d',
                    autoclose: true,
                    orientation: "bottom right"
                });
            }
            else {
                $(".date-picker1").datepicker({
                    startDate: '+<%= Aircall.Common.General.GetSitesettingsValue("MaintenanceServicesWithinDays") %>d',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });
            }
        }
        function getServiceEndTime() {
            var unitCount = $("#chkUnits input:checked").length;
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
    </script>
</asp:Content>
