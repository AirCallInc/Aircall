<%@ Page Title="" Language="C#" MasterPageFile="~/partner/PartnerMaster.Master" AutoEventWireup="true" CodeBehind="Ticket_Add.aspx.cs" Inherits="Aircall.partner.Ticket_Add" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Ticket Add</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>partner/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>partner/Ticket_List.aspx">Ticket List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Ticket Add</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-magic"></i>Tickte Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                             <div class="control-group">
                                <label class="control-label">Type of Ticket&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                   <asp:DropDownList ID="ddlTicket" runat="server"></asp:DropDownList>
                                     <asp:RequiredFieldValidator ID="rqfvPurpose" Display="Dynamic" runat="server" ControlToValidate="ddlTicket" CssClass="error_required" ErrorMessage="Required" Font-Size="12px" Font-Bold="true"
                                                     ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Subject&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtSubject" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvFName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtSubject"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Notes&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvLName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtNote" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnAdd" Text="Add" CssClass="btn btn-success" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Ticket_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
