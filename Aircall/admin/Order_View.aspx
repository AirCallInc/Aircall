<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Order_View.aspx.cs" Inherits="Aircall.admin.Order_View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Order Detail</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Order_List.aspx">Order List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Order Detail</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-money"></i>&nbsp;Order Informations</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div id="dvError" runat="server" visible="false" class="alert alert-error">
                        </div>
                        <div class="form-horizontal">
                            <div class="widget" id="">
                                <div class="widget-title">
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Order Detail</h4>
                                </div>
                                <div class="widget-body">
                                    <table width="100%">
                                        <tbody>
                                            <tr>
                                                <td width="50%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="control-view">Order No:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrOrderNo" runat="server"></asp:Literal></td>
                                                        </tr>
                                                         <tr>
                                                            <td class="control-view">Added Date:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrAddedDate" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="control-view">Client Name:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrClientName" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="control-view">Email:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrEmail" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="control-view">Address:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrAddress" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="control-view">State:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrState" runat="server"></asp:Literal>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="control-view">City:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrCity" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="control-view">Zip Code:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrZip" runat="server"></asp:Literal></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td width="50%">
                                                    <table width="100%">
                                                        <%--<tr>
                                                            <td class="control-view">Package Name:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrPackage" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="control-view">Unit Name:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrUnits" runat="server"></asp:Literal></td>
                                                        </tr>--%>
                                                        <tr>
                                                            <td class="control-view">Employee Name:
                                                            </td>
                                                            <td>
                                                                <asp:Literal ID="ltrEmployee" runat="server"></asp:Literal></td>
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
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Item Detail</h4>
                                </div>
                                <div class="widget-body">
                                    <table class="table table-striped table-bordered" width="100%">
                                        <thead>
                                            <tr>
                                                <th>Sr No</th>
                                                <th>Item Name</th>
                                                <th>Qty</th>
                                                <th>Price</th>
                                                <th>Total Price</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:ListView ID="lstPartList" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><%# Container.DataItemIndex + 1  %></td>
                                                        <td><%#Eval("PartName") %> - <%#Eval("PartSize") %></td>
                                                        <td><%#Eval("Quantity") %></td>
                                                        <td>$ <%#Eval("Amount") %></td>
                                                        <td>$ <%#Eval("TotalAmount") %></td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="widget" id="">
                                <div class="widget-title">
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Payment Detail</h4>
                                </div>
                                <div class="widget-body">
                                    <table width="100%">
                                        <tbody>
                                            <tr>
                                                <td width="50%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="control-view">Charge By:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrChargeBy" runat="server"></asp:Literal></td>
                                                        </tr>
                                                        <div id="dvCheck" runat="server">
                                                            <tr>
                                                                <td class="control-view">Check #:</td>
                                                                <td>
                                                                    <asp:Literal ID="ltrCheckNo" runat="server"></asp:Literal></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="control-view">Check Date:</td>
                                                                <td>
                                                                    <asp:Literal ID="ltrCheckDate" runat="server"></asp:Literal></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="control-view">Accounting Notes:</td>
                                                                <td>
                                                                    <asp:Literal ID="ltrAccNotes" runat="server"></asp:Literal></td>
                                                            </tr>
                                                            <tr>
                                                                <td class="control-view">Front Image:</td>
                                                                <td><a href="" id="lnkFront" runat="server" target="_blank">
                                                                    <asp:Literal ID="ltrFront" runat="server"></asp:Literal>
                                                                </a>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="control-view">Back Image:</td>
                                                                <td><a href="" id="lnkBack" runat="server" target="_blank">
                                                                    <asp:Literal ID="ltrBack" runat="server"></asp:Literal>
                                                                </a>
                                                                </td>
                                                            </tr>
                                                        </div>
                                                        <tr>
                                                            <td class="control-view">Transaction Id:</td>
                                                            <td>
                                                                <asp:Literal ID="ltrTransaction" runat="server"></asp:Literal></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td width="50%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="control-view">Subtotal Amount : $
                                                                <asp:Literal ID="ltrAmount" runat="server"></asp:Literal></td>
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
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Recommendations</h4>
                                </div>
                                <div class="widget-body">
                                    <table width="100%">
                                        <tbody>
                                            <tr>
                                                <td width="100%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="control-view">Notes : </td>
                                                            <td>
                                                                <asp:Literal ID="ltrRecommendation" runat="server"></asp:Literal></td>
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
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Client's Signature</h4>
                                </div>
                                <div class="widget-body">
                                    <table width="100%">
                                        <tbody>
                                            <tr>
                                                <td width="100%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td>
                                                                <asp:Image ID="imgSignature" runat="server" Style="height: 200px;" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class="form-actions">
                            <%--<button type="button" class="btn btn-success" id="btnSubmit" onclick="window.location.href = '<%=Application["SiteAddress"]%>admin/order_list.aspx'">Send Copy to Email</button>--%>
                            <input type="button" class="btn" value="Cancel" onclick="location.href = 'Order_List.aspx'" />
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
