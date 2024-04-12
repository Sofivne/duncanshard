/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using System.Text;

namespace DuncanShard.Utils;

public static class Utils
{
        
        /// <summary>
        /// Gets a header from the request.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="header">The header name you want to find.</param>
        /// <returns>Null if header doesn't exist. The header value otherwise.</returns>
        public static string? GetHeader(HttpRequest request, string header) => request.Headers[header].FirstOrDefault();
    
}