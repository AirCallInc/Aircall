using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceFactory
    {
        public static IUsersService UsersService
        {
            get { return new UsersService(); }
        }

        public static IStateService StateService
        {
            get { return new StateService(); }
        }

        public static IRolesService RolesService
        {
            get { return new RolesService(); }
        }

        public static IContactRequestService ContactRequestService
        {
            get { return new ContactRequestService(); }
        }

        public static IClientService ClientService
        {
            get { return new ClientService(); }
        }

        public static IClientAddressService ClientAddressService
        {
            get { return new ClientAddressService(); }
        }

        public static IClientPaymentMethodService ClientPaymentMethodService
        {
            get { return new ClientPaymentMethodService(); }
        }

        public static IPartnerService PartnerService
        {
            get { return new PartnerService(); }
        }

        public static ICitiesService CitiesService
        {
            get { return new CitiesService(); }
        }

        public static IZipCodeService ZipCodeService
        {
            get { return new ZipCodeService(); }
        }

        public static IAreasService AreasService
        {
            get { return new AreasService(); }
        }

        public static IWorkAreaService WorkAreaService
        {
            get { return new WorkAreaService(); }
        }

        public static IPlanService PlanService
        {
            get { return new PlanService(); }
        }

        public static IClientUnitService ClientUnitService
        {
            get { return new ClientUnitService(); }
        }

        public static IPartsService PartsService
        {
            get { return new PartsService(); }
        }

        public static IClientUnitPartService ClientUnitPartService
        {
            get { return new ClientUnitPartService(); }
        }

        public static IClientUnitPicturesService ClientUnitPicturesService
        {
            get { return new ClientUnitPicturesService(); }
        }

        public static IClientUnitManualsService ClientUnitManualsService
        {
            get { return new ClientUnitManualsService(); }
        }

        public static IUnitExtraInfoService UnitExtraInfoService
        {
            get { return new UnitExtraInfoService(); }
        }

        public static IClientUnitServiceCountService ClientUnitServiceCountService
        {
            get { return new ClientUnitServiceCountService(); }
        }

        public static IEmployeeService EmployeeService
        {
            get { return new EmployeeService(); }
        }

        public static IEmployeeWorkAreaService EmployeeWorkAreaService
        {
            get { return new EmployeeWorkAreaService(); }
        }

        public static IWaitingApprovalService WaitingApprovalService
        {
            get { return new WaitingApprovalService(); }
        }

        public static IServiceUnitService ServiceUnitService
        {
            get { return new ServiceUnitService(); }
        }

        public static IServicesService ServicesService
        {
            get { return new ServicesService(); }
        }

        public static ISiteSettingService SiteSettingService
        {
            get { return new SiteSettingService(); }
        }

        public static IRequestServicesService RequestServicesService
        {
            get { return new RequestServicesService(); }
        }

        public static IRequestServiceUnitsService RequestServiceUnitsService
        {
            get { return new RequestServiceUnitsService(); }
        }

        public static IRescheduleServicesService RescheduleServicesService
        {
            get { return new RescheduleServicesService(); }
        }

        public static IServiceReportService ServiceReportService
        {
            get { return new ServiceReportService(); }
        }

        public static IServiceRatingReviewService ServiceRatingReviewService
        {
            get { return new ServiceRatingReviewService(); }
        }

        public static IDailyPartListMasterService DailyPartListMasterService
        {
            get { return new DailyPartListMasterService(); }
        }

        public static IUnitsService UnitsService
        {
            get { return new UnitsService(); }
        }

        public static IUnitManualsService UnitManualsService
        {
            get { return new UnitManualsService(); }
        }

        public static IEmployeePartRequestMasterService EmployeePartRequestMasterService
        {
            get { return new EmployeePartRequestMasterService(); }
        }

        public static IEmployeePartRequestService EmployeePartRequestService
        {
            get { return new EmployeePartRequestService(); }
        }

        public static IEmailTemplateService EmailTemplateService
        {
            get { return new EmailTemplateService(); }
        }

        public static IStripeErrorLogService StripeErrorLogService
        {
            get { return new StripeErrorLogService(); }
        }

        public static IUserNotificationService UserNotificationService
        {
            get { return new UserNotificationService(); }
        }

        public static INewsService NewsService
        {
            get { return new NewsService(); }
        }

        public static ICMSPagesService CMSPagesService
        {
            get { return new CMSPagesService(); }
        }

        public static IBlocksService BlocksService
        {
            get { return new BlocksService(); }
        }

        public static IBillingHistoryService BillingHistoryService
        {
            get { return new BillingHistoryService(); }
        }

        public static IOrderService OrderService
        {
            get { return new OrderService(); }
        }

        public static IServiceAttemptCountService ServiceAttemptCountService
        {
            get { return new ServiceAttemptCountService(); }
        }

        public static IEmployeePlanTypeService EmployeePlanTypeService
        {
            get { return new EmployeePlanTypeService(); }
        }
		
		public static IServiceNoShowService ServiceNoShowService
        {
            get { return new ServiceNoShowService(); }
        }

        public static IClientUnitInvoiceService ClientUnitInvoiceService
        {
            get { return new ClientUnitInvoiceService(); }
        }

        public static IEmployeeLeaveService EmployeeLeaveService
        {
            get { return new EmployeeLeaveService(); }
        }

        public static IEmployeeScheduleService EmployeeScheduleService
        {
            get { return new EmployeeScheduleService(); }
        }

        public static ISalesVisitRequestService SalesVisitRequestService
        {
            get { return new SalesVisitRequestService(); }
        }

        public static IPartnerTicketRequestService PartnerTicketRequestService
        {
            get { return new PartnerTicketRequestService(); }
        }

        public static IPartnerTicketConversationService PartnerTicketConversationService
        {
            get { return new PartnerTicketConversationService(); }
        }

        public static IServiceReportImagesService ServiceReportImagesService
        {
            get { return new ServiceReportImagesService(); }
        }

        public static IServiceReportUnitsService ServiceReportUnitsService
        {
            get { return new ServiceReportUnitsService(); }
        }

        public static IServicePartListService ServicePartListService
        {
            get { return new ServicePartListService(); }
        }

        public static IServiceDateChangeService ServiceDateChangeService
        {
            get { return new ServiceDateChangeService(); }
        }

        public static IMobileScreensServices MobileScreensServices
        {
            get { return new MobileScreensServices(); }
        }

        public static INotificationMasterService NotificationMasterService
        {
            get { return new NotificationMasterService(); }
        }
		
		public static IOrderItemsService OrderItemsService
        {
            get { return new OrderItemsService(); }
        }

        public static IPagingService PagingService
        {
            get { return new PagingService(); }
        }
		
		public static ICalendarService CalendarService
        {
            get { return new CalendarService(); }
        }

        public static IClientUnitSubscriptionService ClientUnitSubscriptionService
        {
            get { return new ClientUnitSubscriptionService(); }
        }
        public static IAdminService AdminService
        {
            get { return new AdminService(); }
        }
    }
}
