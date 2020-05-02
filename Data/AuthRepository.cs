using LoginAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAPI.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);  //sprawdzenie czy podana nazwa użytkownika jest poprawna.

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))   
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i=0; i<passwordHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }

            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash,  passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);  // Dodanie do bazy danych użytkownika.
            await _context.SaveChangesAsync(); //zapisanie zmian w bazie danych

            return user;
        }

        private void CreatePasswordHash(string password,  out byte[] passwordHash,  out byte[] passwordSalt)   //out służące do przesyłania tylko referencji.
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())    //sposób kodowania hasła.
            {
                passwordSalt = hmac.Key;   //przydzielanie losowego klucza 
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));  // hashowanie hasła podanego w stringu.
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))  // używamy _context aby odwołać się do bazy danych, tabeli Users, 
                return true;
            return false;
                
        }
    }
}
