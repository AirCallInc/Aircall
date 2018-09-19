<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="PartnerTicket_Edit.aspx.cs" Inherits="Aircall.admin.PartnerTicket_Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Ticket Details </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="<%=Application["SiteAddress"]%>admin/PartnerTicket_List.aspx">Partner Ticket List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Partner Ticket Details</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div id="dvMessage" runat="server" visible="false"></div>
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-magic"></i>&nbsp;Partner Ticket Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div id="dvError" runat="server" visible="false" class="alert alert-error">
                        </div>
                        <div class="form-horizontal">
                            <div class="widget" id="">
                                <div class="widget-title">
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Ticket Detail</h4>
                                </div>
                                <div class="widget-body">
                                    <table width="100%">
                                        <tbody>
                                            <tr>
                                                <td width="50%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="viewlabel">Subject:</td>
                                                            <td>
                                                                <asp:Label ID="lblSubject" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="viewlabel">Added Date:</td>
                                                            <td>
                                                                <asp:Label ID="lblAddDate" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="viewlabel">Notes:</td>
                                                            <td>
                                                                <asp:Label ID="lblNote" runat="server"></asp:Label></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td width="50%">
                                                    <table width="100%">
                                                        <tr>
                                                            <td class="viewlabel">Partner Name:</td>
                                                            <td>
                                                                <asp:Label ID="lblPartner" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                        <tr>
                                                            <td class="viewlabel">Type of Ticket:</td>
                                                            <td>
                                                                <asp:Label ID="lblType" runat="server" Text=""></asp:Label></td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>

                                </div>
                            </div>
                            
                            <div class="widget" id="">
                                <div class="widget-title">
                                    <h4 style="float: left;"><i class="icon-comments-alt"></i>&nbsp;Conversation</h4>
                                </div>
                                <div class="widget-body">
                                    <ul class="chats normal-chat">
                                        <asp:ListView ID="lstConversion" runat="server" OnItemDataBound="lstConversion_ItemDataBound">
                                            <ItemTemplate>
                                                <li class="<%# Eval("ClassIdentifier") %>">
                                                    <asp:Image ID="Image1" class="avatar" runat="server" ImageUrl="" />
                                                    <div class="message ">
                                                        <span class="arrow"></span><a href="" class="name"><%# Eval("UserName") %></a>
                                                        <span class="datetime">at  <asp:Literal ID="ltrMessageDate" runat="server"></asp:Literal></span>
                                                        <span class="body"><%# Eval("Message") %>
                                                        </span>
                                                    </div>
                                                </li>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </ul>
                                </div>
                            </div>
                            <div class="widget" id="Conversation" runat="server" clientidmode="static">
                                <div class="widget-title">
                                    <h4><i class="icon-comments-alt"></i>&nbsp;Reply</h4>
                                </div>
                                <div class="widget-body">
                                    <div class="control-group">
                                        <label class="control-label">Message&nbsp;<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:TextBox ID="txtMessage" name="txtMessage" runat="server" TextMode="MultiLine"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rqfvMessage" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="txtMessage"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="form-actions">
                                            <asp:Button ID="btnsave" Text="Submit" class="btn btn-success" ValidationGroup="ChangeGroup" runat="server" OnClick="btnsave_Click"/>
                                            <button type="button" onclick="window.location.href = '<%=Application["SiteAddress"]%>admin/PartnerTicket_List.aspx'" id="btncancel" class="btn">Cancel</button>
                                            <asp:Button ID="btnstatus" Text="Close Ticket" class="btn btninactive" runat="server" OnClick="btnstatus_Click"/>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
