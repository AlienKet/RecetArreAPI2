using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RecetArreAPI2.Migrations
{
    /// <inheritdoc />
    public partial class TRecetas_TComentarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredientes_AspNetUsers_CreadoPorUsuarioId",
                table: "Ingredientes");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ingredientes",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "Recetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Instrucciones = table.Column<string>(type: "character varying(15000)", maxLength: 15000, nullable: false),
                    TiempoPreparacionMinutos = table.Column<int>(type: "integer", nullable: false),
                    TiempoCoccionMinutos = table.Column<int>(type: "integer", nullable: false),
                    Porciones = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    EstaPublicado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreadoUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ModificadoUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    AutorId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recetas_AspNetUsers_AutorId",
                        column: x => x.AutorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Contenido = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RecetaId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentarios_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecetaCategorias",
                columns: table => new
                {
                    CategoriasId = table.Column<int>(type: "integer", nullable: false),
                    RecetasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecetaCategorias", x => new { x.CategoriasId, x.RecetasId });
                    table.ForeignKey(
                        name: "FK_RecetaCategorias_Categorias_CategoriasId",
                        column: x => x.CategoriasId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecetaCategorias_Recetas_RecetasId",
                        column: x => x.RecetasId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecetaIngredientes",
                columns: table => new
                {
                    IngredientesId = table.Column<int>(type: "integer", nullable: false),
                    RecetasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecetaIngredientes", x => new { x.IngredientesId, x.RecetasId });
                    table.ForeignKey(
                        name: "FK_RecetaIngredientes_Ingredientes_IngredientesId",
                        column: x => x.IngredientesId,
                        principalTable: "Ingredientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecetaIngredientes_Recetas_RecetasId",
                        column: x => x.RecetasId,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_CreadoUtc",
                table: "Comentarios",
                column: "CreadoUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_RecetaId",
                table: "Comentarios",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaCategorias_RecetasId",
                table: "RecetaCategorias",
                column: "RecetasId");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaIngredientes_RecetasId",
                table: "RecetaIngredientes",
                column: "RecetasId");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_AutorId",
                table: "Recetas",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_CreadoUtc",
                table: "Recetas",
                column: "CreadoUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_EstaPublicado",
                table: "Recetas",
                column: "EstaPublicado");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredientes_AspNetUsers_CreadoPorUsuarioId",
                table: "Ingredientes",
                column: "CreadoPorUsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredientes_AspNetUsers_CreadoPorUsuarioId",
                table: "Ingredientes");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "RecetaCategorias");

            migrationBuilder.DropTable(
                name: "RecetaIngredientes");

            migrationBuilder.DropTable(
                name: "Recetas");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Ingredientes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredientes_AspNetUsers_CreadoPorUsuarioId",
                table: "Ingredientes",
                column: "CreadoPorUsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
