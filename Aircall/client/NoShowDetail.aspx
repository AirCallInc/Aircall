<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="NoShowDetail.aspx.cs" Inherits="Aircall.client.NoShowDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>No Show Service Detail</h1>
                </div>
                <div class="border-block">
                    <div class="main-from">
                        <div class="single-row cf" runat="server" id="dvNoShow">
                            <div class="left-side">
                                <label>Service No :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceNo" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvPart">
                            <div class="left-side">
                                <label>Reason :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrReson" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvUnit">
                            <div class="left-side">
                                <label>Schedule Date :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrScheduleDate" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvUnit1">
                            <div class="left-side">
                                <label>Employee :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrEmp" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Message :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrMessage" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Amount :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrAmount" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row button-bar cf">
                            <asp:HiddenField ID="hdnServiceType" runat="server" />
                            <asp:Button ID="btnPayment" class="main-btn" runat="server" Text="Make Payment" OnClick="btnPayment_Click" />                            
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
