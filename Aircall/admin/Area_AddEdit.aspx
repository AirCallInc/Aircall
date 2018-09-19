<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Area_AddEdit.aspx.cs" Inherits="Aircall.admin.Area_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Area Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Area_List.aspx">Area List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Area Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-map-marker"></i>Area Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Area Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtAreaName" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvAreaName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtAreaName"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">State Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpState" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpState" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">City Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="Updatepanel1" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpCity" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpCity_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpCity" InitialValue="0"></asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpState" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label">Zip Code<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPZipCode" runat="server" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                function jScriptmsg() {
                                                    if (!jQuery().uniform) {
                                                        return;
                                                    }
                                                    if (test = $("#UPZipCode input[type=checkbox]:not(.toggle)")) {
                                                        test.uniform();
                                                    }
                                                }
                                                Sys.Application.add_load(jScriptmsg);
                                            </script>

                                            <%--<label class="checkbox line">
                                                        <input type="checkbox" value="" />
                                                        Checkbox 1 </label>
                                                    <label class="checkbox line">
                                                        <input type="checkbox" value="" />
                                                        Checkbox 2 </label>--%>
                                            <div id="dvChkAll" runat="server">
                                                <label class="checkbox line">
                                                    <asp:CheckBox ID="chkAll" runat="server" CssClass="checker" AutoPostBack="true" OnCheckedChanged="chkAll_CheckedChanged" />Select All
                                                </label>
                                            </div>
                                            <asp:Panel ID="Panel1" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:CheckBoxList ID="chkZipCodes" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="chkZipCodes_SelectedIndexChanged">
                                                </asp:CheckBoxList>
                                            </asp:Panel>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpCity" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    Status
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button2">
                                        <input type="checkbox" class="toggle" id="chkActive" checked="checked" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Area_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        // It is important to place this JavaScript code after ScriptManager1
        var xPos, yPos;
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        function BeginRequestHandler(sender, args) {
            try {
                if ($get('<%=Panel1.ClientID%>') != null) {
                    // Get X and Y positions of scrollbar before the partial postback
                    xPos = $get('<%=Panel1.ClientID%>').scrollLeft;
                    yPos = $get('<%=Panel1.ClientID%>').scrollTop;
                }
            } catch (e) {

            }
        }

        function EndRequestHandler(sender, args) {
            if ($get('<%=Panel1.ClientID%>') != null) {
              // Set X and Y positions back to the scrollbar
              // after partial postback
              try {
                  $get('<%=Panel1.ClientID%>').scrollLeft = xPos;
                  $get('<%=Panel1.ClientID%>').scrollTop = yPos;
              } catch (e) {

              }
          }
      }

      prm.add_beginRequest(BeginRequestHandler);
      prm.add_endRequest(EndRequestHandler);
    </script>
</asp:Content>
