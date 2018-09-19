<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Order_AddEdit.aspx.cs" Inherits="Aircall.admin.Order_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Add New Order</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Order_List.aspx">Order List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Order Add/Edit </a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-money"></i>&nbsp;Orders Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Items</label>
                                <asp:Panel ID="PNLPart" CssClass="controls" DefaultButton="lnkParts" runat="server">
                                    <asp:TextBox ID="txtPart" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvParts" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="PartSearchGroup" CssClass="error_required" ControlToValidate="txtPart"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkParts" runat="server" CssClass="btn btn-success" ValidationGroup="PartSearchGroup" OnClick="lnkParts_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                            </div>
                            <asp:UpdatePanel ID="UPParts" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPParts input[type=radio]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PNLParts" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblParts" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkParts" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="control-group">
                                <label class="control-label">Qty&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtQty" runat="server" CssClass="span2" MaxLength="3" ValidationGroup="AddPartGroup"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvQty" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="AddPartGroup" CssClass="error_required" ControlToValidate="txtQty"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regExpQty" CssClass="error_required" runat="server" ControlToValidate="txtQty" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Qty." Display="Dynamic" ValidationGroup="AddPartGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                    <asp:LinkButton ID="lnkAddPart" runat="server" CssClass="btn btn-info" ValidationGroup="AddPartGroup" OnClick="lnkAddPart_Click"><i class="icon-plus icon-white"></i>Add Part</asp:LinkButton>
                                </div>
                            </div>
                            <div class="control-group" style="margin-top: 20px;">
                                <table class="table table-striped table-bordered table-advance table-hover">
                                    <thead>
                                        <tr>
                                            <th>Sr No</th>
                                            <th>Item Name</th>
                                            <th>Qty</th>
                                            <th>Price</th>
                                            <th>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstParts" runat="server" OnItemCommand="lstParts_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%#Eval("Id") %></td>
                                                    <td><%#Eval("PartName") %></td>
                                                    <td>
                                                        <asp:TextBox ID="txtQty" runat="server" Text='<%#Eval("Quantity") %>' CssClass="span2"></asp:TextBox></td>
                                                        <asp:RegularExpressionValidator ID="regExp" CssClass="error_required" runat="server" ControlToValidate="txtQty" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Qty." Display="Dynamic" ValidationGroup="UpdatePartGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                                    <td>$ <%#Eval("Price") %></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkUpdate" runat="server" Text="Update" CommandName="UpdatePart" CommandArgument='<%#Eval("Id") %>' ValidationGroup="UpdatePartGroup"></asp:LinkButton>
                                                        | 
                                                        <asp:LinkButton ID="lnkDeletePart" runat="server" Text="Delete" CommandName="RemovePart" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>

                                        </asp:ListView>
                                    </tbody>
                                </table>
                            </div>

                            <div class="control-group">
                                <label class="control-label">Subtotal Amount</label>
                                <div class="controls">
                                    <label>
                                        $
                                        <asp:Literal ID="ltrSubtotal" runat="server"></asp:Literal></label>
                                </div>
                            </div>

                            <%--<div class="control-group">
                                <label class="control-label">Technician</label>
                                <asp:Panel ID="pnlEmployee" CssClass="controls" DefaultButton="lnkSearch" runat="server">
                                    <asp:TextBox ID="txtEmployee" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmployee" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="EmpSearchGroup" CssClass="error_required" ControlToValidate="txtEmployee"></asp:RequiredFieldValidator>
                                    <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" ValidationGroup="EmpSearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                            </div>
                            <asp:UpdatePanel ID="UPEmployee" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPEmployee input[type=radio]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PEmployee" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblEmployee" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>--%>
                            <div class="control-group">
                                <label class="control-label">Client</label>
                                <asp:Panel ID="pnlClient" CssClass="controls" DefaultButton="lnkClientSearch" runat="server">
                                    <asp:TextBox ID="txtClient" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvClient" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ClientSearchGroup" CssClass="error_required" ControlToValidate="txtClient"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkClientSearch" runat="server" CssClass="btn btn-success" ValidationGroup="ClientSearchGroup" OnClick="lnkClientSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                            </div>
                            <asp:UpdatePanel ID="UPClient" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPClient input[type=radio]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PClient" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblClient" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="rblClient_SelectedIndexChanged">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="control-group">
                                        <label class="control-label">Address&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">State&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpState" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpState" InitialValue="0"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">City&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpCity" runat="server"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpCity" InitialValue="0"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Zip Code&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="regZip" CssClass="error_required" runat="server" ControlToValidate="txtZip" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Zip Code." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Email&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvEmail" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtEmail"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="Invalid Email" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Charge By&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpCharge" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpCharge_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvCharge" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpCharge" InitialValue="0"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkClientSearch" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="drpState" EventName="SelectedIndexChanged" />
                                    <asp:AsyncPostBackTrigger ControlID="rblClient" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <%--Add Check Details --%>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) {
                                            $('.date-picker').datepicker();
                                        });
                                    </script>
                                    <div id="dvCheck" runat="server" visible="false">
                                        <div class="control-group">
                                            <label class="control-label">Check Number<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtCheckNo" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvCheckNo" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCheckNo"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Check Date<span class="required">*</span></label>
                                            <div class="controls">
                                                <div class="input-append date left date-picker" data-date-format="mm/dd/yyyy" >
                                                    <input id="txtCheckDate" runat="server" class="input-large date-picker" size="16" type="text" />
                                                    <span class="add-on"><i class="icon-calendar"></i></span>
                                                    <asp:RequiredFieldValidator ID="rqfvCheckDate" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCheckDate"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Accounting Notes</label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtAccNotes" runat="server" TextMode="MultiLine"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Front Image<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:FileUpload ID="fpFront" runat="server" />
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Back Image<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:FileUpload ID="fpBack" runat="server" />
                                            </div>
                                        </div>
                                        <%--<div class="control-group">
                                            <label class="control-label">Bank Name<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtBank" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvBank" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtBank"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>--%>
                                        
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <%--Add New Card controls--%>
                            <asp:UpdatePanel ID="UPdvCard" runat="server">
                                <ContentTemplate>
                                    <div id="dvCard" runat="server" visible="false">
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
                                                <%--<asp:TextBox ID="txtMonth" runat="server" CssClass="input-small" MaxLength="2" placeholder="MM"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="refvMonth" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMonth"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regMonth" CssClass="error_required" runat="server" ControlToValidate="txtMonth" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="0[1-9]|1[0-2]"></asp:RegularExpressionValidator>
                                                <span>/</span>
                                                <asp:TextBox ID="txtYear" runat="server" CssClass="input-small" MaxLength="4" placeholder="YYYY"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvYear" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtYear"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regYear" CssClass="error_required" runat="server" ControlToValidate="txtYear" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{4}"></asp:RegularExpressionValidator>--%>
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
                                        <div class="control-group">
                                            <label class="control-label">Save as payment method</label>
                                            <div class="controls">
                                                <input type="checkbox" id="chkPayment" />
                                            </div>
                                        </div>
                                    </div>
                                    <div id="dvCVV" runat="server" visible="false">
                                        <div class="control-group">
                                            <label class="control-label">CVV<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtCardCVV" runat="server" CssClass="input-small" MaxLength="3"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvCardCVV" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCardCVV"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="drpCharge" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="control-group">
                                <label class="control-label">Recommendations for customer</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email to client's email</label>
                                <div class="controls">
                                    <asp:CheckBox ID="chkEmailToClient" runat="server" Checked="true"/>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">CC email address</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCCEmail" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regExpCCEmail" runat="server" ErrorMessage="Invalid Email" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCCEmail" ValidationExpression="^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[;]{0,1}\s*)+$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Client Signature</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpUpdSignature" runat="server" />
                                </div>
                            </div>

                            <div class="form-actions">
                                <asp:Button ID="btnSave" UseSubmitBehavior="false" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Order_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
