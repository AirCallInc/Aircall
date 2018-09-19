<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ClientSendNotification.aspx.cs" Inherits="Aircall.admin.ClientSendNotification" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Send Notification To Client</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Send Notification To Client</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-bell"></i>Send Notification</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client</label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLClient" runat="server" CssClass="controls" DefaultButton="lnkClientSearch">
                                    <asp:TextBox ID="txtClientName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvClientName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ClientSearchGroup" CssClass="error_required" ControlToValidate="txtClientName"></asp:RequiredFieldValidator>
                                    <asp:LinkButton ID="lnkClientSearch" runat="server" CssClass="btn btn-success" ValidationGroup="ClientSearchGroup" OnClick="lnkClientSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                                <%--</div>--%>
                            </div>
                            <asp:UpdatePanel ID="UPClient" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPClient input[type=checkbox]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <div id="dvChkAll" runat="server">
                                                <label class="checkbox line">
                                                    <asp:CheckBox ID="chkAll" runat="server" CssClass="checker" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />Select All
                                                </label>
                                            </div>
                                            <asp:Panel ID="PClients" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:CheckBoxList ID="chkClients" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="chkClients_SelectedIndexChanged">
                                                </asp:CheckBoxList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkClientSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="control-group">
                                <label class="control-label">Notification Message&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMessage" runat="server" CssClass="input-xlarge" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvMessage" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="SendGroup" CssClass="error_required" ControlToValidate="txtMessage"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSend" UseSubmitBehavior="false" Text="Send" CssClass="btn btn-primary" ValidationGroup="SendGroup" runat="server" OnClick="btnSend_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'dashboard.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
