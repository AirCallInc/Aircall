<%@ Page Title="" Language="C#"  MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ClientAcUnit_AddEdit.aspx.cs" Inherits="Aircall.admin.ClientAcUnit_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client AC Unit Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/ClientAcUnit_List.aspx">Client AC Unit List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Client AC Unit Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-list"></i>Client AC Unit Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name<span class="required">*</span></label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLClient" runat="server" CssClass="controls" DefaultButton="lnkSearch">
                                    <asp:TextBox ID="txtClientName" runat="server" CssClass="input-large"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvClientName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="SearchGroup" CssClass="error_required" ControlToValidate="txtClientName"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" ValidationGroup="SearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                                <%--</div>--%>
                            </div>
                            <div class="control-group">
                                <label class="control-label">&nbsp;</label>
                                <div class="controls">
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
                                            <asp:Panel ID="Panel2" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblClient" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="rblClient_SelectedIndexChanged">
                                                </asp:RadioButtonList>
                                                <asp:HiddenField ID="hdnClient" runat="server" />
                                            </asp:Panel>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Name</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtUnitName" runat="server" CssClass="input-large" placeholder="i.e. 1st Flr, unit 230, AC1"></asp:TextBox>
                                    <%--<asp:RegularExpressionValidator ID="regUnitName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtUnitName" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>--%>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Address where Unit Located<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPAddress" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpAddress" runat="server" CssClass="input-xxlarge" AutoPostBack="true" OnSelectedIndexChanged="drpAddress_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpAddress" InitialValue="0"></asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="rblClient" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>

                            <%--Add New address controls--%>
                            <asp:UpdatePanel ID="UPdvAddress" runat="server">
                                <ContentTemplate>
                                    <div id="dvAddress" runat="server" visible="false">
                                        <div class="control-group">
                                            <label class="control-label">Address<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvtxtAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">State<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:DropDownList ID="drpState" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpState" InitialValue="0"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">City<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:DropDownList ID="drpCity" runat="server"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpCity" InitialValue="0"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                        <div class="control-group">
                                            <label class="control-label">Zip Code<span class="required">*</span></label>
                                            <div class="controls">
                                                <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regZip" CssClass="error_required" runat="server" ControlToValidate="txtZip" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Zip Code." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="drpAddress" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="control-group">
                                <label class="control-label">Plan Type<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPlanType" runat="server" CssClass="input-large"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvPlanType" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpPlanType" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Type of Unit<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpUnitType" runat="server" CssClass="input-large" AutoPostBack="true" OnSelectedIndexChanged="drpUnitType_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvUnitType" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpUnitType" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>

                            <div class="control-group" id="dvCardLeft" runat="server">
                                <label class="control-label">Select Card<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPCard" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpCard" runat="server" CssClass="input-large" AutoPostBack="true" OnSelectedIndexChanged="drpCard_SelectedIndexChanged"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvCard" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpCard" InitialValue="0"></asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="rblClient" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
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
                                                <asp:TextBox ID="txtMonth" runat="server" CssClass="input-small" MaxLength="2" placeholder="MM"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="refvMonth" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMonth"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regMonth" CssClass="error_required" runat="server" ControlToValidate="txtMonth" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                                                <span>/</span>
                                                <asp:TextBox ID="txtYear" runat="server" CssClass="input-small" MaxLength="4" placeholder="YYYY"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvYear" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtYear"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regYear" CssClass="error_required" runat="server" ControlToValidate="txtYear" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d+"></asp:RegularExpressionValidator>
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
                                    <asp:AsyncPostBackTrigger ControlID="drpCard" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:Panel ID="PNLParts" runat="server">
                                <div id="dvCooling" runat="server">
                                    <div class="control-group" id="dvCoolinglbl" runat="server" visible="false">
                                        <label class="control-label bold">Cooling Information</label>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Manufacture Brand</label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtMfgBrandCool" runat="server" CssClass="input-large"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Model Number</label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtModelNoCool" runat="server" CssClass="input-large"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvModelNoCool" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="FindUnitTypeCool" CssClass="error_required" ControlToValidate="txtModelNoCool"></asp:RequiredFieldValidator>
                                            <asp:LinkButton ID="lnkFindUnitTypesCool" runat="server" ValidationGroup="FindUnitTypeCool" CssClass="btn btn-success" OnClick="lnkFindUnitTypesCool_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Manufacture Date</label>
                                        <div class="controls">
                                            <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                                <input id="txtMfgDateCool" runat="server" class="input-large date-picker-month" size="16" type="text" />
                                                <span class="add-on"><i class="icon-calendar"></i></span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Serial Number</label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtSerialCool" runat="server" CssClass="input-large"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator ID="rqfvSerialCool" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="FindUnitTypeCool" CssClass="error_required" ControlToValidate="txtSerialCool"></asp:RequiredFieldValidator>--%>
                                            
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Quantity of Filters</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpFilterQtyCool" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpFilterQtyCool_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Select Filter Quantity</asp:ListItem>
                                                <asp:ListItem Value="1">1</asp:ListItem>
                                                <asp:ListItem Value="2">2</asp:ListItem>
                                                <asp:ListItem Value="3">3</asp:ListItem>
                                                <asp:ListItem Value="4">4</asp:ListItem>
                                                <asp:ListItem Value="5">5</asp:ListItem>
                                                <asp:ListItem Value="6">6</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <asp:UpdatePanel ID="UPFilter" runat="server" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                function jScriptmsg() {
                                                    if (!jQuery().uniform) {
                                                        return;
                                                    }
                                                    if (test = $("#UPFilter")) {
                                                        $(".chosen-with-diselect").chosen({
                                                            allow_single_deselect: true
                                                        });
                                                    }
                                                }
                                                Sys.Application.add_load(jScriptmsg);
                                            </script>
                                            <div id="dvFilterCool1" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeCool1" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationCool1" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterCool2" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 2</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeCool2" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 2</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationCool2" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterCool3" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 3</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeCool3" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 3</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationCool3" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterCool4" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 4</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeCool4" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 4</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationCool4" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterCool5" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 5</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeCool5" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 5</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationCool5" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterCool6" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 6</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeCool6" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 6</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationCool6" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpFilterQtyCool" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                    <div class="control-group">
                                        <label class="control-label">Quantity of Fuses</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpFuseQtyCool" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpFuseQtyCool_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Select Fuses Quantity</asp:ListItem>
                                                <asp:ListItem Value="1">1</asp:ListItem>
                                                <asp:ListItem Value="2">2</asp:ListItem>
                                                <asp:ListItem Value="3">3</asp:ListItem>
                                                <asp:ListItem Value="4">4</asp:ListItem>
                                                <asp:ListItem Value="5">5</asp:ListItem>
                                                <asp:ListItem Value="6">6</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <asp:UpdatePanel ID="UPFuse" runat="server" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <div id="dvFuseCool1" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeCool1" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseCool2" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 2</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeCool2" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseCool3" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 3</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeCool3" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseCool4" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeCool4" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseCool5" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeCool5" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseCool6" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeCool6" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpFuseQtyCool" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                    <div class="control-group">
                                        <label class="control-label">Booster</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpBoosterCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Unit Ton</label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtUnitTonCool" runat="server" CssClass="input-large" placeholder="i.e.5 Ton"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Refrigerant Type</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpRefTypeCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Electrical Service</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpElecServiceCool" runat="server">
                                                <%--<asp:ListItem>230/1/60</asp:ListItem>
                                                <asp:ListItem>230/3/60</asp:ListItem>
                                                <asp:ListItem>460/3/60</asp:ListItem>
                                                <asp:ListItem>120/1/60</asp:ListItem>--%>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Max Breaker</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpMaxBreakerCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Breaker</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpBreakerCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Compressor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpCompressorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Capacitor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpCapacitorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Contactor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpContactorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Filter dryer</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpFilterdryerCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Defrost board</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpDefrostboardCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Relay</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpRelayCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">TXV Valve</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpTXVValveCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Reversing Valve</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpReversingValveCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Blower Motor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpBlowerMotorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Condensing fan motor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpCondensingMotorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Inducer draft motor/ flu vent motor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpInducerCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Transformer</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpTransformerCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Control board</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpControlboardCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Limit switch</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpLimitSwitchCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Ignitor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpIgnitorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Gas valve</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpGasCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Pressure switch</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpPressureswitchCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Flame sensor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpFlamesensorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Roll out sensor</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpRolloutsensorCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Door switch</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpDoorswitchCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Ignition control board</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpIgControlBoardCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Coil Cleaner</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpCoilCleanerCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Misc</label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpMiscCool" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Unit Pictures</label>
                                        <div class="controls">
                                            <asp:FileUpload ID="fpUnitPicCool" runat="server" AllowMultiple="true" />
                                            <table class="table table-striped table-bordered">
                                                <thead>
                                                    <tr>
                                                        <th>FileName</th>
                                                        <th>Image</th>
                                                        <th>Action</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:ListView ID="lstImageCool" runat="server" OnItemCommand="lstImageCool_ItemCommand">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td><a href="<%=Application["SiteAddress"]%>uploads/unitImages/<%#Eval("UnitImage") %>" target="_blank"><%#Eval("UnitImage") %></a></td>
                                                                <td>
                                                                    <asp:Image ID="ImgCool" runat="server" Height="40px" Width="80px" ImageUrl='<%# "/uploads/unitImages/" + Eval("UnitImage")%>' />
                                                                </td>
                                                                <td>
                                                                    <asp:LinkButton ID="lnkDeleteCool" runat="server" CssClass="main-btn dark-grey" Text="Delete" CommandName="RemoveCoolImage" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:ListView>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Unit Manuals</label>
                                        <div class="controls">
                                            <asp:FileUpload ID="fpManualCool" runat="server" AllowMultiple="true" />
                                            <table class="table table-striped table-bordered">
                                                <thead>
                                                    <tr>
                                                        <th>Manual Name</th>
                                                        <th>Action</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <asp:ListView ID="lstManualCool" runat="server" OnItemCommand="lstManualCool_ItemCommand">
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td>
                                                                    <a href="<%=Application["SiteAddress"]%>uploads/unitManuals/<%#Eval("ManualName") %>" target="_blank"><%#Eval("ManualName") %></a>
                                                                </td>
                                                                <td>
                                                                    <asp:LinkButton ID="lnkDeleteManualCool" runat="server" CssClass="main-btn dark-grey" Text="Delete" CommandName="RemoveManualCool" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:ListView>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>

                                <asp:UpdatePanel ID="UPHeating" runat="server" ClientIDMode="Static">
                                    <ContentTemplate>
                                        <script type="text/javascript">
                                            function jScriptmsg() {
                                                if (!jQuery().uniform) {
                                                    return;
                                                }
                                                if (test = $("#UPHeating")) {
                                                    $(".chosen-with-diselect").chosen({
                                                        allow_single_deselect: true
                                                    });
                                                }
                                            }
                                            Sys.Application.add_load(jScriptmsg);
                                            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) {
                                                var EndDt = new Date();
                                                $('.date-picker-month').datepicker({ format: "mm/yyyy",
                                                    viewMode: "months", 
                                                    minViewMode: "months", endDate: EndDt, autoclose: true,
                                                    orientation: "bottom right"
                                                });
                                            });
                                        </script>
                                        <div id="dvHeating" runat="server">
                                            <div class="control-group">
                                                <label class="control-label bold">Heating Information</label>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Manufacture Brand</label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtMfgBrandHeat" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Model Number</label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtModelNoHeat" runat="server" CssClass="input-large"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rqfvModelNoHeat" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="FindUnitTypeHeat" CssClass="error_required" ControlToValidate="txtModelNoHeat"></asp:RequiredFieldValidator>
                                                    <asp:LinkButton ID="lnkFindUnitTypesHeat" runat="server" ValidationGroup="FindUnitTypeHeat" CssClass="btn btn-success" OnClick="lnkFindUnitTypesHeat_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Manufacture Date</label>
                                                <div class="controls">
                                                    <div class="input-append date left" data-date="12-02-2012" data-date-format="mm/dd/yyyy">
                                                        <input id="txtMfgDateHeat" runat="server" class="input-large date-picker-month" size="16" type="text" />
                                                        <span class="add-on"><i class="icon-calendar"></i></span>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Serial Number</label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtSerialHeat" runat="server" CssClass="input-large"></asp:TextBox>
                                                    <%--<asp:RequiredFieldValidator ID="rqfvSerialHeat" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="FindUnitTypeHeat" CssClass="error_required" ControlToValidate="txtSerialHeat"></asp:RequiredFieldValidator>--%>
                                                    
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Quantity of Filters</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpFilterQtyHeat" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpFilterQtyHeat_SelectedIndexChanged">
                                                        <asp:ListItem Value="0">Select Filter Quantity</asp:ListItem>
                                                        <asp:ListItem Value="1">1</asp:ListItem>
                                                        <asp:ListItem Value="2">2</asp:ListItem>
                                                        <asp:ListItem Value="3">3</asp:ListItem>
                                                        <asp:ListItem Value="4">4</asp:ListItem>
                                                        <asp:ListItem Value="5">5</asp:ListItem>
                                                        <asp:ListItem Value="6">6</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div id="dvFilterHeat1" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeHeat1" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationHeat1" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterHeat2" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 2</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeHeat2" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 2</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationHeat2" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterHeat3" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 3</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeHeat3" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 3</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationHeat3" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterHeat4" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 4</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeHeat4" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 4</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationHeat4" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterHeat5" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 5</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeHeat5" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 5</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationHeat5" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFilterHeat6" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Filter Size 6</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterSizeHeat6" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="control-group">
                                                    <label class="control-label">Filter Location 6</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFilterLocationHeat6" runat="server">
                                                            <asp:ListItem Value="1">Inside Equipment</asp:ListItem>
                                                            <asp:ListItem Value="0">Inside Space</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Quantity of Fuses</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpFuseQtyHeat" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpFuseQtyHeat_SelectedIndexChanged">
                                                        <asp:ListItem Value="0">Select Fuses Quantity</asp:ListItem>
                                                        <asp:ListItem Value="1">1</asp:ListItem>
                                                        <asp:ListItem Value="2">2</asp:ListItem>
                                                        <asp:ListItem Value="3">3</asp:ListItem>
                                                        <asp:ListItem Value="4">4</asp:ListItem>
                                                        <asp:ListItem Value="5">5</asp:ListItem>
                                                        <asp:ListItem Value="6">6</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div id="dvFuseHeat1" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 1</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeHeat1" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseHeat2" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 2</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeHeat2" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseHeat3" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 3</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeHeat3" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseHeat4" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 4</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeHeat4" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseHeat5" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 5</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeHeat5" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div id="dvFuseHeat6" runat="server">
                                                <div class="control-group">
                                                    <label class="control-label">Fuse Type 6</label>
                                                    <div class="controls">
                                                        <asp:DropDownList ID="drpFuseTypeHeat6" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="control-group">
                                                <label class="control-label">Booster</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpBoosterHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Unit Ton</label>
                                                <div class="controls">
                                                    <asp:TextBox ID="txtUnitTonHeat" runat="server" CssClass="input-large" placeholder="i.e.5 Ton"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Refrigerant Type</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpRefTypeHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Electrical Service</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpElecServiceHeat" runat="server">
                                                        <%--<asp:ListItem>230/1/60</asp:ListItem>
                                                        <asp:ListItem>230/3/60</asp:ListItem>
                                                        <asp:ListItem>460/3/60</asp:ListItem>
                                                        <asp:ListItem>120/1/60</asp:ListItem>--%>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">MaxBreaker</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpMaxBreakerHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Breaker</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpBreakerHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Compressor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpCompressorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Capacitor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpCapacitorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Contactor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpContactorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Filter dryer</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpFilterdryerHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Defrost board</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpDefrostboardHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Relay</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpRelayHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">TXV Valve</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpTXVValveHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Reversing Valve</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpReversingValveHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Blower Motor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpBlowerMotorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Condensing fan motor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpCondensingMotorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Inducer draft motor/ flu vent motor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpInducerHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Transformer</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpTransformerHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Control board</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpControlboardHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Limit switch</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpLimitSwitchHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Ignitor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpIgnitorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Gas valve</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpGasHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Pressure switch</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpPressureswitchHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Flame sensor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpFlamesensorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Roll out sensor</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpRolloutsensorHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Door switch</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpDoorswitchHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Ignition control board</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpIgControlBoardHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Coil Cleaner</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpCoilCleanerHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Misc</label>
                                                <div class="controls">
                                                    <asp:DropDownList ID="drpMiscHeat" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Unit Pictures</label>
                                                <div class="controls">
                                                    <asp:FileUpload ID="fpUnitPicHeat" runat="server" AllowMultiple="true" />
                                                    <table class="table table-striped table-bordered">
                                                        <thead>
                                                            <tr>
                                                                <th>FileName</th>
                                                                <th>Image</th>
                                                                <th>Action</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:ListView ID="lstImageHeat" runat="server" OnItemCommand="lstImageHeat_ItemCommand">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><a href="<%=Application["SiteAddress"]%>uploads/unitImages/<%#Eval("UnitImage") %>" target="_blank"><%#Eval("UnitImage") %></a></td>
                                                                        <td>
                                                                            <asp:Image ID="ImgHeat" runat="server" Height="40px" Width="80px" ImageUrl='<%# "/uploads/unitImages/" + Eval("UnitImage")%>' />
                                                                        </td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteHeat" runat="server" CssClass="main-btn dark-grey" Text="Delete" CommandName="RemoveHeatImage" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:ListView>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                            <div class="control-group">
                                                <label class="control-label">Unit Manuals</label>
                                                <div class="controls">
                                                    <asp:FileUpload ID="fpManualsHeat" runat="server" AllowMultiple="true" />
                                                    <table class="table table-striped table-bordered">
                                                        <thead>
                                                            <tr>
                                                                <th>Manual Name</th>
                                                                <th>Action</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <asp:ListView ID="lstManualHeat" runat="server" OnItemCommand="lstManualHeat_ItemCommand">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td>
                                                                            <a href="<%=Application["SiteAddress"]%>uploads/unitManuals/<%#Eval("ManualName") %>" target="_blank"><%#Eval("ManualName") %></a>
                                                                        </td>
                                                                        <td>
                                                                            <asp:LinkButton ID="lnkDeleteManual" runat="server" CssClass="main-btn dark-grey" Text="Delete" CommandName="RemoveManualHeat" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:ListView>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="drpUnitType" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </asp:Panel>
                            <div class="control-group">
                                <label class="control-label">Unit Status</label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpStatus" runat="server">
                                        <asp:ListItem Value="2">Service Soon</asp:ListItem>
                                        <asp:ListItem Value="1">Serviced</asp:ListItem>
                                        <asp:ListItem Value="3">Need Repair</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Notes</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>

                            <div class="control-group" runat="server" id="dvServicereport">
                                <label class="control-label">Service Report History</label>
                                <div class="controls">
                                    <asp:ListView ID="lstServicereport" runat="server">
                                        <ItemTemplate>
                                            <a href="<%=Application["SiteAddress"]%>admin/ServiceReport_View.aspx?ReportId=<%#Eval("Id") %>"><%#Eval("ServiceReportNumber") %></a><br />
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" UseSubmitBehavior="false" Text="Save" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'ClientAcUnit_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
