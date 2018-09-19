<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="unit_report.aspx.cs" Inherits="Aircall.admin.unit_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=Application["SiteAddress"]%>admin/js/highcharts.js"></script>
    <script src="<%=Application["SiteAddress"]%>admin/js/exporting.js"></script>
    <script src="https://code.highcharts.com/modules/data.js"></script>
    <%--<script src="https://code.highcharts.com/modules/drilldown.js"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Unit Report</h3>
                <ul class="breadcrumb">
                    <li>
                        <a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a>
                        <span class="divider">&nbsp;</span>
                    </li>
                    <li><a href="#">Unit Report</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-filter"></i>&nbsp;Unit Report Filter</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="widget-body form">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <div class="control-group">
                                        <label class="control-label">
                                            Manufacture Name
                                        </label>
                                        <div class="controls input-icon">
                                            <asp:TextBox ID="txtManufactureName" ClientIDMode="Static" runat="server" CssClass="input-large"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">
                                            Age Group&nbsp;<span class="required">*</span>
                                        </label>
                                        <div class="controls input-icon">
                                            <label class="radio">
                                                <asp:RadioButton type="radio" ID="rbBelow10" ClientIDMode="Static" runat="server" GroupName="rdpFaqType" Checked="True" />
                                                Below 10 Years</label>
                                            <label class="radio">
                                                <asp:RadioButton type="radio" ID="rbAbove10" ClientIDMode="Static" runat="server" GroupName="rdpFaqType" />
                                                Above 10 Years</label>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Date Range:</label>
                                        <div class="controls">
                                            <input class=" m-ctrl-medium" runat="server" readonly="readonly" clientidmode="static" size="16" type="text" id="txtStartDate" />
                                            <span class="add-on"><i class="icon-calendar"></i></span>
                                            <span id="drangeerror" class="fielderror" style="display: none;"></span>
                                        </div>
                                    </div>
                                    <div class="form-actions">
                                        <asp:Button ID="Button1" class="btn btn-success" runat="server" ValidationGroup="ChangeGroup" OnClientClick="Comparedates(event); return false;" UseSubmitBehavior="false" Text="Search" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-bar-chart"></i>&nbsp;Unit Report</h4>
                    </div>
                    <div class="widget-body">
                        <div id="site_statistics_loading">
                            <img src="<%=Application["SiteAddress"]%>img/loading.gif" alt="loading" />
                        </div>
                        <div id="site_statistics_content" class="hide">
                            <div id="barChart" class="chart"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-bar-chart"></i>&nbsp;Unit Age Report</h4>
                    </div>
                    <div class="widget-body">
                        <div id="PieChart" class="chart"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        jQuery(document).ready(function () {
            atpageload();
            $("#txtStartDate").daterangepicker({
                "startDate": "<%= strstartdt %>",
                "endDate": "<%= strenddt %>",
                "opens":"<%= opens %>"
            }, function (start, end, label) {
                //$("#txtStartDate").val(start.format('MM/dd/yyyy'));
                //$('#txtEndDate').val(end.format('MM/dd/yyyy'))
            });
        });
        function Comparedates(e) {
            if (true) {
                var strmonth = $('#txtStartDate').val().split(" - ")[0];
                var stryear = $('#txtStartDate').val().split(" - ")[1];
                BindClientData(strmonth, stryear);
            }
            e.preventDefault();
            return false;
        }
        function atpageload() {
            var strmonth = "<%= strstartdt %>";
            var stryear = "<%= strenddt %>";
            BindClientData(strmonth, stryear);
        }
        function BindClientData(strmonth, stryear) {
            var IsPackaged = true;
            if ($('#rbAbove10').is(':checked')) {
                IsPackaged = false;
            }
            $.ajax({
                type: "POST",
                url: "unit_report.aspx/BindChart",
                data: "{'ManufactureName':'" + $("#txtManufactureName").val() + "','strmonth':'" + strmonth + "','stryear':'" + stryear + "','IsPackaged':" + IsPackaged + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessSchedule,
                failure: function (response) {
                    alert(response.d);
                }
            });
            BindClientData1();
        }
        function OnSuccessSchedule(response) {
            var pageviews = [[, ]];
            var rowcount = response.d.split('##').length;
            var minvalue = 0, maxvalue = 0;
            for (var i = 0; i < rowcount; i++) {
                var Arr = response.d.split('##');
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
            BindBarChart(pageviews);
        }

        function BindClientData1() {
            $.ajax({
                type: "POST",
                url: "unit_report.aspx/BindChartAge",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessSchedule1,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }
        function OnSuccessSchedule1(response) {
            var pageviews = [];
            var rowcount = response.d.split('##').length;
            var minvalue = 0, maxvalue = 0;
            for (var i = 0; i < rowcount; i++) {
                var Arr = response.d.split('##');
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
            BindPieChart(pageviews);
        }

        function BindBarChart(pageviews) {
            $('#barChart').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Units By Manufacturers'
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
                    title: {
                        text: 'Total Units Serviced'
                    }
                },
                legend: {
                    enabled: false
                },
                tooltip: {
                    pointFormat: 'Units Serviced of <b>{point.name}</b> : <b>{point.y} </b>'
                },
                series: [{
                    name: 'Population',
                    data: pageviews,
                    dataLabels: {
                        enabled: true,
                        rotation: -90,
                        color: '#FFFFFF',
                        align: 'right',
                        //format: '{point.y:.1f}', // one decimal
                        format: '{point.y:.0f}',
                        y: 5, // 10 pixels down from the top
                        style: {
                            fontSize: '13px',
                            fontFamily: 'Verdana, sans-serif'
                        }
                    }
                }]
            });
        }

        function BindPieChart(pageviews) {
            $(function () {
                // Create the chart
                $('#PieChart').highcharts({
                    chart: {
                        type: 'pie'
                    },
                    title: {
                        text: 'Age Group'
                    },
                    plotOptions: {
                        series: {
                            dataLabels: {
                                enabled: true,
                                format: '{point.name}: {point.y:.2f}%'
                            }
                        }
                    },

                    tooltip: {
                        headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
                        pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}%</b> of total<br/>'
                    },
                    series: [{
                        name: 'Units Age',
                        colorByPoint: true,
                        data: pageviews
                    }]
                });
            });
        }

    </script>
</asp:Content>
