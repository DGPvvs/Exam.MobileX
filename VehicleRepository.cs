using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.MobileX
{
    public class VehicleRepository : IVehicleRepository
    {
        private HashSet<Vehicle> vehicles = new HashSet<Vehicle>();
        private Dictionary<string, Vehicle> vehiclesById = new Dictionary<string, Vehicle>();
        private Dictionary<string, HashSet<Vehicle>> selersByName = new Dictionary<string, HashSet<Vehicle>>();

        public void AddVehicleForSale(Vehicle vehicle, string sellerName)
        {
			if (!selersByName.ContainsKey(sellerName))
			{
                this.selersByName.Add(sellerName, new HashSet<Vehicle>());
			}

            vehicle.SellerName = sellerName;

            this.selersByName[sellerName].Add(vehicle);
            this.vehicles.Add(vehicle);
            this.vehiclesById.Add(vehicle.Id, vehicle);
        }

        public bool Contains(Vehicle vehicle) => this.vehiclesById.ContainsKey(vehicle.Id);

        public int Count => this.vehicles.Count;

		public IEnumerable<Vehicle> GetVehicles(List<string> keywords) => this.vehicles
			.Where(v => keywords.Any(k => k.Equals(v.Brand)) || keywords.Any(k => k.Equals(v.Model))
															 || keywords.Any(k => k.Equals(v.Location))
															 || keywords.Any(k => k.Equals(v.Color)))
			.OrderByDescending(v => v.IsVIP)
			.ThenBy(v => v.Price);

        public IEnumerable<Vehicle> GetVehiclesBySeller(string sellerName) => this.selersByName.ContainsKey(sellerName)
                                                                                    ? this.selersByName[sellerName]
                                                                                    : throw new ArgumentException();

        public IEnumerable<Vehicle> GetVehiclesInPriceRange(double lowerBound, double upperBound) => this.vehicles
                .Where(v => v.Price >= lowerBound && v.Price <= upperBound)
                .OrderByDescending(v => v.Horsepower);

        public Dictionary<string, List<Vehicle>> GetAllVehiclesGroupedByBrand()
        {
            if (this.vehicles.Count() == 0)
            {
                throw new ArgumentException();
            }

            return vehicles.GroupBy(v => v.Brand).ToDictionary(t => t.Key, t => t.ToList().OrderBy(v => v.Price).ToList());
        }

        public void RemoveVehicle(string vehicleId)
        {
			if (!this.vehiclesById.ContainsKey(vehicleId))
			{
                throw new ArgumentException();
			}

            Vehicle vehicle = this.vehiclesById[vehicleId];
            this.selersByName[vehicle.SellerName].Remove(vehicle);
            this.vehicles.Remove(vehicle);
            this.vehiclesById.Remove(vehicleId);
        }

        public IEnumerable<Vehicle> GetAllVehiclesOrderedByHorsepowerDescendingThenByPriceThenBySellerName() => this.vehicles
            .OrderByDescending(v => v.Horsepower)
            .ThenBy(v => v.Price)
            .ThenBy(v => v.SellerName);



        public Vehicle BuyCheapestFromSeller(string sellerName)
        {
			if (!this.selersByName.ContainsKey(sellerName) || this.selersByName[sellerName].Count == 0)
			{
                throw new ArgumentException();
			}

            Vehicle vehicle = this.selersByName[sellerName].OrderBy(v => v.Price).First();

            this.selersByName[sellerName].Remove(vehicle);
            this.vehicles.Remove(vehicle);
            this.vehiclesById.Remove(vehicle.Id);

            return vehicle;
        }
    }
}
