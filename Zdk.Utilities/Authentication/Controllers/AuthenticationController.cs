using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using OpenIddict.Abstractions;

namespace Zdk.Utilities.Authentication;

[Route("[controller]")]
[ApiController]
public class AuthenticationController : Controller
{
    private readonly SignInManager<ZdkUser> _signInManager;
    private readonly UserManager<ZdkUser> _userManager;
    private readonly IUserStore<ZdkUser> _userStore;
    private readonly IUserEmailStore<ZdkUser> _emailStore;

    public AuthenticationController(SignInManager<ZdkUser> signInManager, UserManager<ZdkUser> userManager, IUserStore<ZdkUser> userStore)
    {
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
    }

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] DTOZdkLoginRequest request)
    {
        request.ReturnUrl ??= Url.Content("~/");

        DTOZdkLoginResponse response = new()
        {
            ReturnUrl = request.ReturnUrl,
        };
        
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            response.Message = "Bad request: No username or password provided.";
            response.StatusCode = StatusCodes.Status400BadRequest;

            return BadRequest(response);
        }

        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Username, request.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            response.Message = "User logged in.";
            response.StatusCode = StatusCodes.Status200OK;

            return Ok(response);
        }

        if (result.IsLockedOut)
        {
            response.Message = "User account locked out.";
            response.StatusCode = StatusCodes.Status401Unauthorized;

            return StatusCode(401, response);
        }
        else
        {
            response.Message = "Invalid login attempt.";
            response.StatusCode = StatusCodes.Status400BadRequest;

            return BadRequest(response);
        }
    }

    [HttpGet]
    public async Task<IActionResult> SignOut(DTOZdkLogoutRequest request)
    {
        request.ReturnUrl ??= Url.Content("~/");

        DTOZdkLogoutResponse response = new()
        {
            ReturnUrl = request.ReturnUrl,
        };

        await _signInManager.SignOutAsync();
        response.Message = "User logged out.";
        if (request.ReturnUrl != null)
        {
            return LocalRedirect(request.ReturnUrl);
        }
        else
        {
            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return Ok(response);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] DTOZdkRegisterRequest request)
    {
        request.ReturnUrl ??= Url.Content("~/");

        DTOZdkLogoutResponse response = new()
        {
            ReturnUrl = request.ReturnUrl,
        };

        ZdkUser user = new();

        await _userStore.SetUserNameAsync(user, request.Username, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, request.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            response.Message = "User created a new account with password.";
            response.StatusCode = StatusCodes.Status200OK;

            await _signInManager.SignInAsync(user, isPersistent: false);
            
            return Ok(response);
        }

        response.Errors = new List<string>();

        foreach (var error in result.Errors)
        {
            response.Errors.Add(error.Description);
        }

        return BadRequest(response);
    }

    private IUserEmailStore<ZdkUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<ZdkUser>) _userStore;
    }
}
