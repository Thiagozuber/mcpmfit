using Microsoft.Playwright;

namespace MfitMcpServer.Infra;

public static class MfitSessao
{
    // URL exata de login do MFIT
    public const string UrlLogin = "https://app.mfitpersonal.com.br/user";
    public const string UrlLista = "https://user.mfitpersonal.com.br/user/client/list";

    // Pasta onde o navegador guarda a sessão (fica fora do repositório)
    public static string PastaPerfil()
    {
        var env = Environment.GetEnvironmentVariable("MFIT_PROFILE_DIR");
        if (!string.IsNullOrWhiteSpace(env)) return env;

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MfitMcpServer",
            "profile");
    }

    // Abre o navegador para você entrar no MFIT à mão. Nenhuma senha é guardada pelo programa.
    public static async Task LoginManualAsync()
    {
        var pasta = PastaPerfil();
        Directory.CreateDirectory(pasta);

        using var playwright = await Playwright.CreateAsync();
        await using var context = await playwright.Chromium.LaunchPersistentContextAsync(
            pasta,
            new BrowserTypeLaunchPersistentContextOptions { Headless = false });

        var page = context.Pages.FirstOrDefault() ?? await context.NewPageAsync();
        await page.GotoAsync(UrlLogin, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

        Console.Error.WriteLine("\n=======================================================");
        Console.Error.WriteLine("1. Faça o login no MFIT na janela do Chromium.");
        Console.Error.WriteLine("2. Navegue até a lista de alunos.");
        Console.Error.WriteLine("3. Quando a lista de alunos estiver visível na tela,");
        Console.Error.WriteLine("   volte neste terminal e aperte ENTER para finalizar.");
        Console.Error.WriteLine("=======================================================\n");
        Console.ReadLine();
    }
}