using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WashMachine.Octopus.OctopusSetting
{
    public partial class BoxcutContext : DbContext
    {
        public BoxcutContext()
        {
        }

        public BoxcutContext(DbContextOptions<BoxcutContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Cost> Cost { get; set; }
        public virtual DbSet<EmailLog> EmailLog { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Package> Package { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PaymentOctopus> PaymentOctopus { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<Pricing> Pricing { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Queue> Queue { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<Shop> Shop { get; set; }
        public virtual DbSet<ShopOwner> ShopOwner { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseSqlServer("Server=localhost;Database=Boxcut;Trusted_Connection=True;");
                optionsBuilder.UseSqlServer(DataSetting.OctopusConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.ResetTime).HasColumnType("datetime");

                entity.Property(e => e.SaltKey)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SecureCode)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ShopCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Cost>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ShopCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Pricing)
                    .WithMany(p => p.Cost)
                    .HasForeignKey(d => d.PricingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cost_Pricing");
            });

            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.Property(e => e.DeviceId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FunctionName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.SentTime).HasColumnType("datetime");

                entity.Property(e => e.ShopCode)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Time)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.BookingTime).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.PaymentId)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.SecureCode)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ShopCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ShopName).HasMaxLength(200);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.Subject).HasMaxLength(200);

                entity.Property(e => e.TicketCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.ProductCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName).HasMaxLength(200);

                entity.Property(e => e.ProductName2)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_Order");
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.Property(e => e.Brief).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Keywords).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name2).HasMaxLength(200);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Photos).HasMaxLength(1000);

                entity.Property(e => e.ShopCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Message).HasMaxLength(300);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payment)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Payment_Order");
            });

            modelBuilder.Entity<PaymentOctopus>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Message).HasMaxLength(300);

                entity.Property(e => e.ShopCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Mission)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Permission)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Permission_User");
            });

            modelBuilder.Entity<Pricing>(entity =>
            {
                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.ShopCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Brief).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Keywords).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name2).HasMaxLength(200);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Photos).HasMaxLength(1000);

                entity.Property(e => e.ShopCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Queue>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.SecureCode)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ShopCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.Property(e => e.Key)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Value).HasMaxLength(200);
            });

            modelBuilder.Entity<Shop>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Coordinate)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Phone).HasMaxLength(100);

                entity.Property(e => e.SecureCode).HasMaxLength(200);

                entity.Property(e => e.ShortName).HasMaxLength(200);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ShopOwner>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.ShopOwner)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShopOwner_User");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopOwner)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShopOwner_Shop");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Image)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.ResetTime).HasColumnType("datetime");

                entity.Property(e => e.SaltKey)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SecureCode)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ShopCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });
        }
    }
}
