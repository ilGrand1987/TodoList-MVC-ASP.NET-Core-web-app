using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using TodoList.Model;

namespace TodoList.Client.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ActivityController> _logger;

        public ActivityController(IHttpClientFactory httpClientFactory,
                ILogger<ActivityController> logger) 
        {
            _httpClientFactory = httpClientFactory ??
                throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async  Task<IActionResult> Index()   
        {
            await LogIdentityInformation();

            List<ActivityViewModel> activitylist = new List<ActivityViewModel>();
            var httpClient = _httpClientFactory.CreateClient("APIClient");

            HttpResponseMessage response = await httpClient.GetAsync("/api/Activities/Get");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                activitylist = JsonConvert.DeserializeObject<List<ActivityViewModel>>(data);
            }
            return View(activitylist);
        }

        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(ActivityViewModel model) 
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                HttpResponseMessage response = await httpClient.PostAsync(
                    "/api/Activities/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Attività Creata.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
     
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id) 
        {
            try
            {
                ActivityViewModel activity = new ActivityViewModel();
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                HttpResponseMessage response = await httpClient.GetAsync(
                 "/api/Activities/GetById/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    activity = JsonConvert.DeserializeObject<ActivityViewModel>(data);
                }
                return View(activity);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id ,ActivityViewModel model) 
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                HttpResponseMessage response = await httpClient.PutAsync(
                    "/api/Activities/Update/" + id , content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Attività aggiornata.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                ActivityViewModel activity = new ActivityViewModel();
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                HttpResponseMessage response = await httpClient.GetAsync(
                    "/api/Activities/GetbyId/" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    activity = JsonConvert.DeserializeObject<ActivityViewModel>(data);
                }
                return View(activity);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirm(int id) 
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("APIClient");
                HttpResponseMessage response = await httpClient.DeleteAsync(
                       "/api/Activities/Delete/" + id);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Attività cancellata.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
        public async Task LogIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // get the saved access token
            var accessToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // get the refresh token
            var refreshToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            var userClaimsStringBuilder = new StringBuilder();
            foreach (var claim in User.Claims)
            {
                userClaimsStringBuilder.AppendLine(
                    $"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }

            // log token & claims
            _logger.LogInformation($"Identity token & user claims: " +
                $"\n{identityToken} \n{userClaimsStringBuilder}");
            _logger.LogInformation($"Access token: " +
                $"\n{accessToken}");
            _logger.LogInformation($"Refresh token: " +
                $"\n{refreshToken}");
        }
    }
}
