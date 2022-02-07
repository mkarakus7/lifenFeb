using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
    
    
        private readonly IUnitOfWork _unitWork;
        private readonly IBasketRepository _basketRepo;

        public OrderService(
            IUnitOfWork unitWork,
        IBasketRepository basketRepo)
        {
    
            _unitWork = unitWork;
            _basketRepo = basketRepo;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail,
         int deliveryMethodId, string basketId, Address shippingAddress)
        {
         

            var basket = await _basketRepo.GetBasketAsync(basketId);
            var items = new List<OrderItem>();
            foreach(var item in basket.Items) 
            {
                var productItem = await _unitWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new 
                ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price,item.Quantity);
                items.Add(orderItem);
            }
            var xdeliveryMethod = await _unitWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var subtotal = items.Sum(item => item.Price * item.Quantity);

            var order = new Order(items, buyerEmail, shippingAddress, xdeliveryMethod, subtotal);

            // todo save
            _unitWork.Repository<Order>().Add(order);
            var result = await _unitWork.Complete();
            if(result <= 0 )
            {
                return null;
            }
            // delete basket
            await _basketRepo.DeleteBasketAsync(basketId);
            return order;

        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            return await _unitWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {           
           var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);
           return await _unitWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async  Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        { 
           var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
           return await _unitWork.Repository<Order>().ListAsync(spec);
        }
    }
}