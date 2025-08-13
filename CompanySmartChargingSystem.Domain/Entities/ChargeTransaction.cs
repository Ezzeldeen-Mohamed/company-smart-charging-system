using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Domain.Entities
{
    public class ChargeTransaction : BaseEntity
    {
        [Required]
        public int MeterId { get; set; }
        public Meter ?Meter { get; set; }

        [Required]
        public int ContractId { get; set; }
        public Contract ?Contract { get; set; }

        public decimal NetValue { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal FeesValue { get; set; }

        public string ChargeNumber { get; set; } = string.Empty;
    }


}
