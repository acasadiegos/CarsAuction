using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Logout
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        [BindProperty]
        public string? LogoutId { get; set; }

        public Index(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, IEventService events)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
        }

        public async Task<IActionResult> OnGet(string? logoutId)
        {
            // Obtener el contexto de logout
            var context = await _interaction.GetLogoutContextAsync(logoutId);

            // Si ShowSignoutPrompt es false, hacer logout automático
            if (context?.ShowSignoutPrompt == false)
            {
                // Hacer logout automáticamente sin mostrar la página
                return await OnPost(logoutId);
            }

            // Si no hay contexto válido, hacer logout de todas formas
            if (string.IsNullOrEmpty(logoutId))
            {
                return await OnPost(logoutId);
            }

            // Mostrar la página de logout solo si es necesario
            return Page();
        }

        public async Task<IActionResult> OnPost(string logoutId)
        {
            // Hacer logout del usuario
            if (User?.Identity?.IsAuthenticated == true)
            {
                // Eliminar la cookie de autenticación
                await _signInManager.SignOutAsync();

                // Registrar el evento de logout
                await _events.RaiseAsync(new UserLogoutSuccessEvent(
                    User.GetSubjectId(),
                    User.GetDisplayName()));
            }

            // Obtener el contexto de logout nuevamente
            var context = await _interaction.GetLogoutContextAsync(logoutId);

            // Redirigir al PostLogoutRedirectUri si existe
            if (!string.IsNullOrEmpty(context?.PostLogoutRedirectUri))
            {
                return Redirect(context.PostLogoutRedirectUri);
            }

            // Si no hay PostLogoutRedirectUri, redirigir al home
            return Redirect("~/");
        }
    }
}
