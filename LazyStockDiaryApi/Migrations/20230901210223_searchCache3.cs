using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LazyStockDiaryApi.Migrations
{
    /// <inheritdoc />
    public partial class searchCache3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SerchSymbol",
                table: "SerchSymbol");

            migrationBuilder.RenameTable(
                name: "SerchSymbol",
                newName: "SearchSymbol");

            migrationBuilder.RenameIndex(
                name: "IX_SerchSymbol_Code",
                table: "SearchSymbol",
                newName: "IX_SearchSymbol_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SearchSymbol",
                table: "SearchSymbol",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SearchSymbol",
                table: "SearchSymbol");

            migrationBuilder.RenameTable(
                name: "SearchSymbol",
                newName: "SerchSymbol");

            migrationBuilder.RenameIndex(
                name: "IX_SearchSymbol_Code",
                table: "SerchSymbol",
                newName: "IX_SerchSymbol_Code");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SerchSymbol",
                table: "SerchSymbol",
                column: "Id");
        }
    }
}
