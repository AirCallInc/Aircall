<%@ Page Title="" Language="C#" MasterPageFile="~/Front.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Aircall._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- banner part -->
    <!-- banner part -->
    <div class="banner-home">
        <div class="banner-slide" style="background-image: url(images/home-banner1.jpg)">
            <div class="container">
                <div class="banner-caption">
                    <div class="title">
                        <h1>Worry-free air conditioning.</h1>
                    </div>

                    <p>Simple, fixed-cost, monthly HVAC maintenance plans for everyone.</p>
                    <a class="yellow-btn" href="signup.aspx">Get Started</a>
                </div>
            </div>
        </div>
    </div>


    <!-- content area part -->
    <div id="content-area">

        <!--Benefits  -->
        <div class="benefits cf">
            <div class="benefits-right equal" style="height: 828px; text-align: center; padding-bottom: 25px; padding-top: 25px; background-color: #ffffff;">
                <img src="images/android.png" /></div>
            <div class="benefits-left equal dis-table">
                <div class="benefits-right-inner dis-table-cell">
                    <div class="benefit-caption" style="text-align: center;">
                        <h3>Download from:</h3>
                        <p><a class="app-store" href="https://itunes.apple.com/us/app/zealttt/id658889707?ls=1&amp;mt=8" target="_blank">
                            <img title="" alt="" src="images/google.png"></a></p>
                    </div>
                </div>
            </div>
        </div>
        <!--testimonial -->
        <div class="testimonial cf">
            <div class="testimonial-left equal" style="height: 828px; text-align: center; padding-bottom: 25px; padding-top: 25px; background-color: #ffffff;">
                <img src="images/iphone.png" /></div>
            <div class="testimonial-right equal dis-table">
                <div class="testimonial-right-inner dis-table-cell">
                    <div class="testimonial-slider">
                        <div class="testimonial-slider-inner">
                            <div class="single-customer">
                                <h3>Download from:</h3>
                                <p><a class="app-store" href="https://itunes.apple.com/us/app/zealttt/id658889707?ls=1&amp;mt=8" target="_blank">
                                    <img title="" alt="" src="images/apple.png"></a></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    </div>
</asp:Content>
