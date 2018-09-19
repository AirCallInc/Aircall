<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="partner_report.aspx.cs" Inherits="Aircall.admin.partner_report" %>

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
                <h3 class="page-title">Partners Sales Report</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Partners Sales Report</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-filter"></i>&nbsp;Partners Sales Report Filter</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="widget-body form">
                            <div class="form-horizontal">
                                <div class="control-group">
                                    <div class="control-group">
                                        <label class="control-label">
                                            Partner
                                        </label>
                                        <div class="controls input-icon">
                                            <asp:TextBox ID="txtPartnerName" ClientIDMode="Static" runat="server" CssClass="input-large"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Year<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="ddlYear" runat="server" ClientIDMode="Static">
                                                <asp:ListItem Value="0">select Year</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvYear" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="ddlYear" InitialValue="0"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="control-group">
                                        <label class="control-label">Month<span class="required">*</span></label>
                                        <div class="controls">
                                            <asp:DropDownList ID="drpMonth" runat="server" ClientIDMode="Static">
                                                <asp:ListItem Value="-1">Select Month</asp:ListItem>
                                                <asp:ListItem Value="0">ALL MONTHS</asp:ListItem>
                                                <asp:ListItem Value="1">JAN</asp:ListItem>
                                                <asp:ListItem Value="2">FEB</asp:ListItem>
                                                <asp:ListItem Value="3">MAR</asp:ListItem>
                                                <asp:ListItem Value="4">APR</asp:ListItem>
                                                <asp:ListItem Value="5">MAY</asp:ListItem>
                                                <asp:ListItem Value="6">JUN</asp:ListItem>
                                                <asp:ListItem Value="7">JULY</asp:ListItem>
                                                <asp:ListItem Value="8">AUG</asp:ListItem>
                                                <asp:ListItem Value="9">SEP</asp:ListItem>
                                                <asp:ListItem Value="10">OCT</asp:ListItem>
                                                <asp:ListItem Value="11">NOV</asp:ListItem>
                                                <asp:ListItem Value="12">DEC</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rqfvMonth" runat="server" ErrorMessage="Required" Font-Size="12px" Font-Bold="true" Display="Dynamic" ValidationGroup="ChangeGroup" CssClass="error_required" ControlToValidate="drpMonth" InitialValue="-1"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div class="form-actions">
                                        <%--<button type="button" class="btn btn-success" id="btnSubmit">Search</button>--%>
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
                        <h4><i class="icon-bar-chart"></i>Partners Monthly Sales: <%= new DateTime(int.Parse(strcurrentYear),int.Parse(strcurrentmonth),1).ToString("MMM / yyyy") %></h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div id="site_statistics_loading">
                            <img src="<%=Application["SiteAddress"]%>admin/img/loading.gif" alt="loading" />
                        </div>
                        <div id="site_statistics_content" class="hide">
                            <div id="site_statistics" class="chart"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-bar-chart"></i>&nbsp;Client List</h4>
                    </div>
                    <div class="widget-body">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <table class="table table-striped table-bordered" id="sample_133">
                                    <thead>
                                        <tr>
                                            <th>Sr. No.</th>
                                            <th>Client Name</th>
                                            <th>Unit Name</th>
                                            <th>Plan</th>
                                            <th>Contract Months</th>
                                            <th>Contract Amount</th>
                                            <th>Date Acquired</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstPartnerClient" runat="server">
                                            <ItemTemplate>
                                                <tr class="odd gradeX">
                                                    <td class="srno hidden-phone"><%# Container.DataItemIndex + 1 %></td>
                                                    <td data-title="Client Name"><%#Eval("FirstName")%> <%#Eval("LastName") %></td>
                                                    <td data-title="Unit Name"><%#Eval("UnitName") %></td>
                                                    <td data-title="Plan"><%#Eval("Name") +" - " + Eval("PackageDisplayName") %></td>
                                                    <td data-title="Contract Months"><%#Eval("DurationInMonth")+" "+"Month" %></td>
                                                    <td data-title="Contract Amount">$ <%#Eval("Price") %></td>
                                                    <td data-title="Date Acquired"><%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
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
        });
        function Comparedates(e) {
            if (Page_ClientValidate()) {
                var stryear = $('#ddlYear option:selected').val();
                var strmonth = $('#drpMonth option:selected').val();
                BindClientData(strmonth, stryear);
                $("#Button2").click();
            }
            e.preventDefault();
            return false;
        }
        function atpageload() {
            var strmonth = "<%= strcurrentmonth %>";
            var stryear = "<%= strcurrentYear %>";
            $('#drpMonth').val("<%= strcurrentmonth %>");
            $('#ddlYear').val("<%= strcurrentYear %>");
            BindClientData(strmonth, stryear);
        }
        function BindClientData(strmonth, stryear) {
            $.ajax({
                type: "POST",
                url: "partner_report.aspx/BindChart",
                data: "{'partner':'" + $("#txtPartnerName").val() + "','strmonth':'" + strmonth + "','stryear':'" + stryear + "'}",
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
            $('#site_statistics').highcharts({
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
                            console.log(parseInt(this.x) + 1);
                            var d1 = parseInt(this.x) + 1;
                            return "Order on <b>" + $('#drpMonth option:selected').val() + "/" + d1 + "/" + $('#ddlYear option:selected').val() + " = $ " + Highcharts.numberFormat(this.y, 2, '.', ',') + "</b>";
                        } else {
                            monthname = month[this.x+1];
                            return 'Order on <b>' + monthname + '/ <%= strcurrentYear %> = $ ' + Highcharts.numberFormat(this.y, 2, '.', ',') + '</b>';
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
    </script>
</asp:Content>
