<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Employee_AddEdit.aspx.cs" Inherits="Aircall.admin.Employee_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Employee Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Employee_List.aspx">Employee List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Employee Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-group"></i>Employee Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">FirstName<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtFName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvFName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFName"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regFName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFName" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">LastName<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtLName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvLName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtLName"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regLName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtLName" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="span4 required" TextMode="Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmail" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group" id="dvPassword">
                                <label class="control-label">Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="span4 required" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPassword" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPassword" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regPassword" runat="server" ErrorMessage="Password length must be Greater than 6 characters." Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPassword" ValidationExpression="^(?:.{6,}|)$"></asp:RegularExpressionValidator>
                                    <asp:HiddenField ID="hdnPassword" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Address<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="span4 required" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtAddress" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">City<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCity" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtCity" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">State<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpState" runat="server" CssClass="span4 required"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="drpState" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <%--<div class="control-group">
                                <label class="control-label">City<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPCity" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpCity" runat="server"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="drpCity" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpState" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>--%>
                            <div class="control-group">
                                <label class="control-label">Zip Code<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtZip" MaxLength="5" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtZip" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regZipCode" CssClass="error_required" runat="server" ControlToValidate="txtZip" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Zip Code." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\s*\d+\s*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Employee Plan Type<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:ListBox ID="lstPlanType" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                    <asp:RequiredFieldValidator ID="rfvPlanType" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="lstPlanType" ValidationGroup="ChangeGroup" InitialValue=""></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Mobile Number<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMob" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvMob" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtMob" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regMob" CssClass="error_required" runat="server" ControlToValidate="txtMob" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{10,15}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Phone Number<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPhone" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPhone" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPhone" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regPhone" CssClass="error_required" runat="server" ControlToValidate="txtPhone" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{8,15}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Profile Image</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpImage" runat="server" />
                                    <asp:HiddenField ID="hdnImage" runat="server" />
                                    <a href="" id="lnkImage" class="fancybox-button" data-rel="fancybox-button" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                </div>
                            </div>
                            <div class="control-group">
                                <div class="controls">
                                    <span class="label label-important">NOTE!</span>
                                    <span>Please upload image of
                                                200 x 200 or higher pixels. For best results, the image pixels should be multiples
                                                of the minimum width and height. </span>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Work Start Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtStart" runat="server" class="span7 timepicker-default" type="text" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtStart" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Work End Time<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtEnd" runat="server" class="span7 timepicker-default" type="text" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEnd" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    Is Sales Person
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button1">
                                        <input type="checkbox" class="toggle" id="chkIsSales" runat="server" />
                                    </div>
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
                                <asp:Button ID="btnAdd" Text="Add" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Employee_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
