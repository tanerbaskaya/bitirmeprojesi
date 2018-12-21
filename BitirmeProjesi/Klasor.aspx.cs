﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;

namespace BitirmeProjesi
{
    public partial class Klasorler : System.Web.UI.Page
    {
        static List<string> listDosyaYolu = new List<string>();
        static List<string> listKlasorAdi = new List<string>();
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
                if(KlasorSorgula(Request.QueryString["kid"],Convert.ToInt32(Session["kullanici_id"])))
                {
                    KlasorListele(Request.QueryString["kid"], Convert.ToInt32(Session["kullanici_id"]));
                    DosyaListele(Request.QueryString["kid"], Convert.ToInt32(Session["kullanici_id"]));
                }
                else
                {
                    Response.Redirect("Anasayfa.aspx");
                }
                 
            }
        }

        

        //Listeleme fonksiyonları
        private void KlasorListele(string klasorId, int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komut_klasorlistele = new SqlCommand("SELECT * FROM Tbl_Klasor WHERE ust_klasor_id=@p1 and sahip_kullanici_id=@p2", bgl.baglanti);
                komut_klasorlistele.Parameters.AddWithValue("@p1", klasorId);
                komut_klasorlistele.Parameters.AddWithValue("@p2", kullaniciId);
                SqlDataReader sqlread = komut_klasorlistele.ExecuteReader();
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
        private bool KlasorSorgula(string klasorId, int kullaniciId)
        {
            try
            {
                int durum;
                bgl.baglanti.Open();
                SqlCommand komut_sorgula = new SqlCommand("SELECT count(*) FROM Tbl_Klasor WHERE klasor_id=@p1 and sahip_kullanici_id=@p2", bgl.baglanti);
                komut_sorgula.Parameters.AddWithValue("@p1", klasorId);
                komut_sorgula.Parameters.AddWithValue("@p2", kullaniciId);
                durum = Convert.ToInt32(komut_sorgula.ExecuteScalar());
                bgl.baglanti.Close();
                if (durum == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Klasör Sorgularken Hata Oluştu')</script>");
                return false;
            }
        }

        private void DosyaListele(string klasorId, int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komut_dosyalistele = new SqlCommand("SELECT * FROM Tbl_Dosya WHERE ust_klasor_id=@p1 and sahip_kullanici_id=@p2", bgl.baglanti);
                komut_dosyalistele.Parameters.AddWithValue("@p1", klasorId);
                komut_dosyalistele.Parameters.AddWithValue("@p2", kullaniciId);
                SqlDataReader sqlread = komut_dosyalistele.ExecuteReader();
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


        //Klasor ekleme ve dosya yükleme fonksiyonları
        private void KlasorOlustur(string klasorAdi, string klasorAciklama, int ustKlasorId,int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komut_klasorolustur = new SqlCommand("INSERT INTO Tbl_Klasor(klasor_adi,klasor_aciklama,ust_klasor_id,sahip_kullanici_id,paylasim_durumu,olusturma_tarihi,silinme_durumu) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7) ", bgl.baglanti);
                komut_klasorolustur.Parameters.AddWithValue("@p1", klasorAdi);
                komut_klasorolustur.Parameters.AddWithValue("@p2", klasorAciklama);
                komut_klasorolustur.Parameters.AddWithValue("@p3", ustKlasorId);
                komut_klasorolustur.Parameters.AddWithValue("@p4", kullaniciId);
                komut_klasorolustur.Parameters.AddWithValue("@p5", 0);
                komut_klasorolustur.Parameters.AddWithValue("@p6", DateTime.Now);
                komut_klasorolustur.Parameters.AddWithValue("@p7", 0);
                komut_klasorolustur.ExecuteNonQuery();
                bgl.baglanti.Close();
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            catch(Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Klasör Oluşturulurken Bir Hata Oluştu')</script>");
            }
        }

        private void DosyaYukle(int kullaniciId,int ustKlasorId)
        {
            string dosyayolu;
            Guid dosyaBenzersizKlasorKodu = Guid.NewGuid();
            string dosyaBenzersizKlasorAdi = "~/Dosyalar/" + dosyaBenzersizKlasorKodu.ToString();
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(dosyaBenzersizKlasorAdi));
            dosyayolu = dosyaBenzersizKlasorAdi + "/" + fucDosyaYukle.PostedFile.FileName.ToString();
            fucDosyaYukle.SaveAs(Server.MapPath(dosyayolu));

            bgl.baglanti.Open();
            SqlCommand komutDosyaYukle = new SqlCommand("INSERT INTO Tbl_Dosya(dosya_adi,dosya_yolu,dosya_boyutu,ust_klasor_id,sahip_kullanici_id,paylasim_durumu,olusturma_tarihi,silinme_durumu) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8) ", bgl.baglanti);
            komutDosyaYukle.Parameters.AddWithValue("@p1", fucDosyaYukle.PostedFile.FileName);
            komutDosyaYukle.Parameters.AddWithValue("@p2", dosyayolu);
            komutDosyaYukle.Parameters.AddWithValue("@p3", fucDosyaYukle.PostedFile.ContentLength);
            komutDosyaYukle.Parameters.AddWithValue("@p4", ustKlasorId);
            komutDosyaYukle.Parameters.AddWithValue("@p5", kullaniciId);
            komutDosyaYukle.Parameters.AddWithValue("@p6", 0);
            komutDosyaYukle.Parameters.AddWithValue("@p7", DateTime.Now);
            komutDosyaYukle.Parameters.AddWithValue("@p8", 0);
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

        private bool KullaniciBosAlanSorgulama(int kullaniciId, int dosyaBoyutu)
        {
            try
            {
                int bosAlan = -1;
                bgl.baglanti.Open();
                SqlCommand komutBosAlan = new SqlCommand("SELECT bos_alan FROM Tbl_Kullanici WHERE kullanici_id=@p1", bgl.baglanti);
                komutBosAlan.Parameters.AddWithValue("@p1", kullaniciId);
                bosAlan = Convert.ToInt32(komutBosAlan.ExecuteScalar());
                bgl.baglanti.Close();
                if (bosAlan > dosyaBoyutu)
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

        private bool KlasorAdiSorgulama(string klasorAdi, int ustKlasorId, int kullaniciId)
        {
            int kontrol = 0;
            bgl.baglanti.Open();
            SqlCommand komutKlasorAdSorgula = new SqlCommand("SELECT Count(*) FROM Tbl_Klasor WHERE klasor_adi=@p1 and sahip_kullanici_id=@p2 and ust_klasor_id=@p3", bgl.baglanti);
            komutKlasorAdSorgula.Parameters.AddWithValue("@p1", klasorAdi);
            komutKlasorAdSorgula.Parameters.AddWithValue("@p2", kullaniciId);
            komutKlasorAdSorgula.Parameters.AddWithValue("@p3", ustKlasorId);
            kontrol = Convert.ToInt32(komutKlasorAdSorgula.ExecuteScalar());
            bgl.baglanti.Close();
            if (kontrol == 0)
            {
                return true;//Klasör adı mevcut değil
            }
            else
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('Bu alanda " + klasorAdi + " isminde bir klasör zaten var. Lütfen başka bir klasör adı giriniz')</script>");
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
            if (durum == 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        private void KlasorPaylasimDurumuDegistir(int durum, int klasorId, int baglantiSayac)
        {
            if (baglantiSayac == 0)
                bgl.baglanti.Open();
            Guid paylasimKodu = Guid.NewGuid();
            SqlCommand komut_gizlilikguncelle = new SqlCommand("UPDATE Tbl_Klasor SET paylasim_durumu=@p1, paylasim_kodu=@p2 WHERE klasor_id=@p3", bgl.baglanti);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p1", durum);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p2", paylasimKodu);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p3", klasorId);
            komut_gizlilikguncelle.ExecuteNonQuery();
            komut_gizlilikguncelle.Dispose();

            SqlCommand komutsorgula = new SqlCommand("SELECT klasor_id FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutsorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutsorgula.ExecuteReader();
            while (dr.Read())
            {
                KlasorPaylasimDurumuDegistir(durum, Convert.ToInt32(dr[0]), 1);
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
            Guid paylasimKodu = Guid.NewGuid();
            SqlCommand komut_gizlilikguncelle = new SqlCommand("UPDATE Tbl_Dosya SET paylasim_durumu=@p1,paylasim_kodu=@p2 WHERE dosya_id=@p3", bgl.baglanti);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p1", durum);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p2", paylasimKodu);
            komut_gizlilikguncelle.Parameters.AddWithValue("@p3", dosyaId);
            komut_gizlilikguncelle.ExecuteNonQuery();

            bgl.baglanti.Close();

        }

        //İndirme işlemleri
        private int KlasorZipIndırme(int klasorId, string klasorAdi, int baglantiSayac, int elemanSayac)
        {
            if (baglantiSayac == 0)//Dosya ilk çağrıldığında bağlantı açılır
                bgl.baglanti.Open();

            int sorguKontrol = 0;
            SqlCommand komutAltDosyaSorgula = new SqlCommand("SELECT dosya_yolu FROM Tbl_Dosya WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltDosyaSorgula.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr = komutAltDosyaSorgula.ExecuteReader();
            while (dr.Read())
            {//klasör içinde dosya varsa klasörü oluşturup içine dosya koymak için
                listKlasorAdi.Add(klasorAdi);
                listDosyaYolu.Add(dr[0].ToString());
                sorguKontrol++;
                elemanSayac++;
            }
            if (sorguKontrol == 0)//klasör içinde dosya yok ise boş klasör oluşturmak için
            {
                listKlasorAdi.Add(klasorAdi);
                listDosyaYolu.Add("");
                elemanSayac++;
            }
            SqlCommand komutAltKlasırSorgla = new SqlCommand("SELECT klasor_id,klasor_adi FROM Tbl_Klasor WHERE ust_klasor_id=@p1", bgl.baglanti);
            komutAltKlasırSorgla.Parameters.AddWithValue("@p1", klasorId);
            SqlDataReader dr2 = komutAltKlasırSorgla.ExecuteReader();
            while (dr2.Read())
            {//alt klasörleri eklemek için sorgu ve döngü
                elemanSayac = KlasorZipIndırme(Convert.ToInt32(dr2[0]), klasorAdi + "/" + dr2[1].ToString(), 1, elemanSayac);
            }
            if (baglantiSayac == 0)//Dosya ilk çağrıldığında bağlantı kapatılır
                bgl.baglanti.Close();
            return elemanSayac;
        }
        private void KlasorZip(int klasorId, string klasorAdi)
        {
            int sayac = KlasorZipIndırme(klasorId, klasorAdi, 0, 0);
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=" + klasorAdi + ".zip");

            using (ZipFile zip = new ZipFile())
            {
                for (int i = 0; i < sayac; i++)
                {
                    if (listDosyaYolu[i] == "")
                        zip.AddDirectoryByName(listKlasorAdi[i]);
                    else
                        zip.AddFile(Server.MapPath(listDosyaYolu[i]), listKlasorAdi[i]);
                }
                zip.Save(Response.OutputStream);
            }
            listKlasorAdi.Clear();
            listDosyaYolu.Clear();
            Response.Flush();
        }
        private bool DosyaIndır(int dosyaId)
        {
            try
            {
                string dosyaAdi = null;
                string dosyaYolu = null;
                bgl.baglanti.Open();
                SqlCommand komutDosyaSorgula = new SqlCommand("SELECT dosya_adi,dosya_yolu FROM Tbl_Dosya WHERE dosya_id=@p1", bgl.baglanti);
                komutDosyaSorgula.Parameters.AddWithValue("@p1", dosyaId);
                SqlDataReader dr = komutDosyaSorgula.ExecuteReader();
                while (dr.Read())
                {
                    dosyaAdi = dr[0].ToString();
                    dosyaYolu = dr[1].ToString();
                }
                bgl.baglanti.Close();
                if (dosyaAdi != null && dosyaYolu != null)
                {
                    System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.ClearContent();
                    response.Clear();
                    response.AddHeader("Content-Disposition", "attachment; filename=" + dosyaAdi + ";");
                    response.TransmitFile(Server.MapPath(dosyaYolu));
                    response.Flush();
                }
                return true;
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('İndirme sırasında bir hata oluştu.')</script>");
                return false;
            }
        }
        private void DosyaIndırılmeBilgisiEkle(int dosyaId, int kullaniciId)
        {
            try
            {
                bgl.baglanti.Open();
                SqlCommand komutDosyaIndırılmeEkle = new SqlCommand("INSERT INTO Tbl_Dosya_Indirilen(indiren_kullanici_id,indirilen_dosya_id,indirilme_zamani) VALUES(@p1,@p2,@p3)", bgl.baglanti);
                komutDosyaIndırılmeEkle.Parameters.AddWithValue("@p1", kullaniciId);
                komutDosyaIndırılmeEkle.Parameters.AddWithValue("@p2", dosyaId);
                komutDosyaIndırılmeEkle.Parameters.AddWithValue("@p3", DateTime.Now);
                komutDosyaIndırılmeEkle.ExecuteNonQuery();
                bgl.baglanti.Close();
            }
            catch (Exception)
            {
                Response.Write("<script LANGUAGE='JavaScript' >alert('İndirme bilgisi eklerken bir hata oluştu.')</script>");
            }
        }

        //Repeater extra fonksiyonlar
        public string PaylasimDurumuGetir(object paylasimDurum)
        {
            int paylasim = int.Parse(paylasimDurum.ToString());
            if (paylasim == 0)
            {
                return "Paylaşıma Kapalı";
            }
            else
            {
                return "Paylaşıma Açık";
            }

        }
        public bool PaylasimDurumuKontrol(object paylasimDurum)
        {
            int paylasim = int.Parse(paylasimDurum.ToString());
            if (paylasim == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool KlasorYorumMevcutmu(object aciklama)
        {
            string aciklamaDurumu = aciklama.ToString();
            if (aciklamaDurumu == "")
            {
                return false;
            }
            else
            {
                return true;
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

        //protected void repeater_Klasorler_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    if (e.CommandName == "KlasorSil")
        //    {
        //        KlasorSil(Convert.ToInt32(e.CommandArgument),0);
        //        Page.Response.Redirect(Page.Request.Url.ToString(), true);
        //    }
        //    else if (e.CommandName == "KlasorGizlilik")
        //    {
        //        KlasorGizlilikDurumuDegistir(KlasorGizlilikBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument),0);
        //        Page.Response.Redirect(Page.Request.Url.ToString(), true);
        //    }
        //    else if (e.CommandName == "KlasorLink")
        //    {
        //        Response.Redirect("/Klasorler.aspx?kid="+ e.CommandArgument + "&kad=" + Request.QueryString["kad"] + "/" + KlasorAdiGetir(Convert.ToInt32(e.CommandArgument)));
        //    }
        //}

        

        //protected void repeater_Dosya_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    if (e.CommandName == "DosyaSil")
        //    {
        //        DosyaSil(Convert.ToInt32(e.CommandArgument));
        //        Page.Response.Redirect(Page.Request.Url.ToString(), true);
        //    }
        //    else if (e.CommandName == "DosyaGizlilik")
        //    {
        //        DosyaGizlilikDurumuDegistir(DosyaGizlilikBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument));
        //        Page.Response.Redirect(Page.Request.Url.ToString(), true);
        //    }
        //}

       

        protected void repeater_Klasor_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "KlasorSil")
            {
                KlasorSil(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorPaylasim")
            {
                KlasorPaylasimDurumuDegistir(KlasorPaylasimBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "KlasorLink")
            {
                Response.Redirect("/Klasor.aspx?kid=" + e.CommandArgument + "&kad=" + KlasorAdiGetir(Convert.ToInt32(e.CommandArgument)));
            }
            else if (e.CommandName == "KlasorAciklama")
            {
                lbl_KlasorAciklama.Text = e.CommandArgument.ToString();

            }
            else if (e.CommandName == "KlasorIndir")
            {
                KlasorZip(Convert.ToInt32(e.CommandArgument), KlasorAdiGetir(Convert.ToInt32(e.CommandArgument)));
            }
        }

        protected void repeater_Dosya_ItemCommand1(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DosyaSil")
            {
                DosyaSil(Convert.ToInt32(e.CommandArgument), 0);
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "DosyaPaylasim")
            {
                DosyaPaylasimDurumuDegistir(DosyaPaylasimBilgisiGetir(Convert.ToInt32(e.CommandArgument)), Convert.ToInt32(e.CommandArgument));
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            else if (e.CommandName == "DosyaIndir")
            {
                if (DosyaIndır(Convert.ToInt32(e.CommandArgument)))
                {
                    DosyaIndırılmeBilgisiEkle(Convert.ToInt32(e.CommandArgument), Convert.ToInt32(Session["kullanici_id"]));
                }
            }
        }

        protected void btn_Klasorolustur_Click(object sender, EventArgs e)
        {
            if (KlasorAdiSorgulama(txt_Klasorolustur.Text.ToString(), Convert.ToInt32(Request.QueryString["kid"]), Convert.ToInt32(Session["kullanici_id"])))
            {
                KlasorOlustur(txt_Klasorolustur.Text.ToString(), txt_KlasorAciklama.Text.ToString(),Convert.ToInt32(Request.QueryString["kid"]), Convert.ToInt32(Session["kullanici_id"]));
            }
        }

        protected void btn_DosyaYukle_Click(object sender, EventArgs e)
        {
            if (KullaniciBosAlanSorgulama(Convert.ToInt32(Session["kullanici_id"]), Convert.ToInt32(fucDosyaYukle.PostedFile.ContentLength)))
            {
                DosyaYukle(Convert.ToInt32(Session["kullanici_id"]), Convert.ToInt32(Request.QueryString["kid"]));
            }
        }
    }
}