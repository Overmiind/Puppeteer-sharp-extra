using System.Net.Http;
using System.Text;

namespace PuppeteerExtraSharpLite;

public static class Extensions {
	/// <summary>
	/// Sends an HTTP request and retries based on the provided conditions.
	/// If the response does not meet the retry condition, it returns immediately.
	/// </summary>
	/// <param name="client"></param>
	/// <param name="httpRequestMessageFactory"></param>
	/// <param name="shouldRetry"></param>
	/// <param name="retries">Amount of retries</param>
	/// <param name="delay">Delay in milliseconds between each retry</param>
	/// <exception cref="HttpRequestException"></exception>
	/// <remarks>
	/// Use a closure to get the value of the response.
	/// </remarks>
	public static async Task SendPollingAsync(
		this HttpClient client,
		Func<HttpRequestMessage> httpRequestMessageFactory,
		Func<HttpResponseMessage, Task<bool>> shouldRetry,
		int retries = 5,
		int delay = 1000
	) {
		for (; retries >= 0; retries--) {
			using var request = httpRequestMessageFactory();

			HttpResponseMessage? response = null;

			try {
				response = await client.SendAsync(request);

				var shouldRetryResult = await shouldRetry(response);

				if (!shouldRetryResult) {
					return;
				}
			} catch { }

			response?.Dispose();

			if (delay > 0) {
				await Task.Delay(delay);
			}
		}

		throw new HttpRequestException("Failed to get a valid response after retries.");
	}

	/// <summary>
	/// Adds the given query parameters to the URL.
	/// </summary>
	/// <param name="queryParameters"></param>
	/// <param name="url"></param>
	/// <returns></returns>
	public static string AddAsQueryTo(this Dictionary<string, string> queryParameters, string url) {
		if (queryParameters.Count == 0) {
			return url;
		}

		var sb = new StringBuilder(url);
		var separator = url.Contains('?') ? '&' : '?';
		sb.Append(separator);
		var first = true;
		foreach (var (key, value) in queryParameters) {
			if (!first) {
				sb.Append('&');
			} else {
				first = false;
			}
			sb.Append(Uri.EscapeDataString(key));
			sb.Append('=');
			sb.Append(Uri.EscapeDataString(value));
		}
		return sb.ToString();
	}
}