using System.IO;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Models.Entities;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Services;
using Moq;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Controllers;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;

namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Tests.UnitTests.Controller
{
    public class AgendamentosControllerTests
    {
        private readonly Mock<IAgendamentoService> _agendamentoServiceMock;
        private readonly AgendamentosController _controller;

        public AgendamentosControllerTests()
        {
            // Cria o mock do IAgendamentoService
            _agendamentoServiceMock = new Mock<IAgendamentoService>();
            _controller = new AgendamentosController(_agendamentoServiceMock.Object);
        }

        // Método para carregar os dados do arquivo JSON
        private List<AgendamentoDto> LoadAgendamentosFromJson()
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Tests", "UnitTests", "Resources", "agendamentos.json");

            // Verificar se o arquivo existe
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException($"Arquivo JSON não encontrado: {jsonPath}");

            try
            {
                var jsonString = File.ReadAllText(jsonPath);
                var agendamentos = JsonConvert.DeserializeObject<List<AgendamentoDto>>(jsonString);
                return agendamentos;
            }
            catch (JsonException jsonEx)
            {
                throw new Exception("Erro ao desserializar o arquivo JSON.", jsonEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao ler o arquivo: {ex.Message}", ex);
            }
        }

        [Fact]
        public void TesteSimples()
        {
            Assert.Equal(4, 2 + 2);
        }


        [Fact]
        public async Task GetAllAgendamentos_ReturnsOk_WithMockedData()
        {
            // Arrange
            var agendamentosDtoList = LoadAgendamentosFromJson(); // Carrega do JSON

            var agendamentos = agendamentosDtoList.Select(dto => new Agendamento
            {
                Id = dto.Id ?? 0, // Define um valor padrão se Id for nulo
                DataAgendamento = dto.DataAgendamento,
                HoraAgendamento = dto.HoraAgendamento,
                Status = dto.Status,
                Observacoes = dto.Observacoes,
                ServicoCategoriaId = dto.ServicoCategoriaId,
                ServicoSubCategoriaId = dto.ServicoSubCategoriaId,
                ProfissionalId = dto.ProfissionalId,
                UsuarioId = dto.UsuarioId
            }).ToList();

            int pageNumber = 1;
            int pageSize = 10;

            // Configurando o mock para retornar a lista de Agendamentos
            var agendamentoServiceMock = new Mock<IAgendamentoService>();
            agendamentoServiceMock
                .Setup(service => service.GetAllAgendamentosAsync(pageNumber, pageSize))
                .ReturnsAsync(agendamentos);

            // Iniciando o controlador com o mock do serviço
            var controller = new AgendamentosController(agendamentoServiceMock.Object);

            // Act
            var result = await controller.GetAllAgendamentos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAgendamentos = Assert.IsAssignableFrom<List<Agendamento>>(okResult.Value); // Aqui também deve ser Agendamento
            Assert.Equal(agendamentos.Count, returnedAgendamentos.Count);
        }   
    }
}
