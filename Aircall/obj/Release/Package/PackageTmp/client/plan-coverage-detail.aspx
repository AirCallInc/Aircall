<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="plan-coverage-detail.aspx.cs" Inherits="Aircall.client.plan_coverage_detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <asp:Literal ID="ltrCSS" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>
                        <asp:Literal ID="ltrPlanName" runat="server"></asp:Literal></h1>
                </div>
                <div class="pricing-coverage-block cf">
                    <div class="pricing-table-block">
                        <div class="pricing-table-item">
                            <div class="pricing-table-title" runat="server" id="dvA">
                                <asp:Literal ID="ltrPackageNameA" runat="server"></asp:Literal>
                            </div>
                            <div class="pricing-table-price" runat="server" id="dvA1">
                                <big runat="server" id="bigA"><sup>$</sup><asp:Literal ID="ltrRateA" runat="server"></asp:Literal></big>
                                <small>For More than 10 years</small>
                            </div>
                            <div class="pricing-table-features">
                                <asp:Literal ID="ltrDescPackA" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                    <div class="pricing-table-block">
                        <div class="pricing-table-item">
                            <div class="pricing-table-title" runat="server" id="dvB">
                                <asp:Literal ID="ltrPackageNameB" runat="server"></asp:Literal>
                            </div>
                            <div class="pricing-table-price" runat="server" id="dvB1">
                                <big runat="server" id="bigB"><sup>$</sup><asp:Literal ID="ltrRateB" runat="server"></asp:Literal></big>
                                <small>For Less than 10 years</small>
                            </div>
                            <div class="pricing-table-features">
                                <asp:Literal ID="ltrDescPackB" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
