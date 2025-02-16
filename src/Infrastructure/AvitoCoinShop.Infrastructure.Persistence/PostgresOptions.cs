namespace AvitoCoinShop.Infrastructure.Persistence;

public class PostgresOptions
{
    public string? Host { get; set;  }

    public string? Port { get; set;  }

    public string? Name { get; set;  }

    public string? User { get; set;  }

    public string? Password { get; set;  }

    public string ConnectionString =>
        $"Host={Host};Port={Port};Username={User};Password={Password};Database={Name}";
}
