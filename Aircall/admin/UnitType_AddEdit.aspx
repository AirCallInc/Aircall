<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="UnitType_AddEdit.aspx.cs" Inherits="Aircall.admin.UnitType_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var EndDt = new Date();
            $('.date-picker-month').datepicker({
                format: "mm/yyyy",
                viewMode: "months",
                minViewMode: "months", endDate: EndDt, autoclose: true,
                orientation: "bottom left"
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Unit Type Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/UnitType_List.aspx">Unit Type List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Unit Type Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-cogs"></i>Unit Type Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Model Number<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtModelNo" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvModelNo" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtModelNo"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Serial Number<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtSerial" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvSerial" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtSerial"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Manufacture Date<span class="required">*</span></label>
                                <div class="controls">
                                    <div class="input-append span4 date left date-picker-month" data-date-format="mm/dd/yyyy" data-date-end-date="0d">
                                        <input id="txtMfgDate" runat="server" class="input-large date-picker-month" size="16" type="text" data-date-format="mm/dd/yyyy" data-date-end-date="0d"/>
                                        <span class="add-on"><i class="icon-calendar"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvMfgDate" runat="server" ErrorMessage="Required" style="margin-left:5px;" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMfgDate"></asp:RequiredFieldValidator>
                                    </div>
                                    <%--<asp:TextBox ID="txtMfgDate" runat="server" CssClass="span4 required"></asp:TextBox>--%>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Manufacture Brand<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMfgBrand" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvMfgBrand" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMfgBrand"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Ton<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtTon" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvTon" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtTon"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <asp:Panel ID="PNLParts" runat="server">
                                <%--<div class="control-group">
                                    <label class="control-label">Booster</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpBooster" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>--%>
                                <div class="control-group">
                                    <label class="control-label">Refrigerant Type</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpRefType" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Electrical Service</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpElecService" runat="server">
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
                                        <asp:DropDownList ID="drpMaxBreaker" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Breaker</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpBreaker" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Compressor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpCompressor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Capacitor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpCapacitor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Contactor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpContactor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Filter dryer</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpFilterdryer" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Defrost board</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpDefrostboard" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Relay</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpRelay" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">TXV Valve</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpTXVValve" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Reversing Valve</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpReversingValve" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Blower Motor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpBlowerMotor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Condensing fan motor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpCondensingMotor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Inducer draft motor/ flu vent motor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpInducer" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Transformer</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpTransformer" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Control board</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpControlboard" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Limit switch</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpLimitSwitch" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Ignitor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpIgnitor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Gas valve</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpGas" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Pressure switch</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpPressureswitch" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Flame sensor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpFlamesensor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Roll out sensor</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpRolloutsensor" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Door switch</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpDoorswitch" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Ignition control board</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpIgControlBoard" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Coil</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpCoilCleaner" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Misc</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpMisc" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                            </asp:Panel>
                            <div class="control-group">
                                <label class="control-label">Unit Manual</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fupdUnitManual" runat="server" AllowMultiple="true" />
                                    <table class="table table-striped table-bordered">
                                        <thead>
                                            <tr>
                                                <th>Manual Name</th>
                                                <th>Action</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:ListView ID="lstManulas" runat="server" OnItemCommand="lstManulas_ItemCommand">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><a href="<%=Application["SiteAddress"]%>uploads/unitPartManuals/<%#Eval("ManualFileName") %>" target="_blank" style="cursor: pointer;"><%#Eval("ManualFileName") %></a></td>
                                                        <td>
                                                            <asp:LinkButton ID="lnkDeleteManual" runat="server" CssClass="main-btn dark-grey" Text="Delete" CommandName="RemoveManual" CommandArgument='<%#Eval("UnitManualId") %>'></asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </tbody>
                                    </table>
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
                            <div class="form-actions">
                                <asp:Button ID="btnAdd" Text="Add" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'UnitType_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
