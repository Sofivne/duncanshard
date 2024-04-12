using DuncanShard.Utils;
using Microsoft.AspNetCore.Http;

namespace DuncanShard.UnitTests.ModelsUT;

public class UtilsUnitTests
{
    

    // Vérifie si la méthode GetHeader renvoie la valeur d'en-tête lorsque l'en-tête existe.
    [Fact]
    public void GetHeader_WhenHeaderExists_ReturnsHeaderValue()
    {
        var request = new DefaultHttpContext().Request;
        request.Headers["HeaderCustom"] = "ResponseCustom";
        var res = Utils.Utils.GetHeader(request, "HeaderCustom");
        Assert.Equal("ResponseCustom", res);
    }

    // Vérifie si la méthode GetHeader renvoie null lorsque l'en-tête n'existe pas.
    [Fact]
    public void GetHeader_WhenHeaderDoesNotExist_ReturnsNull()
    {
        var request = new DefaultHttpContext().Request;
        var res = Utils.Utils.GetHeader(request, "NonExistingHeader");
        Assert.Null(res);
    }
}