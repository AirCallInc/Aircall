<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="SalesPerDay.aspx.cs" Inherits="Aircall.admin.SalesPerDay" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=Application["SiteAddress"]%>admin/js/highcharts.js"></script>
    <script src="<%=Application["SiteAddress"]%>admin/js/exporting.js"></script>
    <script src="https://code.highcharts.com/modules/data.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Sales Report</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Sales Report Per Day</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <table class="table table-striped table-bordered" id="sample_12">
                    <thead>
                        <tr>
                            <th>Sr. No.</th>
                            <th>Client Name</th>
                            <th>Company</th>
                            <th>Charge Time</th>
                            <th>Charge Amount</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:ListView ID="lstSales" runat="server">
                            <ItemTemplate>
                                <tr class="odd gradeX" runat="server" id="trTd">
                                    <td><%# Container.DataItemIndex + 1 %>
                                    </td>
                                    <td><%#Eval("ClientName") %></td>
                                    <td><%#Eval("Company") %></td>
                                    <td>
                                        <%#Eval("SaleDay") %>
                                    </td>
                                    <td><%#Eval("SaleAmount") %></td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="8">No Data Found</td>
                                </tr>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
