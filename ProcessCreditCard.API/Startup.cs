using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessCreditCard.Data.EFCore;
using Microsoft.EntityFrameworkCore;
using ProcessCreditCard.Data.Repository;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using ProcessCreditCard.Domain.Model;
using ProcessCreditCard.Domain.Biz;
using ProcessCreditCard.Domain.Service;
using ProcessCreditCard.Domain.Validator;
using ProcessCreditCard.API.ActionFilters;
using FluentValidation.AspNetCore;

namespace ProcessCreditCard.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            }).AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<AddCreditCard>();
                fv.RegisterValidatorsFromAssemblyContaining<CreditCardActions>();
            });
            services.AddControllers();
            services.AddDbContext<CreditCardContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("ProcessCreditCard", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
            services.AddScoped<ICreditCardBiz, CreditCardBiz>();
            services.AddScoped<ICreditCardService, CreditCardService>();
            services.AddScoped<ICommandService, CommandService>();
            services.AddScoped<IValidationBuilder, ValidationBuilder>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                                { 
                                    Title = "ProcessCreditCard.API",
                                    Version = "v1"
                                });
                c.ExampleFilters();
            });

            services.AddSwaggerExamplesFromAssemblyOf<AddCreditCard>();
            services.AddSwaggerExamplesFromAssemblyOf<CreditCardActions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Process CreditCard API"));
            app.UseCors("ProcessCreditCard");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
