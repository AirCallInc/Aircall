<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeRatingReview_View.aspx.cs" Inherits="Aircall.admin.EmployeeRatingReview_View" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/jstarbox.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Rating & Reviews </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="<%=Application["SiteAddress"]%>admin/EmployeeRatingReview_List.aspx">Rating & Reviews List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Rating & Reviews</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-star"></i>&nbsp;Rating & Reviews</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div id="dvError" runat="server" visible="false" class="alert alert-error">
                        </div>
                        <div class="form-horizontal">
                            <div class="widget" id="">
                                <div class="widget-title">
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Ratings</h4>
                                </div>
                                <div class="widget-body">
                                    <table width="50%">
                                        <tbody>
                                            <tr>
                                                <td width="50%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="viewlabel">Service Case #:</td>
                                                            <td><asp:Literal ID="ltrServiceCase" runat="server"></asp:Literal> </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="viewlabel">Client Name:</td>
                                                            <td><asp:Literal ID="ltrClient" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="viewlabel">Ratings:</td>
                                                            <td><div class="starbox" data-rate="" id="dvRating" runat="server"></div></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="widget" id="">
                                <div class="widget-title">
                                    <h4 style="float:left;"><i class="icon-comments-alt"></i>&nbsp;Reviews</h4>
                                </div>
                                <div class="widget-body">
                                    <ul class="chats normal-chat">
                                        <li class="in">
                                            <asp:Image ID="imgClient" class="avatar" runat="server" ImageUrl="" />
                                            <div class="message ">
                                                <span class="arrow"></span><a id="lnkClient" runat="server" href="" class="name"><asp:Literal ID="ltrClientName2" runat="server"></asp:Literal></a>
                                                <span class="datetime">at  <asp:Literal ID="ltrReviewDate" runat="server"></asp:Literal> </span>
                                                <span class="body"><asp:Literal ID="ltrReview" runat="server"></asp:Literal>
                                                </span>
                                            </div>
                                        </li>
                                        <li class="out" id="Employee" runat="server">
                                            <asp:Image ID="imgEmployee" class="avatar" runat="server" ImageUrl="" />
                                            <div class="message ">
                                                <span class="arrow"></span><a id="lnkEmp" runat="server" href="" class="name"><asp:Literal ID="ltrEmpName" runat="server"></asp:Literal></a>
                                                <span class="datetime">at  <asp:Literal ID="ltrNoteDate" runat="server"></asp:Literal> </span>
                                                <span class="body"><asp:Literal ID="ltrNotes" runat="server"></asp:Literal>
                                                </span>
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            /* star-rating script */
            $('.starbox').each(function () {
                var starbox = jQuery(this);
                starbox.starbox({
                    average: parseFloat(starbox.attr("data-rate")),
                    stars: 5,
                    buttons: 5, //false will allow any value between 0 and 1 to be set
                    ghosting: true,
                    changeable: false, // true, false, or "once"
                    autoUpdateAverage: true
                });
            });
        });
    </script>
</asp:Content>
