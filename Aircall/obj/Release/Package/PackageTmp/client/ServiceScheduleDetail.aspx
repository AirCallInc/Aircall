<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="ServiceScheduleDetail.aspx.cs" Inherits="Aircall.client.ServiceScheduleDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }
    </style>
    <script>
        function check24HoursForCancel() {
            $("#dvMessage").removeClass("error");
            $("#dvMessage").html("");
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

            var LateRescheduleHours = parseInt(<%= Aircall.Common.General.GetSitesettingsValue("LateRescheduleHours") %>);
            var HourDiff = parseInt($("#hdnHourDiff").val());
            if (HourDiff <= LateRescheduleHours) {
                $("#spnAlert").text("<%= Aircall.Common.General.GetSitesettingsValue("LateCancelDisplayMessage") %>");
                $("#dialog-confirm").dialog({
                    resizable: false,
                    height: "auto",
                    width: dialogWidth,
                    modal: true,
                    buttons: {
                        "Yes": function () {
                            $(this).dialog("close");
                            $("#btnCancelService").click();
                            return true;
                        },
                        "No": function () {
                            $(this).dialog("close");
                            return false;
                        }
                    }
                });
            } else {
                $("#btnCancelService").click();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="dialog-confirm" title="Aircall System" style="display: none;">
        <p>
            <span id="spnAlert"></span>
        </p>
    </div>
    <asp:HiddenField ID="hdnHourDiff" ClientIDMode="Static" runat="server" />
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Service Detail</h1>
                </div>
                <div class="technician-block">
                    <div class="profile-image">
                        <figure>
                            <asp:Image ID="imgTechPer" CssClass="empPhoto" runat="server" />
                        </figure>
                        <span>Service Person
                            <br>
                            <strong>
                                <asp:Literal ID="ltrEmpName" runat="server"></asp:Literal>
                                <asp:HiddenField ID="hdnEmployeeId" runat="server" />
                            </strong>

                        </span>
                    </div>
                </div>
                <div class="border-block">
                    <div id="dvMessage" runat="server" clientidmode="static"></div>
                    <div class="main-from">
                        <%--<div class="single-row cf">
                            <div class="left-side">
                                <label>Service Person :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrEmployee" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>--%>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Case #:</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceNumber" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Date :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrDate" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Time :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" runat="server" id="dvUnit1">
                            <div class="left-side">
                                <label>Unit will be serviced :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrUnit" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Complaint :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrComplaint" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row button-bar cf">
                            <asp:HiddenField ID="hdnRequestId" runat="server" />
                            <asp:Button ID="btnReschedule" runat="server" CssClass="main-btn" Text="Reschedule" OnClick="btnReschedule_Click" />
                            <a class="main-btn" href="javascript:;" runat="server" id="btnCan" onclick="check24HoursForCancel();">Cancel Service</a>
                            <asp:Button ID="btnCancelService" ClientIDMode="Static" runat="server" style="display:none;" CssClass="main-btn" Text="Cancel Service" OnClick="btnCancelService_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
