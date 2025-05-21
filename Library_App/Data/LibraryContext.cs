using System.IO;
using Library_App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library_App.Data;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookDistribution> BookDistributions { get; set; }

    public virtual DbSet<BookDistributionBook> BookDistributionBooks { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingBook> BookingBooks { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventExhibit> EventExhibits { get; set; }

    public virtual DbSet<Exhibit> Exhibits { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PriceList> PriceLists { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DB");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.IdBook).HasName("PK_Books");

            entity.ToTable("Book");

            entity.Property(e => e.IdBook).HasColumnName("ID_Book");
            entity.Property(e => e.AgeRestrictions).HasColumnName("Age_restrictions");
            entity.Property(e => e.AuthorBook)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Author_book");
            entity.Property(e => e.CopiesNumber)
                .HasDefaultValue(1)
                .HasColumnName("Copies_number");
            entity.Property(e => e.Genre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PermissionToIssuance).HasColumnName("Permission_to_issuance");
            entity.Property(e => e.Publishing)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TitleBook)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Title_book");
            entity.Property(e => e.YearOfPublication).HasColumnName("Year_of_publication");
        });

        modelBuilder.Entity<BookDistribution>(entity =>
        {
            entity.HasKey(e => e.IdBookDistribution);

            entity.ToTable("Book_distribution");

            entity.Property(e => e.IdBookDistribution).HasColumnName("ID_Book_distribution");
            entity.Property(e => e.IssuanceDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Issuance_date");
            entity.Property(e => e.ReaderTicket).HasColumnName("Reader_ticket");
            entity.Property(e => e.ReturnDate).HasColumnName("Return_date");

            entity.HasOne(d => d.ReaderTicketNavigation).WithMany(p => p.BookDistributions)
                .HasForeignKey(d => d.ReaderTicket)
                .HasConstraintName("FK_Book_distribution_Reader");
        });

        modelBuilder.Entity<BookDistributionBook>(entity =>
        {
            entity.HasKey(e => e.IdBookDistributionBook).HasName("PK_Book_distribution_Books");

            entity.ToTable("Book_distribution_Book");

            entity.Property(e => e.IdBookDistributionBook).HasColumnName("ID_Book_distribution_Book");
            entity.Property(e => e.IdBook).HasColumnName("ID_Book");
            entity.Property(e => e.IdBookDistribution).HasColumnName("ID_Book_distribution");

            entity.HasOne(d => d.IdBookNavigation).WithMany(p => p.BookDistributionBooks)
                .HasForeignKey(d => d.IdBook)
                .HasConstraintName("FK_Book_distribution_Book_Book1");

            entity.HasOne(d => d.IdBookDistributionNavigation).WithMany(p => p.BookDistributionBooks)
                .HasForeignKey(d => d.IdBookDistribution)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Book_distribution_Book_Book_distribution");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.IdBooking);

            entity.ToTable("Booking");

            entity.Property(e => e.IdBooking).HasColumnName("ID_Booking");
            entity.Property(e => e.BookingDate).HasColumnName("Booking_date");
            entity.Property(e => e.ReaderTicket).HasColumnName("Reader_ticket");

            entity.HasOne(d => d.ReaderTicketNavigation).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ReaderTicket)
                .HasConstraintName("FK_Booking_Reader");
        });

        modelBuilder.Entity<BookingBook>(entity =>
        {
            entity.HasKey(e => e.IdBookingBook).HasName("PK_Booking_Books");

            entity.ToTable("Booking_Book");

            entity.Property(e => e.IdBookingBook).HasColumnName("ID_Booking_Book");
            entity.Property(e => e.IdBook).HasColumnName("ID_Book");
            entity.Property(e => e.IdBooking).HasColumnName("ID_Booking");

            entity.HasOne(d => d.IdBookNavigation).WithMany(p => p.BookingBooks)
                .HasForeignKey(d => d.IdBook)
                .HasConstraintName("FK_Booking_Book_Book1");

            entity.HasOne(d => d.IdBookingNavigation).WithMany(p => p.BookingBooks)
                .HasForeignKey(d => d.IdBooking)
                .HasConstraintName("FK_Booking_Book_Booking");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("PK_Events");

            entity.ToTable("Event");

            entity.Property(e => e.IdEvent).HasColumnName("ID_Event");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.EventDate)
                .HasColumnType("datetime")
                .HasColumnName("Event_date");
            entity.Property(e => e.EventLocation)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Event_location");
            entity.Property(e => e.EventName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Event_name");
            entity.Property(e => e.EventType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Event_type");
        });

        modelBuilder.Entity<EventExhibit>(entity =>
        {
            entity.HasKey(e => e.IdEventExhibit).HasName("PK_Events_Exhibits");

            entity.ToTable("Event_Exhibit");

            entity.Property(e => e.IdEventExhibit).HasColumnName("ID_Event_Exhibit");
            entity.Property(e => e.IdEvent).HasColumnName("ID_Event");
            entity.Property(e => e.IdExhibit).HasColumnName("ID_Exhibit");

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.EventExhibits)
                .HasForeignKey(d => d.IdEvent)
                .HasConstraintName("FK_Event_Exhibit_Event");

            entity.HasOne(d => d.IdExhibitNavigation).WithMany(p => p.EventExhibits)
                .HasForeignKey(d => d.IdExhibit)
                .HasConstraintName("FK_Event_Exhibit_Exhibit");
        });

        modelBuilder.Entity<Exhibit>(entity =>
        {
            entity.HasKey(e => e.IdExhibit).HasName("PK_Exhibits");

            entity.ToTable("Exhibit");

            entity.Property(e => e.IdExhibit).HasColumnName("ID_Exhibit");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("неизвестен");
            entity.Property(e => e.CreationDate).HasColumnName("Creation_date");
            entity.Property(e => e.Subject)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.IdPayment);

            entity.ToTable("Payment");

            entity.Property(e => e.IdPayment).HasColumnName("ID_Payment");
            entity.Property(e => e.Cost).HasColumnType("money");
            entity.Property(e => e.NameService)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Name_service");
            entity.Property(e => e.PaymentDate).HasColumnName("Payment_date");
            entity.Property(e => e.ReaderTicket).HasColumnName("Reader_ticket");

            entity.HasOne(d => d.NameServiceNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.NameService)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payment_Price_list");

            entity.HasOne(d => d.ReaderTicketNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReaderTicket)
                .HasConstraintName("FK_Payment_Reader");
        });

        modelBuilder.Entity<PriceList>(entity =>
        {
            entity.HasKey(e => e.NameService);

            entity.ToTable("Price_list");

            entity.Property(e => e.NameService)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Name_service");
            entity.Property(e => e.Price).HasColumnType("money");
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.HasKey(e => e.ReaderTicket).HasName("PK_Readers");

            entity.ToTable("Reader");

            entity.Property(e => e.ReaderTicket).HasColumnName("Reader_ticket");
            entity.Property(e => e.BirthDate).HasColumnName("Birth_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fine).HasColumnType("money");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("First_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Last_name");
            entity.Property(e => e.Telephone)
                .HasMaxLength(11)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole);

            entity.ToTable("Role");

            entity.HasIndex(e => e.NameRole, "IX_Role").IsUnique();

            entity.Property(e => e.IdRole).HasColumnName("ID_Role");
            entity.Property(e => e.NameRole)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Name_role");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.LoginUser).HasName("PK_Users");

            entity.ToTable("User");

            entity.HasIndex(e => e.PasswordUser, "IX_Users").IsUnique();

            entity.Property(e => e.LoginUser)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Login_user");
            entity.Property(e => e.IdRole).HasColumnName("ID_Role");
            entity.Property(e => e.NameUser)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Name_user");
            entity.Property(e => e.PasswordUser)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Password_user");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
