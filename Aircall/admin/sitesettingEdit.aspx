<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="sitesettingEdit.aspx.cs" Inherits="Aircall.admin.sitesettingEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Site Setting Edit </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/sitesetting_list.aspx">Site Setting</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Site Setting Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>&nbsp;Site Setting Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Site Setting Name&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <input type="text" name="txtName" runat="server" id="txtName" readonly="readonly" class="span4 required" />
                                </div>
                            </div>
                            <div class="control-group" id="dvNonFile" runat="server">
                                <label class="control-label">Site Setting Value&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <input type="text" name="txtValue" runat="server" id="txtValue" class="span4 required" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtValue" ErrorMessage="Required" Display="Dynamic"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <asp:HiddenField ID="hdnValue" runat="server" />
                            <div class="control-group" id="dvFile" runat="server">
                                <label class="control-label">Site Setting Value&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpdSalesAgreement" runat="server" />
                                    <asp:Literal ID="ltrSalesAgreement" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnUpate" runat="server" Text="Update" CssClass="btn btn-success" OnClick="btnUpate_Click" />
                                <button type="button" onclick="window.location.href = '<%=Application["SiteAddress"]%>admin/sitesetting_list.aspx'" id="btncancel" class="btn">Cancel</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
