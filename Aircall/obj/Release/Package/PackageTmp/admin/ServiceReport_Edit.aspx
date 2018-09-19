<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ServiceReport_Edit.aspx.cs" Inherits="Aircall.admin.ServiceReport_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Service Report</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/ServiceReport_List.aspx">Service Reports List </a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">View Service Report </a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-edit"></i>Service Report Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Service Report Number</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrReportNo" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Contact Name</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblContactName" runat="server"></asp:Literal>
                                        <asp:HiddenField ID="hdnClientId" runat="server" />
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Company</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrCompany" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                            </div>
                            <div class="control-group">
                                <label class="control-label">Address&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <%--<asp:Panel ID="PNLAddress" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                        <asp:RadioButtonList class="radio" ID="rblAddress" name="rblAddress" runat="Server">
                                        </asp:RadioButtonList>
                                    </asp:Panel>--%>
                                    <asp:Literal ID="ltrAddress" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Purpose of visit&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <%--<asp:DropDownList ID="ddlPurposeofVisit" runat="server"></asp:DropDownList>--%>
                                    <asp:Literal ID="ltrPurposeofVisit" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Employee Name</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblTechnician" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Billing Type&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpBilling" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label">Service Date</label>
                                <div class="controls">
                                    <%--<asp:TextBox ID="txtScheduleOn" runat="server" CssClass="date-picker"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvScheduleOn" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtScheduleOn" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>--%>
                                    <asp:Literal ID="ltrScheduleOn" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Time started work&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtStart" runat="server" class="span5 timepicker-default" type="text" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvStart" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtStart" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Time completed work&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <div class="input-append bootstrap-timepicker-component">
                                        <input id="txtEnd" runat="server" class="span5 timepicker-default" type="text" /><span class="add-on"><i
                                            class="icon-time"></i></span>
                                        <asp:RequiredFieldValidator ID="rqfvEnd" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEnd" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Serviced&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:Panel ID="Panel2" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                        <asp:CheckBoxList ID="cblUnit" runat="Server">
                                        </asp:CheckBoxList>
                                    </asp:Panel>

                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Work Performed</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtWorkedP" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Material Used</label>
                                <div class="controls">
                                    <asp:Panel ID="Panel3" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                        <asp:CheckBoxList ID="cblMaterialU" runat="Server">
                                        </asp:CheckBoxList>
                                    </asp:Panel>
                                </div>
                            </div>

                            <%--<div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                    <asp:TextBox ID="TextBox1" runat="server" class="span4 required"></asp:TextBox>
                                    <a href="" id="lnkAddPart" onclick="addParts();" runat="server" clientidmode="static" style="margin-bottom: 0px;" class="btn btn-info add1"><i class="icon-plus icon-white"></i>&nbsp; Add Item</a>
                                </div>
                            </div>--%>
                            <div id="dvParts" style="display: none;">
                                <div class="control-group">
                                    <label class="control-label">Part</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpParts" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Quantity<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="span4 required"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvQty" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtQuantity" ValidationGroup="PartGroup"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="form-actions">
                                    <asp:Button ID="btnAddPart" runat="server" Text="Add Part" CssClass="btn btn-primary" ValidationGroup="PartGroup" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Pictures</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpdPicture" runat="server" AllowMultiple="true" />
                                    <asp:HiddenField ID="hdnPictureCount" runat="server" />
                                    <table class="table table-striped table-bordered">
                                        <thead>
                                            <tr>
                                                <th>FileName</th>
                                                <th>Image</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:ListView ID="lstPicture" runat="server">
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><a href="<%=Application["SiteAddress"]%>uploads/reportimage/<%#Eval("ServiceImage") %>" target="_blank"><%#Eval("ServiceImage") %></a></td>
                                                        <td>
                                                            <asp:Image ID="Img" runat="server" Height="40px" Width="80px" ImageUrl='<%# "/uploads/reportimage/" + Eval("ServiceImage")%>' />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Recommendations to customer</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtRecomm" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email to client's email</label>
                                <div class="controls">
                                    <div class="checker" id="uniform-chksendEmail">
                                        <span class="checked">
                                            <input type="checkbox" id="chksendEmail" runat="server" style="opacity: 0;">
                                        </span>
                                    </div>
                                    <asp:TextBox class="control-label" ID="txtEmailC" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label">CC email address</label>
                                <div class="controls">
                                    <asp:TextBox class="control-label" ID="txtCCEmail" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regExpCCEmail" runat="server" ErrorMessage="Invalid Email" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCCEmail" ValidationExpression="^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[;]{0,1}\s*)+$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Client Signature</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpclientsig" runat="server" />
                                    <asp:HiddenField ID="hdnclientsig" runat="server" />
                                    <a href="" id="lnkclientsig" class="fancybox-button" data-rel="fancybox-button" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Employee Notes:</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmployeeNot" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnrespond" class="btn btn-success" ValidationGroup="ChangeGroup" type="button" runat="server" Text="Save" OnClick="btnrespond_Click"/>
                                <%--<asp:Button ID="btncancel" class="btn" type="button" runat="server" Text="Back to list" OnClientClick="ValidateRadioButtonList()" />--%>
                                <input type="button" class="btn" value="Back to list" onclick="location.href = 'ServiceReport_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function addParts() {

            $("#lnkAddPart").fancybox({
                'href': '#dvParts',
                'titleShow': false,
                'transitionIn': 'elastic',
                'transitionOut': 'elastic'
            });
            setTimeout(function () { $(".fancybox-overlay").appendTo("#aspNetForm") }, 1000);
        }
    </script>
</asp:Content>
