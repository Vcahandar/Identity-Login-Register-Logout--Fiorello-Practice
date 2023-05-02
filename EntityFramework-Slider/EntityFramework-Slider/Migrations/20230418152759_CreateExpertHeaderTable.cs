using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework_Slider.Migrations
{
    public partial class CreateExpertHeaderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpertHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoftDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertHeaders", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ExpertHeaders",
                columns: new[] { "Id", "Description", "SoftDelete", "Title" },
                values: new object[] { 1, "A perfect blend of creativity, energy, communication, happiness and love. Let us arrange a smile for you.", false, "Flower Experts" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpertHeaders");
        }
    }
}
