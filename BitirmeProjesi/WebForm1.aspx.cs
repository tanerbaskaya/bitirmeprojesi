using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie cerezoku = Request.Cookies["Giris"];
            if (Request.Cookies["Giris"] != null)
            {
               Label1.Text = cerezoku["kullanici_id"];
            }
        }
    }
}