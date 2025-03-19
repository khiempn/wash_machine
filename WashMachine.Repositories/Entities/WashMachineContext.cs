using Microsoft.EntityFrameworkCore;

namespace WashMachine.Repositories.Entities
{
    public partial class WashMachineContext : DbContext
    {
        public WashMachineContext()
        {
        }

        public WashMachineContext(DbContextOptions<WashMachineContext> options)
            : base(options)
        {
        }

        public virtual DbSet<EmailLog> EmailLog { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<Shop> Shop { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Coupon> Coupon { get; set; }
        public virtual DbSet<ShopCom> ShopCom { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<MachineCommand> MachineCommand { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=45.32.106.7,63632;Initial Catalog=WashMachine;Integrated Security=False;Persist Security Info=False;User ID=electric_admin;Password=Electric@123!@@@");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                entity.Property(e => e.InsertTime).HasColumnType("datetime");
                entity.Property(e => e.Amount).HasColumnType("float");
                entity.Property(e => e.PaymentId).HasColumnType("int");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.InsertTime).HasColumnType("datetime");
                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
                entity.Property(e => e.Amount).HasColumnType("float");
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

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Phone).HasMaxLength(100);

                entity.Property(e => e.ShortName).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
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

                entity.Property(e => e.InsertTime).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(1000);

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.Property(e => e.UsedDate).HasColumnType("datetime");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.ShopCode)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<ShopCom>(entity =>
            {
                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.ShopComs)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShopCom_Shop");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Time).HasColumnType("datetime");
            });
        }
    }
}
