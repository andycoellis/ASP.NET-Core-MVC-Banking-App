using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WDT2020_a2.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(nullable: false),
                    CustomerName = table.Column<string>(maxLength: 50, nullable: false),
                    TFN = table.Column<string>(maxLength: 9, nullable: true),
                    Address = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 40, nullable: true),
                    State = table.Column<string>(maxLength: 3, nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerID);
                    table.CheckConstraint("CH_Customer_CustomerID", "len(CustomerID) = 4");
                    table.CheckConstraint("CH_Customer_PostCode", "len(PostCode) = 4");
                });

            migrationBuilder.CreateTable(
                name: "Payees",
                columns: table => new
                {
                    PayeeID = table.Column<int>(nullable: false),
                    PayeeName = table.Column<string>(maxLength: 50, nullable: false),
                    Address = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 40, nullable: true),
                    State = table.Column<string>(maxLength: 20, nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payees", x => x.PayeeID);
                    table.CheckConstraint("CH_Payee_PayeeID", "len(PayeeID) = 4");
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountNumber = table.Column<int>(nullable: false),
                    AccountType = table.Column<string>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    ModifyDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountNumber);
                    table.CheckConstraint("CH_Account_AccountNumber", "len(AccountNumber) = 4");
                    table.CheckConstraint("CH_Account_AccountType", "AccountType in ('C', 'S')");
                    table.ForeignKey(
                        name: "FK_Accounts_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    UserID = table.Column<string>(maxLength: 8, nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    ModifyDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.UserID);
                    table.CheckConstraint("CH_Login_CustomerID", "len(CustomerID) = 4");
                    table.CheckConstraint("CH_Login_Password", "len(Password) = 64");
                    table.ForeignKey(
                        name: "FK_Logins_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillPays",
                columns: table => new
                {
                    BillPayID = table.Column<int>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: false),
                    PayeeID = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    ScheduleDate = table.Column<DateTime>(nullable: false),
                    Period = table.Column<string>(nullable: false),
                    ModifyDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPays", x => x.BillPayID);
                    table.CheckConstraint("CH_BillPay_BillPayID", "len(BillPayID) = 4");
                    table.CheckConstraint("CH_BillPay_Period", "Period in ('M', 'Q', 'A', 'S')");
                    table.CheckConstraint("CH_BillPay_Amount", "Amount > 0");
                    table.ForeignKey(
                        name: "FK_BillPays_Accounts_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillPays_Payees_PayeeID",
                        column: x => x.PayeeID,
                        principalTable: "Payees",
                        principalColumn: "PayeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<int>(nullable: false),
                    TransactionType = table.Column<string>(nullable: false),
                    AccountNumber = table.Column<int>(nullable: false),
                    DestAccount = table.Column<int>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Comment = table.Column<string>(maxLength: 255, nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                    table.CheckConstraint("CH_Transaction_TransactionID", "len(TransactionID) = 4");
                    table.CheckConstraint("CH_Transaction_TransactionType", "TransactionType in ('D', 'W', 'T', 'S', 'B')");
                    table.CheckConstraint("CH_Transaction_Amount", "Amount > 0");
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountNumber",
                        column: x => x.AccountNumber,
                        principalTable: "Accounts",
                        principalColumn: "AccountNumber",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CustomerID",
                table: "Accounts",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_BillPays_AccountNumber",
                table: "BillPays",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_BillPays_PayeeID",
                table: "BillPays",
                column: "PayeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_CustomerID",
                table: "Logins",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountNumber",
                table: "Transactions",
                column: "AccountNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillPays");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Payees");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
