using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fortuity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PolicyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IncidentDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ReportedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ReserveAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    AssignedRepairShopId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StreetAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    PricingBasis = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Coverage = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    StartsOn = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndsOn = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AnnualPremium = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    PolicyType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Coverage = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    VehicleOrPropertyAgeYears = table.Column<int>(type: "INTEGER", nullable: false),
                    NoClaimsYears = table.Column<int>(type: "INTEGER", nullable: false),
                    BasisUsed = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    BasePremium = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RiskMultiplier = table.Column<decimal>(type: "TEXT", precision: 9, scale: 4, nullable: false),
                    CoverageMultiplier = table.Column<decimal>(type: "TEXT", precision: 9, scale: 4, nullable: false),
                    NoClaimsMultiplier = table.Column<decimal>(type: "TEXT", precision: 9, scale: 4, nullable: false),
                    RegionalMultiplier = table.Column<decimal>(type: "TEXT", precision: 9, scale: 4, nullable: false),
                    FinalPremium = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegionalFactors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    PolicyType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Basis = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Multiplier = table.Column<decimal>(type: "TEXT", precision: 9, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionalFactors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairShops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartnerReference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IsAcceptingWork = table.Column<bool>(type: "INTEGER", nullable: false),
                    SyncState = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LastSyncedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LastSyncError = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairShops", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_Number",
                table: "Claims",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PolicyId",
                table: "Claims",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerNumber",
                table: "Customers",
                column: "CustomerNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Policies_CustomerId",
                table: "Policies",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_Number",
                table: "Policies",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_CustomerId",
                table: "Quotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionalFactors_Region_PolicyType_Basis_EffectiveFrom",
                table: "RegionalFactors",
                columns: new[] { "Region", "PolicyType", "Basis", "EffectiveFrom" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RepairShops_PartnerReference",
                table: "RepairShops",
                column: "PartnerReference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "RegionalFactors");

            migrationBuilder.DropTable(
                name: "RepairShops");
        }
    }
}
