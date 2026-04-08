using System;
using System.Windows.Forms;
using System.Drawing;

ApplicationConfiguration.Initialize();
using var menuStrip = new MenuStrip();
var file = new ToolStripMenuItem("нд╪Ч");
var view = new ToolStripMenuItem("йсм╪");
menuStrip.Items.AddRange(new ToolStripItem[]{ file, view });
using var form = new Form();
form.MainMenuStrip = menuStrip;
form.Controls.Add(menuStrip);
form.CreateControl();
menuStrip.CreateControl();
menuStrip.PerformLayout();
Console.WriteLine($"MenuStrip.Height={menuStrip.Height}");
Console.WriteLine($"File.Bounds={file.Bounds.X},{file.Bounds.Y},{file.Bounds.Width},{file.Bounds.Height}");
Console.WriteLine($"View.Bounds={view.Bounds.X},{view.Bounds.Y},{view.Bounds.Width},{view.Bounds.Height}");
