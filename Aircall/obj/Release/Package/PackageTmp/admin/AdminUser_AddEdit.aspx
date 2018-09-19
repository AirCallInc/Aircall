<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AdminUser_AddEdit.aspx.cs" Inherits="Aircall.admin.AdminUser_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Add/Edit Admin User</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/AdminUser_List.aspx">Admin User List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Add/Edit Admin User</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-group"></i>User Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">User Role<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpRole" runat="server" CssClass="span4 required"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvRole" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpRole" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">First Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtFirstname" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvFName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFirstname"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regFName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFirstname" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Last Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtLastname" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvLName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtLastname" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regLName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtLastname" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Username<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvUsername" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtUsername" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="span4 required" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPassword" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPassword" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regPassword" runat="server" ErrorMessage="Password length must be Greater than 6 characters." Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPassword" ValidationExpression="^(?:.{6,}|)$"></asp:RegularExpressionValidator>
                                    <asp:HiddenField ID="hdnPassword" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmail" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="Invalid Email" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Profile Image</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpImage" runat="server" />
                                    <asp:HiddenField ID="hdnImage" runat="server" />
                                    <a href="" class="fancybox-button" data-rel="fancybox-button" id="lnkImage" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                </div>
                            </div>
                            <div class="control-group">
                                <div class="controls">
                                    <span class="label label-important">NOTE!</span>
                                    <span>Please upload image of
                                                200 x 200 or higher pixels. For best results, the image pixels should be multiples
                                                of the minimum width and height.
                                    </span>
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
                                <asp:Button ID="btnAdd" Text="Add" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'AdminUser_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
