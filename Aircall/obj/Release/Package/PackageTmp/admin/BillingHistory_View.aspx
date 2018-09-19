<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="BillingHistory_View.aspx.cs" Inherits="Aircall.admin.BillingHistory_View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Billing History</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Billing History</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMessage" runat="server" visible="false">
                            <div class="clear">
                                <!-- -->
                            </div>
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-money"></i>
                            Billing History Detail
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <div style="width:50%">
                                    <table class="table table-striped table-bordered" id="sample_12">
                                        <thead>
                                            <tr>
                                                <th>Sr #</th>
                                                <th>Unit Name</th>
                                                <th>Plan Name</th>
                                                <th>Visit Per Year</th>
                                                <th>Price Per Month</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:ListView ID="lstUnits" runat="server">
                                                <ItemTemplate>
                                                    <tr class='odd gradeX'>
                                                        <td>
                                                            <%#Container.DataItemIndex + 1 %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("UnitName") %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("PlanName") %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("VisitPerYear") %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("PricePerMonth") %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="control-group" runat="server" id="dvNoShow">
                                <label class="control-label">Service No :</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrServiceNo" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group" runat="server" id="dvPart">
                                <label class="control-label">Order No :</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrOrderNo" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group" runat="server" id="dvUnit" visible="false">
                                <label class="control-label">Plan :</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrPlan" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group" runat="server" id="dvUnit1" visible="false">
                                <label class="control-label">Unit :</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrUnit" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Client Name :</label>
                                <div class="controls">
                                    <label class="control-label" style="width:300px">
                                        <asp:Literal ID="ltrClientName" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Company :</label>
                                <div class="controls">
                                    <label class="control-label" style="width:300px">
                                        <asp:Literal ID="ltrCompany" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Transaction Id :</label>
                                <div class="controls">
                                    <label class="control-label" style="width:300px">
                                        <asp:Literal ID="ltrTransactionId" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Date :</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrDate" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Time :</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrTime" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Amount :</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrAmount" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Billing Type:</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrBillingType" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group" id="divCardNumber" runat="server">
                                <label class="control-label">
                                    <asp:Literal ID="ltrPaymentMethod" runat="server"></asp:Literal>:</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrPaymentByNumber" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group" runat="server" id="divPONumber">
                                <label class="control-label">PO:</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrPO" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group" id="divCheckNumbers" runat="server">
                                <div style="width:50%">
                                    <table class="table table-striped table-bordered" id="sample_13">
                                        <thead>
                                            <tr>
                                                <th>Sr #</th>
                                                <th>Check Number</th>
                                                <th>Amount</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:ListView ID="lstChecks" runat="server">
                                                <ItemTemplate>
                                                    <tr class='odd gradeX'>
                                                        <td>
                                                            <%#Container.DataItemIndex + 1 %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("CheckNumber") %>
                                                        </td>
                                                        <td>
                                                            <%#Eval("Amount") %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="control-group" runat="server" id="dvPart1">
                                <label class="control-label">Parts</label>
                                <div class="controls">
                                    <label class="span8">
                                        <table cellspacing="0" cellpadding="0" border="none" class="table table-striped table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Part</th>
                                                    <th>Quantity</th>
                                                    <th>Size</th>
                                                    <th>Rate</th>
                                                    <th>Total</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:ListView ID="lstParts" runat="server">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td><%#Eval("PartName") %></td>
                                                            <td><%#Eval("Quantity") %></td>
                                                            <td><%#Eval("PartSize") %></td>
                                                            <td>$ <%#Eval("Amount") %></td>
                                                            <td>$ <%#decimal.Parse(Eval("Quantity").ToString()) * decimal.Parse(Eval("Amount").ToString()) %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </tbody>
                                        </table>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Accounting Notes:</label>
                                <div class="controls">
                                    <label class="">
                                        <asp:Literal ID="ltrAccountingNotes" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Status</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpStatus" runat="server" CssClass="input-large">
                                        <asp:ListItem Value="Paid">Paid</asp:ListItem>
                                        <asp:ListItem Value="UnPaid">UnPaid</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:HiddenField runat="server" ID="hdfOriginalStatus" />
                                <asp:HiddenField runat="server" ID="hdfClientId" />
                                <asp:HiddenField runat="server" ID="hdfClientName" />
                                <asp:HiddenField runat="server" ID="hdfCompany" />
                                <asp:HiddenField runat="server" ID="hdfCheckNumbers" />
                                <asp:HiddenField runat="server" ID="hdfTransactionDate" />
                                <asp:Button ID="btnSave" Text="Save" UseSubmitBehavior="false" ClientIDMode="Static" CssClass="btn btn-primary" runat="server" OnClick="btnSave_Click" />
                                <button type="button" onclick="window.location.href = '<%=Application["SiteAddress"]%>admin/BillingHistory_List.aspx'" class="btn">Back to list</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
