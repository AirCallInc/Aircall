<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="our-story.aspx.cs" Inherits="Aircall.our_story" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="banner-product" id="BanngerImg" runat="server" src="">
        <div class="container">
            <h1>
                <asp:Literal ID="ltBannerText" runat="server"></asp:Literal></h1>
        </div>
    </div>

    <!-- content area part -->
    <div id="content-area">
        <asp:Literal ID="ltMiddle" runat="server"></asp:Literal>
        <asp:Literal ID="ltContent" runat="server"></asp:Literal>
        <asp:Literal ID="ltBottom" runat="server"></asp:Literal>
    </div>
</asp:Content>
