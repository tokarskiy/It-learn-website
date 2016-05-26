using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Data.Odbc;
using MySql.Data.MySqlClient;
using System.Data;

namespace ItLearn.Models
{
    public class DatabaseConnection
    {
        private static DatabaseConnection _instance = null;
        private MySqlConnectionStringBuilder _connectionString;

        private DatabaseConnection()
        {
            _connectionString = new MySqlConnectionStringBuilder();
            _connectionString.Server = "localhost";
            _connectionString.Database = "it_learn_database";
            _connectionString.UserID = "root";
            _connectionString.Password = "";
        }

        /// <summary>
        ///     Получение экземпляра класса
        /// </summary>
        public static DatabaseConnection GetInstance()
        {
            return _instance ?? new DatabaseConnection();
        }

        /// <summary>
        ///     Получение превью статей, что находятся в главном меню
        /// </summary>
        /// <param name="count">Количество статей</param>
        public List<ArticlePreview> GetMenuArticles(int count)
        {
            List<ArticlePreview> result = new List<ArticlePreview>();
            string query = $"SELECT ArticleName, Summary FROM Articles WHERE OnMenu = 1 LIMIT {count}";
            DataTable table = new DataTable();
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            table.Load(dr);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var name = table.Rows[i].ItemArray[0] as string;
                var summary = table.Rows[i].ItemArray[1] as string;
                result.Add(new ArticlePreview { Name = name, Summary = summary });
            }
            return result;
        }

        public UserInfo GetUserInfo(string name)
        {
            return GetUserInfo(name, null);
        }

        public UserInfo GetUserInfo(string name, string email)
        {
            UserInfo result = new UserInfo();
            
            string query = $"SELECT UserName, Country, IsAdmin, Gender, Info FROM Articles WHERE UserName = '{name}' OR Email = '{email}'";
            if (name == null)
            {
                query = $"SELECT UserName, Country, IsAdmin, Gender, Info FROM Articles WHERE Email = '{email}'";
            }
            if (email == null)
            {
                query = $"SELECT UserName, Country, IsAdmin, Gender, Info FROM Articles WHERE UserName = '{name}'";
            }
            DataTable table = new DataTable();

            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            table.Load(dr);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            result.Name = table.Rows[0].ItemArray[0] as string;
            result.Country = table.Rows[0].ItemArray[1] as string;
            result.Role = (table.Rows[0].ItemArray[2] as string) == "1" ? Role.ADMIN : Role.USER;
            switch (table.Rows[0].ItemArray[3] as string)
            {
                case "Male":
                    result.Gender = Gender.MALE;
                    break;
                case "Female":
                    result.Gender = Gender.FEMALE;
                    break;
                default:
                    result.Gender = Gender.UNDEFINED;
                    break;
            }
            result.Info = table.Rows[0].ItemArray[4] as string;
            return result;
        }

        public void AddUser(string name, bool admin, string email, string password, string info, string country)
        {
            string query =
                $"INSERT INTO Users(UserName, IsAdmin, Email, UserPassword, Info, Country) VALUES('{name}', {(admin ? 1 : 0)}, '{email}', '{password}', '{info}', '{country}')";
            DataTable table = new DataTable();
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public Article GetArticle(string name)
        {
            Article result = new Article();
            string query = $"SELECT ArticleName, Summary, Contents, User FROM Articles WHERE ArticleName = '{name}';";
            DataTable table = new DataTable();
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            table.Load(dr);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            if (table.Rows.Count == 0)
            {
                return null;
            }
            result.Name = table.Rows[0].ItemArray[0] as string;
            result.Summary = table.Rows[0].ItemArray[1] as string;
            result.Contents = table.Rows[0].ItemArray[2] as string;
            result.Author = table.Rows[0].ItemArray[3] as string;
            table.Clear();

            query = $"SELECT Tag FROM ArticleTags WHERE Article = '{name}';";
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            table.Load(dr);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var elem = table.Rows[i].ItemArray[0] as string;
                result.Tags.Add(elem);
            }

            return result;
        }

        public void AddArticle(string name, List<string> tags, string summary, string contents, string user)
        {
            string query = $"INSERT INTO Articles(ArticleName, Summary, Contents, User) VALUES('{name}', '{summary}', '{contents}', '{user}')";
            DataTable table = new DataTable();
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return;
                }
            }

            foreach (var tag in tags)
            {
                query = $"INSERT INTO ArticleTags(Article, Tag) VALUES({name}, {tag})";
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = _connectionString.ConnectionString;
                    var command = new MySqlCommand(query, connection);
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }
        }

        public void AddComment(string article, string user, string contents)
        {
            var query = $"INSERT INTO Comments(Article, Contents, User) VALUES('{article}', '{contents}', '{user}')";
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public UserInfo Login(string name, string password)
        {
            UserInfo result = new UserInfo();
            string query = $"SELECT UserName, Country, IsAdmin, Gender, Info FROM Users WHERE UserName = '{name}' AND UserPassword = '{password}'";
            DataTable table = new DataTable();

            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            table.Load(dr);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            result.Name = table.Rows[0].ItemArray[0] as string;
            result.Country = table.Rows[0].ItemArray[1] as string;
            result.Role = (table.Rows[0].ItemArray[2] as string) == "1" ? Role.ADMIN : Role.USER;
            switch (table.Rows[0].ItemArray[3] as string)
            {
                case "Male":
                    result.Gender = Gender.MALE;
                    break;
                case "Female":
                    result.Gender = Gender.FEMALE;
                    break;
                default:
                    result.Gender = Gender.UNDEFINED;
                    break;
            }
            result.Info = table.Rows[0].ItemArray[4] as string;
            return result;
        }

        public List<string> GetCountries()
        {
            var result = new List<string>();
            string query = $"SELECT CountryName FROM Countries;";
            DataTable table = new DataTable();
            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionString.ConnectionString;
                var command = new MySqlCommand(query, connection);
                connection.Open();
                try
                {
                    using (MySqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            table.Load(dr);
                        }
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }

            for (int i = 0; i < table.Rows.Count; i++)
            {
                result.Add(table.Rows[i].ItemArray[0] as string);
            }
            return result;
        }
    }
}