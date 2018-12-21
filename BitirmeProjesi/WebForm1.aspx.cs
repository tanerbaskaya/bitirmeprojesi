using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ionic.Zip;

namespace BitirmeProjesi
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        static List<string> listDosyaYolu = new List<string>();
        static List<string> listKlasorAdi = new List<string>();
        SqlBaglantisi bgl = new SqlBaglantisi();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public int KlasorZipIndırme(int klasorId, string klasorAdi, int baglantiSayac, int elemanSayac)
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
            if(sorguKontrol==0)//klasör içinde dosya yok ise boş klasör oluşturmak için
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
                elemanSayac = KlasorZipIndırme(Convert.ToInt32(dr2[0]), klasorAdi + "/" + dr2[1].ToString(), 1,elemanSayac);
            }
            if (baglantiSayac == 0)//Dosya ilk çağrıldığında bağlantı kapatılır
                bgl.baglanti.Close();
            return elemanSayac;
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            int sayac = KlasorZipIndırme(19, "Deneme", 0,0);
            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=myZipFile.zip");

            using (ZipFile zip = new ZipFile())
            {
                for(int i=0;i<sayac;i++)
                {
                    if (listDosyaYolu[i] == "")
                        zip.AddDirectoryByName(listKlasorAdi[i]);
                    else
                        zip.AddFile(Server.MapPath(listDosyaYolu[i]), listKlasorAdi[i]);
                }
                zip.Save(Response.OutputStream);
            }
        }
    }
}