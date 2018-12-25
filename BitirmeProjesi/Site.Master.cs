using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public int toplamAlan;
        public int bosAlan;
        SqlBaglantisi bgl = new SqlBaglantisi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["kullanici_id"] != null)
            {
                AlanBilgisiGetir(Convert.ToInt32(Session["kullanici_id"]));
            }
            else
            {
                Response.Redirect("Giris.aspx");
            }
        }

        protected void lnk_but_Cikis_Click(object sender, EventArgs e)
        {
            Session.RemoveAll();
            Response.Redirect("Giris.aspx");
        }

        public void AlanBilgisiGetir(int kullaniciId)
        {
            bgl.baglanti.Open();
            SqlCommand komutDizinSorgula = new SqlCommand("SELECT toplam_alan,bos_alan FROM Tbl_Kullanici WHERE kullanici_id=@p1", bgl.baglanti);
            komutDizinSorgula.Parameters.AddWithValue("@p1", kullaniciId);
            SqlDataReader dr = komutDizinSorgula.ExecuteReader();
            while (dr.Read())
            {
                toplamAlan=Convert.ToInt32(dr[0])/1024/1024;
                bosAlan = Convert.ToInt32(dr[1])/1024/1024;
            }
            bgl.baglanti.Close();
        }
        
        protected void lnk_but_kullanici_arama_Click(object sender, EventArgs e)
        {
            int kontrol = 0;
            bgl.baglanti.Open();
            SqlCommand komutKullaniciSorgula = new SqlCommand("SELECT Count(*) FROM Tbl_Kullanici WHERE kullanici_adi=@p1", bgl.baglanti);
            komutKullaniciSorgula.Parameters.AddWithValue("@p1", txt_kisiarama.Text.ToString());
            kontrol = Convert.ToInt32(komutKullaniciSorgula.ExecuteScalar());

            bgl.baglanti.Close();
            if (kontrol == 1)
            {
                Response.Redirect("Profil.aspx?kadi=" + txt_kisiarama.Text.ToString());
            }
            else
            {
                Response.Redirect("Anasayfa.aspx");
            }
        }
    }
}