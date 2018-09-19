using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class AddUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                IUsersService objUsersService;
                objUsersService = ServiceFactory.UsersService;
                BizObjects.Users objAdminUser = new BizObjects.Users();

                objAdminUser.RoleId = 1;
                objAdminUser.FirstName = "Aircall";
                objAdminUser.LastName = "Admin";
                objAdminUser.UserName = "aircalladmin";
                using (MD5 md5Hash = MD5.Create())
                {
                    objAdminUser.Password = Md5Encrypt.GetMd5Hash(md5Hash, "This@admin08");
                }
                objAdminUser.Email = "testlocalcoding@gmail.com";
                objAdminUser.IsActive = true;
                objAdminUser.AddedDate = DateTime.UtcNow;

                //objUsersService.AddUser(ref objAdminUser);
            }
        }
    }
}