<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="Aircall.client.dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }

        .ui-dialog {
            z-index: 1000000000;
            top: 0 !important;
            left: 0 !important;
            margin: auto;
            position: absolute;
            max-width: 100%;
            max-height: 100%;            
            flex-direction: column;
            align-items: stretch;
            right:0;
        }

            .ui-dialog .ui-dialog-content {                
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- banner area part -->
<!-- Google Code for Successful Registration Conversion Page -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 851483616;
var google_conversion_language = "en";
var google_conversion_format = "3";
var google_conversion_color = "ffffff";
var google_conversion_label = "K6qjCLW_1XEQ4LeClgM";
var google_remarketing_only = false;
/* ]]> */
</script>
<script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="//www.googleadservices.com/pagead/conversion/851483616/?label=K6qjCLW_1XEQ4LeClgM&amp;guid=ON&amp;script=0"/>
</div>
</noscript>
    <div class="banner-dashboard">
        <div class="container">
            <div class="login-name">
                Hello
                <asp:Literal ID="ltrClientName" runat="server"></asp:Literal>,
            </div>
            <div class="unit-section">
                <header class="unit-head cf">
                    <h4>Your units</h4>
                    <a class="main-btn dark" onclick="return CheckFailedUnits();" href="add-ac-unit.aspx">Add unit</a>
                    <a class="main-btn " href="my_units.aspx" style="margin-right: 10px;">See All</a>
                </header>
                <asp:HiddenField ID="hdnFailedUnit" runat="server" ClientIDMode="Static" />
                <div class="all-unit-blocks cf">
                    <asp:ListView ID="lstUnits" runat="server">
                        <ItemTemplate>
                            <div class="single-unit-block">
                                <a href="unit_details.aspx?uid=<%#Eval("Id") %>" class="single-unit-inner">
                                    <figure>
                                        <img src="images/units-img.png" alt="" title="">
                                    </figure>
                                    <span class="icon-status <%#Eval("Status").ToString()=="1"?"success":Eval("Status").ToString()=="2"?"confuse":"broken" %>"></span>
                                    <div class="name"><%#Eval("UnitName") %></div>
                                    <div class="status">
                                        <%#Eval("Status").ToString()=="1"?"Serviced":Eval("Status").ToString()=="2"?"Service Soon":"Need Repair" %>
                                    </div>
                                </a>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="single-unit-block">
                                <div class="status">
                                    Click on Add Unit button to add new Unit.
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </div>

    <!-- content area part -->
    <div id="content-area">
        <div class="schedule-section">
            <div class="container">
                <div id="dvMessage" runat="server" visible="false"></div>
                <div class="next-scheduled">
                    <div class="title">
                        Appointments needing your approval
                        <span>
                            <img src="images/calender-icon.png" alt="" title="">
                        </span>
                    </div>
                    <asp:ListView ID="lstNextScheduleService" runat="server" OnItemDataBound="lstNextScheduleService_ItemDataBound" OnItemCommand="lstNextScheduleService_ItemCommand">
                        <ItemTemplate>
                            <div class="dis-table schedule-row">
                                <div class="dis-table-cell date-block">
                                    <span class="date"><%#Eval("Day") %></span><br>
                                    <span class="month"><%#Eval("Month") %></span><br>
                                    <span class="year"><%#Eval("Year") %></span>
                                </div>
                                <div class="dis-table-cell time"><%#Eval("ScheduleStartTime") %> - <%#DateTime.Parse(Eval("ScheduleEndTime").ToString()).AddHours(1).ToString("hh:mm tt") %></div>
                                <div class="dis-table-cell message">
                                    <%#Eval("Message") %>
                                </div>
                                <div class="dis-table-cell selection-btns">
                                    <asp:HiddenField ID="hfServiceId" Value='<%#Eval("ServiceId") %>' runat="server" />
                                    <asp:LinkButton ID="lnkAccept" runat="server" CssClass="main-btn" Text="View" CommandName="View" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="dis-table schedule-row">
                                <div class="dis-table-cell message">
                                    Currently, there are no appointments in need of your approval.
                                </div>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
                <div class="history-scheduled">
                    <asp:ListView ID="lstSchedule" runat="server" OnItemCommand="lstSchedule_ItemCommand">
                        <ItemTemplate>
                            <div class="dis-table schedule-row-history">
                                <div class="dis-table-cell blue-notification">
                                    <img src="images/blue-notification.png" alt="" title="">
                                </div>
                                <div class="dis-table-cell message">
                                    <asp:HiddenField ID="hfServiceId" Value='<%#Eval("ServiceId") %>' runat="server" />
                                    <asp:LinkButton ID="lnkMsg" CommandName="View" CommandArgument='<%#Eval("Id") %>' runat="server"><%#Eval("Message") %></asp:LinkButton>
                                </div>
                                <div class="dis-table-cell person-img">
                                    <asp:Image ID="imgPersonSRC" runat="server" ImageUrl='<%#Eval("EmpImgURL") %>' CssClass="empPhoto" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
                <div class="history-scheduled">
                    <asp:ListView ID="lstCCExp" runat="server" OnItemCommand="lstCCExp_ItemCommand">
                        <ItemTemplate>
                            <div class="dis-table schedule-row-history">
                                <div class="dis-table-cell blue-notification">
                                    <img src="images/blue-notification.png" alt="" title="">
                                </div>
                                <div class="dis-table-cell message">
                                    <asp:HiddenField ID="hfServiceId" Value='<%#Eval("ServiceId") %>' runat="server" />
                                    <asp:LinkButton ID="lnkMsg" CommandName="View" CommandArgument='<%#Eval("Id") %>' runat="server"><%#Eval("Message") %></asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
                <div class="history-scheduled">
                    <asp:ListView ID="lstNoShow" runat="server" OnItemCommand="lstNoShow_ItemCommand">
                        <ItemTemplate>
                            <div class="dis-table schedule-row-history">
                                <div class="dis-table-cell blue-notification">
                                    <img src="images/blue-notification.png" alt="" title="">
                                </div>
                                <div class="dis-table-cell message">
                                    <asp:HiddenField ID="hfServiceId" Value='<%#Eval("ServiceId") %>' runat="server" />
                                    <asp:LinkButton ID="lnkMsg" CommandName="View" CommandArgument='<%#Eval("Id") %>' runat="server"><%#Eval("Message") %></asp:LinkButton>
                                </div>
                                <div class="dis-table-cell person-img">
                                    <asp:Image ID="imgPersonSRC" runat="server" ImageUrl='<%#Eval("EmpImgURL") %>' CssClass="empPhoto" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
                <div class="history-scheduled">
                    <asp:ListView ID="lstPlanExp" runat="server" OnItemCommand="lstPlanExp_ItemCommand">
                        <ItemTemplate>
                            <div class="dis-table schedule-row-history">
                                <div class="dis-table-cell blue-notification">
                                    <img src="images/blue-notification.png" alt="" title="">
                                </div>
                                <div class="dis-table-cell message">
                                    <asp:HiddenField ID="hfServiceId" Value='<%#Eval("ServiceId") %>' runat="server" />
                                    <asp:LinkButton ID="lnkMsg" CommandName="View" CommandArgument='<%#Eval("Id") %>' runat="server"><%#Eval("Message") %></asp:LinkButton>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </div>
    <div id="dialog-confirm" title="Aircall System" style="display: none;">
        <p>
            Previously added unit failed to complete. Did you want to load them first? If you answer NO they will be deleted.
        </p>
    </div>
    <script type="text/javascript">
        function CheckFailedUnits() {

            var IsUnitFailed = $("#hdnFailedUnit").val();
            if (IsUnitFailed == "0") {
                return true;
            }
            else {
                var screenWidth, screenHeight, dialogWidth, dialogHeight, isDesktop;

                screenWidth = window.screen.width;
                screenHeight = window.screen.height;

                if (screenWidth < 500) {
                    dialogWidth = screenWidth * .95;
                    dialogHeight = screenHeight * .95;
                } else {
                    dialogWidth = 500;
                    dialogHeight = 500;
                    isDesktop = true;
                }

                $("#dialog-confirm").dialog({
                    resizable: false,
                    height: "auto",
                    width: dialogWidth,
                    modal: true,
                    buttons: {
                        "Yes": function () {
                            $(this).dialog("close");
                            window.location.href = "summary.aspx";
                            return false;
                        },
                        "No": function () {
                            RemoveUnits();
                        }
                    }
                });
                return false;
                //var result = confirm("Previously added unit failed to complete. Did you want to load them first?");
                //if (result) {
                //    window.location.href = "summary.aspx";
                //    return false;
                //}
                //else {
                //    RemoveUnits();
                //}

            }
        }

        function RemoveUnits() {
            $.ajax({
                type: "POST",
                url: "dashboard.aspx/DeleteFailedPaymentUnits",
                data: null,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnSuccess,
                failure: function (response) {
                    alert(response.d);
                }
            });
        }
        function OnSuccess(response) {
            var res = response.d;
            if (res) {
                window.location.href = "add-ac-unit.aspx"
            }
        }
    </script>
</asp:Content>
