using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatServer.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountDbId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountLoginId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AccountUniqueId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountDbId);
                });

            migrationBuilder.CreateTable(
                name: "ChatRoom",
                columns: table => new
                {
                    ChatRoomDbId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatRoomNumber = table.Column<long>(type: "bigint", nullable: false),
                    ChatRoomName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoom", x => x.ChatRoomDbId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserDbId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AccountDbId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserDbId);
                    table.ForeignKey(
                        name: "FK_User_Account_AccountDbId",
                        column: x => x.AccountDbId,
                        principalTable: "Account",
                        principalColumn: "AccountDbId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    ChatDbId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatType = table.Column<int>(type: "int", nullable: false),
                    IconId = table.Column<long>(type: "bigint", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderDbId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RoomDbId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.ChatDbId);
                    table.ForeignKey(
                        name: "FK_Chat_ChatRoom_RoomDbId",
                        column: x => x.RoomDbId,
                        principalTable: "ChatRoom",
                        principalColumn: "ChatRoomDbId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chat_User_SenderDbId",
                        column: x => x.SenderDbId,
                        principalTable: "User",
                        principalColumn: "UserDbId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatRoomDbUserDb",
                columns: table => new
                {
                    RoomsChatRoomDbId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    UsersUserDbId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomDbUserDb", x => new { x.RoomsChatRoomDbId, x.UsersUserDbId });
                    table.ForeignKey(
                        name: "FK_ChatRoomDbUserDb_ChatRoom_RoomsChatRoomDbId",
                        column: x => x.RoomsChatRoomDbId,
                        principalTable: "ChatRoom",
                        principalColumn: "ChatRoomDbId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatRoomDbUserDb_User_UsersUserDbId",
                        column: x => x.UsersUserDbId,
                        principalTable: "User",
                        principalColumn: "UserDbId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccountLoginId",
                table: "Account",
                column: "AccountLoginId",
                unique: true,
                filter: "[AccountLoginId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_RoomDbId",
                table: "Chat",
                column: "RoomDbId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_SenderDbId",
                table: "Chat",
                column: "SenderDbId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoomDbUserDb_UsersUserDbId",
                table: "ChatRoomDbUserDb",
                column: "UsersUserDbId");

            migrationBuilder.CreateIndex(
                name: "IX_User_AccountDbId",
                table: "User",
                column: "AccountDbId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                table: "User",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "ChatRoomDbUserDb");

            migrationBuilder.DropTable(
                name: "ChatRoom");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
