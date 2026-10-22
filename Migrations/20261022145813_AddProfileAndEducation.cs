using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeginnerMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileAndEducation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Education",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Qualification = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Institution = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Dates = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Education", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Initials = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    ProfessionalTitle = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Introduction = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    AboutMore = table.Column<string>(type: "TEXT", maxLength: 3000, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    PhoneLink = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    ContactLocation = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 160, nullable: true),
                    Availability = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    CvUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteProfiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Education");

            migrationBuilder.DropTable(
                name: "SiteProfiles");
        }
    }
}
