using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Models;

namespace Src.Controllers
{
    [Route("api/statistical")]
    [ApiController]
    public class StatisticalController(ApplicationDBContext context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly ApplicationDBContext _context = context;
        private readonly UserManager<AppUser> _userManager = userManager;

        [HttpGet("overview")]
        public async Task<IActionResult> GetDashboardOverview()
        {
            if (_context.Comments == null || _context.Posts == null || _context.PageViews == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            var totalPosts = await _context.Posts.CountAsync();
            var totalUsers = await _userManager.Users.CountAsync();
            var totalComments = await _context.Comments.CountAsync();
            var totalViews = await _context.PageViews.SumAsync(pv => pv.Views);

            return Ok(new
            {
                TotalPosts = totalPosts,
                TotalUsers = totalUsers,
                TotalComments = totalComments,
                TotalPageViews = totalViews
            }
            );
        }

        [HttpPost("increment")]
        public async Task<IActionResult> IncrementPageView([FromBody] string pageName)
        {
            if (string.IsNullOrEmpty(pageName))
            {
                return BadRequest("Page name is required.");
            }
            if (_context.PageViews == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }

            var today = DateTime.UtcNow.Date;
            var pageView = await _context.PageViews
                .FirstOrDefaultAsync(pv => pv.PageName == pageName && pv.Date == today);

            if (pageView == null)
            {
                pageView = new PageView
                {
                    PageName = pageName,
                    Date = today,
                    Views = 1
                };
                _context.PageViews.Add(pageView);
            }
            else
            {
                pageView.Views += 1;
                _context.PageViews.Update(pageView);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Page view incremented successfully." });
        }


        [HttpGet("stats")]
        public async Task<IActionResult> GetPageViewsStats(string timeRange = "weekly", DateTime? startDate = null, DateTime? endDate = null)
        {
            if (_context.PageViews == null)
            {
                throw new InvalidOperationException("comments statuses data source is unavailable.");
            }
            IQueryable<PageView> query = _context.PageViews;

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(pv => pv.Date >= startDate.Value && pv.Date <= endDate.Value);
            }
            else
            {
                switch (timeRange.ToLower())
                {
                    case "weekly":
                        query = query.Where(pv => pv.Date >= DateTime.UtcNow.AddDays(-7));
                        break;
                    case "monthly":
                        query = query.Where(pv => pv.Date >= DateTime.UtcNow.AddMonths(-1));
                        break;
                    case "yearly":
                        query = query.Where(pv => pv.Date >= DateTime.UtcNow.AddYears(-1));
                        break;
                    case "daily":
                    default:
                        query = query.Where(pv => pv.Date == DateTime.UtcNow.Date);
                        break;
                }
            }

            var stats = await query
                .GroupBy(pv => pv.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalViews = g.Sum(pv => pv.Views)
                })
                .OrderByDescending(g => g.Date)
                .ToListAsync();

            return Ok(stats);
        }


    }
}