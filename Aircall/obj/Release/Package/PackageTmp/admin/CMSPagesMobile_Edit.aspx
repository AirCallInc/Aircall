<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="CMSPagesMobile_Edit.aspx.cs" Inherits="Aircall.admin.CMSPagesMobile_Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Mobile CMS Page Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/CMSPagesMobile_List.aspx">Mobile CMS Page List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Mobile CMS Page Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-home"></i>Mobile CMS Page Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Title<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPageTitle" runat="server" CssClass="span4 required" ClientIDMode="Static" onblur="FillUrl()" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPageTitle" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPageTitle"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Description<span class="required"></span></label>
                                <div class="controls input-icon">
                                    <textarea name="ctl00$ContentPlaceHolder1$CKEditor1" runat="server" id="CKEditor" style="resize: none; visibility: hidden; display: none;" class="span5 ckeditor" rows="5" cols="5" maxlength="250" placeholder="Description"></textarea>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnUpdate_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'CMSPagesMobile_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
