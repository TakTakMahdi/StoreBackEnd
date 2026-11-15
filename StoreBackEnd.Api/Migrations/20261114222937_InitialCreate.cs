using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreBackEnd.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategorieID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategorieID);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    UtilisateurID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MotDePasseHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.UtilisateurID);
                });

            migrationBuilder.CreateTable(
                name: "Produits",
                columns: table => new
                {
                    ProduitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Prix = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    StockMinimum = table.Column<int>(type: "int", nullable: false),
                    CategorieID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produits", x => x.ProduitID);
                    table.ForeignKey(
                        name: "FK_Produits_Categories_CategorieID",
                        column: x => x.CategorieID,
                        principalTable: "Categories",
                        principalColumn: "CategorieID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilisateurID = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Entite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntiteID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnciennesValeurs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NouvellesValeurs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogID);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Utilisateurs_UtilisateurID",
                        column: x => x.UtilisateurID,
                        principalTable: "Utilisateurs",
                        principalColumn: "UtilisateurID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Commandes",
                columns: table => new
                {
                    CommandeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: false),
                    LivreurID = table.Column<int>(type: "int", nullable: true),
                    DateCommande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    MotifRejet = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commandes", x => x.CommandeID);
                    table.ForeignKey(
                        name: "FK_Commandes_Utilisateurs_ClientID",
                        column: x => x.ClientID,
                        principalTable: "Utilisateurs",
                        principalColumn: "UtilisateurID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commandes_Utilisateurs_LivreurID",
                        column: x => x.LivreurID,
                        principalTable: "Utilisateurs",
                        principalColumn: "UtilisateurID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilisateurID = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstRevoque = table.Column<bool>(type: "bit", nullable: false),
                    TokenRemplacant = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenID);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Utilisateurs_UtilisateurID",
                        column: x => x.UtilisateurID,
                        principalTable: "Utilisateurs",
                        principalColumn: "UtilisateurID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LignesCommande",
                columns: table => new
                {
                    LigneCommandeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommandeID = table.Column<int>(type: "int", nullable: false),
                    ProduitID = table.Column<int>(type: "int", nullable: false),
                    Quantite = table.Column<int>(type: "int", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesCommande", x => x.LigneCommandeID);
                    table.ForeignKey(
                        name: "FK_LignesCommande_Commandes_CommandeID",
                        column: x => x.CommandeID,
                        principalTable: "Commandes",
                        principalColumn: "CommandeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LignesCommande_Produits_ProduitID",
                        column: x => x.ProduitID,
                        principalTable: "Produits",
                        principalColumn: "ProduitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UtilisateurID",
                table: "AuditLogs",
                column: "UtilisateurID");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_ClientID",
                table: "Commandes",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_LivreurID",
                table: "Commandes",
                column: "LivreurID");

            migrationBuilder.CreateIndex(
                name: "IX_LignesCommande_CommandeID",
                table: "LignesCommande",
                column: "CommandeID");

            migrationBuilder.CreateIndex(
                name: "IX_LignesCommande_ProduitID",
                table: "LignesCommande",
                column: "ProduitID");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_CategorieID",
                table: "Produits",
                column: "CategorieID");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UtilisateurID",
                table: "RefreshTokens",
                column: "UtilisateurID");

            migrationBuilder.CreateIndex(
                name: "IX_Utilisateurs_Email",
                table: "Utilisateurs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "LignesCommande");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Commandes");

            migrationBuilder.DropTable(
                name: "Produits");

            migrationBuilder.DropTable(
                name: "Utilisateurs");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
