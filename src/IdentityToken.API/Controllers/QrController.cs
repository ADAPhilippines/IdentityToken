using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityToken.API.Data;
using IdentityToken.Common.Helpers;
using IdentityToken.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Drawing;
using QRCoder;

[ApiController]
[Route("[controller]")]
public class QrController : ControllerBase
{

  [HttpGet("generate")]
  public IActionResult GenerateQr([FromQuery] string data)
  {
    var qrGenerator = new QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
    var qrCode = new PngByteQRCode(qrCodeData);
    var qrCodeImage = qrCode.GetGraphic(20);
    return File(qrCodeImage, "image/png");
  }
}