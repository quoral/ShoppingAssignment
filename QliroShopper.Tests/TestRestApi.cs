using System.Collections.Generic;
using System.Net;
using QliroShopper.Controllers;
using QliroShopper.Models;
using QliroShopper.Tests.Utils;
using Xunit;

namespace QliroShopper.Tests
{
    public class TestRestApi
    {
        [Fact]
        public void TestGetEmptyList()
        {
            using (var connection = new TestSqliteSetup(TestDatabaseService.connection_string))
            {
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.GetAllOrders();
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                    var orders = (List<Order>) response.Value;
                    Assert.Empty(orders);
                }
            }
        }
        [Fact]
        public void TestGetNotExistingOrder()
        {
            using (var connection = new TestSqliteSetup(TestDatabaseService.connection_string))
            {
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.GetOrder(1);
                    Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }

        [Fact]
        public void TestCreateOrderAndGetIt()
        {
            using (var connection = new TestSqliteSetup(TestDatabaseService.connection_string))
            {
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.AddOrder(new Order());
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                }
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.GetOrder(1);
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                }
            }
        }
        [Fact]
        public void TestCreateOrderAndRemoveIt()
        {
            using (var connection = new TestSqliteSetup(TestDatabaseService.connection_string))
            {
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.AddOrder(new Order());
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                }
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.GetOrder(1);
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                    var remove_response = controller.RemoveOrder(1);
                    Assert.Equal((int)HttpStatusCode.OK, remove_response.StatusCode);                    
                }
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.GetOrder(1);
                    Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }
        [Fact]
        public void TestCreateOrderAndModifyIt()
        {
            using (var connection = new TestSqliteSetup(TestDatabaseService.connection_string))
            {
                // Add the order
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.AddOrder(new Order());
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                }
                // Modify the order
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.GetOrder(1);
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                    var order = (Order)response.Value;
                    Assert.Empty(order.OrderItems);
                    order.OrderItems.Add(new Item{
                        Quantity = 1,
                        Price = 2,
                        Description = "Apple"
                    });
                    var modify_response = controller.EditOrder(1, order);
                    Assert.Equal((int)HttpStatusCode.OK, modify_response.StatusCode);
                }
                // Verify the order
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var response = controller.GetOrder(1);
                    Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
                    var order = (Order)response.Value;
                    Assert.Equal(1, order.OrderItems.Count);
                    Assert.Equal(1, order.OrderItems[0].Quantity);
                    Assert.Equal("Apple", order.OrderItems[0].Description);
                    Assert.Equal(2, order.OrderItems[0].Price);
                    Assert.Equal(order.TotalPrice, 2);
                }
            }
        }
        public void TestEditNotExistingOrder()
        {
            using (var connection = new TestSqliteSetup(TestDatabaseService.connection_string))
            {
                using (var context = new OrderContext(connection.Options))
                {   
                    var controller = new OrderController(context);
                    var order = new Order();
                    order.OrderItems.Add(new Item{
                        Quantity = 1,
                        Price = 2,
                        Description = "Apple"
                    });
                    var response = controller.EditOrder(1, order);
                    Assert.Equal((int)HttpStatusCode.NotFound, response.StatusCode);
                }
            }
        }
    }
}