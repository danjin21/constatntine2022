using Microsoft.EntityFrameworkCore.Migrations;

namespace Server.Migrations
{
    public partial class @new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountDbId);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    PlayerDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AccountDbId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<float>(type: "real", nullable: false),
                    TotalExp = table.Column<int>(type: "int", nullable: false),
                    Map = table.Column<int>(type: "int", nullable: false),
                    PosX = table.Column<int>(type: "int", nullable: false),
                    PosY = table.Column<int>(type: "int", nullable: false),
                    Hair = table.Column<int>(type: "int", nullable: false),
                    HairColor = table.Column<int>(type: "int", nullable: false),
                    Face = table.Column<int>(type: "int", nullable: false),
                    Skin = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Mp = table.Column<int>(type: "int", nullable: false),
                    MaxMp = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false),
                    Gold = table.Column<int>(type: "int", nullable: false),
                    Str = table.Column<int>(type: "int", nullable: false),
                    Dex = table.Column<int>(type: "int", nullable: false),
                    Int = table.Column<int>(type: "int", nullable: false),
                    Luk = table.Column<int>(type: "int", nullable: false),
                    Def = table.Column<int>(type: "int", nullable: false),
                    StatPoint = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.PlayerDbId);
                    table.ForeignKey(
                        name: "FK_Player_Account_AccountDbId",
                        column: x => x.AccountDbId,
                        principalTable: "Account",
                        principalColumn: "AccountDbId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    Equipped = table.Column<bool>(type: "bit", nullable: false),
                    ReqStr = table.Column<int>(type: "int", nullable: false),
                    ReqDex = table.Column<int>(type: "int", nullable: false),
                    ReqInt = table.Column<int>(type: "int", nullable: false),
                    ReqLuk = table.Column<int>(type: "int", nullable: false),
                    ReqLev = table.Column<int>(type: "int", nullable: false),
                    ReqPop = table.Column<int>(type: "int", nullable: false),
                    UpgradeSlot = table.Column<int>(type: "int", nullable: false),
                    Str = table.Column<int>(type: "int", nullable: false),
                    Dex = table.Column<int>(type: "int", nullable: false),
                    Int = table.Column<int>(type: "int", nullable: false),
                    Luk = table.Column<int>(type: "int", nullable: false),
                    Hp = table.Column<int>(type: "int", nullable: false),
                    Mp = table.Column<int>(type: "int", nullable: false),
                    WAtk = table.Column<int>(type: "int", nullable: false),
                    MAtk = table.Column<int>(type: "int", nullable: false),
                    WDef = table.Column<int>(type: "int", nullable: false),
                    MDef = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    AtkSpeed = table.Column<int>(type: "int", nullable: false),
                    Durability = table.Column<int>(type: "int", nullable: false),
                    Enhance = table.Column<int>(type: "int", nullable: false),
                    WPnt = table.Column<int>(type: "int", nullable: false),
                    MPnt = table.Column<int>(type: "int", nullable: false),
                    OwnerDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemDbId);
                    table.ForeignKey(
                        name: "FK_Item_Player_OwnerDbId",
                        column: x => x.OwnerDbId,
                        principalTable: "Player",
                        principalColumn: "PlayerDbId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KeySetting",
                columns: table => new
                {
                    KeySettingDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    key = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<int>(type: "int", nullable: false),
                    OwnerDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeySetting", x => x.KeySettingDbId);
                    table.ForeignKey(
                        name: "FK_KeySetting_Player_OwnerDbId",
                        column: x => x.OwnerDbId,
                        principalTable: "Player",
                        principalColumn: "PlayerDbId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestStatus",
                columns: table => new
                {
                    QuestDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestDbTemplateId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OwnerDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestStatus", x => x.QuestDbId);
                    table.ForeignKey(
                        name: "FK_QuestStatus_Player_OwnerDbId",
                        column: x => x.OwnerDbId,
                        principalTable: "Player",
                        principalColumn: "PlayerDbId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    SkillDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillTemplateId = table.Column<int>(type: "int", nullable: false),
                    SkillLevel = table.Column<int>(type: "int", nullable: false),
                    Slot = table.Column<int>(type: "int", nullable: false),
                    OwnerDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.SkillDbId);
                    table.ForeignKey(
                        name: "FK_Skill_Player_OwnerDbId",
                        column: x => x.OwnerDbId,
                        principalTable: "Player",
                        principalColumn: "PlayerDbId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestMobCount",
                columns: table => new
                {
                    QuestMobCountDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MobId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    QuestDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestMobCount", x => x.QuestMobCountDbId);
                    table.ForeignKey(
                        name: "FK_QuestMobCount_QuestStatus_QuestDbId",
                        column: x => x.QuestDbId,
                        principalTable: "QuestStatus",
                        principalColumn: "QuestDbId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccountName",
                table: "Account",
                column: "AccountName",
                unique: true,
                filter: "[AccountName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Item_OwnerDbId",
                table: "Item",
                column: "OwnerDbId");

            migrationBuilder.CreateIndex(
                name: "IX_KeySetting_OwnerDbId",
                table: "KeySetting",
                column: "OwnerDbId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_AccountDbId",
                table: "Player",
                column: "AccountDbId");

            migrationBuilder.CreateIndex(
                name: "IX_Player_PlayerName",
                table: "Player",
                column: "PlayerName",
                unique: true,
                filter: "[PlayerName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_QuestMobCount_QuestDbId",
                table: "QuestMobCount",
                column: "QuestDbId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestStatus_OwnerDbId",
                table: "QuestStatus",
                column: "OwnerDbId");

            migrationBuilder.CreateIndex(
                name: "IX_Skill_OwnerDbId",
                table: "Skill",
                column: "OwnerDbId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "KeySetting");

            migrationBuilder.DropTable(
                name: "QuestMobCount");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "QuestStatus");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
