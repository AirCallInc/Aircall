<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="footer.ascx.cs" Inherits="Aircall.controls.footer" %>

<%--<footer class="main-footer">
    <div class="footer-top">
        <div class="container cf">
            <div class="single-block">
                <div class="title">Company</div>
                <ul>
                    <li><a href="<%=Application["SiteAddress"] %>our-story.aspx">Our Story</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>News.aspx">News</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>partner/">Partners</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>Contact-Us.aspx">Contact Us</a></li>
                </ul>

            </div>
            <div class="single-block">
                <div class="title">Services</div>
                <ul>
                    <li><a href="<%=Application["SiteAddress"] %>residential.aspx">Residential</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>commercial.aspx">Commercial</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>multi_family.aspx">Multi-family</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>industrial.aspx">Industrial</a></li>
                </ul>
            </div>
            <div class="single-block for-border">
                <div class="title">Company</div>
                <ul>
                    <li><a href="<%=Application["SiteAddress"] %>our-story.aspx">About Us</a></li>
                    <li><a href="<%=Application["SiteAddress"] %>News.aspx">Press Media</a></li>
                    <li><a href="">Blog</a></li>
                </ul>

            </div>
            <div class="single-block">
                <div class="title">Follow us</div>
                <div class="social-icons">
                    <a class="twitter" href="#" target="_blank"></a>
                    <a class="facebook" href="#" target="_blank"></a>
                    <a class="google-plus" href="#" target="_blank"></a>
                </div>
            </div>
        </div>
    </div>

    <div class="footer-bottom">
        <div class="container cf">
            <p>Copyright &copy; <%=DateTime.Now.Year %> AirCall All Rights Reserved.</p>
            <ul>
                <li><a href="<%=Application["SiteAddress"] %>aboutus.aspx">About Us</a></li>
                <li><a href="<%=Application["SiteAddress"] %>Contact.aspx">Contact</a></li>
            </ul>
        </div>
    </div>
</footer>--%>
<asp:Literal ID="ltrFooter" runat="server"></asp:Literal>
