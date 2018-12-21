using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class Indırılenler : System.Web.UI.Page
    {
        SqlBaglantisi bgl = new SqlBaglantisi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["kullanici_id"] == null)
            {
                Response.Redirect("Giris.aspx");
            }
            else if(Request.QueryString["kid"]!=null)
            {
                IndirilenAltKlasorleriListele(Convert.ToInt32(Session["kullanici_id"]), Convert.ToInt32(Request.QueryString["kid"]));
                IndirilenAltDosyaListele(Convert.ToInt32(Session["kullanici_id"]), Convert.ToInt32(Request.QueryString["kid"]));
            }
            else
            {
                IndirilenKlasorleriListele(Convert.ToInt32(Session["kullanici_id"]));
                IndirilenDosyaListele(Convert.ToInt32(Session["kullanici_id"]));
            }
        }

        //Listeleme işlemleri
        private void IndirilenKlasorleriListele(int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutKlasorListele = new SqlCommand("SELECT klasor_indirilme_id,indirilen_klasor_adi,indiren_kullanici_id,sahip_kullanici_id,FORMAT(indirilme_tarihi, 'dd/MM/yyyy', 'en-US' ) AS 'indirilme_tarihi' FROM Tbl_Klasor_Indirilen WHERE indiren_kullanici_id=@p1 and ust_indirilen_klasor_id is null", bgl.baglanti);
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
        private void IndirilenDosyaListele(int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutDosyaListele = new SqlCommand("SELECT dosya_indirilme_id,indirilen_dosya_adi,indiren_kullanici_id,sahip_kullanici_id,FORMAT(indirilme_tarihi, 'dd/MM/yyyy', 'en-US' ) AS 'indirilme_tarihi' FROM Tbl_Dosya_Indirilen WHERE indiren_kullanici_id=@p1 and ust_indirilen_klasor_id is null", bgl.baglanti);
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

        //Alt klasor ve dosya listeleme
        private void IndirilenAltKlasorleriListele(int kullaniciId, int ustKlasorId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutKlasorListele = new SqlCommand("SELECT klasor_indirilme_id,indirilen_klasor_adi,indiren_kullanici_id,sahip_kullanici_id,FORMAT(indirilme_tarihi, 'dd/MM/yyyy', 'en-US' ) AS 'indirilme_tarihi' FROM Tbl_Klasor_Indirilen WHERE indiren_kullanici_id=@p1 and ust_indirilen_klasor_id=@p2", bgl.baglanti);
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
        private void IndirilenAltDosyaListele(int kullaniciId, int ustKlasorId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutDosyaListele = new SqlCommand("SELECT dosya_indirilme_id,indirilen_dosya_adi,indiren_kullanici_id,sahip_kullanici_id,FORMAT(indirilme_tarihi, 'dd/MM/yyyy', 'en-US' ) AS 'indirilme_tarihi' FROM Tbl_Dosya_Indirilen WHERE indiren_kullanici_id=@p1 and ust_indirilen_klasor_id=@p2", bgl.baglanti);
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

        //Silme işlemleri
        public void IndirilenKlasorSil(int klasorId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komutKlasorSil = new SqlCommand("DELETE FROM Tbl_Klasor_Indirilen WHERE klasor_indirilme_id=@p1", bgl.baglanti);
            komutKlasorSil.Parameters.AddWithValue("@p1", klasorId);
            komutKlasorSil.ExecuteNonQuery();

            SqlCommand komutAltKlasorSil = new SqlCommand("SELECT klasor_indirilme_id FROM Tbl_Klasor_Indirilen WHERE ust_indirilen_klasor_id=@p1", bgl.baglanti);
            komutAltKlasorSil.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutAltKlasorSil.ExecuteReader();
            while(dr.Read())
            {
                IndirilenKlasorSil(Convert.ToInt32(dr[0]), 1);
            }

            SqlCommand komutAltDosyaSil = new SqlCommand("SELECT dosya_indirilme_id FROM Tbl_Dosya_Indirilen WHERE ust_indirilen_klasor_id=@p1", bgl.baglanti);
            komutAltDosyaSil.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr2 = komutAltDosyaSil.ExecuteReader();
            while (dr2.Read())
            {
                IndirilenDosyaSil(Convert.ToInt32(dr2[0]), 1);
            }
            if (baglantiSayac == 0)
                bgl.baglanti.Close();
        }
        public void IndirilenDosyaSil(int dosyaId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komutDosyaSil = new SqlCommand("DELETE FROM Tbl_Dosya_Indirilen WHERE dosya_indirilme_id=@p1", bgl.baglanti);
            komutDosyaSil.Parameters.AddWithValue("@p1", dosyaId);
            komutDosyaSil.ExecuteNonQuery();
            if (baglantiSayac == 0)
                bgl.baglanti.Close();
        }

        //Ekstra işlemler
        public string KullaniciAdiGetir(object sahipKullaniciId)
        {
            int kullaniciId = int.Parse(sahipKullaniciId.ToString());
            SqlCommand komutKullaniciAdi = new SqlCommand("SELECT kullanici_adi FROM Tbl_Kullanici WHERE kullanici_id=@p1", bgl.baglanti);
            komutKullaniciAdi.Parameters.AddWithValue("@p1", kullaniciId);
            string kullaniciAdi = komutKullaniciAdi.ExecuteScalar().ToString();
            return kullaniciAdi;

        }
        protected void repeater_Klasor_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "IndirilenKlasorSil")
            {
                IndirilenKlasorSil(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorLink")
            {
                Response.Redirect("/Indirilenler.aspx?kid=" + e.CommandArgument);
            }
        }

        protected void repeater_Dosya_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "IndirilenDosyaSil")
            {
                IndirilenDosyaSil(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
        }
    }
}