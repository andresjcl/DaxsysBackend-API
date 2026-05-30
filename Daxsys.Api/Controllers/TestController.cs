using Daxsys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Daxsys.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly AppDbContext _context;

    public TestController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("db-check")]
    public async Task<IActionResult> CheckDatabase()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalCompanies = await _context.Companies.CountAsync();
        var totalBranches = await _context.Branches.CountAsync();
        var totalWarehouses = await _context.Warehouses.CountAsync();

        return Ok(new
        {
            Users = totalUsers,
            Companies = totalCompanies,
            Branches = totalBranches,
            Warehouses = totalWarehouses
        });
    }
}