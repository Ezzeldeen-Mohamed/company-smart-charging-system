using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Domain.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(14)]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Serial { get; set; } = string.Empty;

        public decimal AmountPaid { get; set; }
        public DateTime Date { get; set; }

        public decimal NetPaid { get; set; }

        public string ChargeNumber { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        public ICollection<Contract> ?Contracts { get; set; }
    }


}
