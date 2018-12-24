using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;

namespace BitirmeProjesi
{
    public partial class Copkutusu : System.Web.UI.Page
    {
        SqlBaglantisi bgl = new SqlBaglantisi();
        public static List<string> listDizinAdi = new List<string>();
        public static List<int> listDizinId = new List<int>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["kullanici_id"] == null)
            {
                Response.Redirect("Giris.aspx");
            }
            else if (Request.QueryString["sid"] != null)
            {
                SilinenAltKlasorleriListele(Convert.ToInt32(Session["kullanici_id"]), Convert.ToInt32(Request.QueryString["sid"]));
                SilinenAltDosyaListele(Convert.ToInt32(Session["kullanici_id"]), Convert.ToInt32(Request.QueryString["sid"]));
                listDizinAdi.Clear();
                listDizinId.Clear();
                DizinListeleme(Convert.ToInt32(Request.QueryString["sid"]), Convert.ToInt32(Session["kullanici_id"]), 0);
            }
            else
            {
                listDizinAdi.Clear();
                listDizinId.Clear();
                SilinenKlasorleriListele(Convert.ToInt32(Session["kullanici_id"]));
                SilinenDosyaListele(Convert.ToInt32(Session["kullanici_id"]));
            }
        }
        private void SilinenKlasorleriListele(int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutKlasorListele = new SqlCommand("SELECT TBL1.klasor_id,TBL1.klasor_adi FROM Tbl_Klasor AS TBL1 WHERE TBL1.silinme_durumu=1 and (TBL1.ust_klasor_id is null or TBL1.ust_klasor_id IN (SELECT TBL2.klasor_id FROM Tbl_Klasor AS TBL2 WHERE TBL1.ust_klasor_id=TBL2.klasor_id and TBL2.silinme_durumu=0))", bgl.baglanti);
                komutKlasorListele.Parameters.AddWithValue("@p1", kullaniciId);
                SqlDataReader sqlread = komutKlasorListele.ExecuteReader();
                repeater_Klasor.DataSource = sqlread;
                repeater_Klasor.DataBind();
                sqlread.Close();
                bgl.baglanti.Close();
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Klasör Listelerken Hata Oluştu')</script>");
            }
        }
        private void SilinenDosyaListele(int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutDosyaListele = new SqlCommand("SELECT dosya_id,dosya_adi FROM Tbl_Dosya WHERE sahip_kullanici_id=@p1 and silinme_durumu=1 and ust_klasor_id is null", bgl.baglanti);
                komutDosyaListele.Parameters.AddWithValue("@p1", kullaniciId);
                SqlDataReader sqlread = komutDosyaListele.ExecuteReader();
                repeater_Dosya.DataSource = sqlread;
                repeater_Dosya.DataBind();
                sqlread.Close();
                bgl.baglanti.Close();
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Dosya Listelerken Hata Oluştu')</script>");
            }
        }
        public void DizinListeleme(int ustKlasorId, int kullaniciId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komutDizinSorgula = new SqlCommand("SELECT klasor_adi,klasor_id FROM Tbl_Klasor WHERE klasor_id=@p1 and sahip_kullanici_id=@p2 and silinme_durumu=1", bgl.baglanti);
            komutDizinSorgula.Parameters.AddWithValue("@p1", ustKlasorId);
            komutDizinSorgula.Parameters.AddWithValue("@p2", kullaniciId);
            SqlDataReader dr = komutDizinSorgula.ExecuteReader();
            while (dr.Read())
            {
                listDizinAdi.Add(dr[0].ToString());
                listDizinId.Add(Convert.ToInt32(dr[1]));
            }

            SqlCommand komutUstDizinSorgula = new SqlCommand("SELECT ust_klasor_id FROM Tbl_Klasor WHERE klasor_id=@p1 and sahip_kullanici_id=@p2 and ust_klasor_id is not null and silinme_durumu=1", bgl.baglanti);
            komutUstDizinSorgula.Parameters.AddWithValue("@p1", ustKlasorId);
            komutUstDizinSorgula.Parameters.AddWithValue("@p2", kullaniciId);
            SqlDataReader dr2 = komutUstDizinSorgula.ExecuteReader();
            while (dr2.Read())
            {
                DizinListeleme(Convert.ToInt32(dr2[0]), kullaniciId, 1);
            }
            if (baglantiSayac == 0)
                bgl.baglanti.Close();
        }

        //Alt klasor ve dosya listeleme
        private void SilinenAltKlasorleriListele(int kullaniciId, int ustKlasorId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutKlasorListele = new SqlCommand("SELECT klasor_id,klasor_adi FROM Tbl_Klasor WHERE sahip_kullanici_id=@p1 and ust_klasor_id=@p2 and silinme_durumu=1", bgl.baglanti);
                komutKlasorListele.Parameters.AddWithValue("@p1", kullaniciId);
                komutKlasorListele.Parameters.AddWithValue("@p2", ustKlasorId);
                SqlDataReader sqlread = komutKlasorListele.ExecuteReader();
                repeater_Klasor.DataSource = sqlread;
                repeater_Klasor.DataBind();
                sqlread.Close();
                bgl.baglanti.Close();
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Klasör Listelerken Hata Oluştu')</script>");
            }
        }
        private void SilinenAltDosyaListele(int kullaniciId, int ustKlasorId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutDosyaListele = new SqlCommand("SELECT dosya_id,dosya_adi FROM Tbl_Dosya WHERE sahip_kullanici_id=@p1 and ust_klasor_id=@p2 and silinme_durumu=1", bgl.baglanti);
                komutDosyaListele.Parameters.AddWithValue("@p1", kullaniciId);
                komutDosyaListele.Parameters.AddWithValue("@p2", ustKlasorId);
                SqlDataReader sqlread = komutDosyaListele.ExecuteReader();
                repeater_Dosya.DataSource = sqlread;
                repeater_Dosya.DataBind();
                sqlread.Close();
                bgl.baglanti.Close();
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Dosya Listelerken Hata Oluştu')</script>");
            }
        }

        //Geri yükleme işlemleri
        public void KlasorGeriYukle(int klasorId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komutAltDosyaSorgula = new SqlCommand("SELECT dosya_id FROM Tbl_Dosya WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltDosyaSorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutAltDosyaSorgula.ExecuteReader();
            while (dr.Read())
            {
                DosyaGeriYukle(Convert.ToInt32(dr[0]), 1);
            }

            SqlCommand komutKlasorSil = new SqlCommand("UPDATE Tbl_Klasor SET silinme_durumu=0 WHERE klasor_id=@p1", bgl.baglanti);
            komutKlasorSil.Parameters.AddWithValue("@p1", klasorId);
            komutKlasorSil.ExecuteNonQuery();

            SqlCommand komutAltKlasırSorgla = new SqlCommand("SELECT klasor_id FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltKlasırSorgla.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr2 = komutAltKlasırSorgla.ExecuteReader();
            while (dr2.Read())
            {
                KlasorGeriYukle(Convert.ToInt32(dr2[0]), 1);
            }
            if (baglantiSayac == 0)
                bgl.baglanti.Close();
        }

        public void DosyaGeriYukle(int dosyaId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();

            SqlCommand komutDosyaSil = new SqlCommand("UPDATE Tbl_Dosya SET silinme_durumu=0 WHERE dosya_id=@p1", bgl.baglanti);
            komutDosyaSil.Parameters.AddWithValue("@p1", dosyaId);
            komutDosyaSil.ExecuteNonQuery();

            if (baglantiSayac == 0)
                bgl.baglanti.Close();

        }


        //Silme işlemleri
        public void KlasorSil(int klasorId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komutAltDosyaSorgula = new SqlCommand("SELECT dosya_id FROM Tbl_Dosya WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltDosyaSorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutAltDosyaSorgula.ExecuteReader();
            while (dr.Read())
            {
                DosyaSil(Convert.ToInt32(dr[0]), 1);
            }

            SqlCommand komutKlasorSil = new SqlCommand("DELETE FROM Tbl_Klasor WHERE klasor_id=@p1", bgl.baglanti);
            komutKlasorSil.Parameters.AddWithValue("@p1", klasorId);
            komutKlasorSil.ExecuteNonQuery();

            SqlCommand komutAltKlasırSorgla = new SqlCommand("SELECT klasor_id FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltKlasırSorgla.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr2 = komutAltKlasırSorgla.ExecuteReader();
            while (dr2.Read())
            {
                KlasorSil(Convert.ToInt32(dr2[0]), 1);
            }
            if (baglantiSayac == 0)
                bgl.baglanti.Close();

        }
        public void DosyaSil(int dosyaId, int baglantiSayac)
        {
            string dosyaYolu = null;
            int dosyaBoyutu = 0;
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komutDosyasorgula = new SqlCommand("SELECT dosya_yolu,dosya_boyutu FROM Tbl_Dosya WHERE dosya_id=@p1", bgl.baglanti);
            komutDosyasorgula.Parameters.AddWithValue("@p1", dosyaId);
            SqlDataReader dr = komutDosyasorgula.ExecuteReader();
            while (dr.Read())
            {
                dosyaYolu = dr[0].ToString();
                dosyaBoyutu = Convert.ToInt32(dr[1]);
            }

            SqlCommand komutDosyaSil = new SqlCommand("DELETE FROM Tbl_Dosya WHERE dosya_id=@p1", bgl.baglanti);
            komutDosyaSil.Parameters.AddWithValue("@p1", dosyaId);
            komutDosyaSil.ExecuteNonQuery();


            SqlCommand komutKullaniciBosAlanGuncelle = new SqlCommand("UPDATE Tbl_Kullanici SET bos_alan=bos_alan+@p1 WHERE kullanici_id=@p2", bgl.baglanti);
            komutKullaniciBosAlanGuncelle.Parameters.AddWithValue("@p1", Convert.ToInt32(dosyaBoyutu));
            komutKullaniciBosAlanGuncelle.Parameters.AddWithValue("@p2", Convert.ToInt32(Session["kullanici_id"]));
            komutKullaniciBosAlanGuncelle.ExecuteNonQuery();

            string klasorYolu = "~/Dosyalar/" + Directory.GetParent(dosyaYolu).Name;
            Directory.Delete(Server.MapPath(klasorYolu), true);
            if (baglantiSayac == 0)
                bgl.baglanti.Close();

        }



        protected void repeater_Klasor_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "KlasorSil")
            {
                KlasorSil(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorGeriYukle")
            {
                KlasorGeriYukle(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorLink")
            {
                Response.Redirect("/Copkutusu.aspx?sid=" + e.CommandArgument);
            }
        }

        protected void repeater_Dosya_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DosyaSil")
            {
                DosyaSil(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "DosyaGeriYukle")
            {
                DosyaGeriYukle(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
        }
    }
}