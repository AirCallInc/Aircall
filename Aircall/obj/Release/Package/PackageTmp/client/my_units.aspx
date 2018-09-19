<%@ Page Title="My Units" Language="C#" MasterPageFile="~/client/Client.Master" AutoEventWireup="true" CodeBehind="my_units.aspx.cs" Inherits="Aircall.client.my_units" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(window).load(function () {
            $('#imgAddress').click(function () {
                if ($(this).hasClass('open')) {
                    $(this).removeClass('open')
                    $('.AddressTable').slideUp();
                }
                else {
                    $(this).addClass('open')
                    $('.AddressTable').slideDown();
                }
            });
        });
    </script>
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
            right: 0;
        }

            .ui-dialog .ui-dialog-content {
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

                    #sample_1 td:before {
                        content: attr(data-title);
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
    <asp:HiddenField ID="hdnCardMode" ClientIDMode="Static" runat="server" />
    <div id="content-area">
        <div class="common-section">
            <div class="container">
                <div class="title">
                    <h1>My Units</h1>
                    <span style="right: 120px !important;">
                        <a onclick="return CheckFailedUnits();" href="add-ac-unit.aspx">
                            <img title="" alt="" id="imgAdd" src="images/plus-icon@3x.png" style="width: 70%; vertical-align: middle;"></a>
                    </span>
                    <span style="right: 60px !important;">
                        <img title="" alt="" id="imgAddress" src="images/addressicon.png" style="width: 70%; vertical-align: middle; cursor: pointer;">
                    </span>
                    <span>
                        <img title="" alt="" src="images/unit-details-icon.png">
                    </span>
                </div>
                <asp:HiddenField ID="hdnFailedUnit" runat="server" ClientIDMode="Static" />
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="table-outer AddressTable" style="display: none;">
                            <div class="title">
                                <h6>My Address</h6>
                            </div>
                            <table id="sample_1" cellspacing="0" cellpadding="0" border="none" class="common-table request-service-table">
                                <thead>
                                    <tr>
                                        <th>Address</th>
                                        <th>Selected Address</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:ListView ID="lstAddress" runat="server" OnItemCommand="lstAddress_ItemCommand" OnItemDataBound="lstAddress_ItemDataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td data-title="Address"><%#Eval("ClientAddress1") %></td>
                                                <td data-title="Default Address"><%# (AddressExpression.ToString() == Eval("Id").ToString())?"Yes":"No" %></td>
                                                <td data-title="" class="edit-remove-btn">
                                                    <asp:LinkButton ID="lnkEdit" runat="server" CssClass="main-btn" Text="Select" CommandName="SelectAddress" CommandArgument='<%#Eval("Id") %>'></asp:LinkButton>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <EmptyDataTemplate>
                                            <tr>
                                                <td colspan="3" style="padding-left: 0; text-align: center;">You have not added AC Unit's address yet.</td>
                                            </tr>
                                        </EmptyDataTemplate>
                                    </asp:ListView>
                                </tbody>
                            </table>
                        </div>
                        <div class="table-outer">
                            <table id="sample_1" class="common-table my-unit-table">
                                <asp:ListView ID="lstSummary" runat="server" OnItemDataBound="lstSummary_ItemDataBound">
                                    <LayoutTemplate>
                                        <thead>
                                            <tr>
                                                <th>Unit Name</th>
                                                <th>Status</th>
                                                <th>Plan Name</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr runat="server" id="itemPlaceholder" />
                                        </tbody>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr runat="server">
                                            <td data-title="Unit Name">
                                                <div class="unit-bg dis-table">
                                                    <div class="dis-table-cell">
                                                        <img src="images/unit-details-icon-grey.png" alt="" title="">
                                                        <strong>
                                                            <asp:Literal ID="litUnitName" runat="server"></asp:Literal></strong>
                                                    </div>
                                                </div>
                                            </td>
                                            <td data-title="Status">
                                                <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                                            </td>
                                            <td data-title="Plan Name">
                                                <asp:Literal ID="litPlan" runat="server"></asp:Literal>
                                            </td>
                                            <td data-title="" class="view-btn-cell"><a class="main-btn view-btn" href="unit_details.aspx?uid=<%# Eval("Id") %>">View</a></td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <tr>
                                            <td colspan="4" style="padding-left: 0; text-align: center;">No records found.</td>
                                        </tr>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </table>
                            <asp:DataPager ID="it" runat="server" class="btn-group btn-group-sm pagination" PagedControlID="lstSummary" OnPreRender="dataPagerRequest_PreRender">
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
                            </asp:DataPager>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
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
            window.location.href = "add-ac-unit.aspx"
            return false;
            var IsUnitFailed = $("#hdnFailedUnit").val();
            if (IsUnitFailed == "0") {
                return true;
            }
            else {

                $("#dialog-confirm").dialog({
                    resizable: false,
                    height: "auto",
                    width: 400,
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
                url: "my_units.aspx/DeleteFailedPaymentUnits",
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
