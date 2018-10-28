namespace MC.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RatingValue = c.String(nullable: false, maxLength: 5),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Writers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 150),
                        LastName = c.String(nullable: false, maxLength: 150),
                        UserName = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Movies", "RatingId", c => c.Int());
            AddColumn("dbo.Movies", "WriterId", c => c.Int());
            CreateIndex("dbo.Movies", "RatingId");
            CreateIndex("dbo.Movies", "WriterId");
            AddForeignKey("dbo.Movies", "RatingId", "dbo.Ratings", "Id");
            AddForeignKey("dbo.Movies", "WriterId", "dbo.Writers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Movies", "WriterId", "dbo.Writers");
            DropForeignKey("dbo.Movies", "RatingId", "dbo.Ratings");
            DropIndex("dbo.Movies", new[] { "WriterId" });
            DropIndex("dbo.Movies", new[] { "RatingId" });
            DropColumn("dbo.Movies", "WriterId");
            DropColumn("dbo.Movies", "RatingId");
            DropTable("dbo.Writers");
            DropTable("dbo.Ratings");
        }
    }
}
