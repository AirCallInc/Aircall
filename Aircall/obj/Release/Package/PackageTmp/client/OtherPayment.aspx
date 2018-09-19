<%@ Page Title="Checkout" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="OtherPayment.aspx.cs" Inherits="Aircall.client.OtherPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @media only screen and (max-width: 800px) {
            #sample_1 {
                width: 100%;
            }
                /* Force table to not be like tables anymore */
                #sample_1 table,
                #sample_1 thead,
                #sample_1 tbody,
                #sample_1 th,
                #sample_1 td,
                #sample_1 tr {
                    display: block;
                }

                    /* Hide table headers (but not display: none;, for accessibility) */
                    #sample_1 thead tr {
                        position: absolute;
                        top: -9999px;
                        left: -9999px;
                    }

                #sample_1 tr {
                    border: 1px solid #ccc;
                }

                #sample_1 td {
                    /* Behave  like a "row" */
                    border: none;
                    border-bottom: 1px solid #eee;
                    position: relative;
                    padding-left: 50%;
                    white-space: normal;
                    text-align: left;
                    width: inherit !important;
                }

                    #sample_1 td:before {
                        /* Now like a table header */
                        position: absolute;
                        /* Top/left values mimic padding */
                        top: 6px;
                        left: 6px;
                        width: 45%;
                        padding-right: 10px;
                        white-space: nowrap;
                        text-align: left;
                        font-weight: bold;
                    }

                    #sample_1 td:before {
                        content: attr(data-title);
                    }
        }
    </style>
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
                                <div id="dvErr" class="err" style="display: none;" runat="server"></div>
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
                                        ValidationGroup="vgSave" CssClass="error" ControlToValidate="txtFirstName"></asp:RequiredFieldValidator>
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
                                        ValidationGroup="vgSave" CssClass="error" ControlToValidate="txtLastName"></asp:RequiredFieldValidator>
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
                                    <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvtxtAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="vgSave" CssClass="error" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">State<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="select-outer max290">
                                    <asp:DropDownList ID="drpState" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="vgSave" CssClass="error" ControlToValidate="drpState" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label class="control-label">City<span class="required">*</span></label>
                            </div>
                            <div class="right-side">
                                <div class="select-outer max290">
                                    <%--<asp:DropDownList ID="drpCity" runat="server"></asp:DropDownList>--%>
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
                                            <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="vgSave" CssClass="error" ControlToValidate="drpCity" InitialValue="0"></asp:RequiredFieldValidator>
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
                                    <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="vgSave" CssClass="error" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Phone</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Mobile</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtMobile" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>                        
                    </div>
                    <div class="new-card-detail">
                        <div class="main-from">
                            <div class="old-payment-detail cf dis-table">
                                <asp:HiddenField ID="hdnCardMode" ClientIDMode="Static" runat="server" />
                                <asp:HiddenField ID="hdnCardId" ClientIDMode="Static" runat="server" />
                                <table id="sample_1" cellspacing="0" cellpadding="0" border="none" class="common-table history-service-table">
                                    <thead>
                                        <tr>
                                            <th>Name On Card</th>
                                            <th>Card Number</th>
                                            <th>Expire Month</th>
                                            <th>Expire Year</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstPaymentMethod" runat="server" OnItemCommand="lstPaymentMethod_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%#Eval("NameOnCard") %></td>
                                                    <td><%#Eval("CardNumber") %></td>
                                                    <td><%#Eval("ExpiryMonth") %></td>
                                                    <td><%#Eval("ExpiryYear") %></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="SelectCard" CommandArgument='<%#Eval("Id") %>'>Select</asp:LinkButton></td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyDataTemplate>
                                                <tr>
                                                    <td colspan="6" style="padding-left: 0;text-align: center;">No Payment method found.</td>
                                                </tr>
                                            </EmptyDataTemplate>
                                        </asp:ListView>
                                    </tbody>
                                </table>
                            </div>
                            <div class="new-card-detail">
                                <div id="dvMessage" runat="server" visible="false"></div>
                                <div class="single-row select-method cf">
                                    <div class="left-side">
                                        <label>Select Payment Method</label>
                                    </div>
                                    <div class="right-side">
                                        <div class="radio-outer-dot">
                                            <asp:UpdatePanel ID="UpdatePanel1" ClientIDMode="Static" runat="server">
                                                <ContentTemplate>
                                                    <script type="text/javascript">
                                                        function jScriptmsg() {
                                                            $(".radio-outer-dot").buttonset();
                                                        }
                                                        Sys.Application.add_load(jScriptmsg);
                                                    </script>
                                                    <input type="radio" id="rblVisa" runat="server" clientidmode="static" name="radio"><label for="rblVisa"><img src="images/card-visa.png" alt="" title=""></label>
                                                    <input type="radio" id="rblMaster" runat="server" clientidmode="static" name="radio"><label for="rblMaster"><img src="images/card-master.png" alt="" title=""></label>
                                                    <input type="radio" id="rblDiscover" runat="server" clientidmode="static" name="radio"><label for="rblDiscover"><img src="images/card-discover.png" alt="" title=""></label>
                                                    <input type="radio" id="rblAmex" runat="server" clientidmode="static" name="radio"><label for="rblAmex"><img src="images/card-other.png" alt="" title=""></label>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
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
                                        <div class="select-outer scroll-select max100 drpMonth">
                                            <asp:DropDownList ID="drpMonth" runat="server"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvMonth" CssClass="error" runat="server" ControlToValidate="drpMonth" ErrorMessage="*" Display="Dynamic" ValidationGroup="vgSave" InitialValue="0"></asp:RequiredFieldValidator>
                                        </div>
                                        <span>/</span>
                                        <div class="select-outer scroll-select max100 drpYear">
                                            <asp:DropDownList ID="drpYear" runat="server"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvYear" CssClass="error" runat="server" ControlToValidate="drpYear" ErrorMessage="*" Display="Dynamic" ValidationGroup="vgSave" InitialValue="0"></asp:RequiredFieldValidator>
                                        </div>
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
                                <div class="single-row  cf">
                                    <div class="checkbox-outer">
                                        <asp:CheckBox ID="chkTerms" runat="server" ClientIDMode="Static" />
                                        <label for="chkTerms">Agree to <a href="terms-condition.aspx" style="text-decoration: underline;" target="_blank">Terms & Conditions</a></label>
                                    </div>
                                </div>
                                <div class="single-row button-bar cf">
                                    <asp:Button ID="Button1" runat="server" CssClass="main-btn" Text="Submit" ValidationGroup="vgSave" OnClick="btnSave_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
