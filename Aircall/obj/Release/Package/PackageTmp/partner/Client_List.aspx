<%@ Page Title="" Language="C#" MasterPageFile="~/partner/PartnerMaster.Master" AutoEventWireup="true" CodeBehind="Client_List.aspx.cs" Inherits="Aircall.partner.Client_List" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }

        @media only screen and (max-width: 800px) {
            #sample_1 {
                width: 100% !important;
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
                        width: auto;
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

                #sample_1 .edit-remove-btn {
                    text-align: center !important;
                    padding-left: 0;
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

        .pagination {
            display: inline-block;
            margin-bottom: 0;
            margin-left: 0;
            margin-top: 15px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Client List</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>partner/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Client</a><span class="divider-last">&nbsp;</span></li>
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
                            Client List
                        </h4>
                        <span class="tools">
                            <a href="javascript:;" class="icon-chevron-down"></a>
                        </span>
                    </div>
                    <div class="widget-body">
                        <div class="clear" style="margin-top: 15px;"></div>
                        <table class="table table-striped table-bordered" id="sample_1">
                            <thead>
                                <tr>
                                    <th data-title="Sr. No.">Sr. No.</th>
                                    <th data-title="Client Name">Client Name</th>
                                    <th data-title="Contract Months">Contract Months</th>
                                    <th data-title="Contract Amount">Contract Amount</th>
                                    <th data-title="Date Acquired">Date Acquired</th>
                                    <th data-title="Commission">Total Commission Amount</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstPartnerClient" runat="server">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td data-title="Sr. No."><%# Container.DataItemIndex + 1 %></td>
                                            <td data-title="Client Name"><%#Eval("FirstName")%> <%#Eval("LastName") %></td>
                                            <td data-title="Contract Months"><%#Eval("DurationInMonth")+" "+"Month" %></td>
                                            <td data-title="Contract Amount">$ <%#Eval("Price") %></td>
                                            <td data-title="Date Acquired"><%#DateTime.Parse(Eval("AddedDate").ToString()).ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt") %></td>
                                            <td data-title="Commission">$ <%#Eval("TotalCommissionAmount") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
