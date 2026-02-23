using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanApprovalSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Loans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnConfirmedAt",
                table: "Loans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnConfirmedByUserId",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApprovedByUserId",
                table: "Loans",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ReturnConfirmedByUserId",
                table: "Loans",
                column: "ReturnConfirmedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_ApprovedByUserId",
                table: "Loans",
                column: "ApprovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_ReturnConfirmedByUserId",
                table: "Loans",
                column: "ReturnConfirmedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_ApprovedByUserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_ReturnConfirmedByUserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ApprovedByUserId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ReturnConfirmedByUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ReturnConfirmedAt",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ReturnConfirmedByUserId",
                table: "Loans");
        }
    }
}
