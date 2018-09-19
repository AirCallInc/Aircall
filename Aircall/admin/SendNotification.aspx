<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="SendNotification.aspx.cs" Inherits="Aircall.admin.SendNotification" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Send Notification</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Send Notification</a><span class="divider-last">&nbsp;</span></li>
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
                                <label class="control-label">Work Area</label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLWorkArea" runat="server" CssClass="controls" DefaultButton="lnkAreaSearch">
                                    <asp:TextBox ID="txtWorkArea" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvWorkArea" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="AreaSearchGroup" CssClass="error_required" ControlToValidate="txtWorkArea"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkAreaSearch" runat="server" CssClass="btn btn-success" ValidationGroup="AreaSearchGroup" OnClick="lnkAreaSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>

                                <%--</div>--%>
                            </div>
                            <asp:UpdatePanel ID="UPWorkArea" ClientIDMode="Static" runat="server">
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
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PWorkArea" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblWorkArea" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkAreaSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="control-group">
                                <label class="control-label">Employee</label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLEmployee" runat="server" CssClass="controls" DefaultButton="lnkEmpSearch">
                                    <asp:TextBox ID="txtEmployee" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvEmployee" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="EmployeeSearchGroup" CssClass="error_required" ControlToValidate="txtEmployee"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkEmpSearch" runat="server" CssClass="btn btn-success" ValidationGroup="EmployeeSearchGroup" OnClick="lnkEmpSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                                <%--</div>--%>
                            </div>
                            <asp:UpdatePanel ID="UPEmployee" ClientIDMode="Static" runat="server">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPEmployee input[type=checkbox]:not(.toggle)")) {
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
                                            <asp:Panel ID="PEmployee" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:CheckBoxList ID="chkEmployee" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="chkEmployee_SelectedIndexChanged">
                                                </asp:CheckBoxList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkEmpSearch" EventName="Click" />
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
    <script type="text/javascript">
        // It is important to place this JavaScript code after ScriptManager1
        var xPos, yPos, xPos1, yPos1;
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        function BeginRequestHandler(sender, args) {
            try {
                if ($get('<%=PWorkArea.ClientID%>') != null) {
                    // Get X and Y positions of scrollbar before the partial postback
                    xPos = $get('<%=PWorkArea.ClientID%>').scrollLeft;
                    yPos = $get('<%=PWorkArea.ClientID%>').scrollTop;
                }
            } catch (e) {

            }
            try {
                if ($get('<%=PEmployee.ClientID%>') != null) {
                    // Get X and Y positions of scrollbar before the partial postback
                    xPos1 = $get('<%=PEmployee.ClientID%>').scrollLeft;
                    yPos1 = $get('<%=PEmployee.ClientID%>').scrollTop;
                }
            } catch (e) {

            }
        }

        function EndRequestHandler(sender, args) {
            if ($get('<%=PWorkArea.ClientID%>') != null) {
                // Set X and Y positions back to the scrollbar
                // after partial postback
                try {
                    $get('<%=PWorkArea.ClientID%>').scrollLeft = xPos;
                  $get('<%=PWorkArea.ClientID%>').scrollTop = yPos;
              } catch (e) {

              }
          }
          if ($get('<%=PEmployee.ClientID%>') != null) {
                // Set X and Y positions back to the scrollbar
                // after partial postback
                try {
                    $get('<%=PEmployee.ClientID%>').scrollLeft = xPos1;
                  $get('<%=PEmployee.ClientID%>').scrollTop = yPos1;
              } catch (e) {

              }
          }
      }

      prm.add_beginRequest(BeginRequestHandler);
      prm.add_endRequest(EndRequestHandler);
    </script>
</asp:Content>
