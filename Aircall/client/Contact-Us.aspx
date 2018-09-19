<%@ Page Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="Contact-Us.aspx.cs" Inherits="Aircall.ClientContact_Us" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="banner-product" id="BanngerImg" runat="server" src="">
        <div class="container">
            <h1>
                <asp:Literal ID="ltBannerText" runat="server"></asp:Literal></h1>
        </div>
    </div>
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div id="dvMessage" runat="server" visible="false"></div>
                <div class="contactus-row cf">
                    <div class="contact-left">
                        <asp:Literal ID="ltMiddle" runat="server"></asp:Literal>
                        <asp:Literal ID="ltContent" runat="server"></asp:Literal>
                    </div>
                    <div class="contact-right">
                        <div class="border-block">
                            <h3>Contact Information:</h3>
                            <div class="main-from">
                                <div class="single-row cf">
                                    <div class="left-side">
                                        <label>Your name: </label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtYourName" runat="server" CssClass="input-large"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvName" CssClass="error" runat="server" ControlToValidate="txtYourName" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="single-row cf">
                                    <div class="left-side">
                                        <label>E-mail address:</label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="input-large"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid Email." Display="Dynamic" ValidationGroup="vgContact" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="single-row cf">
                                    <div class="left-side">
                                        <label>Phone Number: </label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtphone" runat="server" CssClass="input-large"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Phone Number is required." Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="regExpPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Invalid Phone Number." Display="Dynamic" ValidationGroup="vgContact" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="single-row cf">
                                    <div class="left-side">
                                        <label>Message: </label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtmsg" runat="server" CssClass="input-large" TextMode="MultiLine"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvMessage" CssClass="error" runat="server" ControlToValidate="txtmsg" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="single-row submit-row cf">
                                    <div class="right-side">
                                        <asp:Button ID="btnSave" Text="Submit" CssClass="btn btn-primary" runat="server" ValidationGroup="vgContact" OnClick="btnSave_Click" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                    <asp:Literal ID="ltBottom" runat="server"></asp:Literal>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

