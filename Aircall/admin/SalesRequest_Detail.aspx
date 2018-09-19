<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="SalesRequest_Detail.aspx.cs" Inherits="Aircall.admin.SalesRequest_Detail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Sales Visit Request Reply</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="<%=Application["SiteAddress"]%>admin/SalesRequest_List.aspx">Sales Visit Requests List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Sales Visit Request Reply</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-envelope"></i>&nbsp;Sales Visit Request Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Request Submitted By</label>
                                <div class="controls">
                                    <label class="control-label"><asp:Literal ID="ltrSubmittedBy" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Client Name</label>
                                <div class="controls">
                                    <label class="control-label"><asp:Literal ID="ltrClient" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Address</label>
                                <div class="controls">
                                    <label class="control-label"><asp:Literal ID="ltrAddress" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Submitted On</label>
                                <div class="controls">
                                    <label class="control-label"><asp:Literal ID="ltrSubmittedOn" runat="server"></asp:Literal></label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Notes</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtNotes" runat="server" CssClass="span4" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Sales Employee Name</label>
                                <%--<div class="controls">--%>
                                <asp:Panel ID="PNLEmployee" runat="server" CssClass="controls" DefaultButton="lnkSalesEmpSearch">
                                    <asp:TextBox ID="txtSalesEmployee" runat="server"></asp:TextBox>
                                    <%--<asp:RequiredFieldValidator ID="rqfvSalesEmployee" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="EmployeeSearchGroup" CssClass="error_required" ControlToValidate="txtSalesEmployee"></asp:RequiredFieldValidator>--%>
                                    <asp:LinkButton ID="lnkSalesEmpSearch" runat="server" CssClass="btn btn-success" ValidationGroup="EmployeeSearchGroup" OnClick="lnkSalesEmpSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>

                                <%--</div>--%>
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
                                    <asp:AsyncPostBackTrigger ControlID="lnkSalesEmpSearch" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="form-actions">
                                <asp:Button ID="btnNotify" UseSubmitBehavior="false" Text="Notify Parties" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnNotify_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'SalesRequest_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
