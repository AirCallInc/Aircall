<%@ Page Title="Schedule" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="schedule.aspx.cs" Inherits="Aircall.client.schedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        @media only screen and (max-width: 768px) {
            .mobile {
                width: 100% !important;
            }
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Your next scheduled visits</h1>
                    <span>
                        <img src="images/calender-icon.png" alt="" title=""></span>
                </div>
                <%--<div class="schedule-calender cf">
                    <div id="datepicker"><b>Date: <%= DateTime.Now.ToString("MM/dd/yyyy") %></b></div>
                </div>--%>
                <div id="dvMessage" runat="server" visible="false"></div>
                <div class="scheduled-bar">
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        var arrayable = new Array();
        $(document).ready(function () {
            GetSelectedScheduleService();
            //GetClientScheduleDates();
        });
        function GetSelectedScheduleService() {
            $(".scheduled-bar").empty();
            $.ajax({
                type: "POST",
                url: "schedule.aspx/GetSelectedScheduleService",
                data: '{dateText: "<%= DateTime.Now.ToString("MM/dd/yyyy") %>" }',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccessSchedule,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }
        function OnSuccessSchedule(response) {
            $(".scheduled-bar").html(response.d);
        }
        function GetClientScheduleDates() {
            $.ajax({
                type: "POST",
                url: "schedule.aspx/GetClientSchedulesDate",
                data: null,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                    alert(response.d);
                }
            });
            function OnSuccess(response) {
                var clientdate = response.d;
                for (var i = 0; i < clientdate.length; i++) {
                    arrayable.push(clientdate[i]);
                }
                $(".schedule-calender #datepicker").datepicker({
                    autoSize: true,
                    showOtherMonths: true,
                    selectOtherMonths: true,
                    minDate: 0,
                    dayNamesMin: ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"],
                    beforeShowDay: function (date) {
                        var datestring = jQuery.datepicker.formatDate('yy/mm/dd', date);
                        var hindex = $.inArray(datestring, arrayable);
                        if (hindex > -1) {
                            return [true, 'selectdate', ''];
                        }
                        return [true, '', ''];
                    },
                    onSelect: function (dateText, inst) {
                        GetSelectedScheduleService(dateText);

                    }

                });
                if ($('#ui-datepicker-div').length) {
                    $('.ui-datepicker-current-day').click();
                }
            }
        }

    </script>
</asp:Content>
