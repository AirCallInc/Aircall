<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="PendingService_List.aspx.cs" Inherits="Aircall.admin.PendingService_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/dhtmlxscheduler.js"></script>
    <script src="js/dhtmlxscheduler_timeline.js"></script>
    <script src="js/dhtmlxscheduler_treetimeline.js"></script>
    <script src="js/dhtmlxscheduler_container_autoresize.js"></script>
    <script src="js/dhtmlxscheduler_tooltip.js"></script>
    <script type="text/javascript" src="//maps.google.com/maps/api/js?sensor=true&key=AIzaSyAqIYKCniqGpTtlp_QSeJPPqRZ1bRt6A9M"></script>
    <script src="js/demo.gmaps.js"></script>
    <script src="js/gmaps.js"></script>
    <link href="css/dhtmlxscheduler.css" rel="stylesheet" />

    <style type="text/css" media="screen">
        .one_line {
            white-space: nowrap;
            overflow: hidden;
            padding-top: 5px;
            padding-left: 5px;
            text-align: left !important;
        }

        .dhx_row_folder {
            color: #000 !important;
        }

        .dhx_cal_light.dhx_cal_light_wide select {
            width: auto;
            height: auto;
        }

        .empschedules {
            width: 100%;
            border: 1px solid #ccc;
        }

            .empschedules tbody {
                width: 100%;
            }

            .empschedules .heading {
                font-size: 15px;
                font-weight: bold;
                margin-right: 10px;
                padding: 5px;
            }

            .empschedules tr.heading {
                height: 20px;
                border-bottom: 3px solid #ccc;
            }

            .empschedules .col1 {
                width: 20%;
                border-right: 3px solid #ccc;
                vertical-align: top;
            }

            .empschedules .col2 {
                width: 50%;
                border-right: 3px solid #ccc;
                vertical-align: top;
            }

            .empschedules .col3 {
                width: 30%;
                vertical-align: top;
            }

        .hidemapcalview {
            width: 80% !important;
        }

        .mapoff {
            padding: 4px 6px 4px 6px;
            background-color: grey;
            border-radius: 5px;
            color: #fff;
            cursor: pointer;
            float: right;
            margin-right: 4px;
            margin-top: 3px;
        }

        .mapon {
            padding: 4px 6px 4px 6px;
            background-color: #DB5E61;
            border-radius: 5px;
            color: #fff;
            cursor: pointer;
            float: right;
            margin-right: 4px;
            margin-top: 3px;
        }

            .mapon:hover {
                padding: 4px 6px 4px 6px;
                text-decoration: none;
                color: #fff;
                background: #BA3235;
                margin-right: 4px;
                margin-top: 3px;
            }

        .mapoff:hover {
            padding: 4px 6px 4px 6px;
            text-decoration: none;
            color: #fff;
            background: #BA3235;
            margin-right: 4px;
            margin-top: 3px;
        }

        #tdCalendarView #calendar {
            max-height: 400px;
            overflow: overlay;
            padding-right: 14px;
        }
    </style>
    <script type="text/javascript" charset="utf-8">
        function init() {

            scheduler.locale.labels.timeline_tab = "Timeline";
            scheduler.locale.labels.section_custom = "Section";
            scheduler.config.details_on_create = true;
            scheduler.config.details_on_dblclick = true;
            scheduler.config.xml_date = "%Y-%m-%d %H:%i";
            scheduler.config.multisection = true;
            scheduler.config.default_date = "%j %F %Y, %l";
            //scheduler.config.readonly = false; //not allowd to drag and resize

            scheduler.attachEvent("onDblClick", function (id, e){
                var obj = scheduler.getEvent(id);
                obj.readonly = false;
                return false;
            });

            scheduler.attachEvent("onBeforeDrag", function(){
                return false;
            });

            scheduler.attachEvent("onBeforeLightbox",function(id, e){
                j = 0;
                var checked=jQuery('#sample_12 .checkboxes').is(":checked");
                if (!checked) {
                    scheduler.deleteEvent(id);
                    alert("Please Select atleast one service");
                    return false;
                }
                return true;
            });

            scheduler.attachEvent("onEventSave",function(id,ev,is_new){
                var SId=0;
                jQuery('#sample_12 .checkboxes').each(function (){
                    if ($(this).is(":checked")) {
                        SId=$(this).val();
                    }
                });
                $.ajax({
                    type: "POST",
                    url: "PendingService_List.aspx/SetEmployeeForService",
                    data: "{'ServiceDate':'" + ev.start_date.format("MM/dd/yyyy") + "','StartTime':'" + ev.start_date.format("HH:mm") + "','EndTime':'" + ev.end_date.format("HH:mm") + "','EmployeeId':'" + ev.section_id + "','ServiceId':'" + SId + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        //alert(response.d);
                        if (response.d=="not allowed") {
                            alert("Only Emegency service schedule on Saturday and Sunday.");
                            scheduler.deleteEvent(id);
                            return false;
                        }
                        window.location.href=response.d;
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
                return true;
            });


            //===============
            //Configuration
            //===============	
            
            var elements=<%= workareaEmployee %>;

            scheduler.ignore_timeline = function(date) {
                //hides Saturdays and Sundays 
                //if (date.getDay() == 6 || date.getDay() == 0) // 0 refers to Sunday, 6 - to Saturday
                //return true;
                //non-working hours
                if (date.getHours() < 8 || date.getHours() > 19)
                    return true;
            };

            //===============
            //Display duration while resizing
            var durations = {
                day: 24 * 60 * 60 * 1000,
                hour: 60 * 60 * 1000,
                minute: 60 * 1000
            };

            var get_formatted_duration = function (start, end) {
                var diff = end - start;

                var days = Math.floor(diff / durations.day);
                diff -= days * durations.day;
                var hours = Math.floor(diff / durations.hour);
                diff -= hours * durations.hour;
                var minutes = Math.floor(diff / durations.minute);

                var results = [];
                if (days) results.push(days + " days");
                if (hours) results.push(hours + " hours");
                if (minutes) results.push(minutes + " minutes");
                return results.join(", ");
            };


            var resize_date_format = scheduler.date.date_to_str(scheduler.config.hour_date);

            scheduler.templates.event_bar_text = function (start, end, event) {
                var state = scheduler.getState();
                if (state.drag_id == event.id) {
                    return resize_date_format(start) + " - " + resize_date_format(end) + " (" + get_formatted_duration(start, end) + ")";
                }
                return event.text; // default
            };
            //===============

            //===============
            // Tooltip related code
            // we want to save "dhx_cal_data" div in a variable to limit look ups
            var scheduler_container = document.getElementById("scheduler_here");
            var scheduler_container_divs = scheduler_container.getElementsByTagName("div");
            var dhx_cal_data = scheduler_container_divs[scheduler_container_divs.length - 1];

            // while target has parent node and we haven't reached dhx_cal_data
            //// we can keep checking if it is timeline section
            scheduler.dhtmlXTooltip.isTooltipTarget = function (target) {
                while (target.parentNode && target != dhx_cal_data) {
                    var css = target.className.split(" ")[0];
                    // if we are over matrix cell or tooltip itself
                    if (css == "dhx_matrix_scell" || css == "dhtmlXTooltip") {
                        return { classname: css };
                    }
                    target = target.parentNode;
                }
                return false;
            };

            scheduler.attachEvent("onMouseMove", function (id, e) {
                var timeline_view = scheduler.matrix[scheduler.getState().mode];

                // if we are over event then we can immediately return
                // or if we are not on timeline view
                if (id || !timeline_view) {
                    return;
                }

                // native mouse event
                e = e || window.event;
                var target = e.target || e.srcElement;


                //make a copy of event, will be used in timed call, ie8 comp
                var ev = {
                    'pageX': undefined,
                    'pageY': undefined,
                    'clientX': undefined,
                    'clientY': undefined,
                    'target': undefined,
                    'srcElement': undefined
                };
                for (var i in ev) {
                    ev[i] = e[i];
                }

                var tooltip = scheduler.dhtmlXTooltip;
                var tooltipTarget = tooltip.isTooltipTarget(target);
                if (tooltipTarget) {
                    if (tooltipTarget.classname == "dhx_matrix_scell") {
                        // we are over cell, need to get what cell it is and display tooltip
                        var section_id = scheduler.getActionData(e).section;
                        var section = timeline_view.y_unit[timeline_view.order[section_id]];

                        // showing tooltip itself
                        var text = "<b>" + section.label + "</b>";
                        //tooltip.delay(tooltip.show, tooltip, [ev, text]);
                    }
                    if (tooltipTarget.classname == "dhtmlXTooltip") {
                        dhtmlxTooltip.delay(tooltip.show, tooltip, [ev, tooltip.tooltip.innerHTML]);
                    }
                }
            });
            //===============

            scheduler.createTimelineView({
                section_autoheight: true,
                name: "timeline",
                x_unit: "minute",
                x_date: "%H:%i",
                x_step: 30,
                x_size: 22,
                x_start: 16,
                x_length: 48,
                //x_unit: "hour",
                //x_date: "%H:%i",
                //x_step: 1,
                //x_size: 72,
                //x_start: 0,
                //x_length: 72,
                y_unit: elements,
                y_property: "section_id",
                render: "tree",
                second_scale: {
                    x_unit: "day", // unit which should be used for second scale
                    x_date: "%F %d" // date format which should be used for second scale, "July 01"
                },
                folder_dy: 20,
                event_dy: 25, //height of event
                dy: 30
            });
            //===============
            //Data loading
            //===============
            scheduler.config.lightbox.sections = [
                //{ name: "description", height: 130, map_to: "text", type: "textarea", focus: true },
                //{ name: "custom", height: 23, type: "timeline", options: null, map_to: "section_id" }, //type should be the same as name of the tab
                { name: "time", height: 72, type: "time", map_to: "auto" }
            ]
            scheduler.init('scheduler_here', new Date(), "timeline");
        
            scheduler.parse(<%= employeeSchedule%>, "json");
            
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Pending Services</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Pending Service List</a><span class="divider-last">&nbsp;</span></li>
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
                            Pending Service List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">

                        <div class="form-horizontal filter" id="dvFilter" runat="server">
                            <div class="heading searchschedule">
                                <label class="filter-label">Service Case #</label>
                                <asp:TextBox ID="txtCaseNo" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Client</label>
                                <asp:TextBox ID="txtClient" runat="server" CssClass="input-medium"></asp:TextBox>
                                <label class="filter-label">Date Range</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtStart" runat="server" class="input-small date-picker" autocomplete="off" size="16" type="text" />
                                </div>
                                <label>to</label>
                                <div class="input-append date left" data-date="02/12/2012" data-date-format="mm/dd/yyyy">
                                    <input id="txtEnd" runat="server" class="input-small date-picker" autocomplete="off" size="16" type="text" />
                                </div>
                                <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-success" Text="Search" OnClick="btnSearch_Click" />
                                <input type="button" class="btn" value="Clear" onclick="location.href = 'PendingService_List.aspx'" />
                            </div>
                        </div>
                        <div class="clear" style="margin-top: 15px;"></div>
                        <div class="dvbuttons">
                            <asp:LinkButton ID="lnkSchedule" runat="server" CssClass="btn btn-success hidden-phone" OnClick="lnkSchedule_Click">
                                <i class="icon-ok icon-white"></i>Run Scheduler
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkSchedulePending" runat="server" CssClass="btn btn-success hidden-phone" OnClick="lnkSchedulePending_Click">
                                <i class="icon-ok icon-white"></i>Schedule Pending Service
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkScheduleRequested" runat="server" CssClass="btn btn-success hidden-phone" OnClick="lnkScheduleRequested_Click">
                                <i class="icon-ok icon-white"></i>Schedule Requested Service
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkReschedulled" runat="server" CssClass="btn btn-success hidden-phone" OnClick="lnkReschedulled_Click">
                                <i class="icon-ok icon-white"></i>Rescheduled Service
                            </asp:LinkButton>
                            <a class="btn btn-info add" href="<%=Application["SiteAddress"]%>admin/RequestService_AddEdit.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Service Request
                            </a>
                        </div>
                        <asp:UpdatePanel runat="server">
                            <ContentTemplate>
                                <script type="text/javascript">
                                    function jScriptmsg() {
                                        if (!jQuery().uniform) {
                                            return;
                                        }
                                        if (test = $("#sample_12 input[type=checkbox]:not(.toggle)")) {
                                            test.uniform();
                                        }
                                        jQuery('#sample_12 .checkboxes').change(function () {
                                            //jQuery('#sample_12 .checkboxes').each(function (){
                                            //    $(this).attr("checked", false);
                                            //});
                                            //$(this).attr("checked", true);
                                            //jQuery.uniform.update(jQuery('#sample_12 .checkboxes'));
                                            ToggleCheckBox(this);
                                        });
                                    }
                                    Sys.Application.add_load(jScriptmsg);
                                </script>
                                <asp:ListView ID="lstPendingService" runat="server" OnSorting="lstPendingService_Sorting" OnItemCommand="lstPendingService_ItemCommand">
                                    <LayoutTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr runat="server" id="tr">
                                                    <th style="width: 8px;"></th>
                                                    <th>Service Case #</th>
                                                    <th runat="server" class="sorting" id="th2" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ClientName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ClientName" OnClick="SortByServiceCase_Click">Client Name</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th3" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ScheduledDate" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ScheduledDate" OnClick="SortByServiceCase_Click">Requested On</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th5" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="RequestFor" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="RequestFor" OnClick="SortByServiceCase_Click">Request For</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th1" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="ScheduleAttempt" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="ScheduleAttempt" OnClick="SortByServiceCase_Click">Schedule Attempt</asp:LinkButton></th>
                                                    <th runat="server" class="sorting" id="th6" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="AreaName" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="AreaName" OnClick="SortByServiceCase_Click">Preferred Workarea</asp:LinkButton></th>
                                                    <th>Expected Start Date</th>
                                                    <th>Expected End Date</th>
                                                    <th runat="server" class="sorting" id="th7" style="padding: 0;">
                                                        <asp:LinkButton runat="server" ID="Status" CommandName="Sort" Style="display: block; padding: 8px;"
                                                            CommandArgument="Status" OnClick="SortByServiceCase_Click">Status</asp:LinkButton></th>
                                                    <th style="width: 20%;">Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr runat="server" id="itemPlaceholder" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr class="odd gradeX <%#Eval("Status").ToString().ToLower()=="rescheduled" || Convert.ToInt32(Eval("AttemptCount").ToString())> 0  ? "waiting-approval": Eval("Status").ToString().ToLower() %>">
                                            <td>
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value='<%#Eval("SId") %>' />
                                                <asp:HiddenField ID="hdnServiceId" runat="server" Value='<%#Eval("SId") %>' />
                                            </td>
                                            <td><%#Eval("ServiceCaseNumber")%></td>
                                            <td><%#Eval("ClientName") %></td>
                                            <td><%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
                                            <td><%#Eval("PurposeOfVisit") %></td>
                                            <td><%#Eval("AttemptCount") %></td>
                                            <td><%#Eval("AreaName") %></td>
                                            <td><%#(Eval("ExpectedStartDate").ToString()==""?"":DateTime.Parse(Eval("ExpectedStartDate").ToString()).ToString("MM/dd/yyyy").Replace("01/01/1900", "-")) %></td>
                                            <td><%#(Eval("ExpectedEndDate").ToString()==""?"":DateTime.Parse(Eval("ExpectedEndDate").ToString()).ToString("MM/dd/yyyy").Replace("01/01/1900", "-")) %></td>
                                            <td><%#(string.IsNullOrWhiteSpace(Eval("Status").ToString())?"Requested":Eval("Status").ToString()) %></td>
                                            <td>
                                                <a href="<%=Application["SiteAddress"]%>admin/RequestService_AddEdit.aspx?RequestServiceId=<%#Eval("Id") %>" style='display: <%#Eval("IsRequestedService") %>;'
                                                    class="btn mini purple"><i class="icon-edit"></i>&nbsp;Edit</a>
                                                <a href="<%=Application["SiteAddress"]%>admin/<%#Eval("RedirectPage") %>.aspx?ServiceId=<%#Eval("Id") %>"
                                                    class="btn mini purple"><i class="icon-time"></i>&nbsp;Schedule</a>
                                                <asp:LinkButton ID="lnkDelete" runat="server" OnClientClick="return confirm('Are you sure want to delete selected Services?')" CssClass="btn mini dark-grey" Visible='<%#Eval("RequestedService").ToString()=="true" ? true : false %>' Text="Delete" CommandName="DeleteService" CommandArgument='<%#Eval("SId") %>'></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th style="width: 8px;"></th>
                                                    <th>Service Case #</th>
                                                    <th>Client Name</th>
                                                    <th>Requested On</th>
                                                    <th>Request For</th>
                                                    <th>Schedule Attempt</th>
                                                    <th>Preferred Workarea</th>
                                                    <th>Status</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="odd gradeX">
                                                    <td colspan="8">No Data Found </td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </EmptyDataTemplate>
                                    <EmptyItemTemplate>
                                        <table class="table table-striped table-bordered" id="sample_12">
                                            <thead>
                                                <tr>
                                                    <th style="width: 8px;"></th>
                                                    <th>Service Case #</th>
                                                    <th>Client Name</th>
                                                    <th>Requested On</th>
                                                    <th>Request For</th>
                                                    <th>Schedule Attempt</th>
                                                    <th>Preferred Workarea</th>
                                                    <th>Status</th>
                                                    <th>Action</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr class="odd gradeX">
                                                    <td colspan="8">No Data Found </td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </EmptyItemTemplate>
                                </asp:ListView>

                                <asp:DataPager ID="dataPagerPendingService" runat="server" PagedControlID="lstPendingService"
                                    OnPreRender="dataPagerPendingService_PreRender">
                                    <Fields>
                                        <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                                            ShowNextPageButton="false" />
                                        <asp:NumericPagerField ButtonType="Link" />
                                        <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false"
                                            ShowPreviousPageButton="false" />
                                    </Fields>
                                </asp:DataPager>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-group"></i>&nbsp;Employees Schedule
                        </h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body" style="padding-bottom: 100px;">
                        <div class="form-horizontal filter">
                        </div>
                        <div class="form-horizontal filter">
                            <div class="heading searchschedule">
                                <div style="display: inline-block;">
                                    <label class="filter-label">City</label>
                                    <asp:DropDownList ID="drpCity" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="drpCity_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                                <div style="display: inline-block;">
                                    <label class="filter-label">Employee</label>
                                    <asp:DropDownList ID="drpEmployee" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="drpEmployee_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                                <a id="lnkMap" onclick="javascript:ToggleMap();" class="mapoff">Map</a>
                            </div>
                        </div>

                        <table class="empschedules">
                            <tr class="heading">
                                <td class="heading col2">Calendar</td>
                                <td class="heading col3 tdMap">Current Location</td>
                            </tr>
                            <tr>
                                <td class="col2" id="tdCalendarView">
                                    <div id="scheduler_here" class="dhx_cal_container" style='width: 100%; height: 100%;'>
                                        <div class="dhx_cal_navline">
                                            <div class="dhx_cal_prev_button">&nbsp;</div>
                                            <div class="dhx_cal_next_button">&nbsp;</div>
                                            <div class="dhx_cal_today_button"></div>
                                            <div class="dhx_cal_date"></div>
                                            <div class="dhx_cal_tab" name="timeline_tab" style="right: 280px;"></div>
                                        </div>
                                        <div class="dhx_cal_header">
                                        </div>
                                        <div class="dhx_cal_data">
                                        </div>
                                    </div>
                                </td>
                                <td class="col3 tdMap">
                                    <div id="gmap_marker" class="gmaps"></div>
                                </td>
                            </tr>
                        </table>

                    </div>
                </div>
            </div>
        </div>
    </div>
    <script>
        var prev,prevStatus;
        $(document).ready(function () {
            var PageSize = '<%=ConfigurationManager.AppSettings["PageSize"]%>';
            //$('#sample_12').dataTable({
            //    "sDom": "<'row-fluid'<'span6'><'span6'f>r>t<'row-fluid'>",
            //    "aoColumnDefs": [{
            //        'bSortable': false,
            //        'aTargets': [0]
            //    }],
            //    "oSearch": { "bSmart": false, "bRegex": true },
            //    "iDisplayLength": PageSize
            //});

            //jQuery('#sample_12 .checkboxes').change(function (e) {
            //    ToggleCheckBox(this);
            //});

            //jQuery('#sample_12_wrapper .dataTables_filter input').addClass("input-medium"); // modify table search input
            ToggleMap();
            init();
        })
        function ToggleCheckBox(e)
        {
            jQuery('#sample_12 .checkboxes').each(function (){
                $(this).attr("checked", false);
            });
            try {
                if (prev.id!=e.id) {
                    $(e).attr("checked", true);
                }
                if (prev.id==e.id && prevStatus==false) {
                    prev=undefined;
                }
            } catch (e1) {
    
            }
            if (prev==undefined) {
                $(e).attr("checked", true);
            }
            jQuery.uniform.update(jQuery('#sample_12 .checkboxes'));
            prev=e;
            prevStatus=$(e).is(":checked");
        }

        function ToggleMap()
        {
            $(".hidemapcalview").height($(".dhx_cal_navline").height() + $(".dhx_cal_header").height() + $(".dhx_cal_data").height()+30);
            
            $(".tdMap").toggle();
            if ($(".tdMap").attr("style").indexOf("display: none;") != -1) {
                $("#tdCalendarView").addClass('hidemapcalview');
                $("#lnkMap").removeClass('mapon');
                $("#lnkMap").addClass('mapoff');
                $(".dhx_cal_container").attr("style","");
            }
            else {
                $("#tdCalendarView").removeClass('hidemapcalview');
                $("#lnkMap").removeClass('mapoff');
                $("#lnkMap").addClass('mapon');
                $(".dhx_cal_container").attr("style","overflow-x: scroll;");
                DemoGMaps.init();
                $(".gmaps").height($(".gmaps").width());
            }
            $("#scheduler_here").height($(".dhx_cal_navline").height() + $(".dhx_cal_header").height() + $(".dhx_cal_data").height()+30);
        }
    </script>
</asp:Content>
