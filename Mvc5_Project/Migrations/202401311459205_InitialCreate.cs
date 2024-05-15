namespace Mvc5_Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(nullable: false, maxLength: 50),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        ExpireDate = c.DateTime(nullable: false, storeType: "date"),
                        Picture = c.String(nullable: false, maxLength: 40),
                        InStock = c.Boolean(nullable: false),
                        SellerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Sellers", t => t.SellerId, cascadeDelete: true)
                .Index(t => t.SellerId);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        SaleId = c.Int(nullable: false, identity: true),
                        SaleDate = c.DateTime(nullable: false, storeType: "date"),
                        Quantity = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SaleId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Sellers",
                c => new
                    {
                        SellerId = c.Int(nullable: false, identity: true),
                        SallerName = c.String(nullable: false, maxLength: 50),
                        SellerAddress = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.SellerId);
            ///////////////////////////////////
            CreateStoredProcedure("InsertProduct", c => new {
                ProductName = c.String(maxLength: 50),
                Price= c.Decimal(),
                ExpireDate = c.DateTime(),
                InStock = c.Boolean(),
                Picture = c.String(maxLength: 40),
                SellerId = c.Int(),
                
            }, @"INSERT INTO Products (ProductName, Price,ExpireDate, InStock, Picture,SellerId)
	                    VALUES (@ProductName, @Price,@ExpireDate, @InStock, @Picture, @SellerId)
	                    SELECT SCOPE_IDENTITY() AS ProductId
                    RETURN ");
            CreateStoredProcedure("UpdateProduct", c => new
            {
                ProductId = c.Int(),
                ProductName = c.String(maxLength: 50),
                Price = c.Decimal(),
                ExpireDate = c.DateTime(),
                InStock = c.Boolean(),
                Picture = c.String(maxLength: 40),
                SellerId = c.Int()
            }, @"UPDATE Products SET ProductName = @ProductName, Price=@Price,ExpireDate=@ExpireDate, InStock=@InStock, Picture=@Picture,SellerId=@SellerId
                    WHERE ProductId = @ProductId
                RETURN");
            
            CreateStoredProcedure("DeleteProduct", c => new
            {
                ProductId = c.Int()

            }, @"DELETE FROM Products
                WHERE ProductId = @ProductId
            RETURN");
            CreateStoredProcedure("DeleteSaleByProductId", c => new
            {
                ProductId = c.Int()

            }, @"DELETE FROM Sales
                WHERE ProductId = @ProductId
            RETURN");
            CreateStoredProcedure("InsertSale", c => new
            {
                SaleDate = c.DateTime(),
                Quantity = c.Int(),
                ProductId = c.Int()

            }, @"INSERT INTO Sales (SaleDate,Quantity, ProductId)
	            VALUES (@SaleDate, @Quantity, @ProductId)
	            SELECT SCOPE_IDENTITY() as SaleId
            RETURN");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "SellerId", "dbo.Sellers");
            DropForeignKey("dbo.Sales", "ProductId", "dbo.Products");
            DropIndex("dbo.Sales", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "SellerId" });
            DropTable("dbo.Sellers");
            DropTable("dbo.Sales");
            DropTable("dbo.Products");
            /////////////////
            DropStoredProcedure("InsertProduct");
            DropStoredProcedure("UpdateProduct");
            DropStoredProcedure("DeleteProduct");
            DropStoredProcedure("DeleteStockByProductId");
            DropStoredProcedure("InsertSale");
        }
    }
}
