<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="Aircall.admin.dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=Application["SiteAddress"]%>admin/js/highcharts.js"></script>
    <script src="<%=Application["SiteAddress"]%>admin/js/exporting.js"></script>
    <script src="https://code.highcharts.com/modules/data.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Dashboard <small>statistics and more</small></h3>
                <ul class="breadcrumb">
                    <li><a href="#"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Dashboard</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div id="page" class="dashboard">
            <div class="alert alert-info">
                Welcome to the <strong>
                    <asp:Literal ID="ltrRoleName" runat="server"></asp:Literal></strong> section. 
            </div>
            <div class="row-fluid" id="dvSales" runat="server">
                <div class="span12">
                    <div class="widget">
                        <div class="widget-title">
                            <h4><i class="icon-bar-chart"></i>Monthly Sales: <%= new DateTime(int.Parse(strcurrentYear),int.Parse(strcurrentmonth),1).ToString("MMM / yyyy") %> <asp:Label ID="ltrTotal" ClientIDMode="Static" runat="server"></asp:Label></h4>
                            <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                        </div>
                        <div class="widget-body">
                            <div id="site_statistics_loading">
                                <img src="<%=Application["SiteAddress"]%>admin/img/loading.gif" alt="loading" />
                            </div>
                            <div id="site_statistics_content" class="hide">
                                <div id="commission_report" class="chart"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <asp:ListView ID="lstDashboardrw" runat="server" OnItemDataBound="lstDashboardrw_ItemDataBound">
                <ItemTemplate>
                    <div class="row-fluid">
                        <asp:ListView ID="lstDashboard" runat="server">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkBtn" runat="server" CssClass="icon-btn span2" PostBackUrl='<%#Eval("RedirectPage") %>'>
                            <i class='<%# Eval("icon") %>'></i>
                            <div><%# Eval("label") %></div>
                            <span class="badge badge-info"><%# Eval("Cnt") %></span>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                </ItemTemplate>
            </asp:ListView>
            <div class="row-fluid" id="dvOrder" runat="server">
                <div class="span12">
                    <div class="widget">
                        <div class="widget-title">
                            <h4><i class="icon-tags"></i>&nbsp;Recent Order List</h4>
                            <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                        </div>
                        <div class="widget-body">
                            <table class="table table-striped table-bordered table-advance table-hover">
                                <thead>
                                    <tr>
                                        <th><i class="icon-shopping-cart"></i><span>&nbsp;Order Number</span></th>
                                        <th><i class="icon-user"></i><span>&nbsp;Client Name</span></th>
                                        <th class="hidden-phone"><i class="icon-user"></i><span>&nbsp;Employee Name</span></th>
                                        <th><i class="icon-money"></i><span>&nbsp;Amount</span></th>
                                        <th class="hidden-phone"><i class="icon-money"></i><span>&nbsp;Charge By</span></th>
                                        <th>Action</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:ListView ID="lstResentOrders" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Eval("OrderNumber") %></td>
                                                <td class="highlight"><%# Eval("ClientName") %></td>
                                                <td class="hidden-phone"><%# Eval("Employee") %></td>
                                                <td>$<%# Eval("OrderAmount") %></td>
                                                <td class="hidden-phone"><%# Eval("ChargeBy") %>
                                                </td>
                                                <td>
                                                    <a href='<%=Application["SiteAddress"]%>admin/Order_View.aspx?OrderId=<%# Eval("Id") %>' class="btn btn-mini">
                                                        <i class="icon-eye-open"></i>&nbsp;View</a></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:ListView>
                                </tbody>
                            </table>
                            <div class="space7"></div>
                            <div class="clearfix">
                                <a href="<%=Application["SiteAddress"]%>admin/order_list.aspx" class="btn btn-mini pull-right"><i class="icon-share-alt"></i>&nbsp;All Orders</a>

                            </div>
                            <div class="clearfix"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>

        jQuery(document).ready(function () {
            App.setMainPage(true); App.init();
            atpageload();
        });
        function atpageload() {
            var strmonth = "<%= strcurrentmonth %>";
            var stryear = "<%= strcurrentYear %>";
            BindClientData(strmonth, stryear);
        }
        function BindClientData(strmonth, stryear) {
            $.ajax({
                type: "POST",
                url: "dashboard.aspx/BindChart",
                data: "{'ClientId':0,'strmonth':'" + strmonth + "','stryear':'" + stryear + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessSchedule,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }

        function OnSuccessSchedule(response) {
            $("#ltrTotal").text("(Total Sales: $ " + response.d.split("^^")[1] + ")");
            var pageviews = [[, ]];
            var rowcount = response.d.split("^^")[0].split('##').length;
            var minvalue = 0, maxvalue = 0;
            for (var i = 0; i < rowcount; i++) {
                var Arr = response.d.split("^^")[0].split('##');
                var FillVal = Arr[i].split('|');
                val1 = FillVal[0];
                val2 = FillVal[1];
                pageviews[i] = [val1, parseFloat(val2)];
                //pageviews.push([val1, parseFloat(val2)]);
                if (val2 > maxvalue)
                    maxvalue = parseInt(val2);
                if (val2 < minvalue)
                    minvalue = parseInt(val2);
            }
            $('#site_statistics_loading').hide();
            $('#site_statistics_content').show();
            maxvalue = (maxvalue > 0 ? maxvalue + 100 : 1);
            BindBarChart(pageviews, maxvalue);
        }

        function BindBarChart(pageviews, maxvalue) {
            $('#commission_report').highcharts({
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    type: 'category',
                    labels: {
                        rotation: -45,
                        style: {
                            fontSize: '13px',
                            fontFamily: 'Verdana, sans-serif'
                        }
                    }
                },
                yAxis: {
                    min: 0,
                    max:maxvalue,
                    title: {
                        text: 'Monthly Sales ($)'
                    }
                },
                legend: {
                    enabled: false
                },
                tooltip: {
                    formatter: function () {
                        var d1 = parseInt(this.x) + 1;
                        return 'Order on <b><%= strcurrentmonth %>/' + d1 + '/<%= strcurrentYear %> : $ ' + Highcharts.numberFormat(this.y, 2, '.', ',') + ' </b>';
                    }
                },
                series: [{
                    name: 'Population',
                    data: pageviews,
                    dataLabels: {
                        enabled: false,
                        rotation: -90,
                        color: '#FFFFFF',
                        align: 'right',
                        //format: '{point.y:.1f}', // one decimal
                        format: '{point.y:.2f}',
                        y: 5, // 10 pixels down from the top
                        style: {
                            fontSize: '13px',
                            fontFamily: 'Verdana, sans-serif'
                        }
                    }
                }]
            });
        }
    </script>
</asp:Content>
