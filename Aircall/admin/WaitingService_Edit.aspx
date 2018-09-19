<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="WaitingService_Edit.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="Aircall.admin.WaitingService_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        var i = 1;
        function FillTimeSlot() {
            var ServiceTimeSlot = $('#hdnServiceTime').val();
            var SlotArr = ServiceTimeSlot.split("|");
            $('.text-toggle-time').toggleButtons('destroy');
            
            if (i==0) {
                $("#chkRequestedTime").wrap("<div class='text-toggle-time'></div>");
            }
            $('.text-toggle-time').toggleButtons({
                width: 300,
                label: {
                    enabled: SlotArr[0],
                    disabled: SlotArr[1]
                }
            });
            i = 1;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Waiting For Approval Service</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/WaitingService_List.aspx">Waiting For Approval List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Edit Waiting For Approval Service</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>Waiting Approval Service Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name</label>
                                <div class="controls">
                                    <%--<asp:TextBox ID="txtClient" runat="server"></asp:TextBox>--%>
                                    <asp:Literal ID="ltrClient" runat="server"></asp:Literal>
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
                                        <asp:CheckBoxList ID="chkUnits" runat="server" CssClass="checker"></asp:CheckBoxList>
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
                                    <asp:DropDownList ID="drpPurpose" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Work Area</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtWorkArea" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvWorkArea" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="AreaSearchGroup" CssClass="error_required" ControlToValidate="txtWorkArea"></asp:RequiredFieldValidator>
                                    <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" ValidationGroup="AreaSearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                    <asp:Panel ID="PWorkArea" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                        <asp:RadioButtonList ID="rblWorkArea" runat="server" CssClass="checker">
                                        </asp:RadioButtonList>
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Technician</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmployee" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmployee" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="EmployeeSearchGroup" CssClass="error_required" ControlToValidate="txtEmployee"></asp:RequiredFieldValidator>
                                    <asp:LinkButton ID="lnkEmpSearch" runat="server" CssClass="btn btn-success" ValidationGroup="EmployeeSearchGroup" OnClick="lnkEmpSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                    <%--<asp:HiddenField ID="hdnEmployeeId" runat="server" />--%>
                                </div>
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
                            <div class="control-group">
                                <label class="control-label">Service Schedule On</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtScheduleOn" runat="server" CssClass="date-picker"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvScheduleOn" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtScheduleOn" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Schedule Start Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtStart" runat="server" class="span7 timepicker-default" type="text" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtStart" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Schedule End Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtEnd" runat="server" class="span7 timepicker-default" type="text" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEnd" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
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
                                <asp:Button ID="btnApprove" Text="Approve & Schedule" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnApprove_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'WaitingService_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>Reschedule Service</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Scheduled Service Date</label>
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
                                    <asp:TextBox ID="txtReschedule" runat="server" CssClass="date-picker"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReschedule" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtReschedule" ValidationGroup="RescheduleGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Preferred Reschedule Time</label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPTimeSlot" runat="server">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                Sys.Application.add_load(FillTimeSlot);
                                            </script>
                                            <asp:HiddenField ID="hdnServiceTime" ClientIDMode="Static" runat="server" />
                                            <asp:HiddenField ID="hdnWeekEndTimeSlot" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="hdnWeekTimeSlot" runat="server" ClientIDMode="Static" />
                                            <div class="text-toggle-time">
                                                <input type="checkbox" class="toggle" clientidmode="static" id="chkRequestedTime" runat="server"  checked="checked"/>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                             <div class="control-group">
                                <label class="control-label">Reschedule/Cancel Reason</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtReason" runat="server" CssClass="input-large" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtReason" ValidationGroup="RescheduleGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <asp:HiddenField ID="hdnPurposeOfVisit" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="hdnMaintenanceServicesWithinDays" runat="server" ClientIDMode="Static"/>
                            <asp:HiddenField ID="hdnEmergencyAndOtherServiceWithinDays" runat="server" ClientIDMode="Static"/>
                            <div class="form-actions">
                                <asp:Button ID="btnReschedule" Text="Reschedule Service" CssClass="btn btn-success" runat="server" ValidationGroup="RescheduleGroup" OnClick="btnReschedule_Click"/>
                                <asp:Button ID="btnCancel" Text="Cancel Request" CssClass="btn btn-primary" runat="server" OnClick="btnCancel_Click" />
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
                                        <label>On: <%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></label>
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
            var MaintenanceServicesWithinDays = $("#hdnMaintenanceServicesWithinDays").val();
            var EmergencyAndOtherServiceWithinDays = $("#hdnEmergencyAndOtherServiceWithinDays").val();
            var PurposeOfVisit = $("#hdnPurposeOfVisit").val();
            if (PurposeOfVisit == 'Continuing Previous Work' || PurposeOfVisit == 'Repair') {
                $(".date-picker").datepicker({
                    startDate: '+' + EmergencyAndOtherServiceWithinDays + 'd',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });
            }
            else if (PurposeOfVisit == 'Emergency') {
                $(".date-picker").datepicker({
                    startDate: '+0d',
                    autoclose: true,
                    orientation: "bottom right"
                }).on('changeDate', function (ev) {
                    if (ev.date.getDay() == 0 || ev.date.getDay()==6) {
                        $("#hdnServiceTime").val($("#hdnWeekEndTimeSlot").val());
                    }
                    else {
                        $("#hdnServiceTime").val($("#hdnWeekTimeSlot").val());
                    }
                    i = 0;
                    FillTimeSlot();
                });
            }
            else {
                $(".date-picker").datepicker({
                    startDate: '+' + MaintenanceServicesWithinDays + 'd',
                    autoclose: true,
                    orientation: "bottom right",
                    daysOfWeekDisabled: [0, 6]
                });
            }
        });

    </script>
</asp:Content>
