<%@ Page Title="SignUp" Language="C#" MasterPageFile="~/Front.Master" AutoEventWireup="true" CodeBehind="signup.aspx.cs" Inherits="Aircall.signup" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- content area part -->
    <div id="content-area">
        <div class="signup-section">
            <div class="container">
                <h1>signup form</h1>
                <p>You are one step away from having a worry free air conditioning maintenance program! Please fill out the form below and enjoy the benefits people enjoy. Following submission, please check your email for login information.</p>
                <div class="border-block">
                    <h3>Register for new account:</h3>
                    <%--<asp:ValidationSummary ID="vgSummary" runat="server" ValidationGroup="vgSignup" CssClass="error" DisplayMode="List" />--%>
                    <div id="dvMessage" runat="server" visible="false">
                    </div>
                    <div class="main-from">
                        <div>
                            <%--<div class="single-row cf">
                                <div class="left-side">
                                    <label>HVAC unit</label>
                                </div>
                                <div class="right-side">
                                    <input type="radio" id="rbtnMore" clientidmode="static" runat="server" name="hvac">
                                    <label for="rbtnMore">My HVAC Unit is less than 10 years old</label>
                                    <input type="radio" id="rbtnLess" clientidmode="static" runat="server" name="hvac">
                                    <label for="rbtnLess">My HVAC Unit is more than 10 years old</label>
                                    <input type="radio" id="rbtnNo" clientidmode="static" runat="server" name="hvac" checked="true">
                                    <label for="rbtnNo">I don't know how old my HVAC Unit is</label>
                                </div>
                            </div>--%>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>First Name</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFName" CssClass="error" runat="server" ControlToValidate="txtFirstName" ErrorMessage="First Name is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Last Name</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvLName" CssClass="error" runat="server" ControlToValidate="txtLastName" ErrorMessage="Last Name is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>

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
                                    <label>Mobile</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Mobile Number is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpPhone" CssClass="error" runat="server" ControlToValidate="txtPhone" ErrorMessage="Invalid Mobile Number." Display="Dynamic" ValidationGroup="vgSignup" ValidationExpression="\d{8,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Email</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revEmail" CssClass="error" runat="server" ControlToValidate="txtEmail" ErrorMessage="Invalid Email." Display="Dynamic" ValidationGroup="vgSignup" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Password</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPassword" CssClass="error" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required." Display="Dynamic" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtPassword" CssClass="error" runat="server" ErrorMessage="Minimum 6 Characters Required!" ValidationGroup="vgSignup" Display="Dynamic" ValidationExpression="^.{6,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Re-type Password</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtRePassword" runat="server" TextMode="Password"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rfvRePassword" CssClass="error" runat="server" ControlToValidate="txtRePassword" ErrorMessage="Re-type Password is required." Display="None" ValidationGroup="vgSignup"></asp:RequiredFieldValidator>--%>
                                    <asp:CompareValidator ID="cmpRePassword" CssClass="error" runat="server" ErrorMessage="Password and Re-type Password must be same." Display="Dynamic" ValidationGroup="vgSignup" ControlToCompare="txtPassword" ControlToValidate="txtRePassword"></asp:CompareValidator>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Partner Code (optional)</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtPartner" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="single-row  cf">
                                <div class="checkbox-outer">
                                    <asp:CheckBox ID="chkTerms" runat="server" ClientIDMode="Static" />
                                    <label for="chkTerms">Agree to <a href="disclosure-agreement.aspx" style="text-decoration: underline;" target="_blank">Terms & Conditions</a></label>
                                </div>
                            </div>
                            <div class="single-row submit-row cf">
                                <div class="right-side">
                                    <asp:Button ID="btnSubmit" runat="server" ValidationGroup="vgSignup" Text="Sign Up" OnClick="btnSubmit_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
