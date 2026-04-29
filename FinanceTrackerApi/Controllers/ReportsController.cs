using FinanceTrackerApi.Models;
using FinanceTrackerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService service, ILogger<ReportsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("monthly")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ReportSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMonthly([FromQuery] int? month, [FromQuery] int? year)
    {
        try
        {
            var report = await _service.GetMonthlySummaryAsync(month, year);
            return Ok(report);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid report request payload");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving monthly report");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }
}
