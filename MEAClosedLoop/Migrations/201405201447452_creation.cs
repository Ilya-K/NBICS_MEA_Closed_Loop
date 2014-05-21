namespace MEAClosedLoop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class creation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Experiments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        About = c.String(),
                        Target = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.MeasureManagers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ExperimentID = c.Int(nullable: false),
                        About = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MeasureManagers");
            DropTable("dbo.Experiments");
        }
    }
}
