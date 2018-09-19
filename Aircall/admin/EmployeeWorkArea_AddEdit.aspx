<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeWorkArea_AddEdit.aspx.cs" Inherits="Aircall.admin.EmployeeWorkArea_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">EmployeeWorkArea Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/EmployeeWorkArea_List.aspx">EmployeeWorkArea List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">EmployeeWorkArea Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-map-marker"></i>Employee WorkArea Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Employee Name<span class="required">*</span></label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="Panel1" DefaultButton="lnkSearch" CssClass="controls" runat="server">
                                    <asp:TextBox ID="txtEmpName" runat="server" CssClass="input-large"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvEmpName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="SearchGroup" CssClass="error_required" ControlToValidate="txtEmpName"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" ValidationGroup="SearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
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
                                            if (test1 = $("#UPEmployee input[type=checkbox]:not(.toggle)")) {
                                                test1.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label">&nbsp;</label>
                                        <div class="controls">
                                            <%--<asp:UpdatePanel ID="UPEmployee" runat="server" ClientIDMode="Static">
                                        <ContentTemplate>--%>
                                            <asp:Panel ID="Panel2" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblEmployee" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                                <asp:HiddenField ID="hdnEmployee" runat="server" />
                                            </asp:Panel>
                                            <%--</ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="lnkSearch" />
                                        </Triggers>
                                    </asp:UpdatePanel>--%>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">1st Priority Area<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:Panel ID="Priority1Panel" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:CheckBoxList ID="chkPriority1" runat="server" AutoPostBack="true" OnSelectedIndexChanged="chkPriority1_SelectedIndexChanged"></asp:CheckBoxList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">2nd Priority Area<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:Panel ID="Priority2Panel" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:CheckBoxList ID="chkPriority2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="chkPriority2_SelectedIndexChanged"></asp:CheckBoxList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" UseSubmitBehavior="false" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'EmployeeWorkArea_List.aspx'" />
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
                if ($get('<%=Priority1Panel.ClientID%>') != null) {
                    // Get X and Y positions of scrollbar before the partial postback
                    xPos = $get('<%=Priority1Panel.ClientID%>').scrollLeft;
                    yPos = $get('<%=Priority1Panel.ClientID%>').scrollTop;
                }
            } catch (e) {

            }
            try {
                if ($get('<%=Priority2Panel.ClientID%>') != null) {
                    // Get X and Y positions of scrollbar before the partial postback
                    xPos1 = $get('<%=Priority2Panel.ClientID%>').scrollLeft;
                    yPos1 = $get('<%=Priority2Panel.ClientID%>').scrollTop;
                }
            } catch (e) {

            }
        }

        function EndRequestHandler(sender, args) {
            if ($get('<%=Priority1Panel.ClientID%>') != null) {
                // Set X and Y positions back to the scrollbar
                // after partial postback
                try {
                    $get('<%=Priority1Panel.ClientID%>').scrollLeft = xPos;
                  $get('<%=Priority1Panel.ClientID%>').scrollTop = yPos;
              } catch (e) {

              }
          }
          if ($get('<%=Priority2Panel.ClientID%>') != null) {
                // Set X and Y positions back to the scrollbar
                // after partial postback
                try {
                    $get('<%=Priority2Panel.ClientID%>').scrollLeft = xPos1;
                  $get('<%=Priority2Panel.ClientID%>').scrollTop = yPos1;
              } catch (e) {

              }
          }
      }

      prm.add_beginRequest(BeginRequestHandler);
      prm.add_endRequest(EndRequestHandler);
    </script>
</asp:Content>
