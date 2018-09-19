<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Plan_AddEdit.aspx.cs" Inherits="Aircall.admin.Plan_AddEdit" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Plan Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Plan_List.aspx">Plan List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Plan Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-magic"></i>Plan Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="span6">
                                <div class="control-group">
                                    <label class="control-label">Plan Name<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPlanName" runat="server" CssClass="input-large"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvPName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPlanName"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Plan Type<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpPlanType" runat="server"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rqfvPlanType" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpPlanType" InitialValue="0"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Short Description<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtSortDesc" runat="server" Rows="5" CssClass="span12" TextMode="MultiLine" MaxLength="256"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvSortDesc" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtSortDesc"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Package A Display Name<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPackageADispName" runat="server" CssClass="input-large"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvAName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPackageADispName"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="span6" style="display:none;">
                                <div class="control-group">
                                    <label class="control-label">Country</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpCountry" runat="server">
                                            <asp:ListItem Text="USA" Value="USA"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">State</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpState" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">City</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpCity" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="span12" style="margin-left:0px;">
                                <div class="control-group">
                                    <label class="control-label">Package A Description<span class="required">*</span></label>
                                    <div class="controls">
                                        <%--<CKEditor:CKEditorControl ID="CKEditor1" runat="server" BasePath="~/admin/ckeditor">
                                    </CKEditor:CKEditorControl>--%>
                                        <textarea class="span12 ckeditor" id="txtPackageA" runat="server" name="editor1" rows="10"></textarea>
                                        <asp:HiddenField ID="hdnPackageA" runat="server" />
                                    </div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">Package B Display Name<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPackageBDispName" runat="server" CssClass="input-large"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvBName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPackageBDispName"></asp:RequiredFieldValidator>
                                    </div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">Package B Description<span class="required">*</span></label>
                                    <div class="controls">
                                        <textarea class="span12 ckeditor" id="txtPackageB" runat="server" name="editor1" rows="10"></textarea>
                                        <asp:HiddenField ID="hdnPackageB" runat="server" />
                                    </div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">Plan Price in $<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPrice" runat="server" CssClass="input-small"></asp:TextBox>
                                        / month
                                    <asp:RequiredFieldValidator ID="rqfvPrice" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPrice"></asp:RequiredFieldValidator> <%--^\d*[0-9]\d*(\.\d+)?$--%>
                                        <asp:RegularExpressionValidator ID="regExpPrice" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPrice" ValidationExpression="^\d{1,9}(\.\d+)?$"></asp:RegularExpressionValidator>
                                        <asp:HiddenField ID="hdnPrice" runat="server" />
                                    </div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">Duration<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDuration" runat="server" CssClass="input-small" MaxLength="4"></asp:TextBox>
                                        Months
                                    <asp:RequiredFieldValidator ID="rqfvDuration" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtDuration"></asp:RequiredFieldValidator>
                                        <%--<asp:RegularExpressionValidator ID="regExpDuration" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtDuration" ValidationExpression="^\d{1,9}$"></asp:RegularExpressionValidator>--%>
                                        <asp:HiddenField ID="hdnDuration" runat="server" />
                                    </div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">Mandatory Services<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtNoOfService" runat="server" CssClass="input-small" MaxLength="3"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvService" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtNoOfService"></asp:RequiredFieldValidator>
                                        <%--<asp:RegularExpressionValidator ID="regExpService" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtNoOfService" ValidationExpression="^\d{1,3}$"></asp:RegularExpressionValidator>--%>
                                        <asp:HiddenField ID="hdnService" runat="server" />
                                    </div>
                                </div>

                                <div class="control-group">
                                    <label class="control-label">Schedule First Service In<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtFirstService" runat="server" CssClass="input-small" MaxLength="4"></asp:TextBox>
                                        Days
                                    <asp:RequiredFieldValidator ID="rqfvFirstService" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFirstService"></asp:RequiredFieldValidator>
                                        <%--<asp:RegularExpressionValidator ID="regExpFirstService" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFirstService" ValidationExpression="^\d{1,5}$"></asp:RegularExpressionValidator>--%>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Other Service Schedule Gap<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtOtherServiceGap" runat="server" CssClass="input-small" MaxLength="4"></asp:TextBox>
                                        Days
                                    <asp:RequiredFieldValidator ID="rqfvOtherServiceGap" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtOtherServiceGap"></asp:RequiredFieldValidator>
                                        <%--<asp:RegularExpressionValidator ID="regExpOtherServiceGap" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtOtherServiceGap" ValidationExpression="^\d{1,5}$"></asp:RegularExpressionValidator>--%>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Drive Time<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDriveTime" runat="server" CssClass="input-small" MaxLength="3"></asp:TextBox>
                                        Minutes
                                    <asp:RequiredFieldValidator ID="rqfvDrive" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtDriveTime"></asp:RequiredFieldValidator>
                                        <%--<asp:RegularExpressionValidator ID="regExpDrive" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtDriveTime" ValidationExpression="^\d{1,5}$"></asp:RegularExpressionValidator>--%>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Service Time For First Unit<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtServiceTimeForFirstUnit" runat="server" CssClass="input-small"></asp:TextBox>
                                        Minutes
                                    <asp:RequiredFieldValidator ID="rqfvServiceTimeForFirstUnit" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtServiceTimeForFirstUnit"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Service Time For Additional Units<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtServiceTimeForAdditionalUnits" runat="server" CssClass="input-small"></asp:TextBox>
                                        Minutes
                                    <asp:RequiredFieldValidator ID="rqfvServiceTimeForAdditionalUnits" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtServiceTimeForAdditionalUnits"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">
                                        Show Special Price
                                    </label>
                                    <div class="controls">
                                        <div class="text-toggle-button1">
                                            <input type="checkbox" class="toggle" id="chkSpecial" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">
                                        Show Auto Renewals
                                    </label>
                                    <div class="controls">
                                        <div class="text-toggle-button1">
                                            <input type="checkbox" class="toggle" id="chkAutorenewal" runat="server" />
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Discount Price in $<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDiscount" runat="server" CssClass="input-small"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvDiscount" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtDiscount"></asp:RequiredFieldValidator> <%--^\d*[0-9]\d*(\.\d+)?$--%>
                                        <asp:RegularExpressionValidator ID="regExpDiscount" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtDiscount" ValidationExpression="^\d{1,9}(\.\d+)?$"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Background Color<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtColor" runat="server" CssClass="input-small colorpicker-default"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvColor" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtColor"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Plan Image</label>
                                    <div class="controls">
                                        <asp:FileUpload ID="fpPlanImage" runat="server" />
                                        <a href="" id="lnkImage" class="fancybox-button" data-rel="fancybox-button" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                        <asp:HiddenField ID="hdnImage" runat="server" />
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
                                    <label class="control-label">Service Slot1</label>
                                    <div class="controls input-icon">
                                        <div class="input-append bootstrap-timepicker-component" style="margin-right: -88px;">
                                            <input id="txtSlot1Start" runat="server" class="span6 timepicker-default" type="text" /><span class="add-on"><i
                                                class="icon-time"></i></span>
                                            <asp:RequiredFieldValidator ID="rqfvSlot1Start" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtSlot1Start" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
                                        <span>To</span>
                                        <div class="input-append bootstrap-timepicker-component">
                                            <input id="txtSlot1End" runat="server" class="span6 timepicker-default" type="text" /><span class="add-on"><i
                                                class="icon-time"></i></span>
                                            <asp:RequiredFieldValidator ID="rqfvSlot1End" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtSlot1End" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Service Slot2</label>
                                    <div class="controls input-icon">
                                        <div class="input-append bootstrap-timepicker-component" style="margin-right: -88px;">
                                            <input id="txtSlot2Start" runat="server" class="span6 timepicker-default" type="text" /><span class="add-on"><i
                                                class="icon-time"></i></span>
                                            <asp:RequiredFieldValidator ID="rqfvSlot2Start" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtSlot2Start" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
                                        <span>To</span>
                                        <div class="input-append bootstrap-timepicker-component">
                                            <input id="txtSlot2End" runat="server" class="span6 timepicker-default" type="text" /><span class="add-on"><i
                                                class="icon-time"></i></span>
                                            <asp:RequiredFieldValidator ID="rqfvSlot2End" runat="server" ErrorMessage="*" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtSlot2End" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                        </div>
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

                                <div class="form-actions">
                                    <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                    <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-success" ValidationGroup="ChangeGroup" runat="server" Visible="false" OnClick="btnUpdate_Click" />
                                    <input type="button" class="btn" value="Cancel" onclick="location.href = 'Plan_List.aspx'" />
                                </div>
                            </div>
                            <div style="clear:both;"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
