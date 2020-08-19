using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Gwen;
using Gwen.Controls;
using linerider.Tools;
using linerider.Utils;
using linerider.IO;
using OpenTK;
using linerider.Game.LineGenerator;

namespace linerider.UI
{
    public class GeneratorWindow : DialogBase
    {
        private CollapsibleList _typecontainer;
        private ControlBase _focus;
        private int _tabscount = 0;
        private bool GeneratePressed = false;
        private GeneratorType _currentgen;

        private ComboBox GeneratorTypeBox;
        private Panel GeneratorOptions;

        private ControlBase CircleGenOptions;
        private Spinner CircleRadius;
        private Spinner CircleLineCount;
        private Spinner CircleOffsetX;
        private Spinner CircleOffsetY;
        private Checkbox CircleInverse;

        private ControlBase TenPCOptions;
        private Spinner TenPCX;
        private Spinner TenPCY;
        private Spinner TenPCRotation;

        

        private CircleGenerator gen_Circle;
        private TenPCGenerator gen_10pc;

        private GeneratorType CurrentGenerator
        {
            get{
                return _currentgen;
            }
            set
            {
                Render_Clear();
                _currentgen = value;
                Render_Preview();
            }
        }

        public GeneratorWindow(GameCanvas parent, Editor editor, Vector2d pos) : base(parent, null)
        {
            Title = $"Line Generator";
            SetSize(450, 500);
            DisableResizing();

            /*_typecontainer = new CollapsibleList(this)
            {
                Dock = Dock.Left,
                AutoSizeToContents = false,
                Width = 100,
                Margin = new Margin(0, 0, 5, 0)
            };*/

            MakeModal(true);

            gen_Circle = new CircleGenerator("Circle Generator", 10.0, pos, 50, false);
            gen_10pc = new TenPCGenerator("10PC Generator", new Vector2d(1.0, 1.0), 0.0);
            gen_10pc.Generate_Preview();

            CurrentGenerator = GeneratorType.TenPC;
            Setup();
        }

        protected override bool OnKeyEscape(bool down)
        {
            gen_Circle.DeleteLines();
            Console.WriteLine("ESCAPE!");
            Close();
            return true;
        }
        protected override bool OnKeyReturn(bool down)
        {
            if (down)
            {
                gen_Circle.DeleteLines();
                Console.WriteLine("ESCAPE!");
                Close();
            }
            return true;
        }
        private void Setup()
        {
            ControlBase top = new ControlBase(this)
            {
                Dock = Dock.Top,
                AutoSizeToContents = true,
            };

            ControlBase panel = new ControlBase(this)
            {
                Margin = new Margin(0, 0, 0, 0),
                Dock = Dock.Fill,
                AutoSizeToContents = true
            };

            ControlBase bottom = new ControlBase(this)
            {
                Dock = Dock.Bottom,
                AutoSizeToContents = true,
            };

            GeneratorOptions = new Panel(panel)
            {
                Dock = Dock.Fill
            };

            Button generate = new Button(bottom)
            {
                Dock = Dock.Left,
                Margin = new Margin(0, 2, 0, 0),
                Text = "Generate"
            };
            generate.Clicked += (o, e) =>
            {
                Render_Final();
                GeneratePressed = true;
                Close();
            };

            Populate10pc();
            PopulateCircle();

            GeneratorTypeBox = GwenHelper.CreateLabeledCombobox(top, "Generator Type:");
            GeneratorTypeBox.Dock = Dock.Top;
            var tenpc = GeneratorTypeBox.AddItem("10-Point Cannon", "", GeneratorType.TenPC);
            tenpc.CheckChanged += (o, e) =>
            {
                GeneratorOptions.Children.Clear();
                TenPCOptions.Parent = GeneratorOptions;
                CurrentGenerator = GeneratorType.TenPC;
            };
            var circle = GeneratorTypeBox.AddItem("Circle", "", GeneratorType.Circle);
            circle.CheckChanged += (o, e) =>
            {
                GeneratorOptions.Children.Clear();
                CircleGenOptions.Parent = GeneratorOptions;
                CurrentGenerator = GeneratorType.Circle;
            };

            GeneratorTypeBox.SelectedItem = tenpc;
            GeneratorOptions.Children.Clear();
            TenPCOptions.Parent = GeneratorOptions;
        }

        private void PopulateCircle()
        {
            CircleGenOptions = new ControlBase(null)
            {
                Margin = new Margin(0, 0, 0, 0),
                Dock = Dock.Top,
                AutoSizeToContents = true
            };
            //Panel configPanel = GwenHelper.CreateHeaderPanel(CircleGenOptions, "Configure Circle Properties");
            CircleRadius = new Spinner(null)
            {
                Min = 0.0,
                Max = 1.0e9,
                Value = gen_Circle.radius
            };
            CircleRadius.ValueChanged += (o, e) =>
            {
                gen_Circle.radius = CircleRadius.Value;
                gen_Circle.ReGenerate_Preview();
            };
            

            CircleLineCount = new Spinner(null)
            {
                Min = 3.0,
                Max = 1.0e7,
                Value = gen_Circle.lineCount
            };
            CircleLineCount.ValueChanged += (o, e) =>
            {
                gen_Circle.lineCount = (int)CircleLineCount.Value;
                gen_Circle.ReGenerate_Preview();
            };

            CircleOffsetX = new Spinner(null)
            {
                Min = -30000000000,
                Max = 30000000000,
                Value = gen_Circle.position.X
            };
            CircleOffsetX.ValueChanged += (o, e) =>
            {
                gen_Circle.position.X = CircleOffsetX.Value;
                gen_Circle.ReGenerate_Preview();
            };
            CircleOffsetY = new Spinner(null)
            {
                Min = -30000000000,
                Max = 30000000000,
                Value = gen_Circle.position.Y
            };
            CircleOffsetY.ValueChanged += (o, e) =>
            {
                gen_Circle.position.Y = CircleOffsetY.Value;
                gen_Circle.ReGenerate_Preview();
            };

            GwenHelper.CreateLabeledControl(CircleGenOptions, "Radius", CircleRadius);
            GwenHelper.CreateLabeledControl(CircleGenOptions, "Line Count", CircleLineCount);
            GwenHelper.CreateLabeledControl(CircleGenOptions, "Centre X", CircleOffsetX);
            GwenHelper.CreateLabeledControl(CircleGenOptions, "Centre Y", CircleOffsetY);
            CircleInverse = GwenHelper.AddCheckbox(CircleGenOptions, "Invert", gen_Circle.invert, (o, e) =>
            {
                gen_Circle.invert = ((Checkbox)o).IsChecked;
                gen_Circle.ReGenerate_Preview();
            });
        }

        private void Populate10pc()
        {
            TenPCOptions = new ControlBase(null)
            {
                Margin = new Margin(0, 0, 0, 0),
                Dock = Dock.Top,
                AutoSizeToContents = true
            };
            TenPCX = new Spinner(null)
            {
                Min = -1.0e9,
                Max = 1.0e9,
                Value = gen_10pc.speed.X
            };
            TenPCX.ValueChanged += (o, e) =>
            {
                gen_10pc.speed.X = TenPCX.Value;
                gen_10pc.ReGenerate_Preview();
            };
            TenPCY = new Spinner(null)
            {
                Min = -1.0e9,
                Max = 1.0e9,
                Value = gen_10pc.speed.Y
            };
            TenPCY.ValueChanged += (o, e) =>
            {
                gen_10pc.speed.Y = TenPCY.Value;
                gen_10pc.ReGenerate_Preview();
            };
            TenPCRotation = new Spinner(null)
            {
                Min = -1.0e9,
                Max = 1.0e9,
                Value = gen_10pc.rotation,
                IncrementSize = 5
            };
            TenPCRotation.ValueChanged += (o, e) =>
            {
                gen_10pc.rotation = TenPCRotation.Value;
                gen_10pc.ReGenerate_Preview();
            };

            GwenHelper.CreateLabeledControl(TenPCOptions, "X Speed", TenPCX);
            GwenHelper.CreateLabeledControl(TenPCOptions, "Y Speed", TenPCY);
            GwenHelper.CreateLabeledControl(TenPCOptions, "Rotation Amount", TenPCRotation);
        }

        private void CategorySelected(object sender, ItemSelectedEventArgs e)
        {
            if (_focus != e.SelectedItem.UserData)
            {
                if (_focus != null)
                {
                    _focus.Hide();
                }
                _focus = (ControlBase)e.SelectedItem.UserData;
                _focus.Show();
                Settings.SettingsPane = (int)_focus.UserData;
                Settings.Save();
            }
        }
        
        private ControlBase AddPage(CollapsibleCategory category, string name)
        {
            var btn = category.Add(name);
            Panel panel = new Panel(this);
            panel.Dock = Dock.Fill;
            panel.Padding = Padding.Five;
            panel.Hide();
            panel.UserData = _tabscount;
            btn.UserData = panel;
            category.Selected += CategorySelected;
            if (_tabscount == Settings.SettingsPane)
                btn.Press();
            _tabscount += 1;
            return panel;
        }

        public override bool Close()
        {
            return base.Close();
        }

        protected override void CloseButtonPressed(ControlBase control, EventArgs args)
        {
            if (!GeneratePressed)
            {
                Render_Clear();
            }
            base.CloseButtonPressed(control, args);
        }

        private void Render_Preview() //Renders the generator's preview lines
        {
            switch (CurrentGenerator)
            {
                default:
                    break;
                case GeneratorType.Circle:
                    gen_Circle.ReGenerate_Preview();
                    break;
                case GeneratorType.TenPC:
                    gen_10pc.ReGenerate_Preview();
                    break;
            }
        }
        private void Render_Final() //Renders the generator's final lines (which are the ones actually added to the track)
        {
            switch (CurrentGenerator)
            {
                default:
                    break;
                case GeneratorType.Circle:
                    gen_Circle.DeleteLines();
                    gen_Circle.Generate();
                    break;
                case GeneratorType.TenPC:
                    gen_10pc.DeleteLines();
                    gen_10pc.Generate();
                    break;
            }
        }
        private void Render_Clear() //Clears all lines rendered by the current generator
        {
            switch (CurrentGenerator)
            {
                default:
                    break;
                case GeneratorType.Circle:
                    gen_Circle.DeleteLines();
                    break;
                case GeneratorType.TenPC:
                    gen_10pc.DeleteLines();
                    break;
            }
        }
    }
}
