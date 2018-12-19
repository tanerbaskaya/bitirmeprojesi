using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BitirmeProjesi
{
    public class SqlBaglantisi
    {
         public SqlConnection baglanti = new SqlConnection("Data Source=DESKTOP-H1JMK6K;Initial Catalog=BitirmeDb;Integrated Security=True;MultipleActiveResultSets=True");  
         public void deneme()
        {

        }
    }
}