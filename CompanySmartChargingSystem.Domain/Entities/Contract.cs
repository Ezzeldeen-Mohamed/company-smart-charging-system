using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CompanySmartChargingSystem.Domain.Entities
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UniqueCode { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        [Required]
        public int MeterId { get; set; }
        public Meter Meter { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomerCode { get; set; } = string.Empty;

        public bool IsClosed { get; set; }
        public DateTime? OnClosed { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<ChargeTransaction>? ChargeTransactions { get; set; }
    }

}
