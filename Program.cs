
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda.Data;
using System.Text;
using System.Text.Json.Serialization;


namespace pmv_si_2024_2_pe6_t2_g06_gestao_de_salao_servico_agenda
{
    public class Program
    {
        // Aqui damos um stard das configuracoes do projeto
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
                //options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            );

            // Configuracoes do serviço de Authenticação
            // para utilizar uma rota, o usuario deve estar autenticado:
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // Adding JWT Bearer
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };
                });



            // Configuracoes do banco de dados --
            builder.Services.AddDbContext<ApplicationDbContext>(options => //options configura tudo abaixo = injecao = DbContext
                //configurations a serem executadas abaixo - DefaultConnection = appsettings.json
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
 
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }


    // passos: 
    // 1 - Criar o modelo onde vamos configurar quais os campos queremos acessar em cada entidade
    // 2 - Criar o DbContext, para fazer o acesso ao banco de dados
    // 3 - Criar a connectionString em appsetings.json
    // 4 - Adicionar o builder para o banco de dados no Program.cs
    // 5 - gerar migracoes para criacao das tabelas no banco:
        // add-migration M00 --- para dar um build nos arquivos de migracao para serem usados para criar tabela no banco
        // Remove-Migration -- remove os arquivos de migracao
        // update-database - comando para criar a tabela no banco
    // Criar o controller para definicao do endpoint
    // Caso queira ignorar campos do modelo no json, criar Data Transfer Objects (dto)
    // Caso precise de uma tabela de relacionamentos n-n, configurar um modelo especifico somente para chaves
    // Configurar o project.cs para authenticacao >> devo tambem nos controladores explicitamente falar que a rota tem que ser "Authorize"
}
