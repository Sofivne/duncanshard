/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using System.Text;
using DuncanShard.Utils;

namespace DuncanShard.Auth;

public class Auth
{
        /// <summary>
        /// Gets user role from "Authorization" request header.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>Null if "Authorization" header doesn't exist. Otherwise, it'll return "admin" if bearer token is ok, "user" if not.</returns>
        public static Role GetUserRole(HttpRequest request)
        {
            string ADMIN_USERNAME = "admin", ADMIN_PASSWORD = "password";
            string WORMHOLE_USERNAME = "shard-fake-remote", WORMHOLE_PASSWORD = "caramba";
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            var encodedUsernamePassword = authHeader?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

            if (authHeader is null || !authHeader.StartsWith("Basic ") || encodedUsernamePassword is null)
                return Role.NotAuthenticated;

            var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
            var username = decodedUsernamePassword.Split(':', 2)[0];
            var password = decodedUsernamePassword.Split(':', 2)[1];
            if (username == ADMIN_USERNAME && password == ADMIN_PASSWORD)
                return Role.Admin;
            
            // var shardPattern = new Regex("shard-server-([0-9]+)");
            if (username == WORMHOLE_USERNAME && password == WORMHOLE_PASSWORD)
                return Role.Shard;  
            return Role.User;
        }

}