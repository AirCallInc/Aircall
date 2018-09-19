<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="City_AddEdit.aspx.cs" Inherits="Aircall.admin.City_AddEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">City Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/City_List.aspx">City List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">City Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-map-marker"></i>City Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">State Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpState" runat="server"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpState" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">City Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCityName" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCityName"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpName" runat="server" ControlToValidate="txtCityName" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ValidationExpression="[a-zA-Z ]*$" ErrorMessage="InValid characters"  />
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
                           <%-- <div class="control-group">
                                <label class="control-label">
                                    Pending Inactive
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button1">
                                        <input type="checkbox" class="toggle" id="chkPendingInactive" runat="server" />
                                    </div>
                                </div>
                            </div>--%>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server"  OnClick="btnSave_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'City_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
