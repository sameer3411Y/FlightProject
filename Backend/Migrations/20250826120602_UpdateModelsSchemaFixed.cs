using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsSchemaFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: only drop if the table exists (may have been removed manually)
            migrationBuilder.Sql(@"IF OBJECT_ID(N'[Cities]', 'U') IS NOT NULL DROP TABLE [Cities];");

            migrationBuilder.Sql(@"IF OBJECT_ID(N'[PasswordResets]', 'U') IS NOT NULL DROP TABLE [PasswordResets];");

            // Drop index only if it exists
            migrationBuilder.Sql(@"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Email' AND object_id = OBJECT_ID(N'[Users]'))
                DROP INDEX [IX_Users_Email] ON [Users];");

            // Drop columns only if they exist
            migrationBuilder.Sql(@"IF COL_LENGTH(N'[Users]', N'DateofBirth') IS NOT NULL
            BEGIN
                DECLARE @var0 sysname;
                SELECT @var0 = [d].[name] FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'DateofBirth');
                IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var0 + '];');
                ALTER TABLE [Users] DROP COLUMN [DateofBirth];
            END");

            migrationBuilder.Sql(@"IF COL_LENGTH(N'[Bookings]', N'CancelledAt') IS NOT NULL
            BEGIN
                DECLARE @var1 sysname;
                SELECT @var1 = [d].[name] FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bookings]') AND [c].[name] = N'CancelledAt');
                IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Bookings] DROP CONSTRAINT [' + @var1 + '];');
                ALTER TABLE [Bookings] DROP COLUMN [CancelledAt];
            END");

            migrationBuilder.Sql(@"IF COL_LENGTH(N'[Bookings]', N'CheckIn') IS NOT NULL
            BEGIN
                DECLARE @var2 sysname;
                SELECT @var2 = [d].[name] FROM [sys].[default_constraints] [d]
                INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
                WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bookings]') AND [c].[name] = N'CheckIn');
                IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Bookings] DROP CONSTRAINT [' + @var2 + '];');
                ALTER TABLE [Bookings] DROP COLUMN [CheckIn];
            END");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "BusinessPrice",
                table: "Flights",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateofBirth",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Passengers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "BusinessPrice",
                table: "Flights",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CheckIn",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Airport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }
    }
}
