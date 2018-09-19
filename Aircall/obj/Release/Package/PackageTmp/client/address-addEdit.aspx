<%@ Page Title="Address Add/Edit" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="address-addEdit.aspx.cs" Inherits="Aircall.client.address_addEdit" %>

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

        $(document).ready(function () {
            $("#chkIsDefault").on("click", function () {
                if ($("#chkIsDefault").is("checked") == false && $("#hdnIsDefault").val().toLowerCase() == "true") {
                    if ($("#dvMessage")) {
                        $("#dvMessage").text("Please select another address as Default first.");
                        $("#dvMessage").addClass("error");
                    } else {
                        $("#dvMsg").text("Please select another address as Default first.");
                        $("#dvMsg").addClass("error");
                    }
                    
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Add Your Address</h1>
                </div>
                <div class="border-block">
                    <div class="main-from">
                        <div id="dvMsg"></div>
                        <div id="dvMessage" clientidmode="static" runat="server" visible="false"></div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Address</label>
                            </div>
                            <div class="right-side">
                                <div class="select-outer max290">
                                    <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAddress" CssClass="error" runat="server" ControlToValidate="txtAddress" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgAddress"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>State</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:DropDownList ID="drpState" CssClass="chosen-select" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvState" CssClass="error" runat="server" ControlToValidate="drpState" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgAddress" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>City</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:UpdatePanel ID="UPCity" runat="server" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                function jScriptmsg() {
                                                    //$(".select-outer select").selectmenu();
                                                    //$(".scroll-select select").selectmenu().selectmenu("menuWidget").addClass("overflow");
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
                                            <asp:RequiredFieldValidator ID="rfvCity" CssClass="error" runat="server" ControlToValidate="drpCity" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgAddress" InitialValue="0"></asp:RequiredFieldValidator>
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
                                <label>Zip Code</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvZip" CssClass="error" runat="server" ControlToValidate="txtZip" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgAddress"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpZip" CssClass="error" runat="server" ControlToValidate="txtZip" ErrorMessage="Invalid Zip." Display="Dynamic" ValidationGroup="vgAddress" ValidationExpression="\s*\d+\s*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Is Default Address</label>
                            </div>
                            <div class="right-side">
                                <div class="checkbox-outer">
                                    <asp:HiddenField ID="hdnIsDefault" ClientIDMode="Static" runat="server" />
                                    <asp:CheckBox ID="chkIsDefault" ClientIDMode="Static" runat="server" Text="Default" />
                                </div>
                            </div>
                        </div>
                        <div class="single-row button-bar cf">
                            <asp:Button ID="btnSubmit" runat="server" CssClass="main-btn" ValidationGroup="vgAddress" Text="Submit" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnUpdate" runat="server" CssClass="main-btn" Text="Update" ValidationGroup="vgAddress" Visible="false" OnClick="btnUpdate_Click" />
                            <input type="button" class="main-btn dark-grey" value="Cancel" onclick="location.href = 'address-list.aspx'" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
