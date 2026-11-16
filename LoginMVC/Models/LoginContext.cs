using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Models;

public partial class LoginContext : DbContext
{
    public LoginContext()
    {
    }

    public LoginContext(DbContextOptions<LoginContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-ODBD2U2\\SQLEXPRESS;Database=DB_LOGIN_PROYECT;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__5B65BF97791D52EC");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A1903DAF990").IsUnique();

            entity.HasIndex(e => e.Nickname, "UQ__Usuarios__CC6CD17EB6DD4C22").IsUnique();

            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
