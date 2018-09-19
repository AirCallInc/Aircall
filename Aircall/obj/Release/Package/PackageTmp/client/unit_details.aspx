<%@ Page Title="Unit Detail" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="unit_details.aspx.cs" Inherits="Aircall.client.unit_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(function () {
            $("#updUnit").click(function (e) {
                if (!Page_ClientValidate()) {

                    return;
                }
                $.ajax({
                    type: "POST",
                    url: "unit_details.aspx/UpdateUnitName",
                    data: '{unitname: "' + $("#litUnitName").val() + '",uid:<%=Request.QueryString["uid"].ToString()  %> }',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        alert(response.d);
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- content area part -->
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Unit details</h1>
                    <span>
                        <img src="images/unit-details-icon.png" alt="" title=""></span>
                </div>
                <div class="border-block">
                    <div class="main-from">
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Unit Name :</label>
                            </div>
                            <div class="right-side">

                                <div class="max380">
                                    <div class="single-row cf">
                                        <div class="left-side" style="width: 70%;">
                                            <asp:TextBox ID="litUnitName" runat="server" ClientIDMode="Static"></asp:TextBox>
                                            <asp:RequiredFieldValidator ControlToValidate="litUnitName" ID="RequiredFieldValidator1" ClientIDMode="Static" runat="server" ErrorMessage="Required" CssClass="error"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="right-side" style="width: 28%;">
                                            <div class='dis-table-cell edit-icon mobile'><a id="updUnit" href="javascript:;">&nbsp;</a></div>
                                        </div>
                                    </div>
                                </div>
                                <%--<asp:Literal ID="litUnitName" runat="server"></asp:Literal>--%>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Unit Ton :</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrUnitTon" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Package/Plan :</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrUnitPlan" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Status :</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrUnitStatus" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf" style="display: none;">
                            <div class="left-side">
                                <label>Unit Manufacture Date :</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrMfgDate" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Unit Age :</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrAge" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Last Service :</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrLastService" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Upcoming Service :</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrUpCommingService" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Employee:</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrEmp" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Visit Per Year:</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrVisitPerYear" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf" style="display:none">
                            <div class="left-side">
                                <label>Total Service:</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrTotalService" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row cf" style="display:none">
                            <div class="left-side">
                                <label>Remaining Service:</label>
                            </div>
                            <div class="right-side">
                                <asp:Literal ID="ltrRemainingService" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="single-row button-bar cf">
                            <a href="my_units.aspx" class="main-btn dark-grey">Back to list</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
