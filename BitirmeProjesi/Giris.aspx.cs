using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class Giris : System.Web.UI.Page
    {
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-H1JMK6K;Initial Catalog=BitirmeDb;Integrated Security=True");

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_mesaj.Text = "";
            if(Session["kullanici_adi"]!=null)
            {
                Response.Redirect("Anasayfa.aspx");
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

        private void KullaniciDogrulama(string email, string parola)
        {
            try
            {
                int sorguSayisi = 0;
                baglanti.Open();
                SqlCommand komutSorgula = new SqlCommand("SELECT kullanici_id,kullanici_adi,email,yetki FROM Tbl_Kullanici WHERE email=@p1 and parola=@p2 and hesap_dogrulandi=1 and hesap_kilitlendi=0", baglanti);
                komutSorgula.Parameters.AddWithValue("@p1", email);
                komutSorgula.Parameters.AddWithValue("@p2", SHA512(parola));
                SqlDataReader dr = komutSorgula.ExecuteReader();
                while (dr.Read())
                {
                    Session.Add("kullanici_id", dr[0].ToString());
                    Session.Add("kullanici_adi", dr[1].ToString());
                    Session.Add("email", dr[2].ToString());
                    Session.Add("yetki", dr[3].ToString());
                    sorguSayisi++;
                }
                if (sorguSayisi == 0)
                {
                    lbl_mesaj.Text = "Email veya Şifre Hatalı";
                }
                else if (sorguSayisi == 1)
                {
                    Response.Redirect("Anasayfa.aspx");
                }
                baglanti.Close();
            }
            catch(Exception)
            {
                lbl_mesaj.Text = "Bir hata oluştu. Lütfen tekrar deneyiniz.";
            }
        }
        protected void giris_Button_Click(object sender, EventArgs e)
        {
            KullaniciDogrulama(txt_Email.Text.ToString(), txt_Sifre.Text.ToString());
            
        }
    }
}