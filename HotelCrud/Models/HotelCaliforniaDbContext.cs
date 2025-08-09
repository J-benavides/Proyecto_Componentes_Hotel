using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HotelCrud.Models;

namespace HotelCrud.Models;

public partial class HotelCaliforniaDbContext : DbContext
{
    public HotelCaliforniaDbContext()
    {
    }

    public HotelCaliforniaDbContext(DbContextOptions<HotelCaliforniaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Habitacione> Habitaciones { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<PersonaRol> PersonaRols { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=JOSHUADESKTOP\\SQLEXPRESS02; Database=HotelCaliforniaDB; Integrated Security=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Habitacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Habitaci__3214EC07470534DA");

            entity.Property(e => e.Estado).HasMaxLength(20);
            entity.Property(e => e.Numero).HasMaxLength(10);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Tipo).HasMaxLength(30);
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personas__3214EC070067F93E");

            entity.Property(e => e.Apellidos).HasMaxLength(50);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<PersonaRol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PersonaR__3214EC0765C00352");

            entity.ToTable("PersonaRol");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.PersonaRols)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PersonaRo__IdPer__4D94879B");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.PersonaRols)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PersonaRo__IdRol__4E88ABD4");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservas__3214EC074A615DA4");

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdHabitacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservas__IdHabi__5441852A");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservas__IdPers__534D60F1");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0759E20D9B");

            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<HotelCrud.Models.Pagos> Pagos { get; set; } = default!;
}
