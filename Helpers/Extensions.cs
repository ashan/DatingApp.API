using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
  public static class Extensions
  {
    public static async Task AddApplicationError(this HttpResponse response, string message)
    {
      response.StatusCode = (int)HttpStatusCode.InternalServerError;
      response.Headers.Add("Application-Error", message);
      response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
      response.Headers.Add("Access-Control-Allow-Origin", "*");
      await response.WriteAsync(message);
    }
  }
}