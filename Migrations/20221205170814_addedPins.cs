using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QualificationSite.Migrations
{
    /// <inheritdoc />
    public partial class addedPins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pins",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    Header = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LinkToAttachment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pins", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pins");
        }
    }
}
