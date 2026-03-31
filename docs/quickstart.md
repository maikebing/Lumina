# Lumina Quick Start

## Packages

```powershell
dotnet add package Lumina.Core
dotnet add package Lumina.WinForms
dotnet add package Lumina.Wpf
dotnet add package Lumina.Forms
```

## WinForms

```csharp
using Lumina.WinForms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        this.SetMica();
    }
}
```

## WPF

```xml
<Window xmlns:lumina="clr-namespace:Lumina.Wpf;assembly=Lumina.Wpf"
        lumina:Effect.Kind="Mica">
</Window>
```

## LuminaForms

```csharp
using Lumina.Forms;

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        ApplicationConfiguration.Initialize();
        Application.ConfigureVisualStyles(settings =>
        {
            settings.ThemeMode = ThemeMode.System;
            settings.ApplyBackdropEffects = true;
            settings.PreferredVisualStyle = VisualStyleKind.System;
        });

        return Application.Run(new DemoForm());
    }
}
```

## LuminaForms Autoscaling

```csharp
public sealed class MainForm : Form
{
    public MainForm()
    {
        AutoScaleMode = AutoScaleMode.Font;
        AutoScaleDimensions = new SizeF(8F, 20F);
    }
}
```

## Theme Files

```csharp
using Lumina.Forms;

Application.LoadTheme("themes/lumina-native-dark.json");

using var form = new MainForm();
form.UseTheme(NativeTheme.CreateDarkTheme());
form.SetPalette(new ThemePalette
{
    Accent = 0xFFFF7A00,
    FocusBorder = 0xFFFF7A00,
});
```

Sample theme files are included under `themes/nativeforms/` in the repository root.

## Analyzer Wiring

Inside this repository, add the analyzer project as an analyzer reference for LuminaForms targets:

```xml
<ItemGroup Condition="'$(UseLuminaForms)' == 'true'">
  <ProjectReference Include="..\Lumina.Forms.Analyzers\Lumina.Forms.Analyzers.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

## Native AOT Publish

```powershell
dotnet publish NativeFormsDemo/NativeFormsDemo.csproj `
  -f net10.0 `
  -r win-x64 `
  -c Release `
  /p:PublishAot=true
```

Use `net10.0-windows` when you want the WinForms designer-friendly demo target. Use `net10.0` when you want the LuminaForms target for AOT.
