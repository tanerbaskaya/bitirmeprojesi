using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void lnk_but_Cikis_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Response.Redirect("Giris.aspx");
        }
    }
}