<%@ Page Title="" Language="C#" MasterPageFile="~/admin/AdminMaster.Master" AutoEventWireup="true" CodeBehind="CompletedService_View.aspx.cs" Inherits="Aircall.admin.CompletedService_View" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/jstarbox.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row-fluid">
            <div class="span12">
                <h3 class="page-title">Completed Services Detail</h3>
                <ul class="breadcrumb">
                    <li><a href="<%=Application["SiteAddress"]%>admin/dashboard.aspx"><i class="icon-home"></i></a><span class="divider">&nbsp;</span></li>
                    <li><a href="<%=Application["SiteAddress"]%>admin/CompletedService_List.aspx">Completed Services List</a><span class="divider">&nbsp;</span></li>
                    <li><a href="#">Completed Service Detail</a><span class="divider-last">&nbsp;</span></li>
                </ul>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span12">
                <div class="widget">
                    <div class="widget-title">
                        <h4><i class="icon-wrench"></i>Completed Services Detail Information</h4>
                        <span class="tools"><a href="javascript:;" class="icon-chevron-down"></a></span>
                    </div>
                    <div class="widget-body form">
                        <div class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label">Service Case #</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblServiceCaseNo" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Client Name</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblClientName" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Address</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblAddress" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Mobile</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblMobile" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Home</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblHome" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Office</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblOffice" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Serviced (Completed)</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblUnitServiced" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Unit Serviced (Not Completed)</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblUnitNotServiced" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Package Name</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblPackageName" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Requestd On</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblServiceReq" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Date</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblServiceDate" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Technician</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblTechnician" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Purpose Of Visit</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblPurposeofVisit" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Assigned Total Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrAssignedTotalTime" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Assigned Start Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrAssignedStart" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Assigned End Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrAssignEnd" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Start Time</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblserviceSTime" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service End Time</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblServiceETime" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Extra Time</label>
                                <div class="controls">
                                    <label class="control-label">
                                        <asp:Literal ID="ltrExtra" runat="server"></asp:Literal>
                                    </label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Service Report</label>
                                <div class="controls">
                                    <asp:ListView ID="lstServicereport" runat="server">
                                        <ItemTemplate>
                                            <a href="<%=Application["SiteAddress"]%>admin/ServiceReport_View.aspx?ReportId=<%#Eval("Id") %>" target="_blank"><%#Eval("ServiceReportNumber") %></a><br />
                                        </ItemTemplate>
                                    </asp:ListView>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Work Performed</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblWorkPerformed" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Recommendations to customer</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblRecommen" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Ratings</label>
                                <div class="controls">
                                    <div class="starbox" data-rate="" id="dvRating" runat="server"></div>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Reviews</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblReview" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="control-group">
                                <label class="control-label">Employee Notes</label>
                                <div class="controls">
                                    <asp:Label class="control-label" ID="lblEmployeeNotes" runat="server"></asp:Label>
                                </div>
                            </div>
                            <div class="form-actions">
                                <input type="button" class="btn" value="Back To List" onclick="location.href = 'CompletedService_List.aspx'" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            /* star-rating script */
            $('.starbox').each(function () {
                var starbox = jQuery(this);
                starbox.starbox({
                    average: parseFloat(starbox.attr("data-rate")),
                    stars: 5,
                    buttons: 5, //false will allow any value between 0 and 1 to be set
                    ghosting: true,
                    changeable: false, // true, false, or "once"
                    autoUpdateAverage: true
                });
            });
        });
    </script>
</asp:Content>
