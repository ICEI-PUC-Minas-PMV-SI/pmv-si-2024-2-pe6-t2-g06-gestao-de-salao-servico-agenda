using AgendamentoService.Data;
using AgendamentoService.Repositories;
using AgendamentoService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure Entity Framework and SQL Server
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped(typeof(IEntidadeBaseRepository<>), typeof(IEntidadeBaseRepository<>));
        services.AddScoped<IProfissionalRepository, IProfissionalRepository>();
        services.AddScoped<IClienteRepository, IClienteRepository>();
        services.AddScoped<IAgendamentoRepository, IAgendamentoRepository>();
        // services.AddScoped<IAgendamentoService, IAgendamentoService>();

        // Other service configurations
        services.AddControllers();
        // ...
    }

    // ...
}
