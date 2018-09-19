<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeePartRequest_AddEdit.aspx.cs" Inherits="Aircall.admin.EmployeePartRequest_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Employee Part Request Add/Edit</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/EmployeePartRequest_List.aspx">Employee Part Request List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Employee Part Request Add/Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>Employee Request Part Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtClient" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvClient" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ClientSearchGroup" CssClass="error_required" ControlToValidate="txtClient"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkClientSearch" runat="server" CssClass="btn btn-success" ValidationGroup="ClientSearchGroup" OnClick="lnkClientSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UPNLClient" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <script type="text/javascript">
                                        function jScriptmsg() {
                                            if (!jQuery().uniform) {
                                                return;
                                            }
                                            if (test = $("#UPNLClient input[type=radio]:not(.toggle)")) {
                                                test.uniform();
                                            }
                                        }
                                        Sys.Application.add_load(jScriptmsg);
                                    </script>
                                    <div class="control-group">
                                        <label class="control-label"></label>
                                        <div class="controls">
                                            <asp:Panel ID="PNLClient" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblClient" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="rblClient_SelectedIndexChanged">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Client Address<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:Panel ID="PNLAddress" runat="server" CssClass="scrollingControlContainer checkboxPanel span12">
                                                <asp:RadioButtonList ID="rblAddress" runat="server" CssClass="checker" AutoPostBack="true" OnSelectedIndexChanged="rblAddress_SelectedIndexChanged">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Client Units<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:Panel ID="PNLUnits" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblClientUnits" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkClientSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <div class="control-group">
                                <label class="control-label">Employee Name<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmpName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmpName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="SearchGroup" CssClass="error_required" ControlToValidate="txtEmpName"></asp:RequiredFieldValidator>
                                    <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" ValidationGroup="SearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPNLEmployee" ClientIDMode="Static" runat="server">
                                        <ContentTemplate>
                                            <script type="text/javascript">
                                                function jScriptmsg() {
                                                    if (!jQuery().uniform) {
                                                        return;
                                                    }
                                                    if (test = $("#UPNLEmployee input[type=radio]:not(.toggle)")) {
                                                        test.uniform();
                                                    }
                                                }
                                                Sys.Application.add_load(jScriptmsg);
                                            </script>
                                            <asp:Panel ID="PNLEmployee" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                                <asp:RadioButtonList ID="rblEmployee" runat="server" CssClass="checker">
                                                </asp:RadioButtonList>
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                    <a href="" id="lnkAddPart" onclick="appendfancy();">Add Parts</a>
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label">Employee Notes</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmpNotes" runat="server" TextMode="MultiLine" CssClass="span4"></asp:TextBox>
                                </div>
                            </div>

                            <div id="dvParts" style="display: none;">
                                <div class="control-group">
                                    <label class="control-label">Part</label>
                                    <div class="controls">
                                        <asp:DropDownList ID="drpParts" runat="server" CssClass="chosen-with-diselect"></asp:DropDownList>
                                    </div>
                                </div>
                                OR
                                <div class="control-group">
                                    <label class="control-label">Part Name<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPartName" runat="server" CssClass="span4 required"></asp:TextBox>
                                        <%--<asp:RequiredFieldValidator ID="rqfvPartname" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtPartName"></asp:RequiredFieldValidator>--%>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Part size<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtPartSize" runat="server" CssClass="span4 required"></asp:TextBox>
                                        <%--<asp:RequiredFieldValidator ID="rqfvPartSize" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPartSize" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>--%>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Part Description<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtDescription" runat="server" CssClass="span4 required" TextMode="MultiLine"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <label class="control-label">Quantity<span class="required">*</span></label>
                                    <div class="controls">
                                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="span4 required"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rqfvQty" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtQuantity" ValidationGroup="PartGroup"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="regExpQty" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtQuantity" ValidationGroup="PartGroup" ValidationExpression="^\d{1,3}$"></asp:RegularExpressionValidator>
                                    </div>
                                </div>
                                <div class="form-actions">
                                    <asp:Button ID="btnAddPart" runat="server" Text="Add Part" CssClass="btn btn-primary" ValidationGroup="PartGroup" OnClick="btnAddPart_Click" />
                                </div>
                            </div>

                            <div class="control-group">
                                <label class="control-label">Status<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpPartStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpPartStatus_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvStatus" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="drpPartStatus" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <asp:UpdatePanel ID="UPNotes" runat="server">
                                <ContentTemplate>
                                    <div class="control-group" id="dvNotes" runat="server" visible="false">
                                        <label class="control-label">Notes</label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtNotes" runat="server" CssClass="span4 required" TextMode="MultiLine"></asp:TextBox>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="drpPartStatus" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-body">
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th>Sr #</th>
                                    <th>Unit Name</th>
                                    <th>Part Name</th>
                                    <th class="hidden-phone">Part Size</th>
                                    <th class="hidden-phone">Description</th>
                                    <th class="hidden-phone">Requested Quantity</th>
                                    <th>Arranged Quantity</th>
                                    <th class="clsAction">Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstParts" runat="server" OnItemCommand="lstParts_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("UnitName") %></td>
                                            <td><%#Eval("Partname") %></td>
                                            <td><%#Eval("PartSize") %></td>
                                            <td><%#Eval("Description") %></td>
                                            <td>
                                                <asp:TextBox ID="txtQty" runat="server" Text='<%#Eval("Quantity") %>' CssClass="input-small"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtArrQty" runat="server" Text='<%#Eval("ArrangedQuantity") %>' CssClass="input-small"></asp:TextBox>
                                            </td>
                                            <td class="clsAction">
                                                <asp:LinkButton ID="lnkDeletePart" runat="server" Text="Delete" CommandName="RemovePart" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-body form">
                        <div class="control-group">
                            <asp:Button ID="btnAdd" Text="Add" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click" />
                            <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" Visible="false" OnClick="btnUpdate_Click" />
                            <input type="button" class="btn" value="Cancel" onclick="location.href = 'EmployeePartRequest_List.aspx'" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function appendfancy() {

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
