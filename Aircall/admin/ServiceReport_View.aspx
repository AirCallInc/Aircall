<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="ServiceReport_View.aspx.cs" Inherits="Aircall.admin.ServiceReport_View" %>

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
                                <label class="control-label">Service Report #</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblServiceReport" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Contact Name</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblContactName" runat="server"></asp:Literal>
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
                                <label class="control-label">Address</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblAddress" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Purpose of visit</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblPurpose" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <%--<div class="control-group">
                                <label class="control-label">Billing Type</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Label ID="lblBillingT" runat="server"></asp:Label>
                                    </label>
                                </div>
                            </div>--%>
                            <div class="control-group">
                                <label class="control-label">Date</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblDate" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Billing Type</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrBillingType" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Technician</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblTechnician" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Assigned Total Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrAssignedTotalTime" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Assigned Start Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrAssignedStart" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Assigned End Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrAssignEnd" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Actual Start Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblTimeS" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Actual End Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblTimeC" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Extra Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrExtra" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Serviced </label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblUnitServiced" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Work Performed</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblWorkPerformed" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Material Used</label>
                                <div class="controls">
                                    <label class="span4">
                                        <%--<asp:Literal ID="lblMaterialU" runat="server"></asp:Literal>--%>
                                        <asp:ListView ID="lstServiceUnitsUsed" runat="server" OnItemDataBound="lstServiceUnitsUsed_ItemDataBound">
                                            <ItemTemplate>
                                                <b>Unit Name: <%#Eval("UnitName") %></b><br />
                                                <asp:HiddenField ID="hdnUnitIdUsed" runat="server" Value='<%#Eval("UnitId") %>' />
                                                <asp:ListView ID="lstMaterialUsed" runat="server">
                                                    <ItemTemplate>
                                                        <%#Eval("Name") %> - <%#Eval("Size") %> (<%#Eval("UsedQuantity") %>)<br />
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Material Not Used</label>
                                <div class="controls">
                                    <label class="span4">
                                        <%--<asp:Literal ID="lblMaterialNU" runat="server"></asp:Literal>--%>
                                        <asp:ListView ID="lstServiceUnitsNotUsed" runat="server" OnItemDataBound="lstServiceUnitsNotUsed_ItemDataBound">
                                            <ItemTemplate>
                                                <b>Unit Name: <%#Eval("UnitName") %></b><br />
                                                <asp:HiddenField ID="hdnUnitIdNotUsed" runat="server" Value='<%#Eval("UnitId") %>' />
                                                <asp:ListView ID="lstMaterialNotUsed" runat="server">
                                                    <ItemTemplate>
                                                        <%#Eval("Name") %> - <%#Eval("Size") %><br />
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Part Requested</label>
                                <div class="controls">
                                    <label class="span4">
                                        <asp:Literal ID="lblPartReq" runat="server"></asp:Literal>
                                        <asp:ListView ID="lstRequestPartUnits" runat="server" OnItemDataBound="lstRequestPartUnits_ItemDataBound">
                                            <ItemTemplate>
                                                <b>Unit Name: <%#Eval("UnitName") %></b><br />
                                                <asp:HiddenField ID="hdnUnitId" runat="server" Value='<%#Eval("Id") %>'/>
                                                <asp:ListView ID="lstRequestedPart" runat="server">
                                                    <ItemTemplate>
                                                        <%#Eval("PartName") %> - <%#Eval("PartSize") %> (<%#Eval("RequestedQuantity") %>)<br />
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Pictures</label>
                                <div class="controls">
                                    <asp:ListView ID="lstimage" runat="server">
                                        <ItemTemplate>
                                            <asp:Image ID="ImgService" Style="padding:3px;" runat="server" Height="130px" Width="200px" ImageUrl='<%# "/uploads/reportimage/" + Eval("ServiceImage")%>' />
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Recommendations to customer</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="lblRecommen" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email to client's email</label>
                                <div class="controls">
                                   <%-- <div class="checker" id="uniform-chksendEmail">
                                        <span class="checked">
                                            <input type="checkbox" id="chksendEmail" runat="server" checked="checked" style="opacity: 0;">
                                        </span>
                                    </div>--%>
                                    <asp:Literal ID="lblEmailToC" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hdnClientEmail" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">CC email address</label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCCEmail" runat="server" CssClass="span4"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="regCCEmail" CssClass="error_required" runat="server" ControlToValidate="txtCCEmail" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid CC Email" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[;]{0,1}\s*)+$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Client Signature</label>
                                <div class="controls">
                                    <asp:Image ID="ClientSig" runat="server" Width="200px" ImageUrl="" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Employee Notes:</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrEmployeeNote" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnrespond" class="btn btn-success" type="button" runat="server" Text="Send Report" ValidationGroup="ChangeGroup" OnClick="btnrespond_Click" />
                                <input type="button" class="btn" value="Back To List" onclick="location.href = 'ServiceReport_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
