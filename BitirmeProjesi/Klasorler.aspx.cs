using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class Klasorler : System.Web.UI.Page
    {
        SqlBaglantisi bgl = new SqlBaglantisi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["kullanici_id"] == null)
            {
                Response.Redirect("Giris.aspx");
            }
            if(Request.QueryString["kid"] == null)
            {
                Response.Redirect("Anasayfa.aspx");
            }
            else
            { 
                if(KlasorSorgula())
                {
                    KlasorListele();
                    DosyaListele();
                }
                else
                {
                    Response.Redirect("Anasayfa.aspx");
                }
                 
            }
        }

        private bool KlasorSorgula()
        {
            int durum;
            bgl.baglanti.Open();
            SqlCommand komut_sorgula = new SqlCommand("SELECT count(*) FROM Tbl_Klasor WHERE klasor_id=@p1", bgl.baglanti);
            komut_sorgula.Parameters.AddWithValue("@p1", Request.QueryString["kid"]);
            durum = Convert.ToInt32(komut_sorgula.ExecuteScalar());
            bgl.baglanti.Close();
            if(durum==1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void KlasorListele()
        {
            bgl.baglanti.Open();
            SqlCommand komut_klasorlistele = new SqlCommand("SELECT * FROM Tbl_Klasor WHERE sahip_kullanici_id=@p1 and ust_klasor_id=@p2", bgl.baglanti);
            komut_klasorlistele.Parameters.AddWithValue("@p1", Session["kullanici_id"]);
            komut_klasorlistele.Parameters.AddWithValue("@p2", Request.QueryString["kid"]);
            SqlDataReader sqlread = komut_klasorlistele.ExecuteReader();
            repeater_Klasorler.DataSource = sqlread;
            repeater_Klasorler.DataBind();
            sqlread.Close();
            bgl.baglanti.Close();
        }

        private void DosyaListele()
        {
            bgl.baglanti.Open();
            SqlCommand komut_dosyalistele = new SqlCommand("SELECT * FROM Tbl_Dosya WHERE sahip_kullanici_id=@p1 and ust_klasor_id=@p2", bgl.baglanti);
            komut_dosyalistele.Parameters.AddWithValue("@p1", Session["kullanici_id"]);
            komut_dosyalistele.Parameters.AddWithValue("@p2", Request.QueryString["kid"]);
            SqlDataReader sqlread = komut_dosyalistele.ExecuteReader();
            repeater_Dosya.DataSource = sqlread;
            repeater_Dosya.DataBind();
            sqlread.Close();
            bgl.baglanti.Close();
        }
        public string PaylasimDurumuGetir(object gizlilikId)
        {
            int gizlilik_durumu = int.Parse(gizlilikId.ToString());
            if (gizlilik_durumu == 0)
            {
                return "Paylaşıma Kapalı";
            }
            else
            {
                return "Paylaşıma Açık";
            }

        }

        private void KlasorOlustur()
        {
            bgl.baglanti.Open();
            SqlCommand komut_klasorolustur = new SqlCommand("INSERT INTO Tbl_Klasor(klasor_adi,ust_klasor_id,sahip_kullanici_id,olusturulma_tarihi,gizlilik_durumu) VALUES(@p1,@p2,@p3,@p4,@p5) ", bgl.baglanti);
            komut_klasorolustur.Parameters.AddWithValue("@p1", txt_klasorolustur.Text.ToString());
            komut_klasorolustur.Parameters.AddWithValue("@p2", Request.QueryString["kid"]);
            komut_klasorolustur.Parameters.AddWithValue("@p3", Session["kullanici_id"]);
            komut_klasorolustur.Parameters.AddWithValue("@p4", DateTime.Now);
            komut_klasorolustur.Parameters.AddWithValue("@p5", 0);
            komut_klasorolustur.ExecuteNonQuery();
            bgl.baglanti.Close();
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

        protected void btn_klasorolustur_Click(object sender, EventArgs e)
        {
            KlasorOlustur();
        }

        public void KlasorSil(int klasorId, int baglantiSayac)
        {
            if(baglantiSayac==0)
                bgl.baglanti.Open();
            SqlCommand komut_klasorsil = new SqlCommand("DELETE FROM Tbl_Klasor WHERE klasor_id=@p1", bgl.baglanti);
            komut_klasorsil.Parameters.AddWithValue("@p1", klasorId);
            komut_klasorsil.ExecuteNonQuery();

            SqlCommand komutsorgula = new SqlCommand("SELECT klasor_id FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutsorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutsorgula.ExecuteReader();
            while (dr.Read())
            {
                KlasorSil(Convert.ToInt32(dr[0]),1);
            }
            if (baglantiSayac == 0)
                bgl.baglanti.Close();

        }
        public int KlasorGizlilikBilgisiGetir(int klasorid)
        {
            int durum;
            bgl.baglanti.Open();
            SqlCommand komut_sorgula = new SqlCommand("SELECT gizlilik_durumu FROM Tbl_Klasor WHERE klasor_id=@p1", bgl.baglanti);
            komut_sorgula.Parameters.AddWithValue("@p1", klasorid);
            durum = Convert.ToInt32(komut_sorgula.ExecuteScalar());
            bgl.baglanti.Close();
            if (durum == 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        private void KlasorGizlilikDurumuDegistir(int durum, int klasorId, int baglantiSayac)
        {
            if(baglantiSayac==0)
                bgl.baglanti.Open();
            SqlCommand komut_gizlilikguncelle = new SqlCommand("UPDATE Tbl_Klasor SET gizlilik_durumu=@p1 WHERE klasor_id=@p2", bgl.baglanti);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p1", durum);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p2", klasorId);
            komut_gizlilikguncelle.ExecuteNonQuery();

            SqlCommand komutsorgula = new SqlCommand("SELECT klasor_id FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutsorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutsorgula.ExecuteReader();
            while (dr.Read())
            {
                KlasorGizlilikDurumuDegistir(durum, Convert.ToInt32(dr[0]),1);
            }
            if (baglantiSayac == 0)
                bgl.baglanti.Close();
        }

        public void DosyaSil(int dosyaId)
        {
            bgl.baglanti.Open();
            SqlCommand komut_dosyasorgula = new SqlCommand("SELECT dosya_yolu FROM Tbl_Dosya WHERE dosya_id=@p1", bgl.baglanti);
            komut_dosyasorgula.Parameters.AddWithValue("@p1", dosyaId);
            string dosyayolu = komut_dosyasorgula.ExecuteScalar().ToString();

            SqlCommand komut_klasorsil = new SqlCommand("DELETE FROM Tbl_Dosya WHERE dosya_id=@p1", bgl.baglanti);
            komut_klasorsil.Parameters.AddWithValue("@p1", dosyaId);
            komut_klasorsil.ExecuteNonQuery();

            File.Delete(Server.MapPath(dosyayolu));
            bgl.baglanti.Close();

        }
        public int DosyaGizlilikBilgisiGetir(int dosyaId)
        {
            int durum;
            bgl.baglanti.Open();
            SqlCommand komut_sorgula = new SqlCommand("SELECT gizlilik_durumu FROM Tbl_Dosya WHERE dosya_id=@p1", bgl.baglanti);
            komut_sorgula.Parameters.AddWithValue("@p1", dosyaId);
            durum = Convert.ToInt32(komut_sorgula.ExecuteScalar());
            bgl.baglanti.Close();
            if (durum == 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


        private void DosyaGizlilikDurumuDegistir(int durum, int dosyaId)
        {
            bgl.baglanti.Open();

            SqlCommand komut_gizlilikguncelle = new SqlCommand("UPDATE Tbl_Dosya SET gizlilik_durumu=@p1 WHERE dosya_id=@p2", bgl.baglanti);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p1", durum);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p2", dosyaId);
            komut_gizlilikguncelle.ExecuteNonQuery();

            bgl.baglanti.Close();

        }

        public string KlasorAdiGetir(int klasorid)
        {
            string durum;
            bgl.baglanti.Open();
            SqlCommand komut_sorgula = new SqlCommand("SELECT klasor_adi FROM Tbl_Klasor WHERE klasor_id=@p1", bgl.baglanti);
            komut_sorgula.Parameters.AddWithValue("@p1", klasorid);
            durum = komut_sorgula.ExecuteScalar().ToString();
            bgl.baglanti.Close();
            return durum;
        }
        protected void repeater_Klasorler_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "KlasorSil")
            {
                KlasorSil(Convert.ToInt32(e.CommandArgument),0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorGizlilik")
            {
                KlasorGizlilikDurumuDegistir(KlasorGizlilikBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument),0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorLink")
            {
                Response.Redirect("/Klasorler.aspx?kid="+ e.CommandArgument + "&kad=" + Request.QueryString["kad"] + "/" + KlasorAdiGetir(Convert.ToInt32(e.CommandArgument)));
            }
        }

        private void DosyaYukle()
        {
            string dosyayolu;
            dosyayolu = "~/Dosyalar/" + fucDosyaYukle.PostedFile.FileName.ToString();
            fucDosyaYukle.SaveAs(Server.MapPath(dosyayolu));
            bgl.baglanti.Open();
            SqlCommand komut_dosyayukle = new SqlCommand("INSERT INTO Tbl_Dosya(dosya_adi,dosya_yolu,dosya_boyutu,ust_klasor_id,sahip_kullanici_id,olusturulma_tarihi,gizlilik_durumu) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7) ", bgl.baglanti);
            komut_dosyayukle.Parameters.AddWithValue("@p1", fucDosyaYukle.PostedFile.FileName);
            komut_dosyayukle.Parameters.AddWithValue("@p2", dosyayolu);
            komut_dosyayukle.Parameters.AddWithValue("@p3", fucDosyaYukle.PostedFile.ContentLength);
            komut_dosyayukle.Parameters.AddWithValue("@p4", Request.QueryString["kid"]);
            komut_dosyayukle.Parameters.AddWithValue("@p5", Session["kullanici_id"]);
            komut_dosyayukle.Parameters.AddWithValue("@p6", DateTime.Now);
            komut_dosyayukle.Parameters.AddWithValue("@p7", 0);
            komut_dosyayukle.ExecuteNonQuery();
            bgl.baglanti.Close();
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

        protected void repeater_Dosya_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DosyaSil")
            {
                DosyaSil(Convert.ToInt32(e.CommandArgument));
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "DosyaGizlilik")
            {
                DosyaGizlilikDurumuDegistir(DosyaGizlilikBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument));
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
        }

        protected void btnDosyaYukle_Click(object sender, EventArgs e)
        {
            DosyaYukle();
        }
    }
}