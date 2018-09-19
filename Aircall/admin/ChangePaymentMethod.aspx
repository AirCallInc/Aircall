<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ChangePaymentMethod.aspx.cs" Inherits="Aircall.admin.ChangePaymentMethod" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client Unit Change Payment Method</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/ClientUnitSubscription_List.aspx">Client Unit Subscription List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Change Payment Method</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>Change Payment Method Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrClientName" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hdnClientId" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Name</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrUnit" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hdnUnitId" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Payment Due Date</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrDueDate" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Current Payment Method</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrCurrentPaymentMethod" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">New Payment Method</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPayment" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpPayment_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvPayment" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpPayment" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UPDPayment" runat="server">
                                <ContentTemplate>
                                    <div id="dvCard" runat="server" visible="false">
                                        <div class="control-group">
                                            <label class="control-label">Card<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:DropDownList ID="drpCard" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpCard_SelectedIndexChanged"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rqfvCard" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpCard" InitialValue="0"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div id="dvNewCard" runat="server" visible="false">
                                            <div class="control-group">
                                                <label class="control-label">Card Type<span class="required">*</span></label>
                                                <div class="controls">
                                                    <input type="radio" class="radio" id="rblVisa" runat="server" name="rblCard" value="Visa" />Visa
                                                <input type="radio" class="radio" id="rblMaster" runat="server" name="rblCard" value="Master Card" />Master Card
                                                <input type="radio" class="radio" id="rblDiscover" runat="server" name="rblCard" value="Discover" />Discover
                                                <input type="radio" class="radio" id="rblAmex" runat="server" name="rblCard" value="Amex" />Amex
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Name On Card<span class="required">*</span></label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtCardName" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rqfvCardName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCardName"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Card Number<span class="required">*</span></label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtCardNumber" runat="server" MaxLength="16"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rqfvCardNumber" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCardNumber"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="regExpCard" CssClass="error_required" runat="server" ControlToValidate="txtCardNumber" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Card Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Expiry date<span class="required">*</span></label>
                                                <div class="controls">
                                                    <%--<asp:TextBox ID="txtMonth" runat="server" CssClass="input-small" MaxLength="2" placeholder="MM"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="refvMonth" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMonth"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="regMonth" CssClass="error_required" runat="server" ControlToValidate="txtMonth" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="0[1-9]|1[0-2]"></asp:RegularExpressionValidator>
                                                    <span>/</span>
                                                    <asp:TextBox ID="txtYear" runat="server" CssClass="input-small" MaxLength="4" placeholder="YYYY"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rqfvYear" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtYear"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="regYear" CssClass="error_required" runat="server" ControlToValidate="txtYear" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{4}"></asp:RegularExpressionValidator>--%>
                                                    <asp:DropDownList ID="drpMonth" runat="server" CssClass="input-small"></asp:DropDownList>
                                                    <span>/</span>
                                                    <asp:DropDownList ID="drpYear" runat="server" CssClass="input-small"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">CVV<span class="required">*</span></label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtCVV" runat="server" CssClass="input-small" MaxLength="3"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rqfvCVV" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCVV"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="drpPayment" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="form-actions">
                                <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-info" ValidationGroup="ChangeGroup" runat="server" OnClick="btnUpdate_Click" />
                                <asp:Button ID="btnCancel" Text="Cancel" CssClass="btn" runat="server" OnClick="btnCancel_Click" />
                                <%--<input type="button" class="btn" value="Cancel" onclick="location.href = 'ClientUnitSubscription_List.aspx'" />--%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
