using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.controls
{
    public partial class footer : System.Web.UI.UserControl
    {
        IBlocksService objBlocksService = ServiceFactory.BlocksService;

        protected void Page_Load(object sender, EventArgs e)
        {
            BindFooter();
        }

        private void BindFooter()
        {
            objBlocksService = ServiceFactory.BlocksService;
            DataTable dtBlock = new DataTable();
            objBlocksService.GetBlockContentByBlockName("Footer", ref dtBlock);
            if (dtBlock.Rows.Count > 0)
            {
                ltrFooter.Text = dtBlock.Rows[0]["Description"].ToString();
            }
        }
    }
}