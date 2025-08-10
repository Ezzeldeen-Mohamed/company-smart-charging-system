namespace CompanySmartChargingSystem.Application.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NationalId { get; set; }
        public string Serial { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime Date { get; set; }
        public decimal NetPaid { get; set; }
        public string ChargeNumber { get; set; }
    }
} 