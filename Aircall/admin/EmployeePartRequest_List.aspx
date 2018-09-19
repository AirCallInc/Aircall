<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="EmployeePartRequest_List.aspx.cs" Inherits="Aircall.admin.EmployeePartRequest_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Employee Request Part</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Employee Requested Part List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-wrench"></i>
                            Employee Request Part List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Employee Name</label>
                                <asp:TextBox ID="txtEmpname" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Part Name</label>
                                <asp:TextBox ID="txtPart" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Part Type</label>
                                <asp:DropDownList ID="drpPartType" runat="server" CssClass="input-medium"></asp:DropDownList>
                                <div class="clear" style="margin-top: 15px;"></div>
                                <label class="filter-label">Status</label>
                                <asp:DropDownList ID="drpStatus" runat="server" CssClass="input-medium"></asp:DropDownList>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'EmployeePartRequest_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons" style="text-align: right;">
                            <a class="btn btn-info" href="<%=Application["SiteAddress"]%>admin/EmployeePartRequest_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Employee Request Part
                            </a>
                        </div>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                    </th>
                                    <th>Sr #</th>
                                    <th>Employee Name</th>
                                    <th class="hidden-phone">Part Name</th>
                                    <th class="hidden-phone">Quantity</th>
                                    <th class="hidden-phone">Status</th>
                                    <th>Requested On</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstEmpPartRequest" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX <%#Eval("Status").ToString()=="Need to Order"?"waiting-approval":"" %>">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnEmpReqPartId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("EmployeeName") %></td>
                                            <td><%#Eval("PartName") %></td>
                                            <td><%#Eval("RequestedQuantity") %></td>
                                            <td>
                                                <span class="label label-<%#Eval("Status").ToString().ToLower()=="true"?"active":"inactive"%>"><%#Eval("Status")%></span>
                                            </td>
                                            <td><%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/EmployeePartRequest_AddEdit.aspx?Id=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="dataPagerEmpPartRequest" runat="server" PagedControlID="lstEmpPartRequest"
                            OnPreRender="dataPagerEmpPartRequest_PreRender">
                            <Fields>
                                <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                                    ShowNextPageButton="false" />
                                <asp:NumericPagerField ButtonType="Link" />
                                <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false"
                                    ShowPreviousPageButton="false" />
                            </Fields>
                        </asp:DataPager>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        $(document).ready(function () {
           

            jQuery('#sample_12 .group-checkable').change(function () {
                var set = jQuery(this).attr("data-set");
                var checked = jQuery(this).is(":checked");
                jQuery(set).each(function () {
                    if (checked) {
                        $(this).attr("checked", true);
                    } else {
                        $(this).attr("checked", false);
                    }
                });
                jQuery.uniform.update(set);
            });

            jQuery('#sample_12_wrapper .dataTables_filter input').addClass("input-medium"); // modify table search input
        })
    </script>
</asp:Content>
