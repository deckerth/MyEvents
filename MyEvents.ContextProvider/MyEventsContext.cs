using Microsoft.EntityFrameworkCore;
using MyEvents.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyEvents.ContextProvider
{
    public class MyEventsContext : DbContext
    {
        public DbSet<Performance> Events {get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Soloist> Soloists { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Composer> Composers { get; set; }

        private bool MassUpdateMode = false;
        private bool UnsavedChangesExist = false;

        public MyEventsContext() : base()
        {
        }

        public MyEventsContext(DbContextOptions<MyEventsContext> options) : base(options)
        {
        }

        public void StartMassUpdate()
        {
            MassUpdateMode = true;
        }

        public async Task<int> EndMassUpdateModeAsync()
        {
            MassUpdateMode = false;
            if (UnsavedChangesExist)
            {
                UnsavedChangesExist = false;
                return await SaveChangesAsync();
            }
            else
            {
                return 0;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (MassUpdateMode)
            {
                UnsavedChangesExist = true;
                return 0;
            }
            else
            {
                try
                {
                    return await base.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite("Data Source=MyBooks.db");
            optionsBuilder.EnableSensitiveDataLogging();
        }

    }
}
