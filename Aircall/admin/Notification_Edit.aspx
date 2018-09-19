<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Notification_Edit.aspx.cs" Inherits="Aircall.admin.Notification_Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Notification Edit </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="<%=Application["SiteAddress"]%>admin/Notification_List.aspx">Notifications</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Notification Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-exclamation-sign"></i>&nbsp;Notification Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Notification Name&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtName" runat="server" CssClass="span6"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Notification Message&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMsg" runat="server" CssClass="span6" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvMsg" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMsg"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Avaliable Tags</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrTags" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Notification_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
