using PixShareLIB.Models;
using System.Collections.Generic;

namespace PixShareLIB.DAL
{
    public interface IRepo
    {
        User AuthUser(string email, string password);
        int LoginUser(string email, string password);
        void CreateUser(User user);
        User GetUserById(int userId);
        void UpdateUser(User user);
        void AddPost(IPost post);
        IPost GetPostById(int id);
        List<IPost> GetAllPosts();
        void UpdatePost(IPost post);
        List<IPost> GetLast10Posts();
        void AddUserAction(UserAction action);
        List<UserAction> GetUserActions(int userId);

        // Admin methods
        void DeletePost(int postId);
        List<User> GetAllUsers();

        List<IPost> GetUserPosts(int userId);
    }
}
