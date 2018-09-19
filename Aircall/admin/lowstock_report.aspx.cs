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
    public partial class lowstock_report : System.Web.UI.Page
    {        
        protected void Page_Load(object sender, EventArgs e)
        {
            IPartsService objPartsService = ServiceFactory.PartsService;
            DataTable dtParts = new DataTable();
            objPartsService.GetLowStockDetails(ref dtParts);

            lstParts.DataSource = dtParts;
            lstParts.DataBind();
        }
    }
}