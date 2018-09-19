<%@ Page Title="" Language="C#" MasterPageFile="~/Front.Master" AutoEventWireup="true" CodeBehind="Plan.aspx.cs" Inherits="Aircall.Plan" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      
     <!-- banner part --> 
           <div class="banner-product" id="BanngerImg" runat="server" src="">
            <div class="container">
                <h1> <asp:Literal ID="ltBannerText" runat="server"></asp:Literal></h1>
            </div>
        </div>

    <!-- content area part --> 
        <div id="content-area">
             <asp:Literal ID="ltBanner" runat="server"></asp:Literal>
             <asp:Literal ID="ltMiddle" runat="server"></asp:Literal>
             <asp:Literal ID="ltContent" runat="server"></asp:Literal>
             <asp:Literal ID="ltBottom" runat="server"></asp:Literal>
            </div>
</asp:Content>
