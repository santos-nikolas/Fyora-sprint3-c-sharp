using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Fyora_sprint3.Data;
using Fyora_sprint3.Models;

namespace Fyora_sprint3.Repositories
{
    public class UserRepository
    {
        public void AddUser(User user)
        {
            using var context = new FyoraContext();

            string nick = user.Nickname.Trim();
            string mail = user.Email.Trim();

            bool nicknameTaken = context.Users
                .AsNoTracking()
                .Any(u => u.Nickname.ToLower() == nick.ToLower());
            if (nicknameTaken)
                throw new InvalidOperationException("Já existe um usuário com este nickname.");

            bool emailTaken = context.Users
                .AsNoTracking()
                .Any(u => u.Email.ToLower() == mail.ToLower());
            if (emailTaken)
                throw new InvalidOperationException("Já existe um usuário com este e-mail.");

            user.Nickname = nick;
            user.Email = mail;

            // ★ Fallback caso venha default (ex.: import do JSON sem CreatedAt)
            if (user.CreatedAt == default)
                user.CreatedAt = DateTime.Now;

            context.Users.Add(user);
            context.SaveChanges();
        }

        public List<User> GetAllUsers()
        {
            using var context = new FyoraContext();
            return context.Users.Include(u => u.ProgressLogs).ToList();
        }

        public User? GetUserById(int id)
        {
            using var context = new FyoraContext();
            return context.Users.Include(u => u.ProgressLogs).FirstOrDefault(u => u.Id == id);
        }

        public void UpdateUser(int id, string newNickname, string? newEmail = null)
        {
            using var context = new FyoraContext();
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return;

            string nick = newNickname.Trim();
            string? mail = newEmail?.Trim();

            bool nicknameTaken = context.Users
                .AsNoTracking()
                .Any(u => u.Id != id && u.Nickname.ToLower() == nick.ToLower());
            if (nicknameTaken)
                throw new InvalidOperationException("Já existe um usuário com este nickname.");

            user.Nickname = nick;

            if (!string.IsNullOrWhiteSpace(mail))
            {
                bool emailTaken = context.Users
                    .AsNoTracking()
                    .Any(u => u.Id != id && u.Email.ToLower() == mail.ToLower());
                if (emailTaken)
                    throw new InvalidOperationException("Já existe um usuário com este e-mail.");

                user.Email = mail!;
            }

            context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            using var context = new FyoraContext();
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return;

            context.Users.Remove(user);
            context.SaveChanges();
        }

        public void AddProgressLog(int userId, ProgressLog log)
        {
            using var context = new FyoraContext();
            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return;

            log.LogDate = log.LogDate == default ? DateTime.Now : log.LogDate;
            user.ProgressLogs.Add(log);
            context.SaveChanges();
        }
    }
}
