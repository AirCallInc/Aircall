<%@ Page Title="Summary" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="summary.aspx.cs" Inherits="Aircall.client.summary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @media only screen and (max-width: 800px) {

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
                height:auto;
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

                /*
	Label the data
	*/
                #sample_1 td:before {
                    content: attr(data-title);
                }
        }

        .removeunit td {
            background-color: #eed3d7 !important;
            color: #b94a48 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">

            <div class="container">
                <div class="title">
                    <h1>Summary</h1>
                </div>
                <div class="receipt-name">
                    <div class="name">
                        <asp:Literal ID="litName" runat="server"></asp:Literal>
                    </div>
                    <span>
                        <asp:Literal ID="litEmail" runat="server"></asp:Literal></span>
                </div>
                <div class="table-outer">
                    <div id="dvMessage" runat="server" visible="false">
                    </div>
                    <table class="common-table receipt-table" id="sample_1">
                        <asp:ListView ID="lstSummary" runat="server" OnItemDataBound="lstSummary_ItemDataBound" OnDataBound="lstSummary_DataBound" OnItemCommand="lstSummary_ItemCommand">
                            <LayoutTemplate>
                                <thead>
                                    <tr>
                                        <th>Unit Name</th>
                                        <th>Package/Plan</th>
                                        <th>Description</th>
                                        <th>Payment Type</th>
                                        <th>Price</th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr runat="server" id="itemPlaceholder" />
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan="6" class="total-amount" style="text-align: right; padding-right: 15px;">Total Amount:
                                            <br>
                                            <big><strong><asp:Literal ID="litTotal" runat="server"></asp:Literal></strong></big>
                                        </td>
                                    </tr>
                                </tfoot>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr runat="server" id="tr">
                                    <td data-title="Unit Name">
                                        <asp:Literal ID="litUnitName" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Package/Plan">
                                        <asp:Literal ID="litPlan" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Description">
                                        <asp:Literal ID="litDesc" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Payment Type" style="width: 10%;">
                                        <asp:Literal ID="litPlanType" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Price" style="width: 10%;">
                                        <asp:Literal ID="litAmount" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Action">
                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="RemoveUnit" CommandArgument='<%#Eval("Id") %>'>Remove</asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </table>
                </div>
                <asp:LinkButton ID="lnkAddunit" runat="server" CssClass="main-btn dark-grey" OnClick="lnkAddunit_Click">Add Another Unit</asp:LinkButton>
                <%--<a class="main-btn dark-grey" href="add-ac-unit.aspx">Add Another Unit</a>--%>
                <asp:LinkButton ID="lnkcheckout" CssClass="main-btn" PostBackUrl="checkout.aspx" runat="server">Checkout</asp:LinkButton>
                <%--<a class="main-btn" href="checkout.aspx" runat="server" id="lnkcheckout">Checkout</a>--%>
            </div>
        </div>
    </div>
</asp:Content>
