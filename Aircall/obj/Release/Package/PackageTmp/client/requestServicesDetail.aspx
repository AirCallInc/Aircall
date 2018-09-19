<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="requestServicesDetail.aspx.cs" Inherits="Aircall.client.requestServicesDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Service Detail</h1>
                </div>
                <div class="border-block">
                    <div id="dvMessage" runat="server" visible="false">
                    </div>
                    <div class="main-from">
                        <div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Service Case</label>
                                </div>
                                <div class="right-side">
                                    <asp:Literal ID="ltrServiceNo" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Address</label>
                                </div>
                                <div class="right-side">
                                    <asp:Literal ID="ltrAddress" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="single-row schedule-time cf">
                                <div class="left-side">
                                    <label>Plan</label>
                                </div>
                                <div class="right-side">
                                    <div class="max290">
                                        <asp:Literal ID="ltrPlan" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Unit</label>
                                </div>
                                <div class="right-side">
                                    <div class="max290">
                                        <asp:Literal ID="ltrUnits" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Schedule Date</label>
                                </div>
                                <div class="right-side">
                                    <div class="datepicker-outer max290">
                                        <asp:Literal ID="txtDate" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row schedule-time cf">
                                <div class="left-side">
                                    <label>Schedule Time</label>
                                </div>
                                <div class="right-side">
                                    <div class="max360">
                                        <asp:Literal ID="ltrTimeSlot" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>

                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Request For</label>
                                </div>
                                <div class="right-side">
                                    <div class="max290">
                                        <asp:Literal ID="ltrPurposeOfVisit" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                            <div class="single-row cf">
                                <div class="left-side">
                                    <label>Notes</label>
                                </div>
                                <div class="right-side">
                                    <asp:TextBox ID="txtNotes" TextMode="MultiLine" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
