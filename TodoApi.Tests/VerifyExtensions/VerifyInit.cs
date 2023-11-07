using System.Runtime.CompilerServices;

namespace TodoApi.VerifyExtensions;

internal class VerifyInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ClipboardAccept.Enable();
        VerifyHttp.Initialize();
        VerifySemanticJson.Initialize();
        VerifyAlba.Initialize();
    }
}