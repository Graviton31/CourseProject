using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectronicJournalApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    id_student = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    surname = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    patronymic = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parent_phone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_student);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    id_subject = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    full_name = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    duration = table.Column<sbyte>(type: "tinyint", nullable: false),
                    lesson_length = table.Column<sbyte>(type: "tinyint", nullable: false),
                    lessons_count = table.Column<sbyte>(type: "tinyint", nullable: false),
                    is_delete = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_subject);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "unvisited_statuses",
                columns: table => new
                {
                    id_unvisited_status = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    short_name = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_unvisited_status);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_users = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    surname = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    patronymic = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    login = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: false),
                    phone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    role = table.Column<string>(type: "enum('администратор','руководитель','учитель')", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_delete = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_users);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id_group = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    student_count = table.Column<sbyte>(type: "tinyint", nullable: true),
                    classroom = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_users = table.Column<int>(type: "int", nullable: false),
                    id_subject = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_group);
                    table.ForeignKey(
                        name: "fk_groups_subjects1",
                        column: x => x.id_subject,
                        principalTable: "subjects",
                        principalColumn: "id_subject");
                    table.ForeignKey(
                        name: "fk_groups_users",
                        column: x => x.id_users,
                        principalTable: "users",
                        principalColumn: "id_users");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "subjects_has_users",
                columns: table => new
                {
                    id_subject = table.Column<int>(type: "int", nullable: false),
                    id_users = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id_subject, x.id_users })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_subjects_has_users_subjects1",
                        column: x => x.id_subject,
                        principalTable: "subjects",
                        principalColumn: "id_subject");
                    table.ForeignKey(
                        name: "fk_subjects_has_users_users1",
                        column: x => x.id_users,
                        principalTable: "users",
                        principalColumn: "id_users");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "journals",
                columns: table => new
                {
                    id_journal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    lesson_date = table.Column<DateOnly>(type: "date", nullable: true),
                    id_group = table.Column<int>(type: "int", nullable: false),
                    id_student = table.Column<int>(type: "int", nullable: false),
                    id_unvisited_status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_journal);
                    table.ForeignKey(
                        name: "fk_journals_groups1",
                        column: x => x.id_group,
                        principalTable: "groups",
                        principalColumn: "id_group");
                    table.ForeignKey(
                        name: "fk_journals_students1",
                        column: x => x.id_student,
                        principalTable: "students",
                        principalColumn: "id_student");
                    table.ForeignKey(
                        name: "fk_journals_unvisited_statuses1",
                        column: x => x.id_unvisited_status,
                        principalTable: "unvisited_statuses",
                        principalColumn: "id_unvisited_status");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "schedule_changes",
                columns: table => new
                {
                    id_schedule_change = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    new_date = table.Column<DateOnly>(type: "date", nullable: true),
                    old_date = table.Column<DateOnly>(type: "date", nullable: true),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    id_group = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_schedule_change);
                    table.ForeignKey(
                        name: "fk_schedule_changes_groups1",
                        column: x => x.id_group,
                        principalTable: "groups",
                        principalColumn: "id_group");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id_schedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    week_day = table.Column<sbyte>(type: "tinyint", nullable: true),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    id_group = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_schedule);
                    table.ForeignKey(
                        name: "fk_schedules_groups1",
                        column: x => x.id_group,
                        principalTable: "groups",
                        principalColumn: "id_group");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "students_has_groups",
                columns: table => new
                {
                    id_student = table.Column<int>(type: "int", nullable: false),
                    id_group = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id_student, x.id_group })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "fk_students_has_groups_groups1",
                        column: x => x.id_group,
                        principalTable: "groups",
                        principalColumn: "id_group");
                    table.ForeignKey(
                        name: "fk_students_has_groups_students1",
                        column: x => x.id_student,
                        principalTable: "students",
                        principalColumn: "id_student");
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "fk_groups_subjects1_idx",
                table: "groups",
                column: "id_subject");

            migrationBuilder.CreateIndex(
                name: "fk_groups_users_idx",
                table: "groups",
                column: "id_users");

            migrationBuilder.CreateIndex(
                name: "fk_journals_groups1_idx",
                table: "journals",
                column: "id_group");

            migrationBuilder.CreateIndex(
                name: "fk_journals_students1_idx",
                table: "journals",
                column: "id_student");

            migrationBuilder.CreateIndex(
                name: "fk_journals_unvisited_statuses1_idx",
                table: "journals",
                column: "id_unvisited_status");

            migrationBuilder.CreateIndex(
                name: "fk_schedule_changes_groups1_idx",
                table: "schedule_changes",
                column: "id_group");

            migrationBuilder.CreateIndex(
                name: "fk_schedules_groups1_idx",
                table: "schedules",
                column: "id_group");

            migrationBuilder.CreateIndex(
                name: "fk_students_has_groups_groups1_idx",
                table: "students_has_groups",
                column: "id_group");

            migrationBuilder.CreateIndex(
                name: "fk_students_has_groups_students1_idx",
                table: "students_has_groups",
                column: "id_student");

            migrationBuilder.CreateIndex(
                name: "fk_subjects_has_users_subjects1_idx",
                table: "subjects_has_users",
                column: "id_subject");

            migrationBuilder.CreateIndex(
                name: "fk_subjects_has_users_users1_idx",
                table: "subjects_has_users",
                column: "id_users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "journals");

            migrationBuilder.DropTable(
                name: "schedule_changes");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "students_has_groups");

            migrationBuilder.DropTable(
                name: "subjects_has_users");

            migrationBuilder.DropTable(
                name: "unvisited_statuses");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
