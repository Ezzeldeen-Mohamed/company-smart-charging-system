using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Domain.Entities
{
    public class Meter : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Serial { get; set; } = string.Empty;

        public string ChargeNumber { get; set; } = string.Empty;

        public bool IsUsed { get; set; }

        public ICollection<Contract>? Contracts { get; set; }
        public ICollection<ChargeTransaction>? ChargeTransactions { get; set; }
    }

}
