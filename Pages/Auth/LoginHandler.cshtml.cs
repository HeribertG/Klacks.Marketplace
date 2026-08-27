// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using System.Security.Claims;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Pages.Auth;

public class LoginHandlerModel : PageModel
{
    private readonly ILoginTokenService _loginTokenService;
    private readonly MarketplaceDbContext _db;

    public LoginHandlerModel(ILoginTokenService loginTokenService, MarketplaceDbContext db)
    {
        _loginTokenService = loginTokenService;
        _db = db;
    }

    public async Task<IActionResult> OnGetAsync(string token, string returnUrl = "/")
    {
        var userId = _loginTokenService.ConsumeToken(token);
        if (userId is null)
        {
            return Redirect(BuildPathBaseRelativeUrl("/login"));
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
        if (user is null)
        {
            return Redirect(BuildPathBaseRelativeUrl("/login"));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.DisplayName)
        };

        if (user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var target = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        return LocalRedirect(BuildPathBaseRelativeUrl(target));
    }

    private string BuildPathBaseRelativeUrl(string path)
        => Request.PathBase.HasValue ? Request.PathBase.Value + path : path;
}
