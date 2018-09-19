<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="RequestService_AddEdit.aspx.cs" Inherits="Aircall.admin.RequestService_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        var i = 1;
        $(document).ready(function () {

            $('.date-picker').datepicker({
                startDate: '+2d',
                autoclose: true,
                orientation: "bottom right",
                daysOfWeekDisabled: [0, 6]
            });
        });

        function checkRequestedUnits() {
            var firstslotunits = $("#firstslotunits").val();
            var secondslotunits = $("#secondslotunits").val();
            var morning = $(".toggle").attr("checked");
            if (morning) {
                $("#dvMsg").removeClass("alert alert-error");
                $("#dvMsg").html("");
                $("#dvMsg").hide();
                $("#btnSave").attr('disabled', false);

            }
            else {
                if ($("#chkUnits input:checked").length > secondslotunits) {
                    $("#dvMsg").html("<strong>More hours required to perform the service. Morning timeslot must be picked.</strong>");
                    $("#dvMsg").addClass("alert alert-error");
                    $("#dvMsg").show();
                    $("#btnSave").attr('disabled', true);
                } else {
                    $("#dvMsg").removeClass("alert alert-error");
                    $("#dvMsg").html("");
                    $("#dvMsg").hide();
                    $("#btnSave").attr('disabled', false);
                }
            }
        }

        function FillTimeSlot() {
            var ServiceTimeSlot = $('#hdnServiceTime').val();
            var SlotArr = ServiceTimeSlot.split("|");
            $('.text-toggle-time').toggleButtons('destroy');

            if (i == 0) {
                $("#chkRequestedTime").wrap("<div class='text-toggle-time'></div>");
            }
            $('.text-toggle-time').toggleButtons({
                width: 300,
                label: {
                    enabled: SlotArr[0],
                    disabled: SlotArr[1]
                }
            });

            $(".toggle").on("change", function () {
                checkRequestedUnits();
            });
            i = 1;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Request Service Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/PendingService_List.aspx">Pending Service List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Request Service Add/Edit</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMsg" style="display:none;"></div>
                        <div id="dvMessage" runat="server" visible="false">
                            <div class="clear">
                                <!-- -->
                            </div>
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>Request Service Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">

                            <div class="control-group">
                                <label class="control-label">Client Name<span class="required">*</span></label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLClient" runat="server" DefaultButton="lnkSearch" CssClass="controls">
                                    <asp:TextBox ID="txtClient" runat="server" CssClass="input-large"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvClient" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="SearchGroup" CssClass="error_required" ControlToValidate="txtClient"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" ValidationGroup="SearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
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

                                            $("#chkUnits input").on("change", function () {
                                                checkRequestedUnits();
                                            });
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="UPClient" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblClient" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="rblClient_SelectedIndexChanged">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Unit Located Address</label>
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
                                        <label class="control-label">Select Plan</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpPlanType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpPlanType_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvPlanType" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpPlanType" InitialValue="0"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Service Unit Names</label>
                                        <div class="controls">
                                            <asp:Panel ID="UPUnits" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:CheckBoxList ID="chkUnits" runat="server" CssClass="checker" ClientIDMode="Static"></asp:CheckBoxList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="control-group">
                                <label class="control-label">Request For</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPurpose" runat="server" ClientIDMode="Static"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Requested On</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtServiceRequested" runat="server" CssClass="date-picker" ClientIDMode="Static"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvRequested" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtServiceRequested"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="control-group" id="dvRequestedTime">
                                <label class="control-label">
                                    Requested Time
                                </label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPTimeSlot" runat="server">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                Sys.Application.add_load(FillTimeSlot);
                                            </script>
                                            <div class="text-toggle-time">
                                                <input type="checkbox" class="toggle" id="chkRequestedTime" runat="server" clientidmode="static" />
                                            </div>
                                            <asp:HiddenField ID="hdnServiceTime" ClientIDMode="Static" runat="server" />
                                            <asp:HiddenField ID="firstslotunits" ClientIDMode="Static" runat="server" />
                                            <asp:HiddenField ID="secondslotunits" ClientIDMode="Static" runat="server" />
                                            <asp:HiddenField ID="hdnWeekEndTimeSlot" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="hdnWeekTimeSlot" runat="server" ClientIDMode="Static" />
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpPlanType" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>

                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Notes</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNotes" runat="server" CssClass="input-xxlarge" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <asp:HiddenField ID="hdnMaintenanceServicesWithinDays" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="hdnEmergencyAndOtherServiceWithinDays" runat="server" ClientIDMode="Static" />
                            <div class="form-actions">
                                <asp:Button ID="btnSave" UseSubmitBehavior="false" ClientIDMode="Static" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'PendingService_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            SetDateForService();
            $("#drpPurpose").on("change", function () {
                SetDateForService();
                if ($("#drpPurpose").val() == '1') {
                    $("#hdnServiceTime").val($("#hdnWeekTimeSlot").val());
                }
                else {
                    $("#hdnServiceTime").val($("#hdnWeekEndTimeSlot").val());
                }
                i = 0;
                FillTimeSlot();
            });
        });

        function SetDateForService() {
            var MaintenanceServicesWithinDays = $("#hdnMaintenanceServicesWithinDays").val();
            var EmergencyAndOtherServiceWithinDays = $("#hdnEmergencyAndOtherServiceWithinDays").val();
            $(".date-picker").datepicker("destroy");
            $("#txtServiceRequested").val('');
            if ($("#drpPurpose").val() == '2' || $("#drpPurpose").val() == '0') {

                $(".date-picker").datepicker({
                    startDate: '+' + EmergencyAndOtherServiceWithinDays + 'd',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                }).on('changeDate', function (ev) {
                    SetServiceTime(ev);
                });
            }
            else if ($("#drpPurpose").val() == '1') {
                $(".date-picker").datepicker({
                    startDate: '+0d',
                    autoclose: true,
                    orientation: "bottom right"
                }).on('changeDate', function (ev) {
                    SetServiceTime(ev);
                });
            }
            else {
                $(".date-picker").datepicker({
                    startDate: '+' + MaintenanceServicesWithinDays + 'd',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                }).on('changeDate', function (ev) {
                    SetServiceTime(ev);
                });
            }
        }

        function SetServiceTime(ev) {
            if ($("#drpPurpose").val() == '1') {
                if (ev.date.getDay() == 0 || ev.date.getDay() == 6) {
                    $("#hdnServiceTime").val($("#hdnWeekEndTimeSlot").val());
                }
                else {
                    $("#hdnServiceTime").val($("#hdnWeekTimeSlot").val());
                }
            }
            i = 0;
            FillTimeSlot();
        }
    </script>
</asp:Content>
