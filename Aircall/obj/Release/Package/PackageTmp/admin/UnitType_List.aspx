<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="UnitType_List.aspx.cs" Inherits="Aircall.admin.UnitType_List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Unit Type List</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Unit Type List</a><span class="divider-last">&nbsp;</span></li>
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
                        <h4><i class="icon-cogs"></i>
                            Unit Type List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Model #</label>
                                <asp:TextBox ID="txtModel" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Serial #</label>
                                <asp:TextBox ID="txtSerial" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Manufacture Brand</label>
                                <asp:TextBox ID="txtMfg" runat="server" CssClass="input-medium"></asp:TextBox>
                                <div style="clear:both;margin-top:40px;"></div>
                                <label class="filter-label">Added By</label>
                                <asp:DropDownList ID="drpAddedBy" runat="server" CssClass="input-medium">
                                    <asp:ListItem Value="0">Select</asp:ListItem>
                                    <asp:ListItem Value="2">Admin</asp:ListItem>
                                    <asp:ListItem Value="5">Employee</asp:ListItem>
                                    <asp:ListItem Value="1">Super Admin</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click"/>
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'UnitType_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkActive" runat="server" CssClass="btn btn-success hidden-phone" OnClick="lnkActive_Click">
                                <i class="icon-ok icon-white"></i>Active
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkInactive" runat="server" CssClass="btn btninactive hidden-phone" OnClick="lnkInactive_Click">
                                <i class="icon-off icon-white"></i>Inactive
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger" OnClick="lnkDelete_Click">
                                <i class="icon-remove icon-white"></i>Delete
                            </asp:LinkButton>
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/UnitType_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Unit Type
                            </a>
                        </div>
                        <table class="table table-striped table-bordered" id="sample_12">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_12 .checkboxes" />
                                    </th>
                                    <th>Sr #</th>
                                    <th class="hidden-phone">Model Number</th>
                                    <th class="hidden-phone">Serial Number</th>
                                    <th>Manufacture Brand</th>
                                    <th class="hidden-phone">Added By</th>
                                    <th>Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstUnitType" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX <%#Eval("Name").ToString().ToLower() %>">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnUnitId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td><%# Container.DataItemIndex + 1 %></td>
                                            <td><%#Eval("ModelNumber") %></td>
                                            <td><%#Eval("SerialNumber") %></td>
                                            <td><%#Eval("ManufactureBrand") %></td>
                                            <td><%#Eval("Name") %></td>
                                            <td>
                                                <span class="label label-<%#Eval("Status").ToString().ToLower()=="true"?"active":"inactive"%>"><%#Eval("Status").ToString().ToLower()=="true"? "Active" :"Inactive"%></span>
                                            </td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/UnitType_AddEdit.aspx?UnitId=<%#Eval("Id") %>" class="btn mini purple"><i class="icon-edit"></i>Edit</a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                        <asp:DataPager ID="dataPagerUnitType" runat="server" PagedControlID="lstUnitType"
                            OnPreRender="dataPagerUnitType_PreRender">
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
            var PageSize = '<%=ConfigurationManager.AppSettings["PageSize"]%>';
            $('#sample_12').dataTable({
                "sDom": "<'row-fluid'<'span6'><'span6'f>r>t<'row-fluid'>",
                "oSearch": { "bSmart": false, "bRegex": true },
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0]
                }],
                "iDisplayLength": PageSize
            });

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
