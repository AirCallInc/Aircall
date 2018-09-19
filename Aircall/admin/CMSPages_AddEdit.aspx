<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="CMSPages_AddEdit.aspx.cs" Inherits="Aircall.admin.CMSPages_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">CMS Page Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/CMSPages_List.aspx">CMS Pages List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">CMS Page Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-home"></i>CMS Page Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                             <%--<div class="control-group">
                            <label class="control-label">
                                Parent Page</label><div class="controls input-icon">

                                    <asp:DropDownList ID="ddlParent" runat="server">
                                    </asp:DropDownList>
                                </div>
                        </div>--%>
                        <div class="control-group">
                            <label class="control-label">
                                Select Block</label><div class="controls">
                                    <asp:Panel ID="Panel2" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                        <asp:CheckBoxList ID="cblBlocks" runat="Server">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                         <%--       <asp:RequiredFieldValidator ID="rqfvUnit" Display="Dynamic" runat="server" ControlToValidate="cblUnit" CssClass="error_required" ErrorMessage="Please select any one option" Font-Size="12px" Font-Bold="true"
                                                     ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>--%>
                                  
                                </div>
                        </div>
                            <div class="control-group">
                                <label class="control-label">Page Title<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPageTitle" runat="server" CssClass="span4 required" ClientIDMode="Static" onblur="FillUrl()" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPageTitle" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPageTitle"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Menu Title<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMenuTitle" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvMenuTitle" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtMenuTitle" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">URL<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtURL" runat="server" CssClass="span4 required" ClientIDMode="Static"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvURL" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtURL" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Description<span class="required"></span></label>
                                <div class="controls input-icon">
                                    <textarea name="ctl00$ContentPlaceHolder1$CKEditor1" runat="server" id="CKEditor" style="resize: none; visibility: hidden; display: none;" class="span5 ckeditor" rows="5" cols="5" maxlength="250" placeholder="Description"></textarea>
                                      <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="CKEditor" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                             <div class="control-group">
                                    <label class="control-label">Banner Image</label>
                                    <div class="controls">
                                        <asp:FileUpload ID="fpBanner" runat="server" />
                                        <asp:HiddenField ID="hdnBanner" runat="server" />
                                    <a href="" id="lnkBanner" runat="server" class="fancybox-button" data-rel="fancybox-button" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                    </div>
                                </div>
                            <div class="control-group">
                                <label class="control-label">Meta Title<span class="required"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMTitle" runat="server" ></asp:TextBox>
                                    
                               </div>
                                </div>
                                  <div class="control-group">
                                <label class="control-label">Meta Keywords<span class="required"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMKeywords" runat="server"  TextMode="MultiLine"></asp:TextBox>
                                    
                                </div>
                                      </div>
                                <div class="control-group">
                                <label class="control-label">Meta Description<span class="required"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMDes" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    
                                </div>                           
                      </div>
                                       <div class="control-group">
                                <label class="control-label">Additional Meta <span class="required"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtAMeta" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    
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
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'CMSPages_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function FillUrl() {
            var Heading = GetURL("txtPageTitle");
            $("#txtURL").val(Heading);
        }

        function GroupReplace() {
            return ('-');
        }

        function GetURL(name) {

            var text = document.getElementById(name).value.toLowerCase();

            text = text.replace(new RegExp('( )', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(,)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\.)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\*)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\()', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\))', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\[)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\])', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\{)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\})', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\@)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\#)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\$)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\%)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\^)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\&)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\|)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\?)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\~)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\`)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\;)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\:)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\+)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\=)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\!)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\")', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\<)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\>)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\/)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\-)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\()', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(\\))', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(__)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(__)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(___)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(___)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(____)', 'gi'), GroupReplace)
            text = text.replace(new RegExp('(____)', 'gi'), GroupReplace)

            var Site = text;
            var newStr = Site.substring(Site.length - 1, Site.length);
            if (newStr == "-") {
                Site = Site.substring(0, Site.length - 1);
            }
            return Site;
        }
    </script>
</asp:Content>
