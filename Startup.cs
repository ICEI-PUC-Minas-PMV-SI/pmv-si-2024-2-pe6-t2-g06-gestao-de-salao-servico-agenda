using AgendamentoService.Data;
using AgendamentoService.Repositories;
using AgendamentoService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Repositories;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure Entity Framework and SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IProfessionalRepository, ProfessionalRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        // Other service configurations
        services.AddControllers();
        // ...
    }

    // ...
}
