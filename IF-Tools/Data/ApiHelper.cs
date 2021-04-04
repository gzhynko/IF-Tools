using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IFTools.Data.Types;

namespace IFTools.Data
{
    public class ApiHelper
    {
        private static readonly Dictionary<Guid, List<FlightEntry>> CachedFlights = new();
        private static readonly Dictionary<Guid, DateTime> FlightsUpdated = new();
        
        public static async Task<List<FlightEntry>> GetFlightsForServer(Guid serverId)
        {
            if (!CachedFlights.ContainsKey(serverId))
            {
                CachedFlights[serverId] = await InfiniteFlightApiService.GetFlightsAsync(serverId);
                FlightsUpdated[serverId] = DateTime.Now;
                
                CachedFlights[serverId].ForEach(flight => flight.ServerId = serverId);
            }
            
            // Refresh cached flights every 7 seconds
            if ((DateTime.Now - FlightsUpdated[serverId]).TotalMilliseconds >= 7000)
            {
                CachedFlights[serverId] = await InfiniteFlightApiService.GetFlightsAsync(serverId);
                FlightsUpdated[serverId] = DateTime.Now;
                
                CachedFlights[serverId].ForEach(flight => flight.ServerId = serverId);
            }
            
            return CachedFlights[serverId];
        }

        public static async Task<FlightEntry> FindFlightFromAllServers(Guid flightId)
        {
            var flights = new List<FlightEntry>();
            
            flights.AddRange(await GetFlightsForServer(ServerGuids.CasualServerId));
            flights.AddRange(await GetFlightsForServer(ServerGuids.TrainingServerId));
            flights.AddRange(await GetFlightsForServer(ServerGuids.ExpertServerId));

            return flights.FirstOrDefault(flight => flight.FlightId == flightId);
        }
        
        public static string BuildUrl(string baseUrl, string endpoint, string parameters)
        {
            return string.Join("", baseUrl, endpoint, "?", parameters);
        }
    }
}