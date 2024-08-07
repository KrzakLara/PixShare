using Microsoft.ApplicationBlocks.Data;
using PixShare.State;
using PixShareLIB.DAL;
using PixShareLIB.Models;
using PixShareLIB.State;
using PS_LIB.State;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

public class DBRepo : IRepo
{
    private static string CS = ConfigurationManager.ConnectionStrings["cs"].ConnectionString;

    public User AuthUser(string email, string password)
    {
        string query = "SELECT * FROM [User] WHERE Email = @Email";
        DataTable result = ExecuteQuery(query, new SqlParameter("@Email", email));

        if (result.Rows.Count == 0)
        {
            return null;
        }

        DataRow row = result.Rows[0];
        string storedPassword = row["Password"].ToString();
        string hashedPassword = HashPassword(password);

        if (row["UserType"].ToString() == "Admin" && storedPassword == password)
        {
            return new User
            {
                IDUser = (int)row["IDUser"],
                Username = row["Username"].ToString(),
                Email = row["Email"].ToString(),
                Password = storedPassword,
                UserType = row["UserType"].ToString(),
                PackageType = row["PackageType"].ToString()
            };
        }

        if (storedPassword == hashedPassword)
        {
            return new User
            {
                IDUser = (int)row["IDUser"],
                Username = row["Username"].ToString(),
                Email = row["Email"].ToString(),
                Password = storedPassword,
                UserType = row["UserType"].ToString(),
                PackageType = row["PackageType"].ToString()
            };
        }

        return null;
    }

    public int LoginUser(string email, string password)
    {
        string query = "SELECT IDUser, Password FROM [User] WHERE Email = @Email";
        DataTable result = ExecuteQuery(query, new SqlParameter("@Email", email));

        if (result.Rows.Count > 0)
        {
            string storedPassword = result.Rows[0]["Password"].ToString();
            string hashedPassword = HashPassword(password);

            if (storedPassword == hashedPassword)
            {
                return Convert.ToInt32(result.Rows[0]["IDUser"]);
            }
        }

        return -1;
    }

    private DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
    {
        DataTable dataTable = new DataTable();
        using (SqlConnection connection = new SqlConnection(CS))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(dataTable);
                }
            }
        }
        return dataTable;
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public void CreateUser(User user)
    {
        string hashedPassword = HashPassword(user.Password);
        string query = "INSERT INTO [User] (Username, Email, Password, UserType, PackageType) VALUES (@Username, @Email, @Password, @UserType, @PackageType)";
        SqlHelper.ExecuteNonQuery(CS, CommandType.Text, query,
            new SqlParameter("@Username", user.Username),
            new SqlParameter("@Email", user.Email),
            new SqlParameter("@Password", hashedPassword),
            new SqlParameter("@UserType", user.UserType),
            new SqlParameter("@PackageType", user.PackageType));
    }

    public User GetUserById(int userId)
    {
        string query = "SELECT * FROM [User] WHERE IDUser = @IDUser";
        DataTable result = ExecuteQuery(query, new SqlParameter("@IDUser", userId));

        if (result.Rows.Count == 0)
        {
            return null;
        }

        DataRow row = result.Rows[0];
        User user = new User
        {
            IDUser = (int)row["IDUser"],
            Username = row["Username"].ToString(),
            Email = row["Email"].ToString(),
            Password = row["Password"].ToString(),
            UserType = row["UserType"].ToString(),
            PackageType = row["PackageType"].ToString(),
            CurrentConsumption = (int)row["CurrentConsumption"],
            LastPackageChangeDate = (DateTime)row["LastPackageChangeDate"]
        };

        IUserPackageState value;
        switch (user.PackageType)
        {
            case "PRO":
                value = new ProPackageState();
                break;
            case "GOLD":
                value = new GoldPackageState();
                break;
            default:
                value = new FreePackageState();
                break;
        }
        user.CurrentPackageState = value;

        return user;
    }

    public void UpdateUser(User user)
    {
        string query = "UPDATE [User] SET Username = @Username, Email = @Email, PackageType = @PackageType WHERE IDUser = @IDUser";
        SqlHelper.ExecuteNonQuery(CS, CommandType.Text, query,
            new SqlParameter("@Username", user.Username),
            new SqlParameter("@Email", user.Email),
            new SqlParameter("@PackageType", user.PackageType),
            new SqlParameter("@IDUser", user.IDUser));
    }



    public void AddPost(IPost post)
    {
        if (!DoesUserIdExist(post.UserID))
        {
            throw new Exception("UserID does not exist.");
        }

        string query = "INSERT INTO [Post] (Description, Hashtags, ImagePath, ThumbnailPath, Date, UserID, Size) VALUES (@Description, @Hashtags, @ImagePath, @ThumbnailPath, @Date, @UserID, @Size)";
        SqlHelper.ExecuteNonQuery(CS, CommandType.Text, query,
            new SqlParameter("@Description", post.Description),
            new SqlParameter("@Hashtags", post.Hashtags),
            new SqlParameter("@ImagePath", post.ImagePath),
            new SqlParameter("@ThumbnailPath", post.ThumbnailPath),
            new SqlParameter("@Date", post.Date),
            new SqlParameter("@UserID", post.UserID),
            new SqlParameter("@Size", post.Size));
    }

    private bool DoesUserIdExist(int userId)
    {
        string query = "SELECT COUNT(*) FROM [User] WHERE IDUser = @IDUser";
        var result = ExecuteScalar(query, new SqlParameter("@IDUser", userId));
        return (int)result > 0;
    }

    private object ExecuteScalar(string query, params SqlParameter[] parameters)
    {
        using (SqlConnection connection = new SqlConnection(CS))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteScalar();
            }
        }
    }

    public IPost GetPostById(int id)
    {
        string query = "SELECT p.*, u.Username FROM [Post] p JOIN [User] u ON p.UserID = u.IDUser WHERE p.IDPost = @IDPost";
        DataTable result = ExecuteQuery(query, new SqlParameter("@IDPost", id));

        if (result.Rows.Count == 0)
        {
            return null;
        }

        DataRow row = result.Rows[0];
        return new Post
        {
            IDPost = (int)row["IDPost"],
            Description = row["Description"].ToString(),
            Hashtags = row["Hashtags"].ToString(),
            ImagePath = row["ImagePath"].ToString(),
            ThumbnailPath = row["ThumbnailPath"].ToString(),
            Date = (DateTime)row["Date"],
            UserID = (int)row["UserID"],
            Size = row["Size"] as long?,
            Username = row["Username"].ToString()
        };
    }

    public List<IPost> GetAllPosts()
    {
        string query = @"
        SELECT p.*, u.Username
        FROM [Post] p
        JOIN [User] u ON p.UserID = u.IDUser
        ORDER BY p.Date DESC";

        DataTable result = ExecuteQuery(query);

        List<IPost> posts = new List<IPost>();
        foreach (DataRow row in result.Rows)
        {
            posts.Add(new Post
            {
                IDPost = (int)row["IDPost"],
                Description = row["Description"].ToString(),
                Hashtags = row["Hashtags"].ToString(),
                ImagePath = row["ImagePath"].ToString(),
                ThumbnailPath = row["ThumbnailPath"].ToString(),
                Date = (DateTime)row["Date"],
                UserID = (int)row["UserID"],
                Size = row["Size"] as long?,
                Username = row["Username"].ToString()
            });
        }

        return posts;
    }

    public void UpdatePost(IPost post)
    {
        string query = "UPDATE [Post] SET Description = @Description, Hashtags = @Hashtags, ImagePath = @ImagePath, Date = @Date, Size = @Size WHERE IDPost = @IDPost";
        SqlHelper.ExecuteNonQuery(CS, CommandType.Text, query,
            new SqlParameter("@Description", post.Description),
            new SqlParameter("@Hashtags", post.Hashtags),
            new SqlParameter("@ImagePath", post.ImagePath),
            new SqlParameter("@Date", post.Date),
            new SqlParameter("@Size", post.Size),
            new SqlParameter("@IDPost", post.IDPost));
    }

    public List<IPost> GetLast10Posts()
    {
        string query = "SELECT TOP 10 p.*, u.Username FROM [Post] p JOIN [User] u ON p.UserID = u.IDUser ORDER BY p.Date DESC";
        DataTable result = ExecuteQuery(query);

        List<IPost> posts = new List<IPost>();
        foreach (DataRow row in result.Rows)
        {
            posts.Add(new Post
            {
                IDPost = (int)row["IDPost"],
                Description = row["Description"].ToString(),
                Hashtags = row["Hashtags"].ToString(),
                ImagePath = row["ImagePath"].ToString(),
                ThumbnailPath = row["ThumbnailPath"].ToString(),
                Date = (DateTime)row["Date"],
                UserID = (int)row["UserID"],
                Size = row["Size"] as long?,
                Username = row["Username"].ToString()
            });
        }

        return posts;
    }

    public void AddUserAction(UserAction action)
    {
        string query = "INSERT INTO [UserAction] (UserID, Action, Timestamp) VALUES (@UserID, @Action, @Timestamp)";
        SqlHelper.ExecuteNonQuery(CS, CommandType.Text, query,
            new SqlParameter("@UserID", action.UserID),
            new SqlParameter("@Action", action.Action),
            new SqlParameter("@Timestamp", action.Timestamp));
    }

    public List<UserAction> GetUserActions(int userId)
    {
        string query = "SELECT * FROM [UserAction] WHERE UserID = @UserID";
        DataTable result = ExecuteQuery(query, new SqlParameter("@UserID", userId));

        List<UserAction> actions = new List<UserAction>();
        foreach (DataRow row in result.Rows)
        {
            actions.Add(new UserAction
            {
                UserID = (int)row["UserID"],
                Action = row["Action"].ToString(),
                Timestamp = (DateTime)row["Timestamp"]
            });
        }

        return actions;
    }

    // Admin methods
    public void DeletePost(int postId)
    {
        string query = "DELETE FROM [Post] WHERE IDPost = @IDPost";
        SqlHelper.ExecuteNonQuery(CS, CommandType.Text, query, new SqlParameter("@IDPost", postId));
    }

    public List<User> GetAllUsers()
    {
        string query = "SELECT * FROM [User]";
        DataTable result = ExecuteQuery(query);

        List<User> users = new List<User>();
        foreach (DataRow row in result.Rows)
        {
            users.Add(new User
            {
                IDUser = (int)row["IDUser"],
                Username = row["Username"].ToString(),
                Email = row["Email"].ToString(),
                Password = row["Password"].ToString(),
                UserType = row["UserType"].ToString(),
                PackageType = row["PackageType"].ToString(),
                CurrentConsumption = (int)row["CurrentConsumption"],
                LastPackageChangeDate = (DateTime)row["LastPackageChangeDate"]
            });
        }

        return users;
    }

    public List<IPost> GetUserPosts(int userId)
    {
        string query = @"
            SELECT p.*, u.Username
            FROM [Post] p
            JOIN [User] u ON p.UserID = u.IDUser
            WHERE p.UserID = @UserID
            ORDER BY p.Date DESC";

        DataTable result = ExecuteQuery(query, new SqlParameter("@UserID", userId));

        List<IPost> posts = new List<IPost>();
        foreach (DataRow row in result.Rows)
        {
            posts.Add(new Post
            {
                IDPost = (int)row["IDPost"],
                Description = row["Description"].ToString(),
                Hashtags = row["Hashtags"].ToString(),
                ImagePath = row["ImagePath"].ToString(),
                ThumbnailPath = row["ThumbnailPath"].ToString(),
                Date = (DateTime)row["Date"],
                UserID = (int)row["UserID"],
                Size = row["Size"] as long?,
                Username = row["Username"].ToString()
            });
        }

        return posts;
    }

}
