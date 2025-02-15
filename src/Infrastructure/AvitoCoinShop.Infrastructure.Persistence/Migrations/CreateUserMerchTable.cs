using FluentMigrator;

namespace AvitoCoinShop.Infrastructure.Persistence.Migrations;

[Migration(20250215_3)]
public class CreateUserMerchTable : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE TABLE user_merch (
                user_id BIGINT NOT NULL,
                merch_id BIGINT NOT NULL,
                purchased_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP NOT NULL,
                PRIMARY KEY (user_id, merch_id),
                CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
                CONSTRAINT fk_merch FOREIGN KEY (merch_id) REFERENCES merch_items(id) ON DELETE CASCADE
            );
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE user_merch;");
    }
}