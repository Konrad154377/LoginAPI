using LoginAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


/* 
 * Aby korzystać z DbContext należy zainstalować Microsoft.EntityFrameworkCore
 * Aby stworzyć migrację należy zainstalować Microsoft.EntityFrameworkCore.Tools
 * Aby była możliwa praca z bazą danych należy w pliku appsettings.json ustawić ConnectionString, 
 * w pliku Startup.cs poleceniem services.AddDbContext<DataContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))); określa się narzędzie do komunikacji z bazą danych 
 * */
namespace LoginAPI.Data
{
    public class DataContext : DbContext  
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }   //DbSet odpowiada za utworzenie tabeli w bazie danych o nazwie Users, polami w bazie danych będą pola z klasy User
    }
}
