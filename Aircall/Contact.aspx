<%@ Page Title="" Language="C#" MasterPageFile="~/Front.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Aircall.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="banner" style="background-image: url('images/product-banner.jpg')">
        <div class="container">
            <h1>Contact us</h1>
        </div>
    </div>

    <!-- content area part -->
    <div id="content-area">
        <div class="common-content">
            <div class="container">
                <div class="contactus-row cf">
                    <div class="contact-left">
                        <address>
                            <p>
                                <strong>Address:</strong>
                                <br>
                                AirCall
                                <br>
                                1234 Streetname St.,
                                <br>
                                Los Angeles, CA 00000
                            </p>
                            <p>
                                Phone: 000-000-0000.
                                <br>
                                Available 9-5 PST Mon-Fri
                            </p>
                        </address>
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
                                        Phone Number<label>: </label>
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
                </div>
            </div>
        </div>
    </div>
    <div class="footer-push"></div>    
</asp:Content>
