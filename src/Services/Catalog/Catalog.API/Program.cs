namespace Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var assembly = typeof(Program).Assembly;

            // Add services to the container.
            builder.Services.AddCarterWithAssemblies(assemblies: assembly);

            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<Program>();
                // ValidationBehavior es genérico abierto (no dice que TRequest ni que TResponse, solo declaras que existe)
                // El contenedor DI sabra instanciarlo cuando MediatR lo necesite para un TRequest especifico
                // AddOpenBehavior le dice a MediatR agrega este behavior para todos los requests que pasen por MediatR
                // De esta manera, no tienes que registrar un ValidationBehavior<CreateProductCommand, CreateProductResult>, otro para GetProductByIdQuery, GetProductByIdResult, etc.
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddValidatorsFromAssembly(assembly:assembly);

            builder.Services.AddMarten(opts =>
            {
                opts.Connection(builder.Configuration.GetConnectionString("Database")!);
            }).UseLightweightSessions();

            builder.Services.AddExceptionHandler<CustomExceptionHandler>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapCarter();

            app.UseExceptionHandler(options =>
            {

            });

            app.Run();
        }
    }
}
