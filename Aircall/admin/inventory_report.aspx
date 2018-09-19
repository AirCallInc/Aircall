<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="inventory_report.aspx.cs" Inherits="Aircall.admin.inventory_report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=Application["SiteAddress"]%>admin/js/highcharts.js"></script>
    <script src="<%=Application["SiteAddress"]%>admin/js/exporting.js"></script>
    <script src="https://code.highcharts.com/modules/data.js"></script>
    <%--<script src="https://code.highcharts.com/modules/drilldown.js"></script>--%>
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
                <h3 class="page-title">Missing Inventory Report</h3>
                <ul class="breadcrumb">
                    <li>
                        <a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a>
                        <span class="divider">&nbsp;</span>
                    </li>
                    <li><a href="#">Missing Inventory Report</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-filter"></i>&nbsp;Missing Inventory Report Filter</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="widget-body form">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <div class="control-group">
                                        <label class="control-label">
                                            Employee Name
                                        </label>
                                        <div class="controls input-icon">
                                            <asp:TextBox ID="txtManufactureName" ClientIDMode="Static" runat="server" CssClass="input-large"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">
                                            Part Name
                                        </label>
                                        <div class="controls input-icon">
                                            <asp:TextBox ID="txtPartName" ClientIDMode="Static" runat="server" CssClass="input-large"></asp:TextBox>
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
                                        <asp:Button ID="Button2" ClientIDMode="Static" class="btn btn-success" runat="server" ValidationGroup="ChangeGroup" OnClick="Button2_Click" Text="Search" Style="display: none;" />
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
                        <h4><i class="icon-bar-chart"></i>&nbsp;Missing Inventory Report</h4>
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
                        <h4><i class="icon-bar-chart"></i>&nbsp;Missing Inventory List</h4>
                    </div>
                    <div class="widget-body">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table class="table table-striped table-bordered" id="sample_133">
                                    <thead>
                                        <tr>
                                            <th style="width: 44px;" class="hidden-phone srno">Sr. No.</th>
                                            <th>Service Report No</th>
                                            <th>Employee Name</th>
                                            <th>Part Name</th>
                                            <th>Quantity</th>
                                            <th>Service Date</th>
                                            <th>Price</th>
                                        </tr>
                                    </thead>
                                    <tbody>

                                        <asp:ListView ID="lstRating" runat="server">
                                            <ItemTemplate>
                                                <tr class="odd gradeX">
                                                    <td class="srno hidden-phone"><%# Container.DataItemIndex + 1 %></td>

                                                    <td data-title="Service Report No"><a href="<%=Application["SiteAddress"]%>admin/ServiceReport_View.aspx?ReportId=<%# Eval("ReportId") %>"><%# Eval("ServiceReportNumber") %>&nbsp;</a></td>
                                                    <td data-title="Employee Name"><%# Eval("EmployeeName") %></td>
                                                    <td data-title="Part Name"><%# Eval("PartName") %></td>
                                                    <td data-title="Quantity"><%# Eval("RequestedQuantity") %></td>
                                                    <td data-title="Service Date"><%# DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
                                                    <td data-title="Price">$ <%# Eval("SellingPrice") %></td>
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
        jQuery(document).ready(function () {
            atpageload();
            $("#txtStartDate").daterangepicker({
                "startDate": "<%= strstartdt1 %>",
                "endDate": "<%= strenddt1 %>",
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
            $("#Button2").click();
            e.preventDefault();
            return false;
        }
        function atpageload() {
            var strmonth = "<%= strstartdt1 %>";
            var stryear = "<%= strenddt1 %>";
            BindClientData(strmonth, stryear);
        }
        function BindClientData(strmonth, stryear, PartName) {
            $.ajax({
                type: "POST",
                url: "inventory_report.aspx/BindChart",
                data: "{'EmpName':'" + $("#txtManufactureName").val() + "','strmonth':'" + strmonth + "','stryear':'" + stryear + "','PartName':'" + $("#txtPartName").val() + "'}",
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
                    maxvalue = parseInt(val2);
                if (val2 < minvalue)
                    minvalue = parseInt(val2);
            }
            $('#site_statistics_loading').hide();
            $('#site_statistics_content').show();
            BindBarChart(pageviews);
        }

        function BindBarChart(pageviews) {
            $('#barChart').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Missing Part Inventory'
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
                        text: 'Total Parts'
                    }
                },
                legend: {
                    enabled: false
                },
                tooltip: {
                    pointFormat: 'Part Missing : <b>{point.y} </b>'
                },
                series: [{
                    name: 'Population',
                    data: pageviews,
                    dataLabels: {
                        enabled: true,
                        rotation: -45,
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
    </script>
</asp:Content>
