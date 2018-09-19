<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="ServiceApprove.aspx.cs" Inherits="Aircall.client.ServiceApprove" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function check24HoursForReschedule() {
            if ($("#is24Hrs").val().toLowerCase() == "true") {
                if (confirm($("#LateRescheduleDisplayMessage").val())) {
                    return true;
                }
            } else {
                return true;
            }
            return false;
        }
        function check24HoursForCancel() {
            if ($("#is24Hrs").val().toLowerCase() == "true") {
                if (confirm($("#LateCancelDisplayMessage").val())) {
                    return true;
                }
            } else {
                return true;
            }
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hdnDate" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="is24Hrs" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="LateRescheduleDisplayMessage" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="LateCancelDisplayMessage" ClientIDMode="Static" runat="server" />
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Service Schedule Detail</h1>
                </div>
                <div class="border-block">
                    <div id="dvMessage" runat="server" clientidmode="static"></div>
                    <div class="main-from">
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Case # :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceCase" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Person :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrEmployee" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hdnEmployeeId" runat="server" />
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Address :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrAddress" runat="server"></asp:Literal>
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
                            <asp:Button ID="btnApprove" runat="server" CssClass="main-btn" Text="Approve" OnClick="btnApprove_Click" />
                            <asp:Button ID="btnReschedule" OnClientClick="return check24HoursForReschedule();" Text="Reschedule" CssClass="main-btn" runat="server" ValidationGroup="vgReschedule" OnClick="btnReschedule_Click" />
                            <asp:Button ID="btnCancel" runat="server" OnClientClick="return check24HoursForCancel();" CssClass="main-btn" Text="Cancel" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
