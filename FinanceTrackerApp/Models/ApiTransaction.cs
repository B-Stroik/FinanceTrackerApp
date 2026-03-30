using FinanceTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FinanceTrackerApp.Models
{
    public sealed class ApiTransaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TransactionType Type { get; set; }

        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
