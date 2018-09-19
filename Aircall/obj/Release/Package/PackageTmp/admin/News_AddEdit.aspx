<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="News_AddEdit.aspx.cs" Inherits="Aircall.admin.News_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">News Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/News_List.aspx">News List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">News Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-magic"></i>News Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Heading<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtHeading" runat="server" ClientIDMode="Static" CssClass="span6" onblur="FillUrl()"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvHeading" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtHeading"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">URL<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtURL" runat="server" ClientIDMode="Static" CssClass="span6 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvURL" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtURL" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Short Description<span class="required">*</span></label>
                                <div class="controls">
                                    <textarea class="span12 ckeditor" id="txtShortDescription" runat="server" name="editor1" rows="10"></textarea>
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label">Content<span class="required">*</span></label>
                                <div class="controls">
                                    <textarea class="span12 ckeditor" id="txtContent" runat="server" name="editor1" rows="10"></textarea>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Publish Date</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPublishDate" runat="server" CssClass="date-picker"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPublishDate" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPublishDate"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            
                            <div class="control-group">
                                <label class="control-label">Meta Title<span class="required"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMTitle" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Meta Keywords<span class="required"></span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMKeywords" runat="server" TextMode="MultiLine"></asp:TextBox>
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
                                        <input type="checkbox" class="toggle" id="chkActive" runat="server"  checked="checked"/>
                                    </div>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-success" ValidationGroup="ChangeGroup" runat="server" Visible="false" OnClick="btnUpdate_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'News_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function FillUrl() {
            var Heading = GetURL("txtHeading");
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
