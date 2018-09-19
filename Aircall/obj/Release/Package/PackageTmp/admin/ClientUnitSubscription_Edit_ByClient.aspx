<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ClientUnitSubscription_Edit_ByClient.aspx.cs" Inherits="Aircall.admin.ClientUnitSubscription_Edit_ByClient" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client Unit Subscription Payment</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/ClientUnitSubscription_List_Unpaid.aspx">Client Unit Unpaid Subscription List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Client Unit Subscription Payment</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="row-fluid">
                        <div id="dvMessage" runat="server" visible="false">
                            <div class="clear">
                            </div>
                        </div>
                    </div>
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>Client Unit Subscription Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrClientName" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hdnClientId" runat="server" />
                                    <asp:HiddenField ID="hdfUnitIds" runat="server" />
                                    <asp:HiddenField ID="hdfPricePerMonth" runat="server" />
                                    <asp:HiddenField ID="hdfTotalAmount" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Company Name</label>
                                <div class="controls">
                                    <asp:Literal ID="ltlCompanyName" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Total Units</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrTotalUnits" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Price Per Month</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrPricePerMonth" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="control-group">
                                        <label class="control-label">Adjust</label>
                                        <div class="controls">
                                            <asp:CheckBox ID="chkAdjust" runat="server" AutoPostBack="true" OnCheckedChanged="chkAdjust_CheckedChanged" />
                                        </div>
                                    </div>
                                    <div class="control-group" id="divAdjust1" runat="server" visible="false">
                                        <label class="control-label">New Price Per Month</label>
                                        <div class="controls">
                                            $<asp:TextBox ID="txtAdjustPricePerMonth" runat="server" AutoPostBack="true" OnTextChanged="txtAdjustPricePerMonth_TextChanged"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group" id="divAdjust2" runat="server" visible="false">
                                        <label class="control-label">Adjust Comment</label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtAdjustCommment" runat="server" TextMode="MultiLine"></asp:TextBox>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="control-group" style="display:none">
                                <label class="control-label">Months</label>
                                <div class="controls">
                                    <asp:Literal ID="Literal1" runat="server" Text="12"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group" style="display:none">
                                <label class="control-label">Total Amount</label>
                                <div class="controls">
                                    <asp:Literal ID="ltrTotalAmount" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Payment Method<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPayment" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpPayment_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvPayment" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpPayment" InitialValue="0"></asp:RequiredFieldValidator>
                                    <asp:HiddenField ID="hdnPayment" runat="server" />
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UPDPayment" runat="server">
                                <ContentTemplate>
                                    <div id="dvCard" runat="server">
                                        <div class="control-group">
                                            <label class="control-label">Card<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:DropDownList ID="drpCard" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpCard_SelectedIndexChanged"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rqfvCard" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpCard" InitialValue="0"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div id="dvNewCard" runat="server" visible="false">
                                            <div class="control-group">
                                                <label class="control-label">Card Type<span class="required">*</span></label>
                                                <div class="controls">
                                                    <input type="radio" class="radio" id="rblVisa" runat="server" name="rblCard" value="Visa" />Visa
                                                <input type="radio" class="radio" id="rblMaster" runat="server" name="rblCard" value="Master Card" />Master Card
                                                <input type="radio" class="radio" id="rblDiscover" runat="server" name="rblCard" value="Discover" />Discover
                                                <input type="radio" class="radio" id="rblAmex" runat="server" name="rblCard" value="Amex" />Amex
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Name On Card<span class="required">*</span></label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtCardName" runat="server"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rqfvCardName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCardName"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Card Number<span class="required">*</span></label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtCardNumber" runat="server" MaxLength="16"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rqfvCardNumber" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCardNumber"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="regExpCard" CssClass="error_required" runat="server" ControlToValidate="txtCardNumber" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Card Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Expiry date<span class="required">*</span></label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpMonth" runat="server" CssClass="input-small"></asp:DropDownList>
                                                    <span>/</span>
                                                    <asp:DropDownList ID="drpYear" runat="server" CssClass="input-small"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">CVV<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtCVV" runat="server" CssClass="input-small" MaxLength="3"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvCVV" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCVV"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="dvPO" runat="server">
                                        <div class="control-group">
                                            <label class="control-label">PO Number</label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtPoNo" runat="server"></asp:TextBox>
                                                <asp:RegularExpressionValidator ID="regExpPONo" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPoNo" ValidationExpression="^[a-zA-Z0-9\-\/]*$"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="dvCheck" runat="server">
                                        <div class="control-group">
                                            <label class="control-label">Paid Months<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:DropDownList ID="drpPaidMonths" runat="server" CssClass="input-large" AutoPostBack="true" OnSelectedIndexChanged="drpPaidMonths_SelectedIndexChanged">
                                                    <asp:ListItem Value="1">1</asp:ListItem>
                                                    <asp:ListItem Value="2">2</asp:ListItem>
                                                    <asp:ListItem Value="3">3</asp:ListItem>
                                                    <asp:ListItem Value="4">4</asp:ListItem>
                                                    <asp:ListItem Value="5">5</asp:ListItem>
                                                    <asp:ListItem Value="6">6</asp:ListItem>
                                                    <asp:ListItem Value="7">7</asp:ListItem>
                                                    <asp:ListItem Value="8">8</asp:ListItem>
                                                    <asp:ListItem Value="9">9</asp:ListItem>
                                                    <asp:ListItem Value="10">10</asp:ListItem>
                                                    <asp:ListItem Value="11">11</asp:ListItem>
                                                    <asp:ListItem Value="12">12</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Pay Amount</label>
                                            <div class="controls">
                                                <asp:Literal ID="ltrPayAmount" runat="server"></asp:Literal>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <div style="width:50%">
                                                <table class="table table-striped table-bordered" id="sample_12">
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
                                                                        <asp:TextBox ID="txtCheckNumber" runat="server" CssClass="input-small" Text='<%#Eval("CheckNumber") %>' Width="500px"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAmount" runat="server" CssClass="input-small" Text='<%#Eval("Amount") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:ListView>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Front Image</label>
                                            <div class="controls">
                                                <asp:FileUpload ID="fpdFront" runat="server" />
                                                <asp:HiddenField ID="hdnFront" runat="server" />
                                                <a href="#" id="lnkFront" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Back Image</label>
                                            <div class="controls">
                                                <asp:FileUpload ID="fpdBack" runat="server" />
                                                <asp:HiddenField ID="hdnBack" runat="server" />
                                                <a href="#" id="lnkBack" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="drpPayment" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="control-group">
                                <label class="control-label">Accounting Notes</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtAccountNotes" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="form-actions">
                            <asp:HiddenField ID="DeviceToken" runat="server" Value='<%#Eval("DeviceToken") %>' />
                            <asp:HiddenField ID="DeviceType" runat="server" Value='<%#Eval("DeviceType") %>' />
                            <asp:HiddenField ID="PaymentDueDate" runat="server" Value='<%#Eval("PaymentDueDate","{0:MMMM yyyy}") %>' />
                            <asp:HiddenField ID="hdnUnitName" runat="server" Value='<%#Eval("UnitName") %>' />
                            <asp:Button ID="btnSave" Text="Charge" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                            <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-info" ValidationGroup="ChangeGroup" runat="server" Visible="false" OnClick="btnUpdate_Click" />
                            <asp:Button ID="btnCancel" Text="Cancel" CssClass="btn" runat="server" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
