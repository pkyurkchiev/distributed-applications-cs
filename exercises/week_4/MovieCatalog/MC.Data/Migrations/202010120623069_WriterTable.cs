namespace MC.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WriterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Writers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 150),
                        LastName = c.String(maxLength: 150),
                        UserName = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Movies", "WriterId", c => c.Int());
            CreateIndex("dbo.Movies", "WriterId");
            AddForeignKey("dbo.Movies", "WriterId", "dbo.Writers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Movies", "WriterId", "dbo.Writers");
            DropIndex("dbo.Movies", new[] { "WriterId" });
            DropColumn("dbo.Movies", "WriterId");
            DropTable("dbo.Writers");
        }
    }
}
