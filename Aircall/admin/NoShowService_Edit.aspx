<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="NoShowService_Edit.aspx.cs" Inherits="Aircall.admin.NoShowService_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">No Show Details</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/NoShowService_List.aspx">No Show Service List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">No Show Details</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>No Show Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name<span class="required">*</span></label>
                                <div class="controls">
                                    <%--<asp:TextBox ID="txtClient" runat="server"></asp:TextBox>--%>
                                    <asp:Label ID="lblClient" runat="server"></asp:Label>
                                </div>
                            </div>
                            <%--<asp:UpdatePanel ID="UPClients" runat="server" ClientIDMode="Static">
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
                                    </script>--%>
                            <div class="control-group">
                                <label class="control-label">Client Addresses</label>
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
                                <label class="control-label">Purpose Of Visit</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPurpose" runat="server" Enabled="false" ClientIDMode="Static"></asp:DropDownList>
                                </div>
                            </div>
                            <div id="dvLateReschedule" runat="server">
                                <div class="control-group">
                                    <label class="control-label">Work Area</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtWorkArea" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvWorkArea" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="AreaSearchGroup" CssClass="error_required" ControlToValidate="txtWorkArea"></asp:RequiredFieldValidator>
                                        <asp:LinkButton ID="lnkAreaSearch" runat="server" CssClass="btn btn-success" ValidationGroup="AreaSearchGroup" OnClick="lnkAreaSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
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
                                <%-- </ContentTemplate>
                            </asp:UpdatePanel>--%>
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
                                    <%--<div class="controls input-icon">
                                        <div class="input-append bootstrap-timepicker-component">
                                            <input id="txtStart" runat="server" class="span5 timepicker-default" type="text" /><span class="add-on"><i
                                                class="icon-time"></i></span>
                                            <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtStart" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>--%>
                                    <div class="controls">
                                        <asp:Literal ID="ltrStart" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Schedule End Time<span class="required">*</span></label>
                                    <%--<div class="controls input-icon">
                                        <div class="input-append bootstrap-timepicker-component">
                                            <input id="txtEnd" runat="server" class="span5 timepicker-default" type="text" /><span class="add-on"><i
                                                class="icon-time"></i></span>
                                            <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEnd" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>--%>
                                    <div class="controls">
                                        <asp:Literal ID="ltrEnd" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Service Status</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpServiceStatus" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">No Show Employee Reason</label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtNoShowEmpReason" runat="server" CssClass="input-xxlarge" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <asp:HiddenField ID="hdnReportCnt" runat="server" />
                            <asp:HiddenField ID="hdnStatus" runat="server" />
                            <div class="control-group">
                                <label class="control-label">No Show Amount</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNoShowAmount" runat="server" CssClass="input-small"></asp:TextBox>$
                                    <asp:RequiredFieldValidator ID="rqfvNoShowAmount" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtNoShowAmount" ValidationGroup="ReasonGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpAmount" runat="server" ErrorMessage="Invalid amount" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtNoShowAmount" ValidationGroup="ReasonGroup" ValidationExpression="^\d{1,9}(\.\d+)?$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnMakePayment" Text="Notify For No Show Payment" CssClass="btn btn-primary" ValidationGroup="ReasonGroup" runat="server" OnClick="btnMakePayment_Click" />
                                <asp:Button ID="btnNotifyAndSchedule" Text="Notify and Schedule Service" CssClass="btn btn-success" runat="server" OnClick="btnNotifyAndSchedule_Click" />
                                <asp:Button ID="btnScheduleWithOutNotify" Text="Schedule Service Without Notify" CssClass="btn btn-success" runat="server" OnClick="btnScheduleWithOutNotify_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'NoShowService_List.aspx'" />
                            </div>
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
            SetDateForService();
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
                }
                else if ($("#drpPurpose").val() == 'Emergency') {
                    $(".date-picker").datepicker({
                        startDate: '+0d',
                        autoclose: true,
                        orientation: "bottom right"
                    });
                }
                else {
                    $(".date-picker").datepicker({
                        startDate: '+<%= Aircall.Common.General.GetSitesettingsValue("MaintenanceServicesWithinDays") %>d',
                        autoclose: true,
                        orientation: "bottom right",
                        daysOfWeekDisabled: [0, 6]
                    });
                    }
            }
        });
    </script>
</asp:Content>
