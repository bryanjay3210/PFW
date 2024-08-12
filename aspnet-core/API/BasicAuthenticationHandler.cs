using System.Text;

namespace API
{
    public class BasicAuthenticationHandler
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly string _realm;

        public BasicAuthenticationHandler(RequestDelegate requestDelegate, string realm)
        {
            _requestDelegate = requestDelegate;
            _realm = realm;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != "/api/Auth/RequestToken")
            {
                await _requestDelegate(context);
                return;
            }

            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var header = context.Request.Headers["Authorization"].ToString();
            var encodedCreds = header.Substring(6);
            
            if (encodedCreds == "Basic")
            {
                var creds = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCreds));
                string userId = creds.Split(':')[0];
                string password = creds.Split(':')[1];

                if (userId != "testuser" || password != "P@ssW0rd1973")
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }

            await _requestDelegate(context);
        }
    }
}
