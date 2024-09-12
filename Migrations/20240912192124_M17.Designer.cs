﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;

#nullable disable

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240912192124_M17")]
    partial class M17
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Agendamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DataAgendamento")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("HoraAgendamento")
                        .HasColumnType("time");

                    b.Property<string>("Observacoes")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("ServicoCategoriaId")
                        .HasColumnType("int");

                    b.Property<int>("ServicoSubCategoriaId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ServicoCategoriaId");

                    b.HasIndex("ServicoSubCategoriaId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("Agendamentos");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.AgendamentoUsuarios", b =>
                {
                    b.Property<int>("AgendamentoId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("AgendamentoId", "UsuarioId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("AgendamentoUsuarios");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.LinkDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AgendamentoId")
                        .HasColumnType("int");

                    b.Property<string>("Href")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Metodo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rel")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AgendamentoId");

                    b.ToTable("LinkDto");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoCategoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("ServicoCategorias");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoSubCategoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<TimeSpan>("Duracao")
                        .HasColumnType("time");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("ServicoCategoriaId")
                        .HasColumnType("int");

                    b.Property<decimal>("Valor")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ServicoCategoriaId");

                    b.ToTable("ServicoSubCategorias");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("DataNascimento")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("Genero")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Perfil")
                        .HasColumnType("int");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Agendamento", b =>
                {
                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoCategoria", "ServicoCategoria")
                        .WithMany("Agendamentos")
                        .HasForeignKey("ServicoCategoriaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoSubCategoria", "ServicoSubCategoria")
                        .WithMany("Agendamentos")
                        .HasForeignKey("ServicoSubCategoriaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServicoCategoria");

                    b.Navigation("ServicoSubCategoria");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.AgendamentoUsuarios", b =>
                {
                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Agendamento", "Agendamento")
                        .WithMany("Usuarios")
                        .HasForeignKey("AgendamentoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Usuario", "Usuario")
                        .WithMany("Agendamentos")
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Agendamento");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.LinkDto", b =>
                {
                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Agendamento", null)
                        .WithMany("Links")
                        .HasForeignKey("AgendamentoId");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoSubCategoria", b =>
                {
                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoCategoria", "ServicoCategoria")
                        .WithMany("ServicoSubCategorias")
                        .HasForeignKey("ServicoCategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServicoCategoria");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Agendamento", b =>
                {
                    b.Navigation("Links");

                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoCategoria", b =>
                {
                    b.Navigation("Agendamentos");

                    b.Navigation("ServicoSubCategorias");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoSubCategoria", b =>
                {
                    b.Navigation("Agendamentos");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Usuario", b =>
                {
                    b.Navigation("Agendamentos");
                });
#pragma warning restore 612, 618
        }
    }
}
