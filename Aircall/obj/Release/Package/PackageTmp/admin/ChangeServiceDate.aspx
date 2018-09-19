<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ChangeServiceDate.aspx.cs" Inherits="Aircall.admin.ChangeServiceDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Change Service Dates</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Change Service Dates</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-group"></i>Service Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Service Case #<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpServiceCase" runat="server" CssClass="chosen-with-diselect" AutoPostBack="true" OnSelectedIndexChanged="drpServiceCase_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvServiceCase" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpServiceCase" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UPDates" runat="server">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) {
                                            $('.date-picker').datepicker();
                                        });
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label">Expected Start Date<span class="required">*</span></label>
                                        <div class="controls">
                                             <asp:TextBox ID="txtExpStart" runat="server" CssClass="date-picker"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtExpStart"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Expected End Date<span class="required">*</span></label>
                                        <div class="controls">
                                             <asp:TextBox ID="txtExpEnd" runat="server" CssClass="date-picker"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtExpEnd"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="drpServiceCase" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="form-actions">
                                <asp:Button ID="btnChange" Text="Update" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server"  OnClick="btnChange_Click"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
