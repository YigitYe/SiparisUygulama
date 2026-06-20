using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SiparisUygulama.Models
{
    public partial class YemekSiparisDBContext : IdentityDbContext<UygulamaKullanicisi>
    {
        public YemekSiparisDBContext()
        {
        }

        public YemekSiparisDBContext(DbContextOptions<YemekSiparisDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Kullanici> Kullanicis { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Odeme> Odemes { get; set; } = null!;
        public virtual DbSet<Personel> Personels { get; set; } = null!;
        public virtual DbSet<Restoran> Restorans { get; set; } = null!;
        public virtual DbSet<Sipari> Siparis { get; set; } = null!;
        public virtual DbSet<SiparisDetay> SiparisDetays { get; set; } = null!;
        public virtual DbSet<Teslimat> Teslimats { get; set; } = null!;
        public virtual DbSet<Yonetici> Yoneticis { get; set; } = null!;
        public virtual DbSet<Yorum> Yorums { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.ToTable("Kullanici");
                entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
                entity.Property(e => e.Sifre).HasMaxLength(255);
                entity.Property(e => e.Telefon).HasMaxLength(20);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.MenuItemId);
                entity.ToTable("Menu");
                entity.HasIndex(e => e.RestaurantId);
                entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
                entity.Property(e => e.Fiyat).HasPrecision(10, 2);
                entity.Property(e => e.ItemName).HasMaxLength(100);
                entity.Property(e => e.Kategori).HasMaxLength(50);
                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
                entity.HasOne(d => d.Restaurant)
                      .WithMany(p => p.Menus)
                      .HasForeignKey(d => d.RestaurantId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Odeme>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.ToTable("Odeme");
                entity.HasIndex(e => e.OrderId);
                entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
                entity.Property(e => e.Amount).HasPrecision(10, 2);
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.HasOne(d => d.Order)
                      .WithMany(p => p.Odemes)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Personel>(entity =>
            {
                entity.ToTable("Personel");
                entity.Property(e => e.PersonelId).HasColumnName("PersonelID");
                entity.Property(e => e.Adi).HasMaxLength(100);
                entity.Property(e => e.IletisimNumarasi).HasMaxLength(20);
                entity.Property(e => e.TeslimatAlani).HasMaxLength(100);
            });

            modelBuilder.Entity<Restoran>(entity =>
            {
                entity.HasKey(e => e.RestaurantId);
                entity.ToTable("Restoran");
                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.RestaurantName).HasMaxLength(100);
                entity.Property(e => e.TelefonNumarasi).HasMaxLength(20);
                entity.Property(e => e.MutfakTuru).HasMaxLength(100);
            });

            modelBuilder.Entity<Sipari>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.HasIndex(e => e.KullaniciId);
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
                entity.Property(e => e.OrderStatus).HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
                entity.HasOne(d => d.Kullanici)
                      .WithMany(p => p.Siparis)
                      .HasForeignKey(d => d.KullaniciId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SiparisDetay>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);
                entity.ToTable("SiparisDetay");
                entity.HasIndex(e => e.MenuItemId);
                entity.HasIndex(e => e.OrderId);
                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
                entity.Property(e => e.Fiyat).HasPrecision(10, 2);
                entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.HasOne(d => d.MenuItem)
                      .WithMany(p => p.SiparisDetays)
                      .HasForeignKey(d => d.MenuItemId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Order)
                      .WithMany(p => p.SiparisDetays)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Teslimat>(entity =>
            {
                entity.HasKey(e => e.DeliveryId);
                entity.ToTable("Teslimat");
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.PersonelId);
                entity.Property(e => e.DeliveryId).HasColumnName("DeliveryID");
                entity.Property(e => e.DeliveryStatus).HasMaxLength(50);
                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.PersonelId).HasColumnName("PersonelID");
                entity.HasOne(d => d.Order)
                      .WithMany(p => p.Teslimats)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Personel)
                      .WithMany(p => p.Teslimats)
                      .HasForeignKey(d => d.PersonelId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Yonetici>(entity =>
            {
                entity.HasKey(e => e.KullaniciId);
                entity.ToTable("Yonetici");
                entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
                entity.Property(e => e.KullaniciAdi).HasMaxLength(50);
                entity.Property(e => e.Rol).HasMaxLength(20);
                entity.Property(e => e.Sifre).HasMaxLength(100);
            });

            modelBuilder.Entity<Yorum>(entity =>
            {
                entity.HasKey(e => e.ReviewId);
                entity.ToTable("Yorum");
                entity.HasIndex(e => e.KullaniciId);
                entity.HasIndex(e => e.RestaurantId);
                entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
                entity.Property(e => e.KullaniciId).HasColumnName("KullaniciID");
                entity.Property(e => e.RestaurantId).HasColumnName("RestaurantID");
                entity.Property(e => e.Yorum1).HasColumnName("Yorum");
                entity.HasOne(d => d.Kullanici)
                      .WithMany(p => p.Yorums)
                      .HasForeignKey(d => d.KullaniciId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(d => d.Restaurant)
                      .WithMany(p => p.Yorums)
                      .HasForeignKey(d => d.RestaurantId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
