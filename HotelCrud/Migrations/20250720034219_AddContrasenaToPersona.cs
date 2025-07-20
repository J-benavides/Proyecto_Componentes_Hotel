using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelCrud.Migrations
{
    /// <inheritdoc />
    public partial class AddContrasenaToPersona : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Habitaci__3214EC07470534DA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Contrasena = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Personas__3214EC070067F93E", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__3214EC0759E20D9B", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    IdHabitacion = table.Column<int>(type: "int", nullable: false),
                    FechaEntrada = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaSalida = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reservas__3214EC074A615DA4", x => x.Id);
                    table.ForeignKey(
                        name: "FK__Reservas__IdHabi__5441852A",
                        column: x => x.IdHabitacion,
                        principalTable: "Habitaciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__Reservas__IdPers__534D60F1",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PersonaRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPersona = table.Column<int>(type: "int", nullable: false),
                    IdRol = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PersonaR__3214EC0765C00352", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PersonaRo__IdPer__4D94879B",
                        column: x => x.IdPersona,
                        principalTable: "Personas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK__PersonaRo__IdRol__4E88ABD4",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonaRol_IdPersona",
                table: "PersonaRol",
                column: "IdPersona");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaRol_IdRol",
                table: "PersonaRol",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdHabitacion",
                table: "Reservas",
                column: "IdHabitacion");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdPersona",
                table: "Reservas",
                column: "IdPersona");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonaRol");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Habitaciones");

            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
