<%@ Page Title="Payment" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="PaymentMethod.aspx.cs" Inherits="Aircall.client.PaymentMethod" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }

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

        .flt {
            float: left;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hdnpdfurl" runat="server" />
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Payment</h1>
                </div>
                <div class="technician-block">
                    <div class="profile-image">
                        <span><strong>Monthly Payment: $<asp:Literal ID="ltrAmount" runat="server"></asp:Literal>
                        </strong>
                        </span>
                    </div>
                </div>
                <div class="border-block">
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
                                                    <td data-title="Name On Card"><%#Eval("NameOnCard") %></td>
                                                    <td data-title="Card Number"><%#Eval("CardNumber") %></td>
                                                    <td data-title="Expire Month"><%#Eval("ExpiryMonth") %></td>
                                                    <td data-title="Expire Year"><%#Eval("ExpiryYear") %></td>
                                                    <td data-title="Action">
                                                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="SelectCard" CommandArgument='<%#Eval("Id") %>'>Select</asp:LinkButton></td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyDataTemplate>
                                                <tr>
                                                    <td colspan="6" style="padding-left: 0; text-align: center;">No Payment method found.</td>
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
                                                    <input type="radio" id="rblVisa" runat="server" clientidmode="static" name="radio" checked><label for="rblVisa"><img src="images/card-visa.png" alt="" title=""></label>
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
                                        <asp:TextBox ID="txtCVV" runat="server" MaxLength="4" ClientIDMode="Static"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvCVV" CssClass="error" runat="server" ControlToValidate="txtCVV" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="regCVV" CssClass="error" runat="server" ControlToValidate="txtCVV" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="vgSave" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="single-row cf">
                                    <div class="checkbox-outer flt max290">
                                        <asp:CheckBox ID="chkTerms" runat="server" ClientIDMode="Static" />
                                        <label for="chkTerms">Agree to <a href="#" onclick="return openPdf();" style="text-decoration: underline;" target="_blank">Sales Agreement</a></label>
                                    </div>
                                    <div class="checkbox-outer flt max290">
                                        <asp:HyperLink ID="lnkPdf" runat="server" style="display:none;" ClientIDMode="Static" Target="_blank">pdf</asp:HyperLink>
                                        <asp:ImageButton ID="imgPdf" runat="server" ImageUrl="~/client/images/mail.png" OnClick="imgPdf_Click" Style="width: 36px;" />
                                    </div>
                                </div>
                                <div class="single-row button-bar cf">  
                                    <asp:HiddenField runat="server" ID="hdfAmount" />
                                    <asp:HiddenField runat="server" ID="hdfUnitIds" />
                                    <asp:HiddenField ID="hdfPricePerMonth" runat="server" />
                                    <asp:HiddenField ID="hdfTotalUnits" runat="server" />
                                    <asp:Button ID="btnSave" runat="server" CssClass="main-btn" Text="Submit" ValidationGroup="vgSave" OnClick="btnSave_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function openPdf() {           
            window.open($("#lnkPdf").attr("href"), "Sales Agreement", "width=900,height=700");
            return false;
        }
        $(window).load(function () {
            $(".select-method input").bind("click", function () {
                if ($(this).attr("Id") == "rblAmex")
                    $("#txtCVV").attr("maxLength", "4");
                else
                    $("#txtCVV").attr("maxLength", "3");

                $("#txtCVV").val("");
            });
        });
        
    </script>
</asp:Content>
