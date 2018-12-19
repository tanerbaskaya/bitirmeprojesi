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
    public partial class Anasayfa : System.Web.UI.Page
    {
        
        SqlBaglantisi bgl = new SqlBaglantisi();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["kullanici_id"]==null)
            {
                Response.Redirect("Giris.aspx");
            }
            else
            {
                KlasorListele(Convert.ToInt32(Session["kullanici_id"]));
                DosyaListele(Convert.ToInt32(Session["kullanici_id"]));
            }
            
        }
        //Listeleme fonksiyonları
        private void KlasorListele(int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutKlasorListele = new SqlCommand("SELECT klasor_id,klasor_adi,ust_klasor_id,sahip_kullanici_id,paylasim_durumu,paylasim_linki,FORMAT(olusturma_tarihi, 'dd/MM/yyyy', 'en-US' ) AS 'olusturma_tarihi' FROM Tbl_Klasor WHERE sahip_kullanici_id=@p1 and ust_klasor_id is null and silinme_durumu=0", bgl.baglanti);
                komutKlasorListele.Parameters.AddWithValue("@p1", kullaniciId);
                SqlDataReader sqlread = komutKlasorListele.ExecuteReader();
                repeater_Klasor.DataSource = sqlread;
                repeater_Klasor.DataBind();
                sqlread.Close();
                bgl.baglanti.Close();
            }
            catch(Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Klasör Listelerken Hata Oluştu')</script>");
            }
        }
        private void DosyaListele(int kullaniciId)
        {
            bgl.baglanti.Open();
            SqlCommand komut_dosyalistele = new SqlCommand("SELECT * FROM Tbl_Dosya WHERE sahip_kullanici_id=@p1 and ust_klasor_id is null", bgl.baglanti);
            komut_dosyalistele.Parameters.AddWithValue("@p1", kullaniciId);
            SqlDataReader sqlread = komut_dosyalistele.ExecuteReader();
            repeater_Dosya.DataSource = sqlread;
            repeater_Dosya.DataBind();
            sqlread.Close();
            bgl.baglanti.Close();
        }


        //Klasor ekleme ve dosya yükleme fonksiyonları

        private void KlasorOlustur(string klasorAdi, string klasorAciklama,int kullaniciId)
        {
            bgl.baglanti.Open();
            SqlCommand komut_klasorolustur = new SqlCommand("INSERT INTO Tbl_Klasor(klasor_adi,klasor_aciklama,sahip_kullanici_id,paylasim_durumu,olusturma_tarihi,silinme_durumu) VALUES(@p1,@p2,@p3,@p4,@p5,@p6) ", bgl.baglanti);
            komut_klasorolustur.Parameters.AddWithValue("@p1", klasorAdi);
            komut_klasorolustur.Parameters.AddWithValue("@p2", klasorAciklama);
            komut_klasorolustur.Parameters.AddWithValue("@p3", kullaniciId);
            komut_klasorolustur.Parameters.AddWithValue("@p4", 0);
            komut_klasorolustur.Parameters.AddWithValue("@p5", DateTime.Now);
            komut_klasorolustur.Parameters.AddWithValue("@p6", 0);
            komut_klasorolustur.ExecuteNonQuery();
            bgl.baglanti.Close();
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

        private void DosyaYukle(int kullaniciId)
        {
            string dosyayolu;
            dosyayolu = "~/Dosyalar/" + fucDosyaYukle.PostedFile.FileName.ToString();
            fucDosyaYukle.SaveAs(Server.MapPath(dosyayolu));

            bgl.baglanti.Open();
            SqlCommand komutDosyaYukle = new SqlCommand("INSERT INTO Tbl_Dosya(dosya_adi,dosya_yolu,dosya_boyutu,sahip_kullanici_id,paylasim_durumu,olusturma_tarihi,silinme_durumu) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7) ", bgl.baglanti);
            komutDosyaYukle.Parameters.AddWithValue("@p1", fucDosyaYukle.PostedFile.FileName);
            komutDosyaYukle.Parameters.AddWithValue("@p2", dosyayolu);
            komutDosyaYukle.Parameters.AddWithValue("@p3", fucDosyaYukle.PostedFile.ContentLength);
            komutDosyaYukle.Parameters.AddWithValue("@p4", kullaniciId);
            komutDosyaYukle.Parameters.AddWithValue("@p5", 0);
            komutDosyaYukle.Parameters.AddWithValue("@p6", DateTime.Now);
            komutDosyaYukle.Parameters.AddWithValue("@p7", 0);
            komutDosyaYukle.ExecuteNonQuery();
            bgl.baglanti.Close();

            bgl.baglanti.Open();
            SqlCommand komutKullaniciAlanGuncelle = new SqlCommand("UPDATE Tbl_Kullanici SET bos_alan=bos_alan-@p1 WHERE kullanici_id=@p2", bgl.baglanti);
            komutKullaniciAlanGuncelle.Parameters.AddWithValue("@p1", Convert.ToInt32(fucDosyaYukle.PostedFile.ContentLength));
            komutKullaniciAlanGuncelle.Parameters.AddWithValue("@p2", kullaniciId);
            komutKullaniciAlanGuncelle.ExecuteNonQuery();
            bgl.baglanti.Close();
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

        private bool KullaniciBosAlanSorgulama(int kullaniciId,int dosyaBoyutu)
        {
            try
            {
                int bosAlan = -1;
                bgl.baglanti.Open();
                SqlCommand komutBosAlan = new SqlCommand("SELECT bos_alan FROM Tbl_Kullanici WHERE kullanici_id=@p1", bgl.baglanti);
                komutBosAlan.Parameters.AddWithValue("@p1", kullaniciId);
                bosAlan = Convert.ToInt32(komutBosAlan.ExecuteScalar());
                bgl.baglanti.Close();
                if(bosAlan>dosyaBoyutu)
                {
                    return true;
                }
                else
                {
                    Response.Write("<script LANGUAGE='JavaScript' >alert('Alanınızda bu dosya için yeteri kadar boşluk yok. Daha küçük boyutlu bir dosya yükleyiniz.')</script>");
                    return false;
                }
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Alan Sorgulama Sırasında Hata Oluştu')</script>");
                return false;
            }
        }

        //Silme fonksiyonları
        public void KlasorSil(int klasorId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komutAltDosyaSorgula = new SqlCommand("SELECT dosya_id FROM Tbl_Dosya WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltDosyaSorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutAltDosyaSorgula.ExecuteReader();
            while (dr.Read())
            {
                DosyaSil(Convert.ToInt32(dr[0]),1);
            }

            SqlCommand komutKlasorSil = new SqlCommand("DELETE FROM Tbl_Klasor WHERE klasor_id=@p1", bgl.baglanti);
            komutKlasorSil.Parameters.AddWithValue("@p1", klasorId);
            komutKlasorSil.ExecuteNonQuery();

            SqlCommand komutAltKlasırSorgla = new SqlCommand("SELECT klasor_id FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltKlasırSorgla.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr2 = komutAltKlasırSorgla.ExecuteReader();
            while (dr2.Read())
            {
                KlasorSil(Convert.ToInt32(dr2[0]),1);
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

            File.Delete(Server.MapPath(dosyaYolu));
            if (baglantiSayac == 0)
                bgl.baglanti.Close();

        }


        //Paylaşım fonksiyonları
        public int KlasorPaylasimBilgisiGetir(int klasorid)
        {
            int durum;
            bgl.baglanti.Open();
            SqlCommand komut_sorgula = new SqlCommand("SELECT paylasim_durumu FROM Tbl_Klasor WHERE klasor_id=@p1", bgl.baglanti);
            komut_sorgula.Parameters.AddWithValue("@p1", klasorid);
            durum = Convert.ToInt32(komut_sorgula.ExecuteScalar());
            bgl.baglanti.Close();
            if (durum==0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        private void KlasorPaylasimDurumuDegistir(int durum,int klasorId,int baglantiSayac)
        {
            if(baglantiSayac == 0)
                bgl.baglanti.Open();
            SqlCommand komut_gizlilikguncelle = new SqlCommand("UPDATE Tbl_Klasor SET paylasim_durumu=@p1 WHERE klasor_id=@p2", bgl.baglanti);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p1", durum);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p2", klasorId);
            komut_gizlilikguncelle.ExecuteNonQuery();
            komut_gizlilikguncelle.Dispose();

            SqlCommand komutsorgula = new SqlCommand("SELECT klasor_id FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutsorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutsorgula.ExecuteReader();
            while (dr.Read())
            {
                KlasorPaylasimDurumuDegistir(durum,Convert.ToInt32(dr[0]),1);
            }
            dr.Close();
            if (baglantiSayac == 0)
                bgl.baglanti.Close();

        }

        
        public int DosyaPaylasimBilgisiGetir(int dosyaId)
        {
            int durum;
            bgl.baglanti.Open();
            SqlCommand komut_sorgula = new SqlCommand("SELECT paylasim_durumu FROM Tbl_Dosya WHERE dosya_id=@p1", bgl.baglanti);
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
        private void DosyaPaylasimDurumuDegistir(int durum, int dosyaId)
        {
            bgl.baglanti.Open();

            SqlCommand komut_gizlilikguncelle = new SqlCommand("UPDATE Tbl_Dosya SET paylasim_durumu=@p1 WHERE dosya_id=@p2", bgl.baglanti);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p1", durum);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p2", dosyaId);
            komut_gizlilikguncelle.ExecuteNonQuery();
            
            bgl.baglanti.Close();

        }
        

        //Repeater extra fonksiyonlar
        public string PaylasimDurumuGetir(object gizlilikId)
        {
            int gizlilik_durumu = int.Parse(gizlilikId.ToString());
            if(gizlilik_durumu==0)
            {
                return "Paylaşıma Kapalı";
            }
            else
            {
                return "Paylaşıma Açık";
            }
            
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


        //Repeater buton
        protected void repeater_Klasor_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "KlasorSil")
            {
                KlasorSil(Convert.ToInt32(e.CommandArgument),0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorPaylasim")
            {
                KlasorPaylasimDurumuDegistir(KlasorPaylasimBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument),0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorLink")
            {
                Response.Redirect("/Klasorler.aspx?kid=" + e.CommandArgument + "&kad="+ KlasorAdiGetir(Convert.ToInt32(e.CommandArgument)));
            }
        }
        protected void repeater_Dosya_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DosyaSil")
            {
                DosyaSil(Convert.ToInt32(e.CommandArgument),0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "DosyaPaylasim")
            {
                DosyaPaylasimDurumuDegistir(DosyaPaylasimBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument));
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "DosyaIndir")
            {

                string FileName = "qwe.png"; // It's a file name displayed on downloaded file on client side.

                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.ClearContent();
                response.Clear();
                response.AddHeader("Content-Disposition", "attachment; filename=" + FileName + ";");
                response.TransmitFile(Server.MapPath("~/Dosyalar/asd.png"));
                response.Flush();
                response.End();
            }
        }

        //buton
        protected void btn_klasorolustur_Click(object sender, EventArgs e)
        {
            KlasorOlustur(txt_Klasorolustur.Text.ToString(), txt_KlasorAciklama.Text.ToString(),Convert.ToInt32(Session["kullanici_id"]));
        }
        protected void btnDosyaYukle_Click(object sender, EventArgs e)
        {
            if(KullaniciBosAlanSorgulama(Convert.ToInt32(Session["kullanici_id"]), Convert.ToInt32(fucDosyaYukle.PostedFile.ContentLength)))
            {
                DosyaYukle(Convert.ToInt32(Session["kullanici_id"]));
            }
        }

        protected void btn_klasorolustur_Click1(object sender, EventArgs e)
        {
            Response.Redirect("Anasayfa.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Anasayfa.aspx");
        }
    }
}