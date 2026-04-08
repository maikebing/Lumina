using System;
using System.Windows.Forms;
using System.Drawing;

ApplicationConfiguration.Initialize();
using var form = new Form();
Console.WriteLine($"DefaultFont={Control.DefaultFont.Name},{Control.DefaultFont.SizeInPoints},{Control.DefaultFont.Style}");
Console.WriteLine($"FormFont={form.Font.Name},{form.Font.SizeInPoints},{form.Font.Style}");
Console.WriteLine($"AutoScaleMode={form.AutoScaleMode}");
Console.WriteLine($"CurrentAutoScaleDimensions={form.CurrentAutoScaleDimensions.Width},{form.CurrentAutoScaleDimensions.Height}");
Console.WriteLine($"AutoScaleDimensions={form.AutoScaleDimensions.Width},{form.AutoScaleDimensions.Height}");
