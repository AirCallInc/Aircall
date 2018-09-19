<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeePart_List.aspx.cs" Inherits="Aircall.admin.EmployeePart_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Part List </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Parts</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-wrench"></i>&nbsp;Part List
                        </h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter">
                            <div class="heading searchschedule">
                                <label class="filter-label">Employee</label>
                                <asp:TextBox ID="txtEmployee" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Service #</label>
                                <asp:TextBox ID="txtCaseNo" runat="server" CssClass="input-medium"></asp:TextBox>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtStart" runat="server" class="input-small date-picker" size="16" type="text" />
                                </div>
                                <label>to</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtEnd" runat="server" class="input-small date-picker" size="16" type="text" />
                                </div>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                            </div>
                        </div>
                        <asp:LinkButton ID="lnkExpand" runat="server" OnClientClick="return ExpandAll()">Expand All</asp:LinkButton>
                        <asp:LinkButton ID="lnkCollapse" runat="server" OnClientClick="return CollapseAll()">Collaspe All</asp:LinkButton>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th class="hidden-phone srno">Service #</th>
                                    <th>Employee Name</th>
                                    <th>Service Date</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstEmployee" runat="server" OnItemDataBound="lstEmployee_ItemDataBound">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td class="srno hidden-phone">
                                                <asp:ImageButton ID="imgPlusCountry" runat="server" ImageUrl="~/images/box_plus.gif"
                                                    CssClass="imgplus_min" OnClientClick="return PlusClick(this)"></asp:ImageButton>
                                                <asp:ImageButton ID="imgMinusCountry" runat="server" ImageUrl="~/images/box_minus.gif"
                                                    Style="display: none" CssClass="imgMinus_min" OnClientClick="return MinusClick(this)"></asp:ImageButton>
                                                <%#Eval("ServiceCaseNumber") %></td>
                                            <td><%#Eval("EmployeeName") %></td>
                                            <td><%#Eval("ScheduleDate","{0:MM/dd/yyyy}") %></td>
                                        </tr>
                                        <tr class="odd gradeX partList" style="display: none;">
                                            <td colspan="3">
                                                <table class="table table-striped table-bordered">
                                                    <thead>
                                                        <tr>
                                                            <th>Part Name</th>
                                                            <th>Quantity</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <asp:ListView ID="lstEmpParts" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td><%#Eval("Name") %> - <%#Eval("Size") %></td>
                                                                    <td><%#Eval("PartQuantity") %></td>
                                                                </tr>
                                                            </ItemTemplate>
                                                            <EmptyDataTemplate>
                                                                <tr>
                                                                    <td colspan="2">No Data Found</td>
                                                                </tr>
                                                            </EmptyDataTemplate>
                                                        </asp:ListView>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>

                                    </ItemTemplate>
                                </asp:ListView>

                                <%--<tr class="odd gradeX" id="trMichael" runat="server" style="display: none;">
                                    <td colspan="3">
                                        <table class="table table-striped table-bordered">
                                            <thead>
                                                <tr class="trShow">
                                                    <th>Part Name</th>
                                                    <th>Quantity</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="trWhite">
                                                    <td>Exhaust Hose</td>
                                                    <td>5</td>
                                                </tr>
                                                <tr class="trWhite">
                                                    <td>Thermistor</td>
                                                    <td>6</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                                <tr class="even gradeX">
                                    <td class="srno hidden-phone">
                                        <asp:ImageButton ID="imgPlusCountry1" runat="server" ImageUrl="~/images/box_plus.gif"
                                            CssClass="imgplus_min"></asp:ImageButton>
                                        <asp:ImageButton ID="imgMinusCountry1" runat="server" ImageUrl="~/images/box_minus.gif"
                                            Style="display: none" CssClass="imgplus_min"></asp:ImageButton>
                                        C2-90038-S1</td>
                                    <td>Mike Johnson</td>
                                    <td>25-March-2016</td>
                                </tr>
                                <tr class="odd gradeX" id="trMike" runat="server" style="display: none;">
                                    <td colspan="3">
                                        <table class="table table-striped table-bordered">
                                            <thead>
                                                <tr class="trShow">
                                                    <th>Part Name</th>
                                                    <th>Quantity</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="trWhite">
                                                    <td>Filter</td>
                                                    <td>7</td>
                                                </tr>
                                                <tr class="trWhite">
                                                    <td>Gas Tube <b>(Provided)</b></td>
                                                    <td>5</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>--%>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        function PlusClick(e) {
            $(e).hide();
            $(e.nextElementSibling).show();
            $(e.parentElement.parentElement.nextElementSibling).show();
            return false;
        }
        function MinusClick(e) {
            $(e).hide();
            $(e.previousElementSibling).show();
            $(e.parentElement.parentElement.nextElementSibling).hide()
            return false;
        }
        function ExpandAll() {
            $(".imgplus_min").hide();
            $(".imgMinus_min").show();
            $(".partList").show();
            return false;
        }
        function CollapseAll() {
            $(".imgplus_min").show();
            $(".imgMinus_min").hide();
            $(".partList").hide();
            return false;
        }
    </script>
</asp:Content>
