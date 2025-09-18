using System.Data.SqlClient;
using System.Collections.Generic;
using WebAPI_Projem.Models;
using System;

namespace WebAPI_Projem
{
    public class DataRepo
    {
        static string baglantiString = "Server=localhost;Database=Veritabanım;User Id=sa;Password=daspwd;";

        public static List<User> GetUsers()
        {
            List<User> users = new List<User>();

            try
            {
                SqlConnection baglanti = new SqlConnection(baglantiString);

                baglanti.Open();

                SqlCommand komut = new SqlCommand("SELECT isim, yas FROM Kisiler", baglanti);
                using (SqlDataReader reader = komut.ExecuteReader())
                {

                    while (reader.Read())
                        users.Add(new User
                        {
                            isim = reader[0].ToString(),
                            yas = (int)reader[1]
                        });
                }

                return users;
            }

            catch
            {
                User user = new User();
                user.isim = "ERROR";
                user.yas = -1;
                return new List<User> { user }; 
            }
        }

        public static User GetUserByID(int id)
        {
            User user = new User();

            try
            {
                SqlConnection baglanti = new SqlConnection(baglantiString);

                baglanti.Open();

                SqlCommand IDVarMi = new SqlCommand("SELECT ID FROM Kisiler", baglanti);
                using (SqlDataReader reader = IDVarMi.ExecuteReader())
                {
                    int i = 0;

                    while(reader.Read())
                    {
                        i++;
                    }

                    if (i < id) throw new Exception("ID yok");
                }

                SqlCommand komut = new SqlCommand("SELECT isim, yas FROM Kisiler WHERE ID=" + id, baglanti);
                using (SqlDataReader reader = komut.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.isim = reader[0].ToString();
                        user.yas = (int)reader[1];
                    }

                    return user;
                }
            }

            catch (Exception e)
            {
                if (e.Message == "ID yok") return new User { isim = "ID yok", yas = -1 };
                return new User { isim = "ERROR", yas = -1 };
            }
        }

        public static bool AddUsers(User user)
        {
            try
            {
                SqlConnection baglanti = new SqlConnection(baglantiString);
                
                baglanti.Open();

                using (SqlCommand komut = new SqlCommand("INSERT INTO Kisiler (isim, yas) VALUES (@isim, @yas)", baglanti))
                {
                    komut.Parameters.AddWithValue("@isim", user.isim);
                    komut.Parameters.AddWithValue("@yas", user.yas);
                    komut.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static List<User> GetAlphabetical()
        {
            List<User> users = new List<User>();

            try
            {
                SqlConnection baglanti = new SqlConnection(baglantiString);

                baglanti.Open();

                SqlCommand komut = new SqlCommand("SELECT isim, yas FROM Kisiler ORDER BY isim", baglanti);
                using (SqlDataReader reader = komut.ExecuteReader())
                {

                    while (reader.Read())
                        users.Add(new User
                        {
                            isim = reader[0].ToString(),
                            yas = (int)reader[1]
                        });
                }

                return users;
            }

            catch
            {
                User user = new User();
                user.isim = "ERROR";
                user.yas = -1;
                return new List<User> { user };
            }
        }
    }
}