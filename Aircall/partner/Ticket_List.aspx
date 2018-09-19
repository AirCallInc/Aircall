<%@ Page Title="" Language="C#" MasterPageFile="~/partner/PartnerMaster.Master" AutoEventWireup="true" CodeBehind="Ticket_List.aspx.cs" Inherits="Aircall.partner.Ticket_List" %>

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
                    padding-left: 40%;
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
                <h3 class="page-title">Ticket List </h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>partner/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>

                    <li><a href="#">Ticket</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div id="dvMessage" runat="server" visible="false"></div>
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4>
                            <i class="icon-magic"></i>&nbsp;Ticket List
                        </h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body">
                        <div class="dvbuttons">
                            <a  style="margin-bottom:10px;" class="btn btn-info add" href="<%=Application["SiteAddress"]%>partner/Ticket_Add.aspx">
                                <i class="icon-plus icon-white"></i>&nbsp; Add Ticket
                            </a>
                        </div>
                         <div style="clear:both;"></div>
                        <table class="table table-striped table-bordered" id="sample_1">
                            <thead>
                                <tr>
                                    <th style="width: 8px;">
                                        <input type="checkbox" class="group-checkable" data-set="#sample_1 .checkboxes" />
                                    </th>
                                    <th>Sr. No.</th>
                                    <th class="hidden-phone">Subject</th>
                                    <th class="hidden-phone">Type</th>
                                    <th class="hidden-phone">Added Date</th>
                                    <th class="hidden-phone">Status</th>
                                    <th>Action</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:ListView ID="lstTicket" runat="server" OnItemDataBound="lstTicket_ItemDataBound">
                                    <ItemTemplate>
                                        <tr class="odd gradeX">
                                            <td data-title="">
                                                <input type="checkbox" class="checkboxes" id="chkcheck" runat="server" value="1" />
                                                <asp:HiddenField ID="hdnUserId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td data-title="Sr. No."><%# Container.DataItemIndex + 1 %>
                                                <asp:HiddenField ID="hdnZipId" runat="server" Value='<%#Eval("Id") %>' />
                                            </td>
                                            <td data-title="Subject"><%#Eval("Subject") %></td>
                                            <td data-title="Type"><%#Eval("TicketType") %></td>
                                            <td data-title="Added Date"><asp:Literal ID="ltrAddedDate" runat="server"></asp:Literal></td>
                                            <td data-title="Status">
                                                <span class="label label-<%#Eval("Status").ToString().ToLower()=="true"?"active":"inactive"%>"><%#Eval("Status").ToString().ToLower()=="true"? "Open" :"Closed"%></span>
                                            </td>
                                            <%-- style="visibility:'<%# Eval("Status")=="0" ? "hidden" :"visible" %>'"--%>
                                            <td class="edit-remove-btn">
                                                <a id="ViewTag" runat="server" class="btn mini purple"><i class="icon-eye-open"></i>&nbsp;View</a>
                                                <a id="EditTag" runat="server" class="btn mini purple"><i class="icon-edit"></i>&nbsp;Update Ticket</a>
                                            </td>
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
