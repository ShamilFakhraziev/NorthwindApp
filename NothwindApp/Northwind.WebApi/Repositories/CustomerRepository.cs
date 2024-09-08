using Packt.Shared;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Concurrent;

namespace Northwind.WebApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private static ConcurrentDictionary<string, Customer>? customersCache;
        private NorthwindContext db;
        public CustomerRepository(NorthwindContext db)
        {
            this.db = db;
            if(customersCache is null)
            {
                customersCache = new ConcurrentDictionary<string, Customer>(db.Customers.ToDictionary(c=>c.CustomerId));
            }
        }
        public async Task<Customer?> CreateAsync(Customer c)
        {
            c.CustomerId = c.CustomerId.ToUpper();

            EntityEntry<Customer> added = await db.Customers.AddAsync(c);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (customersCache is null) return c;
                // нового клиента добавляем в кэш, иначе вызываем метод UpdateCache
                return customersCache.AddOrUpdate(c.CustomerId, c, UpdateCache);
            }
            else
            {
                return null;
            }
        }
        private Customer UpdateCache(string id, Customer c)
        {
            Customer? old;
            if (customersCache is not null)
            {
                if (customersCache.TryGetValue(id, out old))
                {
                    if (customersCache.TryUpdate(id, c, old))
                    {
                        return c;
                    }
                }
            }
            return null!;
        }


        public async Task<bool?> DeleteAsync(string id)
        {
            id = id.ToUpper();
            // удаляем из базы данных
            Customer? c = db.Customers.Find(id);
            if (c is null) return null;
            db.Customers.Remove(c);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                if (customersCache is null) return null;
                // удаляем из кэша
                return customersCache.TryRemove(id, out c);
            }
            else
            {
                return null;
            }
        }

        public Task<IEnumerable<Customer>> RetrieveAllAsync()
        {
            // в целях производительности извлекаем из кэша
            return Task.FromResult(customersCache is null ? Enumerable.Empty<Customer>() : customersCache.Values);
        }
        public Task<Customer?> RetrieveAsync(string id)
        {
            // в целях производительности извлекаем из кэша
            id = id.ToUpper();
            if (customersCache is null) return null!;
            customersCache.TryGetValue(id, out Customer? c);
            return Task.FromResult(c);
        }

        public async Task<Customer?> UpdateAsync(string id, Customer c)
        {
            // нормализуем идентификатор клиента
            id = id.ToUpper();
            c.CustomerId = c.CustomerId.ToUpper();
            // обновляем в базе
            db.Customers.Update(c);
            int affected = await db.SaveChangesAsync();
            if (affected == 1)
            {
                // обновляем в кэше
                return UpdateCache(id, c);
            }
            return null;
        }
    }
}