using CodeFirstGenerator.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Principal;

namespace CodeFirstGenerator.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.ChangeTracker.Tracking += OnTracking;
        }

        private void OnTracking(object? sender, EntityTrackingEventArgs e)
        {
            if (e.Entry.Entity is IEntity longEntity && longEntity.Id == 0)
            {
                longEntity.Id = Yitter.IdGenerator.YitIdHelper.NextId();
            }
        }

        public virtual DbSet<EntityInfo> EntityInfos { get; set; }
        public virtual DbSet<PropertyInfo> PropertyInfos { get; set; }
        public virtual DbSet<EntityAttribute> EntityAttributes { get; set; }
        public virtual DbSet<PropertyAttribute> PropertyAttributes { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<Solutions> Solutionss { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //设置Id非自增
            modelBuilder.Entity<EntityInfo>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(e => e.Id).ValueGeneratedNever();
                builder.HasIndex(e => e.ClassName).IsUnique();
            });

            modelBuilder.Entity<PropertyInfo>(builder =>
            {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Template>(builder =>
            {
                builder.HasKey(t => t.Id);
                builder.Property(t => t.Id).ValueGeneratedNever();
                builder.HasIndex(e => e.Name).IsUnique();
                builder.HasIndex(e => e.OutputPath).IsUnique();
            });
            modelBuilder.Entity<Configuration>(builder =>
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Id).ValueGeneratedNever();
            });
            modelBuilder.Entity<EntityAttribute>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(c => c.Id).ValueGeneratedNever();
            });
            modelBuilder.Entity<PropertyAttribute>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(c => c.Id).ValueGeneratedNever();
            });
            modelBuilder.Entity<Solutions>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder.Property(c => c.Id).ValueGeneratedNever();
                builder.HasMany(s => s.Templates).WithMany();
                builder.HasIndex(e => e.Name).IsUnique();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.AddInterceptors(new IdSettingInterceptor());
        }
    }

}
