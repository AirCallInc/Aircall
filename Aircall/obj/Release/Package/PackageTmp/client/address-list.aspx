<%@ Page Title="Addresses" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="address-list.aspx.cs" Inherits="Aircall.client.address_list" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Your Addresses</h1>
                    <span>
                        <a href="address-addEdit.aspx">
                            <img title="" alt="" id="imgAdd" src="images/plus-icon@3x.png" style="width: 70%; vertical-align: middle;"></a>
                    </span>
                </div>
                <div id="dvMessage" runat="server" visible="false"></div>
                <div class="table-outer">

                    <table cellspacing="0" cellpadding="0" border="none" class="common-table request-service-table">
                        <thead>
                            <tr>
                                <th>Address</th>
                                <th>Default Address</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:ListView ID="lstAddress" runat="server" OnItemCommand="lstAddress_ItemCommand" OnItemDataBound="lstAddress_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("ClientAddress1") %></td>
                                        <td><%#Eval("IsDefaultAddress").ToString().ToLower()=="true"?"Yes":"No" %></td>
                                        <td class="edit-remove-btn">
                                            <asp:LinkButton ID="lnkEdit" runat="server" CssClass="main-btn" Text="Edit" CommandName="Modify" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="main-btn dark-grey" Text="Delete" CommandName="DeleteAddress" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <tr>
                                        <td colspan="3" style="padding-left: 0;text-align: center;">You have not added AC Unit's address yet. Click on + icon to add address.</td>
                                    </tr>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
