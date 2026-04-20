using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecetArreAPI2.Models;

namespace RecetArreAPI2.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<Receta> Recetas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de Categoria
            builder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500)
                    .IsRequired(false);

                entity.Property(e => e.CreadoUtc)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con ApplicationUser
                entity.HasOne(e => e.CreadoPorUsuario)
                    .WithMany()
                    .HasForeignKey(e => e.CreadoPorUsuarioId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                // Índices
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.HasIndex(e => e.CreadoPorUsuarioId);
            });

            // Configuración de Ingrediente
            builder.Entity<Ingrediente>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(80);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.CreadoUtc)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Índice único para evitar nombres duplicados
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de Receta
            builder.Entity<Receta>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo)
                    .IsRequired()
                    .HasMaxLength(120);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(1000)
                    .IsRequired(false);

                entity.Property(e => e.Instrucciones)
                    .IsRequired()
                    .HasMaxLength(15000);

                entity.Property(e => e.Porciones)
                    .HasDefaultValue(1);

                entity.Property(e => e.EstaPublicado)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreadoUtc)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ModificadoUtc)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Autor)
                    .WithMany(u => u.RecetasPublicadas)
                    .HasForeignKey(e => e.AutorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasMany(e => e.Categorias)
                    .WithMany(c => c.Recetas)
                    .UsingEntity(j => j.ToTable("RecetaCategorias"));

                entity.HasMany(e => e.Ingredientes)
                    .WithMany(i => i.Recetas)
                    .UsingEntity(j => j.ToTable("RecetaIngredientes"));

                entity.HasIndex(e => e.AutorId);
                entity.HasIndex(e => e.CreadoUtc);
                entity.HasIndex(e => e.EstaPublicado);
            });

            // Configuración de Comentario
            builder.Entity<Comentario>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Contenido)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.CreadoUtc)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Receta)
                    .WithMany(r => r.Comentarios)
                    .HasForeignKey(e => e.RecetaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Comentarios)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                entity.HasIndex(e => e.RecetaId);
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => e.CreadoUtc);
            });

            // Configuración de Rating
            builder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Calificacion)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relación con Receta
                entity.HasOne(e => e.Receta)
                    .WithMany(r => r.Ratings)
                    .HasForeignKey(e => e.RecetaId)
                    .OnDelete(DeleteBehavior.Cascade); // Si se borra la receta, se borran sus ratings

                // Relación con Usuario
                entity.HasOne(e => e.Usuario)
                    .WithMany() // Un usuario puede tener muchos ratings
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Evita que un usuario califique la misma receta más de una vez
                entity.HasIndex(e => new { e.UsuarioId, e.RecetaId }).IsUnique();
            });


        }
    }
}