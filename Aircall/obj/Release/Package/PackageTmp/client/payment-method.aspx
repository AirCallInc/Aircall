<%@ Page Title="Payment Methods" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="payment-method.aspx.cs" Inherits="Aircall.client.payment_method" %>

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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Payment Method</h1>
                    <span>
                        <asp:Panel ID="Panel1" runat="server" DefaultButton="lnkAdd">
                            <asp:ImageButton ID="lnkAdd" runat="server" OnClick="lnkAdd_Click" UseSubmitBehavior="false" OnClientClick="ShowAddCard();" ImageUrl="images/plus-icon@3x.png" Style="width: 70%; vertical-align: middle;" />
                        </asp:Panel>
                    </span>
                </div>
                <div id="dvMessage" runat="server" visible="false"></div>
                <div class="old-payment-detail cf dis-table">
                    <asp:HiddenField ID="hdnCardMode" ClientIDMode="Static" runat="server" />
                    <table id="sample_1" cellspacing="0" cellpadding="0" border="none" class="common-table history-service-table">
                        <thead>
                            <tr>
                                <th>Name On Card</th>
                                <th>Card Number</th>
                                <th>Expire Month</th>
                                <th>Expire Year</th>
                                <th>Is Default Card</th>
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
                                        <td data-title="Is Default Card"><%#Eval("IsDefaultPayment") %></td>
                                        <td data-title="Action">
                                            <asp:LinkButton CssClass="add-new-card" ID="lnkEdit" runat="server" CommandName="EditCard" CommandArgument='<%#Eval("Id") %>'>Edit</asp:LinkButton>
                                        </td>
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
                    <a class="add-new-card" href="#">
                        <asp:Literal ID="ltrCardText" runat="server"></asp:Literal></a>
                    <div class="new-card-detail-inner">
                        <div class="main-from">
                            <div id="dvMessage1" runat="server" visible="false"></div>
                            <asp:Panel runat="server" id="pnsCC">
                                <div class="single-row select-method cf">
                                    <div class="left-side">
                                        <label>Select Payment Method</label>
                                    </div>
                                    <div class="right-side">
                                        <div class="radio-outer-dot">
                                            <input type="radio" id="rblVisa" runat="server" clientidmode="static" name="radio1" checked="true"><label for="rblVisa"><img src="images/card-visa.png" alt="" title=""></label>
                                            <input type="radio" id="rblMaster" runat="server" clientidmode="static" name="radio1"><label for="rblMaster"><img src="images/card-master.png" alt="" title=""></label>
                                            <input type="radio" id="rblDiscover" runat="server" clientidmode="static" name="radio1"><label for="rblDiscover"><img src="images/card-discover.png" alt="" title=""></label>
                                            <input type="radio" id="rblAmex" runat="server" clientidmode="static" name="radio1"><label for="rblAmex"><img src="images/card-other.png" alt="" title=""></label>
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
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" CssClass="error" runat="server" ControlToValidate="txtName" ValidationGroup="vgSave" ErrorMessage="Invalid Data" Display="Dynamic" ValidationExpression="[a-zA-Z ]*$"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="single-row card-number cf">
                                    <div class="left-side">
                                        <label>Card Number</label>
                                    </div>
                                    <div class="right-side">
                                        <asp:TextBox ID="txtCardNumber" runat="server" MaxLength="16"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvCardNo" CssClass="error" runat="server" ControlToValidate="txtCardNumber" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="regExpCard" CssClass="error" runat="server" ControlToValidate="txtCardNumber" ErrorMessage="Invalid Card Number." Display="Dynamic" ValidationGroup="vgSave" ValidationExpression="\d+"></asp:RegularExpressionValidator>
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
                                        <asp:TextBox ID="txtCVV" ClientIDMode="Static" runat="server" MaxLength="4" autocomplete="off"></asp:TextBox>
                                        <%--<asp:RequiredFieldValidator ID="rfvCVV" CssClass="error" runat="server" ControlToValidate="txtCVV" ErrorMessage="Required" Display="Dynamic" ValidationGroup="vgSave"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="regCVV" CssClass="error" runat="server" ControlToValidate="txtCVV" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="vgSave" ValidationExpression="\d+"></asp:RegularExpressionValidator>--%>
                                    </div>
                                </div>
                                <div class="single-row cf">
                                    <div class="left-side">
                                        <label>Is Default</label>
                                    </div>
                                    <div class="right-side">
                                        <div class="checkbox-outer">
                                            <asp:CheckBox ID="chkIsDefault" runat="server" Text="Default" />
                                            <asp:CheckBox ID="hdnChkIsDefault" runat="server" Visible="false" />
                                        </div>
                                    </div>
                                </div>
                                <div class="single-row button-bar cf">
                                    <asp:Button ID="btnSave" runat="server" CssClass="main-btn" Text="Submit" ValidationGroup="vgSave" OnClick="btnSave_Click" />
                                    <asp:Button ID="btnUpdate" runat="server" CssClass="main-btn" Text="Update" ValidationGroup="vgSave" Visible="false" OnClick="btnUpdate_Click" />
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(window).load(function () {
            if ($("#hdnCardMode").val() == 1) {
                if ($(this).hasClass('open')) {
                    $(this).removeClass('open')
                    $('.new-card-detail-inner').slideUp();
                }
                else {
                    $(this).addClass('open')                   
                    $('.new-card-detail-inner').slideDown();
                    goToByScroll("new-card-detail-inner");
                }
            }
            $(".select-method input").bind("click", function () {
                if ($(this).attr("Id") == "rblAmex")
                    $("#txtCVV").attr("maxLength", "4");
                else
                    $("#txtCVV").attr("maxLength", "3");

                $("#txtCVV").val("");
            });
        });

        function ShowAddCard() {
            $("#hdnCardMode").val(0);
            if ($(this).hasClass('open')) {
                $(this).removeClass('open')
                $('.new-card-detail-inner').slideUp();
            }
            else {
                $(this).addClass('open')
                $('.new-card-detail-inner').slideDown();
                goToByScroll("new-card-detail-inner");
            }
        }
        function goToByScroll(id) {
            // Remove "link" from the ID
            id = id.replace("link", "");
            // Scroll
            $('html,body').animate({
                scrollTop: $("." + id).offset().top
            },'slow');
        }
    </script>
</asp:Content>
