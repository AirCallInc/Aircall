<%@ Page Title="Service Detail" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="ServiceDetails.aspx.cs" Inherits="Aircall.client.ServiceDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div id="dvMessage" runat="server" visible="false"></div>
            <div class="container">
                <div class="title">
                    <h1>Service detail</h1>
                    <span>
                        <img src="images/calender-icon.png" alt="" title=""></span>
                </div>
                <div class="technician-block">
                    <div class="profile-image">
                        <figure>
                            <asp:Image ID="imgTechPer" CssClass="empPhoto" runat="server" />
                        </figure>
                        <span>Technician
                            <br>
                            <strong>
                                <asp:Literal ID="ltrEmpName" runat="server"></asp:Literal>
                                <asp:HiddenField ID="hdnEmployeeId" runat="server" />
                            </strong>

                        </span>
                    </div>
                </div>
                <div class="border-block">
                    <div class="main-from">

                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service No : </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceNo" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="UnitNameBlock" runat="server">
                            <div class="left-side">
                                <label>Unit Name :</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrUnitName" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Address: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrAddress" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Date: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceDate" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Service Type: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrPurposeOfVisit" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="dvAssignedTotalTime" runat="server">
                            <div class="left-side">
                                <label>Assigned Total Time: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrAssignedTotalTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="dvAssignedStartTime" runat="server">
                            <div class="left-side">
                                <label>Assigned Start Time: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrAssignedStartTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="dvAssignedEndTime" runat="server">
                            <div class="left-side">
                                <label>Assigned End Time: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrAssignedEndTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="ServiceStartTimeBlock" runat="server">
                            <div class="left-side">
                                <label>Time Started Work: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceStartTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="ServiceCompletedTimeBlock" runat="server">
                            <div class="left-side">
                                <label>Time Completed Work: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrServiceCompletedTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="dvExtraTime" runat="server">
                            <div class="left-side">
                                <label>Extra Time: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrExtraTime" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Work Performed: </label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrWorkPerformed" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf">
                            <div class="left-side">
                                <label>Recommendation:</label>
                            </div>
                            <div class="right-side">
                                <p>
                                    <asp:Literal ID="ltrRecommendation" runat="server"></asp:Literal>
                                </p>
                            </div>
                        </div>
                        <div class="single-row cf" id="dvRateBox" runat="server">
                            <div class="left-side">
                                <label>Ratings : </label>
                            </div>
                            <div class="right-side">
                                <div class="starbox starbox2" title="" runat="server" id="dvRating"></div>
                                <asp:HiddenField ID="hdnRate" ClientIDMode="Static" runat="server" />
                                <label class="err" id="lblRequired" runat="server" visible="false">Required</label>
                            </div>
                        </div>
                        <div class="single-row cf" id="dvReveiwBox" runat="server">
                            <div class="left-side">
                                <label>Reviews : </label>
                            </div>
                            <div class="right-side">
                                <asp:TextBox ID="txtReview" runat="server" TextMode="MultiLine"></asp:TextBox>
                            </div>
                        </div>
                        <div class="single-row button-bar no-border cf" id="dvNoteBox" runat="server">
                            <p class="notes"><strong>Note:</strong> Once you provide rating and review, you will not be able to change it.</p>
                            <asp:Button ID="btnSubmit" CssClass="main-btn" runat="server" ValidationGroup="vgSignup" Text="Submit review" OnClick="btnSubmit_Click" />
                            <input type="button" class="main-btn dark-grey" value="Cancel" onclick="location.href = 'past_services.aspx'" />
                        </div>

                    </div>
                </div>
            </div>
    </div>
    </div>
</asp:Content>
