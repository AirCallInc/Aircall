<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="RenewCancel_UnitSubscription.aspx.cs" Inherits="Aircall.admin.RenewCancel_UnitSubscription" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Plan Renew/Cancel</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/RenewCancel_List.aspx">Unit Subscription List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Plan Renew/Cancel</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-map-marker"></i>Plan Renew/Cancel</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Reason For Cancel Plan<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCancelReason" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvReason" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="PlanGroup" CssClass="error_required" ControlToValidate="txtCancelReason"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnRenew" Text="Renew Subscription" CssClass="btn btn-primary" runat="server" OnClick="btnRenew_Click"/>
                                <asp:Button ID="btnCancel" Text="Cancel Subscription" CssClass="btn btn-primary" ValidationGroup="PlanGroup" runat="server" OnClick="btnCancel_Click"/>
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'RenewCancel_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
