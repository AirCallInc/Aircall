<%@ Page Title="" Language="C#" MasterPageFile="~/partner/PartnerMaster.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Aircall.partner.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Profile</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>partner/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Profile</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-user"></i>Profile</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">First Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtFirstname" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvFName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFirstname"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Last Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtLastname" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvLName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtLastname" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">User Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="input-large" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Password</label>
                                <div class="controls">
                                    <a href="<%=Application["SiteAddress"] %>partner/ChangePassword.aspx">Change Password</a>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmail" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Invalid Email" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Company Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCompany" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvCompany" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtCompany" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Image</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fImage" runat="server" CssClass="default" />
                                    <asp:HiddenField ID="hdnImage" runat="server" />
                                    <a href="" id="lnkImage" class="fancybox-button" data-rel="fancybox-button" runat="server" visible="false" target="_blank" style="cursor: pointer;display: block;">View Image</a>
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
                                <label class="control-label">Address&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtAddress" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">State&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <asp:DropDownList ID="drpState" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="drpState" InitialValue="0" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">City&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <asp:UpdatePanel ID="UPCity" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpCity" runat="server"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="drpCity" InitialValue="0" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
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
                                    <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regZip" CssClass="error_required" runat="server" ControlToValidate="txtZip" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Zip Code." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Phone No&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPhone" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPhone" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Assigned Affiliate ID&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <asp:TextBox ID="txtAffiliate" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Sales Commission %</label>
                                <div class="controls input-icon">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrSalesComm" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'dashboard.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>