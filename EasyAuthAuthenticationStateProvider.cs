using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Web;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EasyAuthDemo
{
    public class EasyAuthAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly IUriHelper _uriHelper;
        public EasyAuthAuthenticationStateProvider(HttpClient httpClient, IJSRuntime jsRuntime,
        IUriHelper uriHelper)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _uriHelper = uriHelper;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            AuthToken token = await GetAuthToken();

            if (token?.AuthenticationToken != null)
            {
                _httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", token.AuthenticationToken);
                try
                {
                    var authresponse = await _httpClient.GetStringAsync(Constants.AzureFunctionAuthURL + Constants.AuthMeEndpoint);
                    
                    //To see the response uncomment the line below
                    //Console.WriteLine(authresponse);

                    await LocalStorage.SetAsync(_jsRuntime, "authtoken", token);
                    var authInfo = JsonSerializer.Parse<List<AuthInfo>>(authresponse);
                    switch (authInfo[0].ProviderName)
                    {
                        case "twitter": return await GetTwitterClaims(authInfo[0]);
                        default: break;
                    }                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            await LocalStorage.DeleteAsync(_jsRuntime, "authtoken");
            var identity = new ClaimsIdentity();
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
        private async Task<AuthToken> GetAuthToken()
        {
            string authTokenFragment = HttpUtility.UrlDecode(new Uri(_uriHelper.GetAbsoluteUri()).Fragment);
            if (string.IsNullOrEmpty(authTokenFragment))
            {
                return await LocalStorage.GetAsync<AuthToken>(_jsRuntime, "authtoken");
            }

            await _jsRuntime.InvokeAsync<string>(
                        "EasyAuthDemoUtilities.updateURLwithoutReload", Constants.BlazorWebsiteURL);

            Regex getJsonRegEx = new Regex(@"\{(.|\s)*\}");
            MatchCollection matches = getJsonRegEx.Matches(authTokenFragment);
            if (matches.Count == 1)
            {
                return JsonSerializer.Parse<AuthToken>(matches[0].Value);
            }
            return new AuthToken();

        }
        private Task<AuthenticationState> GetTwitterClaims(AuthInfo authinfo)
        {
            List<Claim> userClaims = new List<Claim>();
            foreach (AuthUserClaim userClaim in authinfo.UserClaims)
            {
                userClaims.Add(new Claim(userClaim.Type, userClaim.Value));
            }
            var identity = new ClaimsIdentity(userClaims, "EasyAuth");

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}