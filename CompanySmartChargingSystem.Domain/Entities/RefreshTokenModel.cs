using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanySmartChargingSystem.Domain.Entities
{
    [Owned]
    public class RefreshTokenModel
    {
        public string Token { get; set; }
        public bool isNew { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsActive => DateTime.UtcNow < Expiration;
        public DateTime createdOn { get; set; } = DateTime.UtcNow;
        public DateTime? revokedOn { get; set; } = null;


    }
}
