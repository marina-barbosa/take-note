using System.Text;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using take_note.Services;

namespace take_note.Domain;

[ApiController]
[Route("v1/export")]
public class ExportController : ControllerBase
{

  private readonly ITrackService _trackService;
  private readonly string arquivo = "logs/trackService/registro.txt";
  private readonly string arquivoSerilog = "logs/serilog/log20240506b.txt";

  public ExportController(ITrackService trackService)
  {
    _trackService = trackService;
  }

  [HttpGet]
  public IActionResult ApiOk()
  {
    return Ok("Api online");
  }


  [HttpGet("txt")]
  public IActionResult ExportarTxt()
  {
    if (!System.IO.File.Exists(arquivoSerilog))
    {
      return NotFound("Arquivo log não encontrado.");
    }

    byte[] fileBytes;
    try
    {
      fileBytes = System.IO.File.ReadAllBytes(arquivoSerilog);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Erro ao ler o arquivo: {ex.Message}");
    }

    return File(fileBytes, "text/plain", arquivo);
  }

  [HttpGet("csv")]
  public IActionResult ExportarCsv()
  {
    if (!System.IO.File.Exists(arquivoSerilog))
    {
      return NotFound("Arquivo log não encontrado.");
    }

    string csvData = "";

    string[] txtLines = System.IO.File.ReadAllLines(arquivoSerilog);

    foreach (string line in txtLines)
    {
      string[] values = line.Split(' ');

      string csvRow = string.Join(",", values);

      csvData += csvRow + "\n";
    }

    try
    {
      byte[] csvBytes = Encoding.UTF8.GetBytes(csvData);

      return File(csvBytes, "text/csv", "regitro.csv");
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"Erro: {ex.Message}");
    }

  }

  [HttpGet("xls")]
  public IActionResult ExportarXls()
  {
    try
    {
      _trackService.ImportLogFromFile(arquivoSerilog);

      var dados = _trackService.GetDados();

      using (XLWorkbook workbook = new XLWorkbook())
      {
        workbook.AddWorksheet(dados, "LogEntrys");
        using (MemoryStream ms = new MemoryStream())
        {

          workbook.SaveAs(ms);
          return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LogEntrys.xlsx");
        }
      }
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex);
    }

  }

  [HttpPost("prepare")]
  public IActionResult PrepareLog()
  {
    try
    {
      _trackService.ImportLogFromFile(arquivoSerilog);

      return StatusCode(200);
    }
    catch (Exception ex)
    {
      return StatusCode(500, ex);
    }
  }

}