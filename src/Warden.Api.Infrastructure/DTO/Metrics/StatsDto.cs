﻿namespace Warden.Api.Infrastructure.DTO.Metrics
{
    public class StatsDto
    {
        public double TotalUptime { get; set; }
        public double TotalDowntime { get; set; }
        public double TotalResults { get; set; }
        public double TotalValidResults { get; set; }
        public double TotalInvalidResults { get; set; }
    }
}