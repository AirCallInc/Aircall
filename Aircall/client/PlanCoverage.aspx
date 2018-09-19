<%@ Page Title="" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="PlanCoverage.aspx.cs" Inherits="Aircall.client.PlanCoverage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Plan Options</h1>
                </div>
                <div style="height:20px;"></div>
                <div class="rows">
                    <a href="<%=Application["SiteAddress"] %>uploads/plan/Aicall_Plan_v2.pdf" target="_blank">Plan Comparison</a>
                </div>
                <div class="plan-coverage-block cf">
                    <asp:ListView runat="server" ID="lstSubscriptionPlans">
                        <ItemTemplate>
                            <div class="plan-coverage-single">
                                <a href="#" id="ContentPlaceHolder1_lstPlans_aPlan_3" class="plan-coverage-inner" style='background-color:<%#Eval("BackgroundColor") %>;'>
                                    <div class="plan-title">
                                        <%#Eval("PlanName") %>
                                    </div>
                                    <div style="color:white">
                                        Basic Fee $<%#Eval("BasicFee") %>/year/visit
                                    </div>
                                    <div style="color:white">
                                        Increasement $<%#Eval("FeeIncrement") %>/year/visit
                                    </div>
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
