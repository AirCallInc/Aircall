<%@ Page Title="Contacts" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="contact-numbers.aspx.cs" Inherits="Aircall.client.contact_numbers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(document).ready(function () {
        });
        function hidedvMessage() {
            $("#dvMessage").hide();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Contact Numbers</h1>
                </div>
                <div class="border-block change-password-block max640 cf">
                    <div class="main-from">
                        <%--<asp:ValidationSummary ID="vgSummary" runat="server" ValidationGroup="vgContact" CssClass="error" DisplayMode="List" />--%>
                        <div id="dvMessage" runat="server" clientidmode="static" visible="false"></div>
                        <div>
                            <div class="password-row cf">
                                <div class="left-side">
                                    <label>Mobile</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtMobile" runat="server" MaxLength="15"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvMobile" CssClass="error" runat="server" ControlToValidate="txtMobile" ErrorMessage="Mobile Number is required." Display="Dynamic" ValidationGroup="vgContact"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpMobile" CssClass="error" runat="server" ControlToValidate="txtMobile" ErrorMessage="Invalid Mobile Number." Display="Dynamic" ValidationGroup="vgContact" ValidationExpression="\d{8,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="password-row cf">
                                <div class="left-side">
                                    <label>Office</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtOffice" runat="server" MaxLength="15"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regExpOffice" CssClass="error" runat="server" ControlToValidate="txtOffice" ErrorMessage="Invalid Office Number." Display="Dynamic" ValidationGroup="vgContact" ValidationExpression="\d{8,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="password-row cf">
                                <div class="left-side">
                                    <label>Home</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtHome" runat="server" MaxLength="15"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regExpHome" CssClass="error" runat="server" ControlToValidate="txtHome" ErrorMessage="Invalid Home Number." Display="Dynamic" ValidationGroup="vgContact" ValidationExpression="\d{8,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="password-row button-bar cf">
                                <asp:Button ID="btnSubmit" runat="server" OnClientClick="javascript:hidedvMessage();" Text="Update" CssClass="main-btn" ValidationGroup="vgContact" OnClick="btnSubmit_Click" />
                                <input type="button" class="main-btn dark-grey" value="Cancel" onclick="location.href = 'account-setting.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
