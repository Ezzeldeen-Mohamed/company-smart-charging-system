namespace CompanySmartChargingSystem.Application.DTOs
{
    public class ChargeTransactionDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int MeterId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
} 