<%@ Page Title="Past Service" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="past_services.aspx.cs" Inherits="Aircall.client.past_services" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }

        @media only screen and (max-width: 800px) {
            #sample_1 {
                width: 100%;
            }
                /* Force table to not be like tables anymore */
                #sample_1 table,
                #sample_1 thead,
                #sample_1 tbody,
                #sample_1 th,
                #sample_1 td,
                #sample_1 tr {
                    display: block;
                }

                    /* Hide table headers (but not display: none;, for accessibility) */
                    #sample_1 thead tr {
                        position: absolute;
                        top: -9999px;
                        left: -9999px;
                    }

                #sample_1 tr {
                    border: 1px solid #ccc;
                }

                #sample_1 td {
                    /* Behave  like a "row" */
                    border: none;
                    border-bottom: 1px solid #eee;
                    position: relative;
                    padding-left: 50%;
                    white-space: normal;
                    text-align: left;
                    width: inherit !important;
                }

                    #sample_1 td:before {
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

                    /*
	Label the data
	*/
                    #sample_1 td:before {
                        content: attr(data-title);
                    }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Service History</h1>
                    <span>
                        <img title="" alt="" src="images/calender-icon.png"></span>
                </div>
                <div class="table-outer">
                    <table id="sample_1" cellspacing="0" cellpadding="0" border="none" class="common-table history-service-table">
                        <asp:ListView ID="lstSummary" runat="server" OnItemDataBound="lstSummary_ItemDataBound">
                            <LayoutTemplate>
                                <thead>
                                    <tr>
                                        <th>Service No</th>
                                        <th>Service Date</th>
                                        <th>Technician</th>
                                        <th></th>
                                        <th>Status</th>
                                        <th>Ratings</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr runat="server" id="itemPlaceholder" />
                                </tbody>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr runat="server">
                                    <td data-title="Service No">
                                        <asp:Literal ID="ltrServiceNo" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Service Date">
                                        <asp:Literal ID="ltrServiceDate" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Technician" class="technician">
                                        <asp:Literal ID="ltrEmpName" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="" class="technician">
                                        <asp:Image ID="imgTechPer" runat="server" CssClass="empPhoto" /></td>
                                    <td data-title="Status">
                                        <asp:Literal ID="ltrUnitName" runat="server"></asp:Literal>
                                    </td>
                                    <td data-title="Ratings" class="ratings">
                                        <div class="starbox starbox1" title="" runat="server" id="dvRating"></div>
                                    </td>
                                    <td data-title="" class="view-btn-cell">
                                        <a class="main-btn view-btn" href="ServiceDetails.aspx?sid=<%# Eval("Id") %>">View</a>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <tr>
                                    <td colspan="7" style="padding-left: 0;text-align: center;">No records found.</td>
                                </tr>
                            </EmptyDataTemplate>
                        </asp:ListView>
                        <%-- <thead>
                            <tr>
                                <th>Service No</th>
                                <th>Unit Name</th>
                                <th>Package/Plan</th>
                                <th>Service Date</th>
                                <th>Technician</th>
                                <th>Ratings</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>C1-90038-S1</td>
                                <td>AC1</td>
                                <td>Residential Plan - Package A</td>
                                <td>25-Nov-2015</td>
                                <td class="technician">Scott Nelson
                                    <img src="images/person50.png" alt="" title=""></td>
                                <td class="ratings">
                                    <div class="starbox starbox1"></div>
                                </td>
                                <td class="view-btn-cell"><a class="main-btn view-btn" href="service-detail.html">View</a></td>
                            </tr>
                            <tr>
                                <td>C1-90038-S1</td>
                                <td>AC1</td>
                                <td>Residential Plan - Package A</td>
                                <td>25-Nov-2015</td>
                                <td class="technician">Scott Nelson
                                    <img src="images/person50.png" alt="" title=""></td>
                                <td class="ratings">
                                    <div class="starbox starbox2"></div>
                                </td>
                                <td class="view-btn-cell"><a class="main-btn view-btn" href="service-detail.html">View</a></td>
                            </tr>
                            <tr>
                                <td>C1-90038-S1</td>
                                <td>AC1</td>
                                <td>Residential Plan - Package A</td>
                                <td>25-Nov-2015</td>
                                <td class="technician">Scott Nelson
                                    <img src="images/person50.png" alt="" title=""></td>
                                <td class="ratings">
                                    <div class="starbox starbox3"></div>
                                </td>
                                <td class="view-btn-cell"><a class="main-btn view-btn" href="service-detail.html">View</a></td>
                            </tr>
                        </tbody>--%>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
