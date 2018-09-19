<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="lowstock_report.aspx.cs" Inherits="Aircall.admin.lowstock_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Low Stock Report</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Low Stock Report</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-bar-chart"></i>&nbsp;Low Stock Report</h4>
                    </div>
                    <div class="widget-body">
                        <table class="table table-striped table-bordered" id="sample_133">
                            <thead>
                                <tr>
                                    <th class="hidden-phone srno">Sr No.</th>
                                    <th>Part Name</th>
                                    <th>Remaining Quantity</th>
                                    <th>Minimum Re-order Quantity</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstParts" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td class="srno hidden-phone"><%# Container.DataItemIndex + 1 %></td>
                                            <td><a href="<%=Application["SiteAddress"]%>admin/Part_AddEdit.aspx?PartId=<%# Eval("Id") %>"><%# Eval("Name") + "-" + Eval("Size") %></a></td>
                                            <td><%# Eval("RemainingQty") %></td>
                                            <td><%# Eval("MinReorderQuantity") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
