using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zambon.OrderManagement.Core.Migrations
{
    public partial class StoredProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // [Stock].[GetOrderTotal]
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE [Stock].[GetOrderTotal]
    @OrderID BIGINT,
    @Total DECIMAL(18, 2) OUTPUT
AS
BEGIN
    SELECT
        @Total = SUM(OP.Qty * OP.UnitPrice)
    FROM
        Stock.Orders O
        INNER JOIN Stock.OrdersProducts OP
            ON OP.OrderID = O.ID AND OP.IsDeleted = 0
    WHERE
        O.IsDeleted = 0
        AND O.ID = @OrderID;
END
GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // [Stock].[GetOrderTotal]
            migrationBuilder.Sql(@"DROP PROCEDURE [Stock].[GetOrderTotal];");
        }
    }
}