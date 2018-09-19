<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="NotFound.aspx.cs" Inherits="Aircall.admin.NotFound" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Page Not Found</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Oops!!</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class=" icon-ban-circle"></i>404 Error Page</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a><a href="javascript:;" class="icon-remove"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="error-page">
                            <img src="./img/404.png" alt="404 error" /><h1><strong>Oops!!</strong><br />
                                Page Not Found </h1>
                            <p>So sorry you reached this page… Some error occurred and we need your help to resolve it. Please press back and try again to see if this page comes up again. If it does,<br /> please document the steps you took to get here and report to <a href="mailto:support@ibcnet.com">Admin</a>. We appreciate your help!</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>