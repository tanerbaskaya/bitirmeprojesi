using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace BitirmeProjesi
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string FileName = @"‪C:\Users\TanerB\Desktop\asd.png";

            Console.WriteLine("Encrypt " + FileName);

            // Encrypt the file.
            AddEncryption(FileName);
        }

        const string exceptionMessage = "Action error. File encryption or decryption process is failing!";

        public static void AddEncryption(string FileName)
        {

            File.Encrypt(FileName);

        }

        // Decrypt a file.
        public static void RemoveEncryption(string FileName)
        {
            File.Decrypt(FileName);
        }
    }
}