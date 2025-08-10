namespace CompanySmartChargingSystem.Application.DTOs
{
    public class ContractDto
    {
        public int Id { get; set; }
        public string UniqueCode { get; set; }
        public int CustomerId { get; set; }
        public int MeterId { get; set; }
        public string CustomerCode { get; set; }
        public bool IsClosed { get; set; }
        public DateTime? OnClosed { get; set; }
    }
} 