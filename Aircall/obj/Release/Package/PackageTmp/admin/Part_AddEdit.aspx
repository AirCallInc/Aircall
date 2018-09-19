<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Part_AddEdit.aspx.cs" Inherits="Aircall.admin.Part_AddEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Part Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Part_List.aspx">Part List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Part Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>Part Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Inventory Type<span class="required">*</span></label>
                                <div class="controls">
                                    <label class="radio">
                                    <asp:RadioButton ID="rblInventory" runat="server" GroupName="GrpInventoryType" Checked="true"/>Inventory
                                        </label>
                                    <label class="radio">
                                        <asp:RadioButton ID="rblNonInventory" runat="server" GroupName="GrpInventoryType" />Non Inventory
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Part Type</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPartType" runat="server" CssClass="span4 chosen"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Part Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPartname" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPartname"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Part Size</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtSize" runat="server" CssClass="input-large"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvSize" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtSize"></asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Part Description</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtDescription" runat="server" CssClass="input-large" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <%--<div class="control-group">
                                <label class="control-label">Inbound Quantity</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtInbound" runat="server" CssClass="input-small"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvInbound" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtInbound"></asp:RequiredFieldValidator>
                                </div>
                            </div>--%>
                            <div class="control-group">
                                <label class="control-label">Receive Quantity</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtReceive" runat="server" CssClass="input-small"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReceive" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtReceive"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpReceive" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtReceive" ValidationExpression="^\d{1,9}$"></asp:RegularExpressionValidator>
                                    <asp:HiddenField ID="hdnReceice" runat="server" Value="0"/>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Total Acquired Quantity<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtAcquired" runat="server" CssClass="input-small"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvAcquired" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtAcquired"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpAcquired" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtAcquired" ValidationExpression="^\d{1,9}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">In-stock Quantity</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtInStock" runat="server" CssClass="input-small"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regExpInStock" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtInStock" ValidationExpression="^\d{1,9}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Reserved Quantity</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtReserved" runat="server" CssClass="input-small"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regExpReserved" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtReserved" ValidationExpression="^\d{1,9}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Purchase Price<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPurchased" runat="server" CssClass="input-small"></asp:TextBox>&nbsp;per item
                                    <asp:RequiredFieldValidator ID="rqfvPurchased" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPurchased"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpPurchasedPrice" runat="server" ErrorMessage="Invalid Price" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPurchased" ValidationExpression="^\d*[0-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Selling Price<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtSelling" runat="server" CssClass="input-small"></asp:TextBox>&nbsp;per item
                                    <asp:RequiredFieldValidator ID="rqfvSelling" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtSelling"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpSellingPrice" runat="server" ErrorMessage="Invalid Price" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtSelling" ValidationExpression="^\d*[0-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Minimum Re-order Quantity<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMinReorder" runat="server" CssClass="input-small"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvMinReorder" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMinReorder"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpMinReOrder" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMinReorder" ValidationExpression="^\d{1,9}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Re-order Quantity<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtReOrder" runat="server" CssClass="input-small"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReorder" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtReOrder"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpReOrder" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtReOrder" ValidationExpression="^\d{1,9}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    Status
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button2">
                                        <input type="checkbox" class="toggle" id="chkActive" runat="server" checked="checked" />
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    Is Default
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button1">
                                        <input type="checkbox" class="toggle" id="chkIsDefault" runat="server"  />
                                    </div>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Part_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
