<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Client_AddEdit.aspx.cs" Inherits="Aircall.admin.Client_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Client_List.aspx">Client List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Client Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-group"></i>Client Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <%--<div class="control-group">
                                <label class="control-label">HVAC Unit<span class="required">*</span></label>
                                <div class="controls">
                                    <label class="radio line">
                                        <asp:RadioButton type="radio" ID="rbtnMore" name="rbtnHVAC" runat="server" GroupName="rbtnHVAC" />
                                        My HVAC Unit is more than 10 years old
                                    </label>
                                    <label class="radio line">
                                        <asp:RadioButton type="radio" ID="rbtnLess" name="rbtnHVAC" runat="server" GroupName="rbtnHVAC" />
                                        My HVAC Unit is less than 10 years old
                                    </label>
                                    <label class="radio line">
                                        <asp:RadioButton type="radio" ID="rbtnNo" name="rbtnHVAC" runat="server" GroupName="rbtnHVAC" />
                                        I don't know how old my HVAC Unit is
                                    </label>
                                </div>
                            </div>--%>
                            <div class="control-group">
                                <label class="control-label">FirstName<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtFName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvFName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFName"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regFName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFName" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">LastName<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtLName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvLName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtLName"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regLName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtLName" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Company</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCompany" runat="server" CssClass="span4 required"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="span4 required" TextMode="Email"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmail" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group" id="dvPassword">
                                <label class="control-label">Password<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="span4 required" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPassword" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPassword" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regPassword" runat="server" ErrorMessage="Password length must be Greater than 6 characters." Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPassword" ValidationExpression="^(?:.{6,}|)$"></asp:RegularExpressionValidator>
                                    <asp:HiddenField ID="hdnPassword" runat="server" />
                                </div>
                            </div>
                            <%--<div class="control-group">
                                <label class="control-label">Phone Number<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPhone" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPhone" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPhone" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>--%>
                            <div class="control-group">
                                <label class="control-label">Mobile Number</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMobile" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regMob" CssClass="error_required" runat="server" ControlToValidate="txtMobile" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{10,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Office Number</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtOffice" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regOffice" CssClass="error_required" runat="server" ControlToValidate="txtOffice" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Home Number</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtHome" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regHome" CssClass="error_required" runat="server" ControlToValidate="txtHome" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{8,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Profile Image</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpImage" runat="server" />
                                    <asp:HiddenField ID="hdnImage" runat="server" />
                                    <a href="" id="lnkImage" class="fancybox-button" data-rel="fancybox-button" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                </div>
                            </div>
                            <div class="control-group">
                                <div class="controls">
                                    <span class="label label-important">NOTE!</span>
                                    <span>Please upload image of
                                                200 x 200 or higher pixels. For best results, the image pixels should be multiples
                                                of the minimum width and height. </span>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">
                                    Status
                                </label>
                                <div class="controls">
                                    <div class="text-toggle-button2">
                                        <input type="checkbox" class="toggle" id="chkActive" checked="checked" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Affiliate ID</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtAffiliate" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <a href="" id="lnkAffiliate" runat="server" visible="false" target="_blank" style="cursor: pointer;"></a>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnAdd" Text="Add" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click" />
                                <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-success" ValidationGroup="ChangeGroup" runat="server" Visible="false" OnClick="btnUpdate_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Client_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--Client Addresses Section-->
        <div class="row-fluid" id="dvClientAddress" runat="server" visible="false">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-group"></i>Client Addresses</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th>Sr #</th>
                                    <th>Address</th>
                                    <th class="hidden-phone">State</th>
                                    <th class="hidden-phone">City</th>
                                    <th class="hidden-phone">Zip</th>
                                    <th>IsDefaultAddress</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstCAddress" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("Address") %></td>
                                            <td><%#Eval("StateName") %></td>
                                            <td><%#Eval("CityName") %></td>
                                            <td><%#Eval("ZipCode") %></td>
                                            <td><%#Eval("IsDefaultAddress") %></td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <tr>
                                            <td colspan="6">Address Not found for Client.
                                            </td>
                                        </tr>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <!--Client Payment Method Section-->
        <div class="row-fluid" id="dvClientPayment" runat="server" visible="false">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-group"></i>Payment Methods</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <asp:UpdatePanel ID="UPdvCard" runat="server">
                            <ContentTemplate>
                                <%--<div class="clearfix">
                                    <div class="btn-group">
                                        <asp:Button ID="btnCardAdd" runat="server" CssClass="btn green" Text='Add New' />
                                    </div>
                                </div>
                                <div class="space15"></div>--%>
                                <table class="table table-striped table-bordered">
                                    <thead>
                                        <tr>
                                            <th>Sr #</th>
                                            <th>Name On Card</th>
                                            <th class="hidden-phone">Card Number</th>
                                            <th class="hidden-phone">Expiry Month</th>
                                            <th class="hidden-phone">Expiry Year</th>
                                            <th>Is Default Card</th>
                                            <th style="display: none;"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstCPayment" runat="server" OnItemCommand="lstCPayment_ItemCommand">
                                            <ItemTemplate>
                                                <tr class="odd gradeX">
                                                    <td><%# Container.DataItemIndex + 1 %></td>
                                                    <td><%#Eval("NameOnCard") %></td>
                                                    <td><%#Eval("CardNumber") %></td>
                                                    <td><%#Eval("ExpiryMonth") %></td>
                                                    <td><%#Eval("ExpiryYear") %></td>
                                                    <td><%#Eval("IsDefaultPayment") %></td>
                                                    <td style="display:none;">
                                                        <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="UpdateCard" Visible="false" CommandArgument='<%#Eval("Id") %>' /></td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyDataTemplate>
                                                <tr>
                                                    <td colspan="6">Payment method Not found for Client.
                                                    </td>
                                                </tr>
                                            </EmptyDataTemplate>
                                        </asp:ListView>
                                    </tbody>
                                </table>
                                <div class="space15"></div>
                                <div class="space15"></div>
                                <div id="dvCard" runat="server" visible="false">
                                    <div id="dvMessage1" runat="server" visible="false">
                                        <div class="clear">
                                            <!-- -->
                                        </div>
                                    </div>
                                    <div class="form-horizontal">
                                        <div class="control-group">
                                            <label class="control-label">Card Type<span class="required">*</span></label>
                                            <div class="controls">
                                                <label class="radio">
                                                    <input type="radio" id="rblVisa" runat="server" name="rblCard" value="Visa" style="margin-right: 10px;" />Visa</label>
                                                <label class="radio">
                                                    <input type="radio" id="rblMaster" runat="server" name="rblCard" value="Master Card" style="margin-right: 10px;" />Master Card</label>
                                                <label class="radio">
                                                    <input type="radio" id="rblDiscover" runat="server" name="rblCard" value="Discover" style="margin-right: 10px;" />Discover</label>
                                                <label class="radio">
                                                    <input type="radio" id="rblAmex" runat="server" name="rblCard" value="Amex" style="margin-right: 10px;" />Amex</label>
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
                                        <div class="control-group">
                                            <label class="control-label">CVV<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtCVV" runat="server" CssClass="input-small" MaxLength="3"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvCVV" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCVV"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <asp:HiddenField ID="hdnStripeCardId" runat="server" />
                                    <asp:HiddenField ID="hdnIsDefault" runat="server" />
                                    <asp:HiddenField ID="hdnCardId" runat="server" />
                                    <div class="form-actions">
                                        <asp:Button ID="btnCardSave" runat="server" Text="Save" OnClick="btnCardSave_Click" />
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
