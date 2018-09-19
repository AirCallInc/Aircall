<%@ Page Title="Account Setting" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="account-setting.aspx.cs" Inherits="Aircall.client.account_setting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .empPhoto{
            height: 71px;width: 67px;border-radius: 50px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Account Settings</h1>
                </div>
                <div class="border-block">
                    <div class="main-from">
                        <asp:ValidationSummary ID="vgSummary" runat="server" ValidationGroup="vgAccount" CssClass="error" DisplayMode="List" />
                        <div id="dvMessage" runat="server" visible="false"></div>
                        <div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>First Name</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFName" CssClass="error" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is required." Display="None" ValidationGroup="vgAccount"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Last Name</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLName" CssClass="error" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is required." Display="None" ValidationGroup="vgAccount"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Email</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtEmail" runat="server" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Company</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtCompany" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Password</label>
                                </div>
                                <div class="right-side">
                                    <p><a class="change-password" href="change-password.aspx">Change Password</a></p>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Profile Image</label>
                                </div>
                                <div class="right-side">
                                    <a class="profile-image" href="#">
                                        <figure>
                                            <asp:HiddenField ID="hdnImage" runat="server" />
                                            <img alt="" src="" title="" class="empPhoto" id="imgClient" runat="server"></figure>
                                        <asp:FileUpload ID="fpImage" runat="server" />
                                    </a>

                                </div>
                            </div>
                            <div class="single-row additional-setting cf">
                                <a class="main-btn transparent" href="contact-numbers.aspx">Contact numbers</a>
                                <a class="main-btn transparent" href="payment-method.aspx">Payment method</a>
                                <a class="main-btn transparent" href="billing-history.aspx">Billing history</a>
                            </div>
                            <div class="single-row button-bar cf">
                                <asp:Button ID="btnSubmit" runat="server" CssClass="main-btn" Text="Submit" ValidationGroup="vgAccount" OnClick="btnSubmit_Click"/>
                                <input type="button" class="main-btn dark-grey" value="Cancel" onclick="location.href = 'dashboard.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
