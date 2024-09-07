﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;

#nullable disable

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<bool>("AgendamentoAtivo")
                        .HasColumnType("bit");

                    b.Property<string>("DataAgendamento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataCancelamento")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HoraAgendamento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProfissionalId")
                        .HasColumnType("int");

                    b.Property<int>("ServicoSubCategoriaId")
                        .HasColumnType("int");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProfissionalId")
                        .IsUnique();

                    b.ToTable("Agendamentos");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Profissional", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cep")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("DataNascimento")
                        .HasMaxLength(10)
                        .HasColumnType("date");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Genero")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServicoCategoriaId")
                        .HasColumnType("int");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ServicoCategoriaId");

                    b.ToTable("Profissionais");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoCategoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Categoria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServicoSubCategoriaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ServicoSubCategoriaId");

                    b.ToTable("ServicoCategoria");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoSubCategoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("SubCategoria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ServicoSubCategorias");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Agendamento", b =>
                {
                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Profissional", null)
                        .WithOne("Agendamento")
                        .HasForeignKey("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Agendamento", "ProfissionalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Profissional", b =>
                {
                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoCategoria", "ServicoCategoria")
                        .WithMany()
                        .HasForeignKey("ServicoCategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServicoCategoria");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoCategoria", b =>
                {
                    b.HasOne("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.ServicoSubCategoria", "SubCategoria")
                        .WithMany()
                        .HasForeignKey("ServicoSubCategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SubCategoria");
                });

            modelBuilder.Entity("pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Profissional", b =>
                {
                    b.Navigation("Agendamento")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
