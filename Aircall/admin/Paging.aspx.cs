using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class Paging : System.Web.UI.Page
    {
        IPagingService objPagingService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BingPaging();
            }
        }

        private void BingPaging()
        {
            objPagingService = ServiceFactory.PagingService;
            DataTable dtPaging = new DataTable();
            objPagingService.GetAll(ref dtPaging);
            if (dtPaging.Rows.Count > 0)
            {
                lstUsers.DataSource = dtPaging;
            }
            lstUsers.DataBind();
        }

        protected void DataPager1_PreRender(object sender, EventArgs e)
        {
            BingPaging();
        }
    }
}