<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="rating_report.aspx.cs" Inherits="Aircall.admin.rating_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=Application["SiteAddress"]%>admin/js/highcharts.js"></script>
    <script src="<%=Application["SiteAddress"]%>admin/js/exporting.js"></script>
    <script src="https://code.highcharts.com/modules/data.js"></script>
    <style>
        @media only screen and (max-width: 800px) {
            #sample_133 {
                width: 100%;
            }
                /* Force table to not be like tables anymore */
                #sample_133 table,
                #sample_133 thead,
                #sample_133 tbody,
                #sample_133 th,
                #sample_133 td,
                #sample_133 tr {
                    display: block;
                }

                    /* Hide table headers (but not display: none;, for accessibility) */
                    #sample_133 thead tr {
                        position: absolute;
                        top: -9999px;
                        left: -9999px;
                    }

                #sample_133 tr {
                    border: 1px solid #ccc;
                }

                #sample_133 td {
                    /* Behave  like a "row" */
                    border: none;
                    border-bottom: 1px solid #eee;
                    position: relative;
                    padding-left: 50%;
                    white-space: normal;
                    text-align: left;
                    width: inherit !important;
                }

                    #sample_133 td:before {
                        /* Now like a table header */
                        position: absolute;
                        /* Top/left values mimic padding */
                        top: 6px;
                        left: 6px;
                        width: 45%;
                        padding-right: 10px;
                        white-space: nowrap;
                        text-align: left;
                        font-weight: bold;
                    }

                    #sample_133 td:before {
                        content: attr(data-title);
                    }
        }

        a.btn.btn-default {
            float: left;
            padding: 4px 12px;
            line-height: 20px;
            text-decoration: none;
            background-color: #fff;
            border: 1px solid #ddd;
        }

        span.btn.btn-primary.disabled {
            float: left;
            padding: 4px 12px;
            line-height: 20px;
            text-decoration: none;
            background-color: #f5f5f5;
            border: 1px solid #ddd;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Rating Report</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Rating Report</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-filter"></i>&nbsp;Rating Report Filter</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="widget-body form">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <div class="control-group">
                                        <label class="control-label">
                                            Employee
                                        </label>
                                        <div class="controls input-icon">
                                            <asp:TextBox ID="txtEmpName" ClientIDMode="Static" runat="server" CssClass="input-large"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Date Range:</label>
                                        <div class="controls">
                                            <input class=" m-ctrl-medium" runat="server" readonly="readonly" clientidmode="static" size="16" type="text" id="txtStartDate" />
                                            <span class="add-on"><i class="icon-calendar"></i></span>
                                        </div>
                                    </div>
                                    <div class="form-actions">
                                        <asp:Button ID="Button1" class="btn btn-success" runat="server" ValidationGroup="ChangeGroup" OnClientClick="Comparedates(event); return false;" UseSubmitBehavior="false" Text="Search" />
                                        <asp:Button ID="Button2" runat="server" Text="Search" Style="display: none;" OnClick="Button2_Click" />
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
                        <h4><i class="icon-bar-chart"></i>&nbsp;Rating Report</h4>
                    </div>
                    <div class="widget-body">
                        <div id="site_statistics_loading">
                            <img src="<%=Application["SiteAddress"]%>img/loading.gif" alt="loading" />
                        </div>
                        <div id="site_statistics_content" class="hide">
                            <div id="graph_2" class="chart"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-bar-chart"></i>&nbsp;Rating By Client</h4>
                    </div>
                    <div class="widget-body">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table class="table table-striped table-bordered" id="sample_133">
                                    <thead>
                                        <tr>
                                            <th style="width: 44px;" class="hidden-phone srno">Sr. No.</th>
                                            <th>Client Name</th>
                                            <th>Rating</th>
                                            <th>Service Date</th>
                                            <th>Service No</th>
                                            <th>Service Report No</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstRating" runat="server">
                                            <ItemTemplate>
                                                <tr class="odd gradeX">
                                                    <td class="srno hidden-phone"><%# Container.DataItemIndex + 1 %></td>
                                                    <td data-title="Client Name"><a href="<%=Application["SiteAddress"]%>admin/Client_AddEdit.aspx?ClientId=<%# Eval("ClientId") %>"><%# Eval("ClientName") %></a></td>
                                                    <td data-title="Rating"><%# Eval("RatingsReport") %></td>
                                                    <td data-title="Service Date"><%# DateTime.Parse(Eval("ScheduleDate").ToString()).ToString("dd-MMM-yyyy") %></td>
                                                    <td data-title="Service No"><a href="<%=Application["SiteAddress"]%>admin/CompletedService_View.aspx?ServiceId=<%# Eval("Id") %>"><%# Eval("ServiceCaseNumber") %></a></td>
                                                    <td data-title="Service Report No"><a href="<%=Application["SiteAddress"]%>admin/ServiceReport_View.aspx?ReportId=<%# Eval("ReportId") %>"><%# Eval("ServiceReportNumber") %></a></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </tbody>
                                </table>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        prm.add_endRequest(function (e) {
            $("#sample_133").dataTable({
                //"sDom": "<'row-fluid'<'span12'f>r>t<'row-fluid'>",
                //"aoColumnDefs": [{
                //    'bSortable': false,
                //    'aTargets': [0]
                //}],
                //"oSearch": { "bSmart": false, "bRegex": true }
                "sDom": "<'row-fluid'<'span6'l><'span6'f>r>t<'row-fluid'<'span6'i><'span6'p>>",
                "sPaginationType": "bootstrap",
                "oLanguage": {
                    "sLengthMenu": "_MENU_ records per page",
                    "oPaginate": {
                        "sPrevious": "Prev",
                        "sNext": "Next"
                    }
                },
                "oSearch": { "bSmart": false, "bRegex": true },
                "aoColumnDefs": [{
                    'bSortable': false,
                    'aTargets': [0]
                }]
            });

        });
        jQuery(document).ready(function () {
            App.setMainPage(false);
            App.init();
            atpageload();
            $("#ContentPlaceHolder1_Button2").click();
            $("#txtStartDate").daterangepicker({
                "startDate": "<%= strstartdt %>",
                "endDate": "<%= strenddt %>",
                "opens": "<%= opens %>"
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
                $("#ContentPlaceHolder1_Button2").click();
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
            $.ajax({
                type: "POST",
                url: "rating_report.aspx/BindChart",
                data: "{'EmpName':'" + $("#txtEmpName").val() + "','strmonth':'" + strmonth + "','stryear':'" + stryear + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessSchedule,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }
        function OnSuccessSchedule(response) {
            var graphData = [];
            var rowcount = response.d.split('##').length;

            for (var i = 0; i < rowcount; i++) {
                var Arr = response.d.split('##');
                var FillVal = Arr[i].split('|');
                val1 = FillVal[0];
                val2 = FillVal[1];
                graphData[i] = {
                    name: "Rating " + val1,
                    y: parseFloat(val2)
                }
            }
            $('#site_statistics_loading').hide();
            $('#site_statistics_content').show();
            graphs(graphData);
        }
        function graphs(graphData) {
            $('#graph_2').highcharts({
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                legend: {
                    enabled: false
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                              format: '<b>{point.name}</b>: {point.percentage:.2f} %',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            },
                           
                        }
                    }
                },
                tooltip: {
                    enabled: false
                    //formatter: function () {
                    //     return "<b>" + this.point.name + ": " + Highcharts.numberFormat(this.point.y, 2) + "%</b>";
                    //}
                },
                series: [{
                    name: 'Ratings',
                    colorByPoint: true,
                    data: graphData
                }]
            });

            
        }
        //function graphs(graphData) {
        //    var series = Math.floor(Math.random() * 10) + 1;
        //    series = graphData.length;

        //    $.plot($("#graph_2"), graphData, {
        //        series: {
        //            pie: {
        //                show: true,
        //                radius: 1,
        //                label: {
        //                    show: true,
        //                    radius: 3 / 4,
        //                    formatter: function (label, series) {
        //                        return '<div style="font-size:8pt;text-align:center;padding:2px;color:white;">' + label + '<br/>' + Math.round(series.percent) + '%</div>';
        //                    },
        //                    background: {
        //                        opacity: 0.5
        //                    }
        //                }
        //            }
        //        },
        //        legend: {
        //            show: false
        //        }
        //    });

        //    function pieHover(event, pos, obj) {
        //        if (!obj) return;
        //        percent = parseFloat(obj.series.percent).toFixed(2);
        //        $("#hover").html('<span style="font-weight: bold; color: ' + obj.series.color + '">' + obj.series.label + ' (' + percent + '%)</span>');
        //    }

        //    function pieClick(event, pos, obj) {
        //        if (!obj) return;
        //        percent = parseFloat(obj.series.percent).toFixed(2);
        //        alert('' + obj.series.label + ': ' + percent + '%');
        //    }
        //}
    </script>
</asp:Content>
