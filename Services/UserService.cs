using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLifeTimeTalents.Data;
using TheLifeTimeTalents.Helpers;
using TheLifeTimeTalents.Models;

namespace TheLifeTimeTalents.Services
{

    public interface IUserService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> CreateUser(User user, string password);
        Task<int> UpdateUser(User userP, int id);
        Task<int> UpdatePaidRole(string email);
        Task<List<User>> GetAll();
        Task<User> GetById(int id);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private readonly AppSettings _appSettings;
        public ApplicationDbContext Database { get; }

        public UserService(IOptions<AppSettings> appSettings, ApplicationDbContext database)
        {
            Database = database;
            _appSettings = appSettings.Value;
        }

        //Authentication service used by HomeController for login
        public async Task<User> AuthenticateAsync(string username, string password)
        {
            if (username.Equals(null) || password.Equals(null))
                return null;
            //As of now it uses the local List to look up the users.
            var user = await Database.Users.Include(u => u.Role).SingleOrDefaultAsync(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            //// authentication successful so generate jwt token [Not using JWT token -Husni]
            //var tokenHandler = new JwtSecurityTokenHandler();

            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //user.Token = tokenHandler.WriteToken(token); token is not in User model.

            // remove password before returning
            user.Password = null;

            return user;
        }

        public async Task<User> CreateUser(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required.");

            if (Database.Users.Any(x => x.Email == user.Email))
                throw new AppException("Email \"" + user.Email + "\" has been registered.");

            if (Database.Users.Any(x => x.Username == user.Username))
            {
                throw new AppException("Username \"" + user.Username + "\" has been registered.");
            }

            //byte[] passwordHash, passwordSalt;
            //CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //user.PasswordHash = passwordHash;
            //user.PasswordSalt = passwordSalt;
            user.Password = password;

            try
            {
                Database.Users.Add(user);
                await Database.SaveChangesAsync();

                return user;
            }
            catch
            {
                throw new AppException("User create operation went wrong.");
            }
        }

        public async Task<int> UpdateUser(User userP, int id)
        {
            string password = userP.Password;
            int result = 0;
            var user = await Database.Users.FindAsync(id);

            if (user == null)
                throw new AppException("User not found");

            if (userP.Email != user.Email)
            {
                // username has changed so check if the new username is already taken
                if (Database.Users.Any(x => x.Email == userP.Email))
                    throw new AppException("Email \"" + user.Email + "\" has been registered.");
            }
            if (Database.Users.Any(x => x.Username == user.Username))
            {
                throw new AppException("Username \"" + user.Username + "\" has been registered.");
            }
            // update user properties
            user.Username = userP.Username;
            user.RoleID = userP.RoleID;
            user.Email = userP.Email;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                //byte[] passwordHash, passwordSalt;
                //CreatePasswordHash(password, out passwordHash, out passwordSalt);

                //user.PasswordHash = passwordHash;
                //user.PasswordSalt = passwordSalt;
                user.Password = password;
            }
            try
            {
                Database.Users.Update(user);
                result = await Database.SaveChangesAsync();
                return result;
            }
            catch
            {
                throw new AppException("User update operation went wrong.");
            }

        }

        public async Task<int> UpdatePaidRole(string email)
        {
            int result = 0;
            var user = await Database.Users.FindAsync(email);

            if (user == null)
                throw new AppException("User not found");

            // update user properties
            user.RoleID = 1;

            try
            {
                Database.Users.Update(user);
                result = await Database.SaveChangesAsync();
                return result;
            }
            catch
            {
                throw new AppException("User update operation went wrong.");
            }

        }

        public async Task<List<User>> GetAll()
        {
            // return users without passwords
            return await Database.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            var user = await Database.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.UserID == id);

            // return user without password
            if (user != null)
                user.Password = null;

            return user;
        }
    }

}
