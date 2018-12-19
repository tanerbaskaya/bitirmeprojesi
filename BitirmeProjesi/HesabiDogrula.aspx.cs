using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class HesabiDogrula : System.Web.UI.Page
    {
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-H1JMK6K;Initial Catalog=BitirmeDb;Integrated Security=True");
        protected void Page_Load(object sender, EventArgs e)
        {
            var kayitolverisi = PreviousPage;
            if (kayitolverisi != null)
            {
                lbl_dogrulamaDurumu.Text = "Hesap doğrulama kodu " + ((TextBox)kayitolverisi.FindControl("txt_Email")).Text + " mail adresinize gönderilmiştir. Lütfen mailiniz kontrol ediniz.";
            }
            else if (Request.QueryString["kod"] != null)
            {
                HesapDogrulama(Request.QueryString["kod"].ToString());
            }
            else
            {
                Response.Redirect("KayitOl.aspx");
            }
            
        }
        private void HesapDogrulama(string aktivasyonKodu)
        {
            try
            {
                baglanti.Open();
                SqlCommand komutactivasyonguncelle = new SqlCommand("UPDATE Tbl_Kullanici SET hesap_dogrulandi=1 WHERE aktivasyon_kodu=@p1 and hesap_dogrulandi=0", baglanti);
                komutactivasyonguncelle.Parameters.AddWithValue("@p1", aktivasyonKodu);
                komutactivasyonguncelle.ExecuteNonQuery();
                baglanti.Close();
                lbl_dogrulamaDurumu.Text = "Hesabınız Başarıyla Doğrulandı. Giriş Sayfasına Yönlendiriliyorsunuz.";
                Response.AddHeader("REFRESH", "5;URL=Giris.aspx");
            }
            catch
            {
                lbl_dogrulamaDurumu.Text = "Hesap doğrulama sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
                Response.AddHeader("REFRESH", "5;URL=Giris.aspx");
            }
        }
    }
}