using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherEmergencyAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    PASSWORD_HASH = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IS_ACTIVE = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EMERGENCY_CONTACTS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PHONE_NUMBER = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    COUNTRY_CODE = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: false),
                    RELATIONSHIP = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    IS_PRIMARY = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMERGENCY_CONTACTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EMERGENCY_CONTACTS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LOCATIONS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    LATITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    LONGITUDE = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    CITY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    STATE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    COUNTRY = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    IS_FAVORITE = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    UPDATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCATIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LOCATIONS_USERS_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EMERGENCY_CONTACTS_USER_ID",
                table: "EMERGENCY_CONTACTS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_LOCATIONS_USER_ID",
                table: "LOCATIONS",
                column: "USER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_EMAIL",
                table: "USERS",
                column: "EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EMERGENCY_CONTACTS");

            migrationBuilder.DropTable(
                name: "LOCATIONS");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
