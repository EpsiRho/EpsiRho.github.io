using EpsiDenTools.Classes;
using MelonUI;
using MelonUI.Default;
using MelonUI.Managers;
using System.Drawing;

ConsoleWindowManager manager = new ConsoleWindowManager();
manager.SetTitle("Epsi's Den DevTools");

OptionsMenu mainMenu = new OptionsMenu()
{
    X="0",
    Y="0",
    Width="50%",
    Height="70%",
};
mainMenu.Options.Add(new MenuItem("Add Music Recommendation", () =>
{
    Thread t = new Thread(() =>
    {
        mainMenu.IsVisible = false;
        MusicCardGenerator.CreateCard(manager);
        mainMenu.IsVisible = true;

    });
    t.Start();
}
));
mainMenu.Options.Add(new MenuItem("Add Custom Music Recommendation", () =>
{
    Thread t = new Thread(() =>
    {
        mainMenu.IsVisible = false;
        MusicCardGenerator.CreateCustomCard(manager);
        mainMenu.IsVisible = true;

    });
    t.Start();
}
));
mainMenu.Options.Add(new MenuItem("Pack Site Files", () =>
{
    Thread t = new Thread(() =>
    {
        mainMenu.IsVisible = false;
        MusicCardGenerator.PackMusicCards(manager);
        PostGenerator.PackPosts(manager);
        ProjectGenerator.PackProjects(manager);
        mainMenu.IsVisible = true;

    });
    t.Start();
}
));
mainMenu.Options.Add(new MenuItem("Add Blog Post", () =>
{
    Thread t = new Thread(() =>
    {
        mainMenu.IsVisible = false;
        PostGenerator.GenerateFromMarkdown(manager);
        mainMenu.IsVisible = true;

    });
    t.Start();
}
));
mainMenu.Options.Add(new MenuItem("Add Project Page", () =>
{
    Thread t = new Thread(() =>
    {
        mainMenu.IsVisible = false;
        ProjectGenerator.GenerateFromMarkdown(manager);
        mainMenu.IsVisible = true;

    });
    t.Start();
}
));
manager.AddElement(mainMenu);

var source = new CancellationTokenSource();
Thread t = new Thread(async () =>
{
    await manager.ManageConsole(source.Token);
});
t.Start();

while (!source.Token.IsCancellationRequested)
{

}