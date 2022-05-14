using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GoodDriving.Models
{
    public partial class goodDrivingContext : DbContext
    {
        public goodDrivingContext()
        {
        }

        public goodDrivingContext(DbContextOptions<goodDrivingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Clase> Clases { get; set; } = null!;
        public virtual DbSet<EstadoClase> EstadoClases { get; set; } = null!;
        public virtual DbSet<EstadoUsuario> EstadoUsuarios { get; set; } = null!;
        public virtual DbSet<HorarioTutor> HorarioTutors { get; set; } = null!;
        public virtual DbSet<Licencium> Licencia { get; set; } = null!;
        public virtual DbSet<MarcaVehiculo> MarcaVehiculos { get; set; } = null!;
        public virtual DbSet<ModeloVehiculo> ModeloVehiculos { get; set; } = null!;
        public virtual DbSet<TipoClase> TipoClases { get; set; } = null!;
        public virtual DbSet<TipoDocumento> TipoDocumentos { get; set; } = null!;
        public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;
        public virtual DbSet<Vehiculo> Vehiculos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-CH407N4; Database=goodDriving;Trusted_Connection=true;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clase>(entity =>
            {
                entity.ToTable("clase");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.FechaFinalizacion)
                    .HasColumnType("date")
                    .HasColumnName("fechaFinalizacion");

                entity.Property(e => e.FechaSolicitud)
                    .HasColumnType("date")
                    .HasColumnName("fechaSolicitud");

                entity.Property(e => e.IdEstado).HasColumnName("idEstado");

                entity.Property(e => e.IdLicencia).HasColumnName("idLicencia");

                entity.Property(e => e.IdTipo).HasColumnName("idTipo");

                entity.Property(e => e.IdTutor).HasColumnName("idTutor");

                entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

                entity.Property(e => e.IdVehiculo).HasColumnName("idVehiculo");

                entity.Property(e => e.ReportePdf)
                    .HasColumnType("text")
                    .HasColumnName("reportePdf");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Clases)
                    .HasForeignKey(d => d.IdEstado)
                    .HasConstraintName("fk_estadoClase");

                entity.HasOne(d => d.IdLicenciaNavigation)
                    .WithMany(p => p.Clases)
                    .HasForeignKey(d => d.IdLicencia)
                    .HasConstraintName("fk_licencia");

                entity.HasOne(d => d.IdTipoNavigation)
                    .WithMany(p => p.Clases)
                    .HasForeignKey(d => d.IdTipo)
                    .HasConstraintName("fk_tipoClase");

                entity.HasOne(d => d.IdTutorNavigation)
                    .WithMany(p => p.ClaseIdTutorNavigations)
                    .HasForeignKey(d => d.IdTutor)
                    .HasConstraintName("fk_tutor");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.ClaseIdUsuarioNavigations)
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("fk_usuario");

                entity.HasOne(d => d.IdVehiculoNavigation)
                    .WithMany(p => p.Clases)
                    .HasForeignKey(d => d.IdVehiculo)
                    .HasConstraintName("fk_vehiculo");
            });

            modelBuilder.Entity<EstadoClase>(entity =>
            {
                entity.ToTable("estadoClase");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");
            });

            modelBuilder.Entity<EstadoUsuario>(entity =>
            {
                entity.ToTable("estadoUsuario");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Estado)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("estado");
            });

            modelBuilder.Entity<HorarioTutor>(entity =>
            {
                entity.ToTable("horarioTutor");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cupo).HasColumnName("cupo");

                entity.Property(e => e.Dia)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("dia");

                entity.Property(e => e.Hora)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("hora");

                entity.Property(e => e.IdTutor).HasColumnName("idTutor");

                entity.HasOne(d => d.IdTutorNavigation)
                    .WithMany(p => p.HorarioTutors)
                    .HasForeignKey(d => d.IdTutor)
                    .HasConstraintName("fk_usuarioTutor");
            });

            modelBuilder.Entity<Licencium>(entity =>
            {
                entity.ToTable("licencia");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Categoria)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("categoria");

                entity.Property(e => e.Descripcion)
                    .HasColumnType("text")
                    .HasColumnName("descripcion");
            });

            modelBuilder.Entity<MarcaVehiculo>(entity =>
            {
                entity.ToTable("marcaVehiculo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");
            });

            modelBuilder.Entity<ModeloVehiculo>(entity =>
            {
                entity.ToTable("modeloVehiculo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");
            });

            modelBuilder.Entity<TipoClase>(entity =>
            {
                entity.ToTable("tipoClase");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("descripcion");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.ToTable("tipoDocumento");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("tipo");
            });

            modelBuilder.Entity<TipoUsuario>(entity =>
            {
                entity.ToTable("tipoUsuario");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("tipo");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");

                entity.HasIndex(e => e.Email, "email")
                    .IsUnique();

                entity.HasIndex(e => e.NoDocumento, "noDocumento")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Apellido1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("apellido1");

                entity.Property(e => e.Apellido2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("apellido2");

                entity.Property(e => e.Direccion)
                    .HasColumnType("text")
                    .HasColumnName("direccion");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.FechaNacimiento)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaNacimiento");

                entity.Property(e => e.IdEstado).HasColumnName("idEstado");

                entity.Property(e => e.IdTipo).HasColumnName("idTipo");

                entity.Property(e => e.IdTipoDocumento).HasColumnName("idTipoDocumento");

                entity.Property(e => e.NoDocumento)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .HasColumnName("noDocumento");

                entity.Property(e => e.Nombre1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nombre1");

                entity.Property(e => e.Nombre2)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("nombre2");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Telefono).HasColumnName("telefono");

                entity.Property(e => e.TokenRecovery)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("token_Recovery");

                entity.HasOne(d => d.IdEstadoNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdEstado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_estadoUsuario");

                entity.HasOne(d => d.IdTipoNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdTipo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_tipoUsuario");

                entity.HasOne(d => d.IdTipoDocumentoNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdTipoDocumento)
                    .HasConstraintName("fk_tipoDocumento");
            });

            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.ToTable("vehiculo");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Cantidad).HasColumnName("cantidad");

                entity.Property(e => e.IdMarca).HasColumnName("idMarca");

                entity.Property(e => e.IdModelo).HasColumnName("idModelo");

                entity.HasOne(d => d.IdMarcaNavigation)
                    .WithMany(p => p.Vehiculos)
                    .HasForeignKey(d => d.IdMarca)
                    .HasConstraintName("fk_marcaVehiculo");

                entity.HasOne(d => d.IdModeloNavigation)
                    .WithMany(p => p.Vehiculos)
                    .HasForeignKey(d => d.IdModelo)
                    .HasConstraintName("fk_modeloVehiculo");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
