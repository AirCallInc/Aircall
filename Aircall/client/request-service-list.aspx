<%@ Page Title="Request Service List" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="request-service-list.aspx.cs" Inherits="Aircall.client.request_service_list" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .empPhoto {
            height: 71px;
            width: 67px;
            border-radius: 50px;
        }

        div.pad {
            padding-left: 50px !important;
        }

        @media only screen and (max-width: 800px) {
            div.pad {
                padding-left: 0px !important;
            }

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

                #sample_1 .edit-remove-btn {
                    text-align: center !important;
                    padding-left: 0;
                }

            table.common-table td {
                height: auto !important;
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

        table.common-table td {
            height: 50px;
        }

        table.common-table tr:last-of-type {
            border-bottom: none;
        }
    </style>
    <script>

        $(document).ready(function () {
            $(".ui-accordion").click(function () {
                if ($(this).parent().next().hasClass("hide")) {
                    $(this).parent().next().slideDown();
                    $(this).parent().next().removeClass("hide");
                    $(this).parent().next().addClass("show");
                    $(this).next().find(".plus").hide();
                    $(this).next().find(".minus").show();

                } else if ($(this).parent().next().hasClass("show")) {
                    $(this).parent().next().slideUp();
                    $(this).parent().next().removeClass("show");
                    $(this).parent().next().addClass("hide");
                    $(this).next().find(".plus").show();
                    $(this).next().find(".minus").hide();
                }
            });
            $(".plus").click(function () {
                if ($(this).parent().parent().next().hasClass("hide")) {
                    $(this).parent().parent().next().slideDown();
                    $(this).parent().parent().next().removeClass("hide");
                    $(this).parent().parent().next().addClass("show");
                    $(this).parent().find(".plus").hide();
                    $(this).parent().find(".minus").show();

                } else if ($(this).parent().parent().next().hasClass("show")) {
                    $(this).parent().parent().next().slideUp();
                    $(this).parent().parent().next().removeClass("show");
                    $(this).parent().parent().next().addClass("hide");
                    $(this).parent().find(".plus").show();
                    $(this).parent().find(".minus").hide();
                }
            });
            $(".minus").click(function () {
                if ($(this).parent().parent().next().hasClass("hide")) {
                    $(this).parent().parent().next().slideDown();
                    $(this).parent().parent().next().removeClass("hide");
                    $(this).parent().parent().next().addClass("show");
                    $(this).parent().find(".plus").hide();
                    $(this).parent().find(".minus").show();

                } else if ($(this).parent().parent().next().hasClass("show")) {
                    $(this).parent().parent().next().slideUp();
                    $(this).parent().parent().next().removeClass("show");
                    $(this).parent().parent().next().addClass("hide");
                    $(this).parent().find(".plus").show();
                    $(this).parent().find(".minus").hide();
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>Request Service List</h1>
                    <span>
                        <a href="request-services.aspx">
                            <img title="" alt="" id="imgAdd" src="images/plus-icon@3x.png" style="width: 70%; vertical-align: middle;"></a>
                    </span>
                </div>
                <div class="table-outer">
                    <div id="dvMessage" runat="server" visible="false"></div>
                    <asp:ListView runat="server" ID="lstAddresses" OnItemDataBound="lstAddresses_ItemDataBound">
                        <ItemTemplate>

                            <div style="padding: 10px 6px 10px 5px; border-bottom: 1px solid #d9d9d9; font-weight: bold;">
                                <a href="javascript:;" class="ui-accordion">
                                    <asp:Literal ID="ltrAddress" runat="server"></asp:Literal></a>
                                <div style="display: inline; height: 22px; float: right;">
                                    <img src="/client/images/plus-icon@3x.png" class="plus" style="height: 22px; display: block; cursor: pointer;" />
                                    <img src="/client/images/minus-icon@3x.png" class="minus" style="height: 22px; display: none; cursor: pointer;" />
                                </div>
                            </div>
                            <div class="pad hide" style="display: none;">
                                <table id="sample_1" cellspacing="0" cellpadding="0" border="none" class="common-table request-service-table">
                                    <thead>
                                        <tr>
                                            <th>Requested Service Date</th>
                                            <th>Service For</th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:ListView ID="lstRequest" runat="server" OnItemCommand="lstRequest_ItemCommand" OnItemDataBound="lstRequest_ItemDataBound">
                                            <ItemTemplate>
                                                <tr>
                                                    <%--<td data-title="Address"><a href='requestServicesDetail.aspx?rid=<%#Eval("Id") %>'><%#Eval("Address") %></a></td>--%>
                                                    <td data-title="Requested Date"><%#Eval("ServiceRequestedOn","{0:MM/dd/yyyy}") %></td>
                                                    <td data-title="Service For"><%#Eval("PurposeOfVisit") %></td>
                                                    <td data-title="" class="edit-remove-btn" style="text-align: left;">
                                                        <a class="main-btn" href='requestServicesDetail.aspx?rid=<%#Eval("Id") %>'>View</a>
                                                        <asp:LinkButton ID="lnkEdit" runat="server" CssClass="main-btn dark-grey" Text="Edit" CommandName="EditRequest" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="main-btn dark-grey" Text="Remove" CommandName="DeleteRequest" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <EmptyItemTemplate>
                                                <tr>
                                                    <td colspan="3" style="padding-left: 0; text-align: center;">No records found.</td>
                                                </tr>
                                            </EmptyItemTemplate>
                                            <EmptyDataTemplate>
                                                <tr>
                                                    <td colspan="3" style="padding-left: 0; text-align: center;">No records found.</td>
                                                </tr>
                                            </EmptyDataTemplate>
                                        </asp:ListView>
                                    </tbody>
                                </table>
                            </div>
                        </ItemTemplate>
                        <EmptyItemTemplate>
                            <div style="padding: 10px 6px 10px 5px; border-bottom: 1px solid #d9d9d9;">
                                No records found.
                            </div>
                        </EmptyItemTemplate>
                        <EmptyDataTemplate>
                            <div style="padding: 10px 6px 10px 5px; border-bottom: 1px solid #d9d9d9;">
                                No records found.
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                    <%--<asp:DataPager ID="dataPagerRequest" runat="server" PagedControlID="lstRequest"
                        OnPreRender="dataPagerRequest_PreRender" class="btn-group btn-group-sm" Visible="false">
                        <Fields>
                            <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                                ShowNextPageButton="false" />
                            <asp:NumericPagerField ButtonType="Link" />
                            <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false"
                                ShowPreviousPageButton="false" />
                        </Fields>
                    </asp:DataPager>
                    <asp:DataPager ID="it" runat="server" class="btn-group btn-group-sm pagination" PagedControlID="lstRequest" OnPreRender="dataPagerRequest_PreRender">
                        <Fields>
                            <asp:NextPreviousPagerField PreviousPageText="<" FirstPageText="|<" ShowPreviousPageButton="true"
                                ShowFirstPageButton="true" ShowNextPageButton="false" ShowLastPageButton="false"
                                ButtonCssClass="btn btn-default" RenderNonBreakingSpacesBetweenControls="false" RenderDisabledButtonsAsLabels="false" />
                            <asp:NumericPagerField ButtonType="Link" CurrentPageLabelCssClass="btn btn-primary disabled" RenderNonBreakingSpacesBetweenControls="false"
                                NumericButtonCssClass="btn btn-default" ButtonCount="10" NextPageText="..." NextPreviousButtonCssClass="btn btn-default" />
                            <asp:NextPreviousPagerField NextPageText=">" LastPageText=">|" ShowNextPageButton="true"
                                ShowLastPageButton="true" ShowPreviousPageButton="false" ShowFirstPageButton="false"
                                ButtonCssClass="btn btn-default" RenderNonBreakingSpacesBetweenControls="false" RenderDisabledButtonsAsLabels="false" />
                        </Fields>
                    </asp:DataPager>--%>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
