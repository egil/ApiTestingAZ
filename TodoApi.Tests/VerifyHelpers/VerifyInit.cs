using System.Runtime.CompilerServices;

namespace Api.VerifyHelpers;

internal class VerifyInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        ClipboardAccept.Enable();
        VerifyHttp.Initialize();
        VerifySemanticJson.Initialize();
    }
}