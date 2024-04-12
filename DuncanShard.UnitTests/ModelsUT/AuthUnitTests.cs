/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using DuncanShard.Utils;
using Microsoft.AspNetCore.Http;

namespace DuncanShard.UnitTests.ModelsUT;

public class AuthUnitTests
{
    // Teste si le rôle utilisateur est correctement extrait lorsque l'en-tête d'autorisation est "admin".
    [Fact]
    public void GetUserRole_WhenAuthorizationHeaderIsAdmin_ReturnsAdminRole()
    {
        var request = new DefaultHttpContext().Request;
        request.Headers["Authorization"] = "Basic YWRtaW46cGFzc3dvcmQ=";
        var role = Auth.Auth.GetUserRole(request);
        
        Assert.Equal(Role.Admin, role);
    }
    
    // Teste si le rôle utilisateur est correctement extrait lorsque l'en-tête d'autorisation est "shard".
    [Fact]
    public void GetUserRole_WhenAuthorizationHeaderIsShard_ReturnsShardRole()
    {
        var request = new DefaultHttpContext().Request;
        request.Headers["Authorization"] = "Basic c2hhcmQtZmFrZS1yZW1vdGU6Y2FyYW1iYQ=="; // shard-fake-remote:caramba
        var role = Auth.Auth.GetUserRole(request);

        Assert.Equal(Role.Shard, role);
    }
    
    // Vérifie si le rôle utilisateur est null lorsque l'en-tête d'autorisation est absent.
    [Fact]
    public void GetUserRole_WhenAuthorizationHeaderIsNull_ReturnsNotAuthenticated()
    {
        var request = new DefaultHttpContext().Request;
        var role = Auth.Auth.GetUserRole(request);
        Assert.Equal(Role.NotAuthenticated, role);
    }
    
    // Vérifie si le rôle utilisateur est null lorsque l'en-tête d'autorisation est absent.
    [Fact]
    public void GetUserRole_WhenAuthorizationHeaderIsNotKnown_ReturnsUser()
    {
        var request = new DefaultHttpContext().Request;
        request.Headers["Authorization"] = "Basic c2hhcmQtZmFrZS1yZW1vdGU6Y2FyYW1iYA=="; // shard-fake-remote:caramba
        var role = Auth.Auth.GetUserRole(request);
        Assert.Equal(Role.User, role);
    }
}