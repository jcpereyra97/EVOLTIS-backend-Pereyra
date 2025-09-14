using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MySql;


namespace UserApi.IntegrationTests.Setup
{
    public class MySqlContainerFixture : IAsyncLifetime
    {
        public MySqlContainer Container { get; private set; } = default!;
        public string ConnectionString => Container.GetConnectionString(); 

        public async Task InitializeAsync()
        {
            Container = new MySqlBuilder()
                .WithImage("mysql:8.0")
                .WithDatabase("userdb_test")
                .WithUsername("root")
                .WithPassword("root")
                .WithPortBinding(3307, 3306)
                .WithReuse(true)
                .Build();

            await Container.StartAsync();
        }

        public Task DisposeAsync() => Container.DisposeAsync().AsTask();
    }
}
