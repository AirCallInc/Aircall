<%@ Page Title="" Language="C#" MasterPageFile="~/partner/PartnerMaster.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="Aircall.partner.dashboard" %>

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
            <div class="alert alert-success">
                Welcome to the <strong>Aircall Partner</strong> section. 
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-bar-chart"></i>Monthly Sales: <%= new DateTime(int.Parse(strcurrentYear),int.Parse(strcurrentmonth),1).ToString("MMM / yyyy") %></h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div id="site_statistics_loading">
                            <img src="<%=Application["SiteAddress"]%>partner/img/loading.gif" alt="loading" />
                        </div>
                        <div id="site_statistics_content" class="hide">
                            <div id="commission_report" class="chart"></div>
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
                    maxvalue = parseFloat(val2);
                if (val2 < minvalue)
                    minvalue = parseFloat(val2);
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
                    max: maxvalue,
                    title: {
                        text: 'Monthly Sales ($)'
                    }
                },
                legend: {
                    enabled: false
                },
                tooltip: {
                    formatter: function () {
                        var month = new Array();
                        month[1] = "JAN";
                        month[2] = "FEB";
                        month[3] = "MARCH";
                        month[4] = "APR";
                        month[5] = "MAY";
                        month[6] = "JUN";
                        month[7] = "JULY";
                        month[8] = "AUG";
                        month[9] = "SEP";
                        month[10] = "OCT";
                        month[11] = "NOV";
                        month[12] = "DEC";
                        var monthname;
                        monthname = month[this.x];
                        if ($('#drpMonth option:selected').val() != "0") {
                            monthname = month[$('#drpMonth option:selected').val()];
                            var d1 = parseInt(this.x) + 1;
                            return "Order on <b><%= strcurrentmonth %>/" + d1 + "/<%= strcurrentYear %> = $ " + Highcharts.numberFormat(this.y, 2, '.', ',') + "</b>";
                        } else {
                            monthname = month[this.x];
                            return 'Order on <b>' + monthname + ' /<%= strcurrentYear %> = $ ' + Highcharts.numberFormat(this.y, 2, '.', ',') + '</b>';
                        }

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
        //window.onresize = function(event) {
        //    atpageload();
        //}
        
    </script>
</asp:Content>
