using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class SifreYenile : System.Web.UI.Page
    {
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-H1JMK6K;Initial Catalog=BitirmeDb;Integrated Security=True");

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_mesaj.Text = "";
            if (Session["kullanici_adi"] != null)
            {
                Response.Redirect("Anasayfa.aspx");
            }
            if(Request.QueryString["kod"] != null)
            {
                if (!AktivasyonKoduSorgula(Request.QueryString["kod"]))
                    Response.Redirect("Giris.aspx");//Geçersiz aktivasyon kodu
            }
            else
            {
                Response.Redirect("Giris.aspx");
            }
        }

        private bool AktivasyonKoduSorgula(string aktivasyonKodu)
        {
            try
            {
                int kontrol = 0;
                baglanti.Open();
                SqlCommand komutSorgula = new SqlCommand("SELECT Count(*) FROM Tbl_Kullanici WHERE aktivasyon_kodu=@p1 and hesap_dogrulandi=1 and hesap_kilitlendi=0", baglanti);
                komutSorgula.Parameters.AddWithValue("@p1", aktivasyonKodu);
                kontrol = Convert.ToInt32(komutSorgula.ExecuteScalar());
                baglanti.Close();
                if (kontrol == 1)
                    return true;//Aktivasyon kodu geçerli
                else
                    return false;//Aktivasyon kodu geçersiz
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static string SHA512(string veri)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(veri);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashliVeriByte = hash.ComputeHash(bytes);
                var hashliVeriByteCiktisi = new System.Text.StringBuilder(128);
                foreach (var b in hashliVeriByte)
                    hashliVeriByteCiktisi.Append(b.ToString("X2"));
                return hashliVeriByteCiktisi.ToString();
            }
        }

        private void ParolaYenileme(string parola, string aktivasyonKodu)
        {
            try
            {
                Guid yeniAktivasyonKodu = Guid.NewGuid();
                baglanti.Open();
                SqlCommand komutSifreYenile = new SqlCommand("UPDATE Tbl_Kullanici SET parola=@p1 , aktivasyon_kodu=@p2 WHERE aktivasyon_kodu=@p3", baglanti);
                komutSifreYenile.Parameters.AddWithValue("@p1", SHA512(parola));
                komutSifreYenile.Parameters.AddWithValue("@p2", yeniAktivasyonKodu);
                komutSifreYenile.Parameters.AddWithValue("@p3", aktivasyonKodu);
                komutSifreYenile.ExecuteNonQuery();
                baglanti.Close();
                Server.Transfer("~/SifreUnuttum.aspx");
            }
            catch(Exception)
            {
                lbl_mesaj.Text = "Yenileme işlemi sırasında bir hata oluştu. Tekrar deneyiniz.";
            }
        }
        protected void sifreyenile_Button_Click(object sender, EventArgs e)
        {
            ParolaYenileme(txt_Sifre.Text.ToString(), Request.QueryString["kod"]);
        }
    }
}