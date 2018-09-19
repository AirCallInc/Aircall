<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="send_mail.aspx.cs" Inherits="Aircall.admin.send_mail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Send Email</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Send Email</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMessage" runat="server">
                            <div class="clear">
                                <!-- -->
                            </div>
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-envelope-alt"></i>&nbsp;Send Email</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">

                            <asp:UpdatePanel runat="server" ID="UPClients" ClientIDMode="Static">
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
                                        <label class="control-label">Send To&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:CheckBoxList ID="chkRole" ClientIDMode="Static" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" RepeatColumns="5" Visible="false"></asp:CheckBoxList>
                                            <label class="radio">
                                                <asp:CheckBox ID="chkClient" OnCheckedChanged="chkClient_CheckedChanged" AutoPostBack="true" runat="server" />
                                                Client
                                            </label>
                                            <label class="radio">
                                                <asp:CheckBox ID="chkPartner" OnCheckedChanged="chkPartner_CheckedChanged" AutoPostBack="true" runat="server" />
                                                Partner</label>
                                            <label class="radio">
                                                <asp:CheckBox ID="chkEmployee" OnCheckedChanged="chkEmployee_CheckedChanged" AutoPostBack="true" runat="server" />
                                                Employee</label>
                                            <label class="radio">
                                                <asp:CheckBox ID="chkWarehouseUser" OnCheckedChanged="chkWarehouseUser_CheckedChanged" AutoPostBack="true" runat="server" />
                                                Warehouse User</label>
                                            <label class="radio">
                                                <asp:CheckBox ID="chkAdmin" OnCheckedChanged="chkAdmin_CheckedChanged" AutoPostBack="true" runat="server" />
                                                Admin</label>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <div class="controls client-name">
                                            <asp:ListView ID="lstUser" runat="server">
                                                <ItemTemplate>
                                                    <label class="radio">
                                                        <asp:CheckBox ID="chkUser" name="rdpFaqType" value="both" runat="server" GroupName="rdpFaqType" Checked="true" />
                                                        <%#Eval("UserName") %>
                                                        <asp:HiddenField ID="hdnEmail" runat="server" Value='<%#Eval("Email") %>' />
                                                        <asp:HiddenField ID="hdnName" runat="server" Value='<%#Eval("UserName1") %>' />
                                                    </label>
                                                    <br />
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="chkClient" EventName="CheckedChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="chkPartner" EventName="CheckedChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="chkEmployee" EventName="CheckedChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="chkWarehouseUser" EventName="CheckedChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="chkAdmin" EventName="CheckedChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="control-group">
                                <label class="control-label">Subject&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <input type="text" name="txtSubject" runat="server" id="txtSubject" class="span6 required" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="ChangeGroup" ControlToValidate="txtSubject" Display="Dynamic" CssClass="error_required" ErrorMessage="Required"></asp:RequiredFieldValidator>

                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Use Email Template</label>
                                <div class="controls">
                                    <label class="checkbox">
                                        <asp:CheckBox ID="chkUseTemplate" runat="server" AutoPostBack="true" OnCheckedChanged="chkUseTemplate_CheckedChanged" />
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email Body</label>
                                <div class="controls">
                                    <textarea class="span12 ckeditor" id="txtBody" runat="server" name="editor1" rows="10"></textarea>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSend" Text="Send Email" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSend_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="window.location.href = '/admin/dashboard.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
