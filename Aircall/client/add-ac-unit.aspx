<%@ Page Title="Add AC Unit" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="add-ac-unit.aspx.cs" Inherits="Aircall.client.add_ac_unit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<script>
        try {
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_endRequest(function () {
                $(".select-outer select").selectmenu({
                    select: function (event, ui) {
                        $(this).trigger("change");
                    }
                });
            });
        } catch (e) {

        }
    </script>--%>
    <script>
        ///<summary>
        ///  This will fire on initial page load, 
        ///  and all subsequent partial page updates made 
        ///  by any update panel on the page
        ///</summary>
        function pageLoad() {
            $(".select-outer select").selectmenu({
                select: function (event, ui) {
                    $(this).trigger("change");
                }
            });
            $(".datepicker1").datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: "mm/yy",
                maxDate: '0',
                showButtonPanel: true,
                onClose: function (dateText, inst) {
                    $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
                }
            });
            $(".checkbox-outer, .radio-outer, .radio-outer-dot").buttonset();
            $("#btnSave").click(function () {
                if (Page_IsValid) {
                    $("#over").show();
                } else {
                    $("#over").hide();
                }
            });
        }
        function displayAmount() {
            var DurationInMonth = $("#hdnDurationInMonth").val();
            var PricePerMonth = $("#hdnPricePerMonth").val();
            var DiscountPrice = $("#hdnDiscountPrice").val();
            var Qty = $("#txtQty").val();
            if ($("#SpecialEnabled").val() == "true") {
                $("#lblPM").text("$" + parseFloat(Math.round((Qty * PricePerMonth) * 100) / 100).toFixed(2));
                var SavedAmount = parseFloat(Math.round((Qty * ((PricePerMonth * DurationInMonth) - DiscountPrice)) * 100) / 100).toFixed(2);
                var PayableAmount = parseFloat(Math.round(((Qty * DiscountPrice)) * 100) / 100).toFixed(2);
                $("#chkSpecialOffer").next().text("Special Offer Save $" + SavedAmount + " & pay $" + PayableAmount);
            }
            else {
                $("#lblPM").text("$" + parseFloat(Math.round((Qty * PricePerMonth) * 100) / 100).toFixed(2));
            }
        }
        $(document).ready(function () {
            pageLoad();
        });
    </script>
    <script type="text/javascript">
        var config = {
            '.chosen-select': {},
            '.chosen-select-deselect': { allow_single_deselect: true },
            '.chosen-select-no-single': { disable_search_threshold: 10 },
            '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
            '.chosen-select-width': { width: "95%" }
        }
        for (var selector in config) {
            $(selector).chosen(config[selector]);
        }
    </script>
    <style>
        .ui-datepicker table {
            margin: 0 0 3px;
        }

        table.ui-datepicker-calendar thead, table.ui-datepicker-calendar tbody {
            display: none;
        }

        .ui-datepicker .ui-datepicker-title select {
            margin: 1px 3px;
        }

        button.ui-datepicker-current.ui-state-default.ui-priority-secondary.ui-corner-all {
            display: none;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Add AC Unit</h1>
                </div>
                <div class="border-block">
                    <div id="dvMessage" runat="server" visible="false">
                    </div>
                    <div class="main-from">
                        <div class="single-row cf">
                            <div class="left-side">
                                <a href="<%=Application["SiteAddress"] %>uploads/plan/Aicall_Plan_v2.pdf" target="_blank">Plan Comparison</a>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Choose Plan</label>
                            </div>
                            <div class="right-side">
                                <div class="select-outer max290">
                                    <asp:DropDownList ID="drpPlan" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpPlan_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvPlan" CssClass="error" runat="server" ControlToValidate="drpPlan" ErrorMessage="Required" Display="Dynamic" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Visit Per Year</label>
                            </div>
                            <div class="right-side">
                                <div class="select-outer max290">
                                    <asp:DropDownList ID="drpVisitsPerYear" runat="server" OnSelectedIndexChanged="drpVisitsPerYear_SelectedIndexChanged" AutoPostBack="true">
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
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" CssClass="error" runat="server" ControlToValidate="drpVisitsPerYear" ErrorMessage="Required" Display="Dynamic" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Unit Name</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtUnitName" runat="server" placeholder="i.e. 1st Flr, unit 230, AC1"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf" style="display:none;">
                            <div class="left-side">
                                <label class="control-label">Unit Size</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtUnitTonSingle" runat="server" CssClass="input-large" placeholder="i.e.5 Ton"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf" style="display:none;">
                            <div class="left-side">
                                <label class="control-label">Manufacture Date</label>
                            </div>
                            <div class="right-side">
                                <div class="max290  date date-picker" data-date="12-02-2012" data-date-format="mm/dd/yyyy">
                                    <asp:TextBox ID="txtMfgDateSingle" runat="server" CssClass="input-large datepicker1"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Address where Unit is located</label>
                            </div>
                            <div class="right-side">
                                <div class="select-outer max290">
                                    <asp:DropDownList ID="drpAddress" runat="server" OnSelectedIndexChanged="drpAddress_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvAddress" CssClass="error" runat="server" ControlToValidate="drpAddress" ErrorMessage="Required" Display="Dynamic" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="UPdvAddress" runat="server">
                            <ContentTemplate>
                                <script type="text/javascript">
                                    function jScriptmsg() {
                                        var config = {
                                            '.chosen-select': {},
                                            '.chosen-select-deselect': { allow_single_deselect: true },
                                            '.chosen-select-no-single': { disable_search_threshold: 10 },
                                            '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
                                            '.chosen-select-width': { width: "95%" }
                                        }
                                        for (var selector in config) {
                                            $(selector).chosen(config[selector]);
                                        }
                                    }
                                    Sys.Application.add_load(jScriptmsg);
                                </script>
                                <div id="dvAddress" runat="server" style="display: none;">
                                    <div class="single-row cf">
                                        <div class="left-side">
                                            <label class="control-label">Address<span class="required">*</span></label>
                                        </div>
                                        <div class="right-side">
                                            <div class="max290">
                                                <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvtxtAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="single-row cf">
                                        <div class="left-side">
                                            <label class="control-label">State<span class="required">*</span></label>
                                        </div>
                                        <div class="right-side">
                                            <div class="max290">
                                                <asp:DropDownList ID="drpState" runat="server" CssClass="chosen-select" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="drpState" InitialValue="0"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="single-row cf">
                                        <div class="left-side">
                                            <label class="control-label">City<span class="required">*</span></label>
                                        </div>
                                        <div class="right-side">
                                            <div class="max290">
                                                <asp:DropDownList ID="drpCity" CssClass="chosen-select" runat="server"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="drpCity" InitialValue="0"></asp:RequiredFieldValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="single-row cf">
                                        <div class="left-side">
                                            <label class="control-label">Zip Code<span class="required">*</span></label>
                                        </div>
                                        <div class="right-side">
                                            <div class="max290">
                                                <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="regExpZip" runat="server" ErrorMessage="Invalid Zipcode" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtZip" ValidationExpression="\s*\d+\s*"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="drpAddress" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:HiddenField ID="AutoRenewalEnable" runat="server" Value="false" />
                                <asp:HiddenField ID="SpecialEnabled" runat="server" ClientIDMode="Static" Value="false" />
                                <asp:HiddenField ID="hdnDurationInMonth" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="hdnPricePerMonth" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="hdnDiscountPrice" runat="server" ClientIDMode="Static" />
                                <div class="single-row cf" id="dvSubscription" runat="server">
                                    <div class="left-side">
                                        <label>Subscription Information</label>
                                    </div>
                                    <div class="right-side">
                                        <div class="checkbox-outer">
                                            <asp:CheckBox ID="chkAutoRenewal" CssClass="chkunit" AutoPostBack="true" OnCheckedChanged="chkAutoRenewal_CheckedChanged" ValidationGroup="subscription" ClientIDMode="Static" runat="server" Text="Auto Renewal" />
                                        </div>
                                    </div>
                                </div>
                                <div class="single-row cf" id="dvSpecial" runat="server">
                                    <div class="left-side" id="dvSpecialLeft" visible="false" runat="server">
                                        <label>Subscription Information</label>
                                    </div>
                                    <div class="right-side">
                                        <div class="checkbox-outer">
                                            <asp:CheckBox ID="chkSpecialOffer" CssClass="chkunit" AutoPostBack="true" OnCheckedChanged="chkSpecialOffer_CheckedChanged" ValidationGroup="subscription" ClientIDMode="Static" runat="server" Text="Special Offer Save $200 & pay $1900 now" />
                                        </div>
                                    </div>
                                </div>
                                <div class="single-row cf" id="dvPM" runat="server">
                                    <div class="left-side">
                                        <label>Plan Cost</label>
                                    </div>
                                    <div class="right-side">
                                        <asp:Label ID="lblPM" runat="server" ClientIDMode="Static"></asp:Label>
                                        Per Month (<asp:Label ID="lblTotalMonth" runat="server"></asp:Label>)
                                    </div>
                                </div>
                                <div class="single-row cf" id="dvTotal" runat="server" visible="false">
                                    <div class="left-side">
                                    </div>
                                    <div class="right-side">
                                        <asp:Label ID="lblTotalPrice" runat="server"></asp:Label>
                                    </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="drpPlan" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="chkAutoRenewal" EventName="CheckedChanged" />
                                <asp:AsyncPostBackTrigger ControlID="chkSpecialOffer" EventName="CheckedChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <div class="single-row cf" id="Div1" runat="server">
                            <div class="left-side">
                                <label>Quantity</label>
                            </div>
                            <div class="right-side">
                                <div class="max290">
                                    <asp:TextBox ID="txtQty" runat="server" type="number" ClientIDMode="Static" onchange="displayAmount();" min="1" Text="1"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic"
                                        ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtQty"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvQty" runat="server" ErrorMessage="" Font-Size="12px" Font-Bold="true" Display="Dynamic"
                                        ValidationGroup="ChangeGroup" CssClass="error" ControlToValidate="txtQty" MinimumValue="1" MaximumValue="5" Type="Integer"></asp:RangeValidator>
                                </div>
                            </div>
                        </div>
                        <div class="single-row cf" style="display: none;">
                            <div class="left-side">
                                <label>Provide Optional Info</label>
                            </div>
                            <div class="right-side">
                                <div class="checkbox-outer">
                                    <asp:CheckBox ID="chkProvideInfo" runat="server" AutoPostBack="true" OnCheckedChanged="chkProvideInfo_CheckedChanged" Text=" " />
                                </div>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <div id="dvUnitTypes" runat="server" style="display: none;">
                                    <div class="single-row cf">
                                        <div class="left-side">
                                            <label class="control-label">Type of Unit</label>
                                        </div>
                                        <div class="right-side">
                                            <div class="select-outer max290">
                                                <asp:DropDownList ID="drpUnitType" runat="server" CssClass="input-large" AutoPostBack="true" OnSelectedIndexChanged="drpUnitType_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="pnlSingle" runat="server" style="display: none;">
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <h6 id="h6" runat="server">Single Information</h6>
                                            </div>
                                            <div class="right-side"></div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Manufacture Brand</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtMfgBrandSingle" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Model Number</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtModelNoSingle" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Serial Number</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtSerialSingle" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Quantity of Filters</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="select-outer max290">
                                                    <asp:DropDownList ID="drpFilterQtySingle" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpFilterQtySingle_SelectedIndexChanged">
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
                                        </div>
                                        <asp:Repeater ID="rptSingleFilter" runat="server" OnItemCreated="rptSingleFilter_ItemCreated">
                                            <ItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Size <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterSizeCool1" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Location <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterLocationCool1" runat="server">
                                                                <asp:ListItem Value="0">Inside Equipment</asp:ListItem>
                                                                <asp:ListItem Value="1">Inside Space</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Size <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterSizeCool1" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Location <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterLocationCool1" runat="server">
                                                                <asp:ListItem Value="0">Inside Equipment</asp:ListItem>
                                                                <asp:ListItem Value="1">Inside Space</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Quantity of Fuses</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="select-outer max290">
                                                    <asp:DropDownList ID="drpFuseQtySingle" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpFuseQtySingle_SelectedIndexChanged">
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
                                        </div>
                                        <asp:Repeater ID="rptSingleFuses" runat="server" OnItemCreated="rptSingleFuses_ItemCreated">
                                            <ItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Fuse Type <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFuseTypeSingle" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Fuse Type <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFuseTypeSingle" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Thermostat</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290 select-outer">
                                                    <asp:DropDownList ID="drpBoosterSingle" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div id="pnlSplit" runat="server" style="display: none;">
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <h6>Cooling Information</h6>
                                            </div>
                                            <div class="right-side"></div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Manufacture Brand</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtMfgBrandCool" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Model Number</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtModelNoCool" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Serial Number</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtSerialCool" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Quantity of Filters</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="select-outer max290">
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
                                        </div>
                                        <asp:Repeater ID="rptCoolingFilter" runat="server" OnItemCreated="rptCoolingFilter_ItemCreated">
                                            <ItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Size <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterSizeCool1" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Location <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterLocationCool" runat="server">
                                                                <asp:ListItem Value="0">Inside Equipment</asp:ListItem>
                                                                <asp:ListItem Value="1">Inside Space</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Size <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterSizeCool1" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Location <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterLocationCool" runat="server">
                                                                <asp:ListItem Value="0">Inside Equipment</asp:ListItem>
                                                                <asp:ListItem Value="1">Inside Space</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Quantity of Fuses</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="select-outer max290">
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
                                        </div>
                                        <asp:Repeater ID="rptCoolingFuses" runat="server" OnItemCreated="rptCoolingFuses_ItemCreated">
                                            <ItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Fuse Type <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFuseTypeCool" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Fuse Type <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFuseTypeCool" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Thermostat</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290 select-outer">
                                                    <asp:DropDownList ID="drpBoosterCool" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <h6>Heating Information</h6>
                                            </div>
                                            <div class="right-side"></div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Manufacture Brand</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtMfgBrandHeat" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Model Number</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtModelNoHeat" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Serial Number</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290">
                                                    <asp:TextBox ID="txtSerialHeat" runat="server" CssClass="input-large"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Quantity of Filters</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="select-outer max290">
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
                                        </div>
                                        <asp:Repeater ID="rptHeatingFilter" runat="server" OnItemCreated="rptHeatingFilter_ItemCreated">
                                            <ItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Size <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterSizeHeat" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Location <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterLocationHeat" runat="server">
                                                                <asp:ListItem Value="0">Inside Equipment</asp:ListItem>
                                                                <asp:ListItem Value="1">Inside Space</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Size <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterSizeHeat" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Filter Location <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFilterLocationHeat" runat="server">
                                                                <asp:ListItem Value="0">Inside Equipment</asp:ListItem>
                                                                <asp:ListItem Value="1">Inside Space</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Quantity of Fuses</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="select-outer max290">
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
                                        </div>
                                        <asp:Repeater ID="rptHeatingFuses" runat="server" OnItemCreated="rptHeatingFuses_ItemCreated">
                                            <ItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Fuse Type <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFuseTypeHeat" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <AlternatingItemTemplate>
                                                <div class="single-row cf">
                                                    <div class="left-side">
                                                        <label class="control-label">Fuse Type <%# Eval("id") %></label>
                                                    </div>
                                                    <div class="right-side">
                                                        <div class="select-outer max290">
                                                            <asp:DropDownList ID="drpFuseTypeHeat" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </AlternatingItemTemplate>
                                        </asp:Repeater>
                                        <div class="single-row cf">
                                            <div class="left-side">
                                                <label class="control-label">Thermostat</label>
                                            </div>
                                            <div class="right-side">
                                                <div class="max290 select-outer">
                                                    <asp:DropDownList ID="drpBoosterHeat" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="chkProvideInfo" EventName="CheckedChanged" />
                                <asp:AsyncPostBackTrigger ControlID="drpUnitType" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="drpFilterQtyCool" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <div class="single-row button-bar cf" style="position: relative;">
                            <div style="position: absolute; top: 0; left: 0; right: 0; bottom: 0; display: none;" id="over"></div>
                            <asp:Button ID="btnSave" Text="Proceed to Summary" ClientIDMode="Static" CssClass="main-btn" ValidationGroup="ChangeGroup" runat="server" OnClick="btnSave_Click" />
                            <input type="button" class="main-btn dark-grey" value="Cancel" onclick="location.href = 'dashboard.aspx'" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
