using System.Net;
using System.Net.Http.Headers;
using System.Text;
using CorePush.Apple;
using CorePush.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OneSignalApi.Client;
using PikaServer.Infras.AppSettings;
using PikaServer.Infras.Constants;
using PikaServer.Infras.DelegateHandler;
using PikaServer.Infras.Helpers;
using PikaServer.Infras.Services.ApiFeature;
using PikaServer.Infras.Services.Auth;
using PikaServer.Infras.Services.Credential;
using PikaServer.Infras.Services.Interfaces;
using PikaServer.Infras.Services.Notification;
using PikaServer.Infras.Services.Verification;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;

namespace PikaServer.Infras;

public static class DependencyInjection
{
	public static IServiceCollection UseHdBankVendor(this IServiceCollection services, Action<HdBankApiSetting> options)
	{
		// register config options
		services.Configure(options);

		var hdBankApiSetting = new HdBankApiSetting();
		options.Invoke(hdBankApiSetting);


		// singleton
		services.AddSingleton<HdBankCredential>();
		services.AddScoped<HdBankHttpHandler>();

		// register transient services
		services.AddTransient<IHdBankAuthService, HdBankAuthService>();
		services.AddTransient<IHdBankCredentialManager, HdBankCredentialManager>();
		services.AddTransient<IHdBankBasicFeature, HdBankBasicFeature>();
		services.AddTransient<RsaCredentialHelper>();
		services.AddTransient<ITwilioVerificationService, TwilioVerificationService>();
		
		

		// register httpClient
		services.AddHttpClient(HttpClientNameConstants.HdBankAuth,
				http =>
				{
					http.BaseAddress = new Uri(hdBankApiSetting.AuthUrl);
					http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
						"application/x-www-form-urlencoded");
				})
			.SetHandlerLifetime(TimeSpan.FromSeconds(30))
			.AddPolicyHandler(GetHttpRetryPolicy());

		services.AddHttpClient(HttpClientNameConstants.HdBank, http =>
			{
				http.BaseAddress = new Uri(hdBankApiSetting.BaseUrl);
				http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
				http.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", hdBankApiSetting.ApiKey);
			})
			.SetHandlerLifetime(TimeSpan.FromSeconds(30))
			.AddPolicyHandler(GetHttpRetryPolicy())
			.AddPolicyHandler(GetUnauthorizedHttpRetryPolicy)
			.AddHttpMessageHandler(s => s.GetRequiredService<HdBankHttpHandler>());

		return services;
	}

	public static IServiceCollection UseJwtAuthentication(this IServiceCollection services,
		Action<JwtAuthSetting> config)
	{
		services.Configure(config);
		var jwtSetting = new JwtAuthSetting();
		config.Invoke(jwtSetting);

		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Secret)),
					ValidateAudience = false,
					ValidateIssuer = false
				};
			});

		services.AddAuthorization();

		services.AddTransient<IJwtAuthService, JwtAuthService>();

		return services;
	}

	public static IServiceCollection UseNotification(this IServiceCollection services, Action<FcmSettings> config)
	{
		var fcmSetting = new FcmSettings();
		config.Invoke(fcmSetting);
		services.Configure(config);
		
		services.AddTransient<INotificationService, NotificationService>();
		services.AddHttpClient<FcmSender>();
		services.AddHttpClient<ApnSender>();
		
		return services;
	}

	private static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy()
	{
		var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
			.WaitAndRetryAsync(delay);
	}

	private static IAsyncPolicy<HttpResponseMessage> GetUnauthorizedHttpRetryPolicy(IServiceProvider provider,
		HttpRequestMessage request)
	{
		var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

		return Policy
			.HandleResult<HttpResponseMessage>(r =>
				r.StatusCode == HttpStatusCode.Unauthorized)
			.WaitAndRetryAsync(delay, async (result, span) =>
			{
				request.Headers.Remove("access-token");
				var authClient = provider.GetRequiredService<IHdBankAuthService>();
				await authClient.OAuth2Async();
			});
	}
	
}
