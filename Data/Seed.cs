using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DatingApp.API.Data
{
  public class Seed
  {
    private readonly DataContext _context;
    public Seed(DataContext context)
    {
      this._context = context;
    }

    public void SeedUsers()
    {
      var userCount = _context.Users.Count();
      if (userCount > 0)
      {
        Console.WriteLine("Deleting Users Data from DB as User count is {0}", userCount);
        _context.Users.RemoveRange(_context.Users);
        _context.SaveChanges();
      }


      // seed users
      var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");

      var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "dd-MM-yyyy" };
      var users = JsonConvert.DeserializeObject<List<Models.User>>(userData, dateTimeConverter);
      foreach (var user in users)
      {
        //create the password hash
        byte[] passwordHash, passwordSalt;
        CreatePasswordHash("password", out passwordHash, out passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        user.Username = user.Username.ToLower();
        _context.Users.Add(user);
      }
      _context.SaveChanges();
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }
  }
}