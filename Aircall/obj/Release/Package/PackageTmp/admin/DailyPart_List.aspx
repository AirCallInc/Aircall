<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="DailyPart_List.aspx.cs" Inherits="Aircall.admin.DailyPart_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Request Service History Detail</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Daily Part List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>Daily Parts List</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <table class="dailypartlist">
                            <tr>
                                <th>Part Name</th>
                                <th>Service</th>
                                <th>Repair</th>
                            </tr>
                            <asp:ListView ID="lstDailyPart" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%#Eval("Name") %></td>
                                        <td>
                                            <input type="checkbox" class="checkboxes" id="chkcheckService" runat="server" checked='<%#Eval("IsIncludeInService") %>'/>
                                            <asp:HiddenField ID="hdnPartId" runat="server" Value='<%#Eval("Id") %>' />
                                        </td>
                                        <td>
                                            <input type="checkbox" class="checkboxes" id="chkcheckRepair" runat="server" checked='<%#Eval("IsIncludeInRepair") %>' />
                                        </td>

                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                            <tr class="last note">
                                <td class="controls" colspan="3">
                                    <span class="label label-important">NOTE!</span> <span>Once you will click on update, all the service scheduled after update will have Daily Part List as per the new updated list. </span>
                                    <br />
                                </td>
                            </tr>
                            <tr class="last">
                                <td colspan="3">
                                    <asp:Button ID="btnUpdate" Text="Update" CssClass="btn btn-primary" runat="server" OnClick="btnUpdate_Click"/>
                                    <button type="button" onclick="window.location.href = '<%=Application["SiteAddress"]%>admin/dashboard.aspx'" id="btncancel" class="btn">Cancel</button>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
