<%@ Page Title="" Language="C#" MasterPageFile="~/Front.Master" AutoEventWireup="true" CodeBehind="aboutus.aspx.cs" Inherits="Aircall.aboutus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- banner part --> 
        <div class="banner" style="background-image:url('images/product-banner.jpg')">
            <div class="container">
                <h1>OUr story</h1>
            </div>
        </div>

        <!-- content area part --> 
        <div id="content-area">
            <div class="common-content">
                <div class="container">
                    
                    <img src="images/about-img.jpg" alt="" title="" class="alignright">
                   
                    <p>Providing air conditioning service is the majority of our daily business operations for over a decade.  Our trained professional technicians are certified for all major manufacturers installation, repair and maintenance tasks.   When asking our clients what is their biggest concern, unanimously they voted the costs creeping from unexpected repairs. The need for a full service, fixed cost, simple, and manageable solution was apparent. AirCall was established to answer this challenge...</p>
                    
                    <p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras aliquam hendrerit leo at congue. Phasellus tempor, urna quis placerat dapibus, metus enim cursus mi, placerat bibendum sapien odio nec arcu. Praesent et orci porta, aliquam dolor at, viverra est. Vestibulum convallis, massa quis tristique tempor, leo lorem volutpat diam, in faucibus risus massa ut ligula.  Praesent et orci porta, aliquam dolor at, viverra est. Vestibulum convallis, massa quis tristique tempor, leo lorem volutpat diam, in faucibus risus massa ut ligula.</p>
                    
                </div>
            </div>
            <div class="our-services">
                <div class="container">
                    <h3>OUr services</h3>
                    <div class="all-services cf">
                        <div class="single-service">
                            <a href="industrial.aspx" class="single-service-inner">
                                <img src="images/img-industrial.png" alt="" title="">
                                <h3>Industrial</h3>
                            </a>
                        </div>
                        <div class="single-service">
                            <a href="commercial.aspx" class="single-service-inner">
                                <img src="images/img-commercial.png" alt="" title="">
                                <h3>commercial</h3>
                            </a>
                        </div>
                        <div class="single-service">
                            <a href="multi_family.aspx" class="single-service-inner">
                                <img src="images/img-multi-family.png" alt="" title="">
                                <h3>multi-family</h3>
                            </a>
                        </div>
                        <div class="single-service">
                            <a href="residential.aspx" class="single-service-inner">
                                <img src="images/img-residential.png" alt="" title="">
                                <h3>residential</h3>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
</asp:Content>
