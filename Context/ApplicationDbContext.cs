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
                entity.HasOne(e => e.CreadoPorUsuario)//una categoría es creada por un usuario
                    .WithMany()
                    .HasForeignKey(e => e.CreadoPorUsuarioId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                // Índices
                entity.HasIndex(e => e.Nombre).IsUnique();// Índice único en el nombre de la categoría
                entity.HasIndex(e => e.CreadoPorUsuarioId);
            });

            //Configuracion de Ingrediente
            builder.Entity<Ingrediente>(entity =>
            {
                entity.HasKey(e => e.Id);//aqui se define la clave primaria de la tabla Ingredientes

                entity.Property(e => e.Nombre)//aqui se define la propiedad Nombre del modelo Ingrediente
                    .IsRequired()//aqui se indica que el campo Nombre es obligatorio
                    .HasMaxLength(100);//aqui se establece una longitud máxima de 100 caracteres para el campo Nombre

                entity.Property(e => e.UnidadMedida)//aqui se define la propiedad UnidadMedida del modelo Ingrediente
                    .IsRequired()
                    .HasMaxLength(15);


                entity.Property(e=>e.Descripcion)//aqui se define la propiedad Descripcion del modelo Ingrediente
                    .HasMaxLength(100)//aqui se establece una longitud máxima de 100 caracteres para el campo Descripcion
                    .IsRequired(false);

                // Relación con ApplicationUser
                entity.HasOne(e => e.CreadoPorUsuario)//un ingrediente es creado por un usuario
                    .WithMany()//usuario puede crear muchos ingredientes
                    .HasForeignKey(e => e.CreadoPorUsuarioId)//la clave foránea es CreadoPorUsuarioId
                    .OnDelete(DeleteBehavior.SetNull)//si el usuario que creó el ingrediente es eliminado, se establece el valor de CreadoPorUsuarioId a null
                    .IsRequired(false);//el campo CreadoPorUsuarioId no es obligatorio

                //Indices
                entity.HasIndex(e => e.Nombre).IsUnique();// Índice único en el nombre del ingrediente
                    entity.HasIndex(e => e.CreadoPorUsuarioId);// Índice en el campo CreadoPorUsuarioId para mejorar el rendimiento de las consultas que filtran por este campo

            });

            }


    }
}
