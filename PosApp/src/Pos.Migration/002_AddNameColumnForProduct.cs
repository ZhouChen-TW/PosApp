using FluentMigrator;

namespace Pos.Migration
{
    [Migration(2)]
    public class AddNameColumnForProduct:FluentMigrator.Migration
    {
        public override void Up()
        {
            Alter.Table("products").AddColumn("name").AsString(64)
                .NotNullable()
                .WithDefaultValue("hello");
        }

        public override void Down()
        {
            Delete.Table("products");
        }
    }
}