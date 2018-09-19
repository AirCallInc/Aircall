<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Block_AddEdit.aspx.cs" Inherits="Aircall.admin.Block_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Block Page Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/block_List.aspx">Block Pages List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Block Page Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-home"></i>Block Page Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Block Title<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtBlockTitle" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvBlockTitle" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtBlockTitle"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Description<span class="required"></span></label>
                                <div class="controls input-icon">
                                    <textarea name="ctl00$ContentPlaceHolder1$CKEditor1" runat="server" id="CKEditor" style="resize: none; visibility: hidden; display: none;" class="span5 ckeditor" rows="5" cols="5" maxlength="250" placeholder="Description"></textarea>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Position<span class="required"></span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="ddlPostion" runat="server">
                                        <asp:ListItem>Top</asp:ListItem>
                                        <asp:ListItem>Middle</asp:ListItem>
                                        <asp:ListItem>Bottom</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <%--<div class="control-group">
                                <label class="control-label">Order<span class="required"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtOrder" runat="server" CssClass="span4 required"></asp:TextBox>

                                </div>
                            </div>--%>
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
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Block_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
