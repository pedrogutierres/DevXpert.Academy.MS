using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevXpert.Academy.Alunos.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: true),
                    DataHoraCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataHoraAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlunosAulasConcluidas",
                columns: table => new
                {
                    AlunoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CursoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AulaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataHoraConclusao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataHoraCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataHoraAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlunosAulasConcluidas", x => new { x.AlunoId, x.CursoId, x.AulaId });
                    table.ForeignKey(
                        name: "FK_AlunosAulasConcluidas_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AlunoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CursoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataHoraConclusaoDoCurso = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Ativa = table.Column<bool>(type: "INTEGER", nullable: false),
                    Concluido = table.Column<bool>(type: "INTEGER", nullable: false),
                    CertificadoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    DataHoraEmissao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataHoraCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataHoraAlteracao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_Alunos_AlunoId",
                        column: x => x.AlunoId,
                        principalTable: "Alunos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matriculas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_AlunoId",
                table: "Matriculas",
                column: "AlunoId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CursoId",
                table: "Matriculas",
                column: "CursoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlunosAulasConcluidas");

            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Alunos");
        }
    }
}
