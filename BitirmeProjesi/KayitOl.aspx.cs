using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BitirmeProjesi
{
    public partial class KayitOl : System.Web.UI.Page
    {
        SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-H1JMK6K;Initial Catalog=BitirmeDb;Integrated Security=True");

        protected void Page_Load(object sender, EventArgs e)
        {
            lbl_HataMesaji.Text = "";
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
        public bool KullaniciAdiSorgulama(string kullaniciAdi)
        {
            int kontrol = 0;
            try
            {
                baglanti.Open();
                SqlCommand komutsorgula = new SqlCommand("SELECT Count(*) FROM Tbl_Kullanici WHERE kullanici_adi=@p1", baglanti);
                komutsorgula.Parameters.AddWithValue("@p1", kullaniciAdi);
                kontrol = Convert.ToInt32(komutsorgula.ExecuteScalar());
                baglanti.Close();
            }
            catch (Exception)
            {
                lbl_HataMesaji.Text = "Sorgulama sırasında bir hata oluştu. Tekrar deneyiniz.";
                return false;
            }
            if (kontrol > 0)
            {
                lbl_HataMesaji.Text = "Bu kullanıcı adı kullanılmaktadır. Lütfen başka bir kullanıcı adı giriniz.";
                return false;//Bu kullanıcı adı mevcut
            }
            else
                return true;//Bu kullanıcı adı mevcut değil
        }
        public bool EmailSorgulama(string email)
        {
            int kontrol = 0;
            try
            {
                baglanti.Open();
                SqlCommand komutsorgula = new SqlCommand("SELECT Count(*) FROM Tbl_Kullanici WHERE email=@p1", baglanti);
                komutsorgula.Parameters.AddWithValue("@p1", email);
                kontrol = Convert.ToInt32(komutsorgula.ExecuteScalar());
                baglanti.Close();
            }
            catch(Exception)
            {
                lbl_HataMesaji.Text = "Sorgulama sırasında bir hata oluştu. Tekrar deneyiniz.";
                return false;
            }
            if (kontrol > 0)
            {
                lbl_HataMesaji.Text = "Bu email adresi zaten kayıtlı. Lütfen başka bir email adresi giriniz.";
                return false;//Bu email mevcut
            }
            else
                return true;//Bu email mevcut değil

        }

        private void AktivasyonMailiYolla(Guid aktivasyonKodu)
        {
            var verifyUrl = "/HesabiDogrula.aspx?kod=" + aktivasyonKodu;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);


            MailMessage mail = new MailMessage(); //yeni bir mail nesnesi Oluşturuldu.
            mail.IsBodyHtml = true; //mail içeriğinde html etiketleri kullanılsın mı?
            mail.To.Add(txt_Email.Text.ToString()); //Kime mail gönderilecek.

            //mail kimden geliyor, hangi ifade görünsün?
            mail.From = new MailAddress("tanerbsky@gmail.com", "Aktivasyon", System.Text.Encoding.UTF8);
            mail.Subject = "Hesap Doğrulama "; //mailin konusu

            //mailin içeriği.. Bu alan isteğe göre genişletilip daraltılabilir.
            mail.Body = "Aktivasyon Kodu= <a href='" + link + "'>" + link + "</a>";
            mail.IsBodyHtml = true;
            SmtpClient smp = new SmtpClient();

            //mailin gönderileceği adres ve şifresi
            smp.Credentials = new NetworkCredential("******@gmail.com", "******");
            smp.Port = 587;
            smp.Host = "smtp.gmail.com";//gmail üzerinden gönderiliyor.
            smp.EnableSsl = true;
            smp.Send(mail);//mail isimli mail gönderiliyor.
        }
        public bool KayitEkle(string kullaniciAdi,string email, string parola, string yetki)
        {
            try
            {
                Guid aktivasyonkodu = Guid.NewGuid();
                baglanti.Open();
                SqlCommand komutkayİtekle = new SqlCommand("INSERT INTO Tbl_Kullanici(kullanici_adi,email,parola,yetki,hesap_dogrulandi,hesap_kilitlendi,aktivasyon_kodu,toplam_alan,bos_alan) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9) ", baglanti);
                komutkayİtekle.Parameters.AddWithValue("@p1", kullaniciAdi);
                komutkayİtekle.Parameters.AddWithValue("@p2", email);
                komutkayİtekle.Parameters.AddWithValue("@p3", SHA512(parola).ToString());
                komutkayİtekle.Parameters.AddWithValue("@p4", yetki);
                komutkayİtekle.Parameters.AddWithValue("@p5", 0);
                komutkayİtekle.Parameters.AddWithValue("@p6", 0);
                komutkayİtekle.Parameters.AddWithValue("@p7", aktivasyonkodu);
                komutkayİtekle.Parameters.AddWithValue("@p8", 104857600);
                komutkayİtekle.Parameters.AddWithValue("@p9", 104857600);
                komutkayİtekle.ExecuteNonQuery();
                baglanti.Close();
                AktivasyonMailiYolla(aktivasyonkodu);
                return true;//Kayıt ekleme başarılı
            }
            catch(Exception)
            {
                lbl_HataMesaji.Text = "Kayıt ekleme işlemi sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
                return false;//Kayıt ekleme başarız
            }
        }
        protected void kayitol_Click(object sender, EventArgs e)
        {
            if(KullaniciAdiSorgulama(txt_Kullaniciadi.Text.ToString()))
            {
                if (EmailSorgulama(txt_Email.Text.ToString()))
                {
                    if (txt_Sifretekrar.Text == txt_Sifre.Text)
                    {
                        if(KayitEkle(txt_Kullaniciadi.Text.ToString(), txt_Email.Text.ToString(), txt_Sifre.Text.ToString(),"User"))
                            Server.Transfer("~/HesabiDogrula.aspx");
                    }
                    else
                        lbl_HataMesaji.Text = "Şifre ve Şifre Tekrarı Aynı Değil!";
                }
            }
        }
    }
}