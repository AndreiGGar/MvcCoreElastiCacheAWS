﻿using Microsoft.Extensions.Caching.Distributed;
using MvcCoreElastiCacheAWS.Helpers;
using MvcCoreElastiCacheAWS.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MvcCoreElastiCacheAWS.Services
{
    public class ServiceAWSCache
    {
        /*private IDatabase cache;

        public ServiceAWSCache()
        {
            this.cache = HelperCacheRedis.Connection.GetDatabase();
        }*/

        private readonly IDistributedCache cache;

        public ServiceAWSCache(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<List<Coche>> GetCochesFavoritosAsync()
        {
            string jsonCoches = await this.cache.GetStringAsync("cochesfavoritos");
            if (jsonCoches == null)
            {
                return null;
            }
            else
            {
                List<Coche> cars = JsonConvert.DeserializeObject<List<Coche>>(jsonCoches);
                return cars;
            }
        }

        public async Task AddCocheAsync(Coche car)
        {
            List<Coche> coches = await this.GetCochesFavoritosAsync();
            if (coches == null)
            {
                coches = new List<Coche>();
            }
            coches.Add(car);
            string jsonCoches = JsonConvert.SerializeObject(coches);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            };
            await this.cache.SetStringAsync("cochesfavoritos", jsonCoches, cacheOptions);
        }

        public async Task DeleteCocheFavoritoAsync(int idcoche)
        {
            List<Coche> cars = await this.GetCochesFavoritosAsync();
            if (cars != null)
            {
                Coche carEliminar = cars.FirstOrDefault(x => x.IdCoche == idcoche);
                cars.Remove(carEliminar);
                if (cars.Count == 0)
                {
                    await this.cache.RemoveAsync("cochesfavoritos");
                }
                else
                {
                    string jsonCoches = JsonConvert.SerializeObject(cars);

                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                    };
                    await this.cache.SetStringAsync("cochesfavoritos", jsonCoches, cacheOptions);
                }
            }
        }
    }
}
