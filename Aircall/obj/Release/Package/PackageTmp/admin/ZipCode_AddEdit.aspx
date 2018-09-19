<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ZipCode_AddEdit.aspx.cs" Inherits="Aircall.admin.ZipCode_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Zip Code Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/ZipCode_List.aspx">Zip Code List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Zip Code Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-map-marker"></i>Zip Code Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
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
                                            <asp:DropDownList ID="drpCity" runat="server"></asp:DropDownList>
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
                                    <asp:HiddenField ID="hdnZip" runat="server" />
                                    <asp:TextBox ID="txtZip" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regZipCode" CssClass="error_required" runat="server" ControlToValidate="txtZip" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Zip Code." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    Status
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button2">
                                        <input type="checkbox" class="toggle" checked="checked" id="chkActive" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <%--<div class="control-group">
                                <label class="control-label">
                                    Pending InActive
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button1">
                                        <input type="checkbox" class="toggle" id="chkPendingInactive" runat="server" />
                                    </div>
                                </div>
                            </div>--%>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server"  OnClick="btnSave_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'ZipCode_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
