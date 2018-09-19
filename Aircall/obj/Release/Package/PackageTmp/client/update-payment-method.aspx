<%@ Page Title="Payment Methods" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="update-payment-method.aspx.cs" Inherits="Aircall.client.update_payment_method" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Update Payment Method</h1>
                </div>                
                <div class="new-card-detail">                    
                    <div class="r">
                        <div class="main-from">
                            <div>
                                <div id="dvMessage" runat="server" visible="false"></div>
                                <div class="single-row select-method cf">
                                    <div class="left-side">
                                        <label>Select Payment Method</label>
                                    </div>
                                    <div class="right-side">
                                        <div class="radio-outer-dot">
                                            <input type="radio" id="rblVisa" runat="server" clientidmode="static" name="radio"><label for="rblVisa"><img src="images/card-visa.png" alt="" title=""></label>
                                            <input type="radio" id="rblMaster" runat="server" clientidmode="static" name="radio" checked="true"><label for="rblMaster"><img src="images/card-master.png" alt="" title=""></label>
                                            <input type="radio" id="rblDiscover" runat="server" clientidmode="static" name="radio"><label for="rblDiscover"><img src="images/card-discover.png" alt="" title=""></label>
                                            <input type="radio" id="rblAmex" runat="server" clientidmode="static" name="radio"><label for="rblAmex"><img src="images/card-other.png" alt="" title=""></label>
                                        </div>
                                    </div>
                                </div>
                                <div class="single-row card-name cf">
                                    <div class="left-side">
                                        <label>Name on Card</label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvName" CssClass="error" runat="server" ControlToValidate="txtName" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="single-row card-number cf">
                                    <div class="left-side">
                                        <label>Card Number</label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtCardNumber" runat="server" MaxLength="16"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvCardNo" CssClass="error" runat="server" ControlToValidate="txtCardNumber" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="single-row exp-date cf">
                                    <div class="left-side">
                                        <label>Expiry date</label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtMonth" runat="server" MaxLength="2" CssClass="month" placeholder="MM"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvMonth" CssClass="error" runat="server" ControlToValidate="txtMonth" ErrorMessage="*" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="error" runat="server" ControlToValidate="txtMonth" ErrorMessage="Invalid Month." Display="Dynamic" ValidationGroup="vgSave" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                        <span>/</span>
                                        <asp:TextBox ID="txtYear" runat="server" MaxLength="4" CssClass="year" placeholder="YYYY"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvYear" CssClass="error" runat="server" ControlToValidate="txtYear" ErrorMessage="*" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" CssClass="error" runat="server" ControlToValidate="txtYear" ErrorMessage="Invalid Year." Display="Dynamic" ValidationGroup="vgSave" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="single-row cvv-num cf">
                                    <div class="left-side">
                                        <label>CVV</label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtCVV" runat="server" MaxLength="4"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvCVV" CssClass="error" runat="server" ControlToValidate="txtCVV" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="single-row cf">
                                    <div class="left-side">
                                        <label>Is Default</label>
                                    </div>
                                    <div class="right-side">
                                        <div class="checkbox-outer">
                                            <asp:CheckBox ID="chkIsDefault" runat="server" Text="Default" />
                                        </div>
                                    </div>
                                </div>
                                <div class="single-row button-bar cf">                                    
                                    <asp:Button ID="btnUpdate" runat="server" CssClass="main-btn" Text="Update" ValidationGroup="vgSave" Visible="false" OnClick="btnUpdate_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
