<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="Partner_AddEdit.aspx.cs" MaintainScrollPositionOnPostback="true" Inherits="Aircall.admin.Partner_AddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Partner Add / Edit </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/Partner_List.aspx">Partner List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Partner Add / Edit</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-group"></i>&nbsp;Partner Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">First Name&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtFName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvFName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFName"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regFName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtFName" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Last Name&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtLName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvLName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtLName"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regLName" runat="server" ErrorMessage="Invalid" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtLName" ValidationExpression="[a-zA-Z .'-]*$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Username&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtUserName" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvUserName" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtUserName"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Password&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtPassword" runat="server" CssClass="span4 required" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvPassword" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtPassword" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:HiddenField ID="hdnPassword" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Email&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvEmail" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="Invalid Email" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtEmail" ValidationGroup="ChangeGroup" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Company Name&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtCompany" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvCompany" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtCompany"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Profile Image&nbsp;</label>
                                <div class="controls">
                                    <asp:FileUpload ID="fpImage" runat="server" />
                                    <asp:HiddenField ID="hdnImage" runat="server" />
                                    <a href="" id="lnkImage" class="fancybox-button" data-rel="fancybox-button" runat="server" visible="false" target="_blank" style="cursor: pointer;">View Image</a>
                                </div>
                            </div>
                            <div>
                                <div class="controls">
                                    <span class="label label-important">NOTE!</span> <span>Please upload image of
                                                200 x 200 or higher pixels. For best results, the image pixels should be multiples
                                                of the minimum width and height. </span>
                                    <br />
                                    <br />
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Address&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtAddress" runat="server" CssClass="span4 required" TextMode="MultiLine"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvAddress" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtAddress" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">State&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:DropDownList ID="drpState" runat="server" CssClass="span4 required" AutoPostBack="true" OnSelectedIndexChanged="drpState_SelectedIndexChanged"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rqfvState" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="drpState" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">City&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:UpdatePanel ID="UPCity" runat="server">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="drpCity" runat="server" CssClass="span4 required"></asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvCity" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="drpCity" ValidationGroup="ChangeGroup" InitialValue="0"></asp:RequiredFieldValidator>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="drpState" EventName="SelectedIndexChanged" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Zip Code<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtZip" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvZip" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtZip"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regZip" CssClass="error_required" runat="server" ControlToValidate="txtZip" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Zip Code." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\s*\d+\s*"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Phone No&nbsp;<span class="required">*</span></label>
                                <div class="controls">
                                    <asp:TextBox ID="txtMob" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvMob" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtMob" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regMob" CssClass="error_required" runat="server" ControlToValidate="txtMob" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid Number." Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{8,}$"></asp:RegularExpressionValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Affiliate ID &nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <asp:TextBox ID="txtAffiliate" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvAffiliate" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtAffiliate" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Sales Commission %&nbsp;<span class="required">*</span></label>
                                <div class="controls input-icon">
                                    <asp:TextBox ID="txtCommission" runat="server" CssClass="span4 required"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvCommission" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" CssClass="error_required" ControlToValidate="txtCommission" ValidationGroup="ChangeGroup"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="regCommission" CssClass="error_required" runat="server" ControlToValidate="txtCommission" Font-Size="12px" Font-Bold="true" ErrorMessage="Invalid" Display="Dynamic" ValidationGroup="ChangeGroup" ValidationExpression="\d{0,2}(\.\d{1,2})?$"></asp:RegularExpressionValidator>
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
                                <asp:Button ID="btnAdd" Text="Add" UseSubmitBehavior="false" CssClass="btn btn-primary" ValidationGroup="ChangeGroup" runat="server" OnClick="btnAdd_Click" />
                                <input type="button" class="btn" value="Cancel" onclick="location.href = 'Partner_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12" id="clientAdd" runat="server">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-group"></i>&nbsp;Affiliate Clients</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Client Name&nbsp;<span class="required">*</span></label>
                                 <asp:Panel ID="pnlClient" CssClass="controls" DefaultButton="lnkSearch" runat="server">
                                    <asp:TextBox ID="txtClient" runat="server" CssClass="input-large"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rqfvClient" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="SearchGroup" CssClass="error_required" ControlToValidate="txtClient"></asp:RequiredFieldValidator>
                                    <asp:LinkButton ID="lnkSearch" runat="server" CssClass="btn btn-success" ValidationGroup="SearchGroup" OnClick="lnkSearch_Click"><i class="icon-search icon-white"></i>Search</asp:LinkButton>
                                </asp:Panel>
                            </div>
                            <div class="control-group">
                                <label class="control-label"></label>
                                <div class="controls">
                                    <asp:Panel ID="UPClient" runat="server" CssClass="scrollingControlContainer checkboxPanel">
                                        <asp:RadioButtonList ID="rblClient" runat="server" CssClass="checker">
                                        </asp:RadioButtonList>
                                    </asp:Panel>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSave" Text="Add Affiliate" UseSubmitBehavior="false" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" />
                            </div>
                        </div>
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th style="width: 44px;" class="hidden-phone srno">Sr. No.</th>
                                    <th>Client Name</th>
                                    <th>Email</th>
                                    <th>Phone</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstClients" runat="server" OnItemCommand="lstClients_ItemCommand">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td><%#Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("FirstName") %>  <%#Eval("LastName") %></td>
                                            <td><%#Eval("Email") %></td>
                                            <td><%#Eval("MobileNumber") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/Client_AddEdit.aspx?ClientId=<%#Eval("Id") %>"
                                                    class="btn mini purple"><i class="icon-eye-open"></i>&nbsp;View</a>
                                                <asp:LinkButton ID="lnkRemove" runat="server" CssClass="btn mini purple"  CommandName="RemoveAffiliate" CommandArgument='<%#Eval("Id") %>'><i class="icon-remove icon-white"></i>Remove</asp:LinkButton>
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
    </div>
</asp:Content>
