using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class SifreUnuttum : System.Web.UI.Page
    {
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-H1JMK6K;Initial Catalog=BitirmeDb;Integrated Security=True");

        protected void Page_Load(object sender, EventArgs e)
        {
            mesaj_Label.Text = "";
            var sifreyenileverisi = PreviousPage;
            if (sifreyenileverisi != null)
            {
                lbl_Email.Visible = false;
                email_TextBox.Visible = false;
                sifredogrula_Button.Visible = false;
                mesaj_Label.Text = "Şifreniz Başarıyla Yenilendi. Giriş sayfasına yönlendiriliyorsunuz.";
                Response.AddHeader("REFRESH", "5;URL=Giris.aspx");
            }
        }
        public bool emailSorgulama(string email)
        {
            baglanti.Open();
            SqlCommand komutSorgula = new SqlCommand("SELECT Count(*) FROM Tbl_Kullanici WHERE email=@p1 and hesap_dogrulandi=1 ", baglanti);
            komutSorgula.Parameters.AddWithValue("@p1", email);
            int kontrol = Convert.ToInt32(komutSorgula.ExecuteScalar());
            baglanti.Close();
            if (kontrol > 0)
                return true;//Bu kullanıcı adı mevcut
            else
                return false;//Bu kullanıcı adı mevcut değil

        }

        private void MailGonder(Guid aktivasyonkodu, string email)
        {
            var verifyUrl = "/SifreYenile.aspx?kod=" + aktivasyonkodu;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);


            MailMessage mail = new MailMessage(); //yeni bir mail nesnesi Oluşturuldu.
            mail.IsBodyHtml = true; //mail içeriğinde html etiketleri kullanılsın mı?
            mail.To.Add(email); //Kime mail gönderilecek.

            //mail kimden geliyor, hangi ifade görünsün?
            mail.From = new MailAddress("tanerbsky@gmail.com", "Aktivasyon", System.Text.Encoding.UTF8);
            mail.Subject = "Hesap Doğrulama "; //mailin konusu

            //mailin içeriği.. Bu alan isteğe göre genişletilip daraltılabilir.
            mail.Body = "Şifre Yenileme Linki = <a href='" + link + "'>" + link + "</a>";
            mail.IsBodyHtml = true;
            SmtpClient smp = new SmtpClient();

            //mailin gönderileceği adres ve şifresi
            smp.Credentials = new NetworkCredential("tanerbsky@gmail.com", "1168gm842*");
            smp.Port = 587;
            smp.Host = "smtp.gmail.com";//gmail üzerinden gönderiliyor.
            smp.EnableSsl = true;
            smp.Send(mail);//mail isimli mail gönderiliyor.
        }

        private void SifreYenilemeKoduGonder(string email)
        {
            try
            {
                Guid aktivasyonkodu = Guid.NewGuid();
                baglanti.Open();
                SqlCommand komutactivasyonguncelle = new SqlCommand("UPDATE Tbl_Kullanici SET aktivasyon_kodu=@p1 WHERE email=@p2 and hesap_dogrulandi=1 and hesap_kilitlendi=0", baglanti);
                komutactivasyonguncelle.Parameters.AddWithValue("@p1", aktivasyonkodu);
                komutactivasyonguncelle.Parameters.AddWithValue("@p2", email);
                komutactivasyonguncelle.ExecuteNonQuery();
                baglanti.Close();

                MailGonder(aktivasyonkodu, email);
                mesaj_Label.Text = "Şifre yenileme linki mail adresinize yollandı.";
            }
            catch (Exception)
            {
                mesaj_Label.Text = "Şifre yenileme işlemi esnasında hata oluştu. Tekrar deneyiniz.";
            }
        }

        protected void sifredogrula_Button_Click(object sender, EventArgs e)
        {
            if(emailSorgulama(email_TextBox.Text.ToString()))
            {
                SifreYenilemeKoduGonder(email_TextBox.Text.ToString());
            }
            else
            {
                mesaj_Label.Text = "Bu email adresi kayıtlı değil.";
            }
            
            
        }
    }
}