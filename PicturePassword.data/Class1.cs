using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace PicturePassword.data
{
    
    public class DBManager
    {
        private string _connection;
        public DBManager(string con)
        {
            _connection = con;
        }
        public void Add(UploadedPicture picture)
        {
            using (var context = new PicturesDBDataContext(_connection))
            {
                context.UploadedPictures.InsertOnSubmit(picture);
                context.SubmitChanges();
            }
        }
        public UploadedPicture GetPic(int id)
        {
            using (var context = new PicturesDBDataContext(_connection))
            
            {
                return context.UploadedPictures.FirstOrDefault(i => i.id == id);
            }
        }
        public void TimesSeen(int id)
        {
             using (var context = new PicturesDBDataContext(_connection))
             {
                    UploadedPicture image = context.UploadedPictures.FirstOrDefault(i => i.id == id);
                    image.TimesSeen = image.TimesSeen + 1;
                    context.SubmitChanges();
             }
        }
        public User GetUser(string email)
        {

            using (var context = new PicturesDBDataContext(_connection))
            {
                return  context.Users.FirstOrDefault(u => u.Email == email);

            }
        }
        public User GetUser(string email, string password)
        {
            using (var context = new PicturesDBDataContext(_connection))
            {
               User user = context.Users.FirstOrDefault(u => u.Email == email);
               bool correct = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
                if (correct)
                {
                     return user;
                }
                return null;
            }      
        }
        public void AddUser(User user, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string hash = PasswordHelper.HashPassword(password, salt);
            using (var connection = new SqlConnection(_connection))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "insert into Users Values(@name, @email, @hash, @salt)";
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@hash", hash);
                command.Parameters.AddWithValue("@salt", salt);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public IEnumerable<UploadedPicture> UsersPictures(int id)
        {
            using (var context = new PicturesDBDataContext(_connection))
            {
                return context.UploadedPictures.Where(u => u.id == id).ToList();
            }
            
        }
        public void delete(int id)
        {
            using (var context = new PicturesDBDataContext(_connection))
            {
                context.ExecuteCommand("delete from UploadedPictures where id = {0}", id);
            }
        }
    }

    
}
