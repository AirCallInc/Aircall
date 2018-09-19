<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="checkout.aspx.cs" Inherits="Aircall.client.checkout" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var config = {
            '.chosen-select': {},
            '.chosen-select-deselect': { allow_single_deselect: true },
            '.chosen-select-no-single': { disable_search_threshold: 10 },
            '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
            '.chosen-select-width': { width: "95%" }
        }
        for (var selector in config) {
            $(selector).chosen(config[selector]);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Checkout</h1>
                </div>
                <div class="border-block">
                    <div class="main-from">
                        <div class="single-row cf">
                            <div class="fullrow">
                                <div id="dvErr" class="err" runat="server"></div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">First Name<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic"
                                        ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtFirstName"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">Last Name<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic"
                                        ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtLastName"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">Company</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtCompany" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">Address<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvtxtAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">State<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:DropDownList ID="drpState" CssClass="chosen-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvState" CssClass="error" runat="server" ControlToValidate="drpState" ErrorMessage="Required" Display="Dynamic" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">City<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:UpdatePanel ID="UPCity" runat="server" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                function jScriptmsg() {
                                                    var config = {
                                                        '.chosen-select': {},
                                                        '.chosen-select-deselect': { allow_single_deselect: true },
                                                        '.chosen-select-no-single': { disable_search_threshold: 10 },
                                                        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
                                                        '.chosen-select-width': { width: "95%" }
                                                    }
                                                    for (var selector in config) {
                                                        $(selector).chosen(config[selector]);
                                                    }
                                                }
                                                Sys.Application.add_load(jScriptmsg);
                                            </script>
                                            <asp:DropDownList ID="drpCity" CssClass="chosen-select" runat="server"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvCity" CssClass="error" runat="server" ControlToValidate="drpCity" ErrorMessage="Required" Display="Dynamic" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpState" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">Zip Code<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic"
                                        ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Phone</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="15"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="error" runat="server" ControlToValidate="txtPhone"
                                        ErrorMessage="Invalid Phone Number." ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Mobile</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtMobile" runat="server" MaxLength="15"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPhone" CssClass="error" runat="server" ControlToValidate="txtMobile"
                                        ErrorMessage="Mobile Number is required." ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpPhone" CssClass="error" runat="server" ControlToValidate="txtMobile"
                                        ErrorMessage="Invalid Phone Number." ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row button-bar cf">
                            <asp:Button ID="btnSave" Text="Proceed to make a payment" CssClass="main-btn" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                            <%--<button type="submit" class="main-btn">Proceed to make a payment</button>--%>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
