<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeLeave_AddEdit.aspx.cs" Inherits="Aircall.admin.EmployeeLeave_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Employee Leave Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/EmployeeLeave_List.aspx">Employee Leave List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Add/Edit Employee Leave</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-calendar"></i>Employee Leave Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Employee Name</label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLEmployee" runat="server" DefaultButton="lnkEmpSearch" CssClass="controls">
                                    <asp:TextBox ID="txtEmployee" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvEmployee" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="EmployeeSearchGroup" CssClass="error_required" ControlToValidate="txtEmployee"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkEmpSearch" runat="server" CssClass="btn btn-success" ValidationGroup="EmployeeSearchGroup" OnClick="lnkEmpSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                                <%--</div>--%>
                            </div>
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
                                            $('.date-picker').datepicker({
                                                autoclose: true,
                                                orientation: "bottom right"
                                            }).on('changeDate', function (ev) {
                                                $(this).datepicker('hide');
                                                $("#txtStart").change();
                                            });
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PEmployee" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblEmployee" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group" style="margin-top: 20px;">
                                        <label class="control-label">Leave Date From&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtStart" runat="server" ClientIDMode="Static" CssClass="date-picker"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtStart" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="control-group" style="margin-top: 20px;">
                                        <label class="control-label">Leave Date To&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtEnd" runat="server" CssClass="date-picker"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEnd" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkEmpSearch" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="txtStart" EventName="TextChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="control-group">
                                <label class="control-label">Available on holiday for emergency services</label>
                                <div class="controls">
                                    <div class="text-toggle-button1">
                                        <input type="checkbox" class="toggle" id="chkAvailable" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Reason&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtReason" runat="server" CssClass="span6" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReason" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtReason" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnAdd" UseSubmitBehavior="false" Text="Add Leave" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'EmployeeLeave_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.date-picker').datepicker({
                autoclose: true,
                orientation: "bottom right"
            }).on('changeDate', function (ev) {
                $(this).datepicker('hide');
                $("#txtStart").change();
            });
        });
    </script>
</asp:Content>
