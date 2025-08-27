//using CompanySmartChargingSystem.Application.Services.IService;
//using CompanySmartChargingSystem.Domain.Entities;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using OfficeOpenXml;
//using System.Diagnostics.Contracts;
//using System.Diagnostics.Metrics;

//namespace company_smart_charging_system.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ExcelController : ControllerBase
//    {
//        private readonly IChargeTransactionService _chargeService;

//        public ExcelController(IChargeTransactionService chargeService)
//        {
//            _chargeService = chargeService;
//        }
//        [HttpGet("Export-Charges")]
//        public async Task<IActionResult> Export()
//        {
//            var charges = await _chargeService.GetAllCharges();
            
//            using(var package = new ExcelPackage())
//            {
//                var worksheet = package.Workbook.Worksheets.Add("Charges");
//                worksheet.Cells[1, 1].Value = "Id";
//                worksheet.Cells[1, 2].Value = "Created Date";
//                worksheet.Cells[1, 3].Value = "Updated Date";
//                worksheet.Cells[1, 4].Value = "Meter Id";
//                worksheet.Cells[1, 5].Value = "Contract Id";
//                worksheet.Cells[1, 6].Value = "Net Value";
//                worksheet.Cells[1, 7].Value = "Amount Paid";
//                worksheet.Cells[1, 8].Value = "Fees Value";
//                worksheet.Cells[1, 9].Value = "Charge Number";

//                int row = 2;
//                foreach(var charge in charges)
//                {
//                    worksheet.Cells[row, 1].Value = charge.Id;
//                    worksheet.Cells[row, 2].Value = charge.CreatedDate;
//                    worksheet.Cells[row, 3].Value = charge.UpdatedDate;
//                    worksheet.Cells[row, 4].Value = charge.MeterId;
//                    worksheet.Cells[row, 5].Value = charge.ContractId;
//                    worksheet.Cells[row, 6].Value = charge.NetValue;
//                    worksheet.Cells[row, 7].Value = charge.AmountPaid;
//                    worksheet.Cells[row, 8].Value = charge.FeesValue;
//                    worksheet.Cells[row, 9].Value = charge.ChargeNumber;
//                    row++;
//                }

//                var stream = new MemoryStream();
//                package.SaveAs(stream);
//                var filename = "Chargetransactions.xlsx";
//                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
//            }
//        }
//        [HttpPost("Import")]
//        public async Task<IActionResult> Import(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//                return BadRequest("No file uploaded.");

//            using (var stream = new MemoryStream())
//            {
//                await file.CopyToAsync(stream);
//                using (var package = new ExcelPackage(stream))
//                {
//                    var worksheet = package.Workbook.Worksheets[0];
//                    var rowCount = worksheet.Dimension.Rows;

//                    var charges = new List<ChargeTransaction>();
//                    for(int row = 2; row < rowCount; row++)
//                    {
//                        var charge = new ChargeTransaction
//                        {
//                            Id = int.Parse(worksheet.Cells[row, 1].Text),
//                            CreatedDate = DateTime.Parse(worksheet.Cells[row, 2].Text),
//                            UpdatedDate = DateTime.Parse(worksheet.Cells[row, 3].Text),
//                            MeterId = int.Parse(worksheet.Cells[row, 4].Text),
//                            ContractId = int.Parse(worksheet.Cells[row, 5].Text),
//                            NetValue = decimal.Parse(worksheet.Cells[row, 6].Text),
//                            AmountPaid = decimal.Parse(worksheet.Cells[row, 7].Text),
//                            FeesValue = decimal.Parse(worksheet.Cells[row, 8].Text),
//                            ChargeNumber = worksheet.Cells[row, 9].Text
//                        };
//                        charges.Add(charge);
//                    }

//                    await _chargeService.ImportCharges(charges);
//                    return Ok(charges);
//                }
//            }
//        }
//    }
//}
