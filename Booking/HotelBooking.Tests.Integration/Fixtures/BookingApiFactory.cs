using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HotelBooking.Persistence;
using Testcontainers.PostgreSql;

namespace HotelBooking.Tests.Integration.Fixtures;

public sealed class BookingApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _container;
    private string? _connectionString;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("booking_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptorType = typeof(DbContextOptions<>).MakeGenericType(typeof(BookingDbContext));
            var descriptor = services.FirstOrDefault(x => x.ServiceType == descriptorType);

            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<BookingDbContext>(options =>
                options.UseNpgsql(_connectionString));
        });

        builder.UseEnvironment("Development");
    }
}