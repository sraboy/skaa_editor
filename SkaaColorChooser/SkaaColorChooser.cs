using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace SkaaColorChooser
{
    public class ActiveColorChangedEventArgs : EventArgs
    {
        public Color PreviousColor, NewColor;

        public ActiveColorChangedEventArgs(Color prevColor, Color newColor)
        {
            PreviousColor = prevColor;
            NewColor = newColor;
        }
    }

    public partial class SkaaColorChooser : UserControl
    {
        private Color _activeColor;// = Color.Black; //todo: have to make the button active

        public event EventHandler ActiveColorChanged;

        public ColorPalette Palette
        {
            get;
            set;
        }
        public Color ActiveColor
        {
            get
            {
                return this._activeColor;
            }
            set
            {
                if(this._activeColor != value)
                {
                    Color prevColor = _activeColor;
                    this._activeColor = value;
                    this.OnActiveColorChanged(new ActiveColorChangedEventArgs(prevColor, _activeColor));
                }

            }
        }

        protected virtual void OnActiveColorChanged(ActiveColorChangedEventArgs e)
        {
            EventHandler handler = ActiveColorChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private Button ActiveButton;

        private List<Button> ColorBoxes;

        public SkaaColorChooser()
        {
            InitializeComponent();

            ColorBoxes = new List<Button>();

            #region Dynamic Button Creation
            //I've gone math retarded and can't get the locations right
            //Taking a break and just manually created 256 buttons with some copy/paste

            //int btn_height = 18, btn_width = 18, btn_space = 3;

            //for (int i = 0; i < 256; i++)
            //{
            //    Button btn = new Button();
            //    btn.Size = new Size(btn_width, btn_height);
            //    btn.BackColor = Color.Transparent;
            //    btn.FlatAppearance.BorderColor = Color.White;
            //    btn.FlatStyle = FlatStyle.Flat;

            //    this.ColorBoxes.Add(btn);
            //}

            //ColorBoxes[0].Location = new Point(3, 3);
            //this.Controls.Add(ColorBoxes[0]);

            //int rows = 32, columns = 8; //32*8 = 256 colors
            //for (int y = 0; y < rows; y++)
            //{
            //    int x = 0;

            //    if (y == 0) //pre-made the first one already, so skip it
            //        x = 1;  //this allows the inner loop to remain simpler
            //                //and reduces the work being done 256x
            //                //doing x = 1 below would skip 0th position

            //    for (; x < columns; x++)
            //    {
            //        Controls.Add(ColorBoxes[y * columns + x]);
            //        Point p = ColorBoxes[(y * columns + x) - 1].Location; //get previous box's loc
            //        ColorBoxes[y * columns + x].Location = new Point(p.X + (x * (btn_width + btn_space)), p.Y + (y * (btn_space + btn_height)));
            //    }
            //}
            #endregion

            //disable until a palette is loaded
            foreach (Button btn in this.Controls)
                btn.Enabled = false;
        }

        public void LoadPalette(String Path)
        {
            ColorPalette pal = new Bitmap(50, 50, PixelFormat.Format8bppIndexed).Palette;// = new ColorPalette();

            FileStream fs = File.OpenRead(Path);
            fs.Seek(8, SeekOrigin.Begin);

            for (int i = 0; i < 256; i++)
            {
                int r = fs.ReadByte();
                int g = fs.ReadByte();
                int b = fs.ReadByte();

                pal.Entries[i] = Color.FromArgb(255, r, g, b);
            }

            this.Palette = pal;

            SetupColorBoxes();
        }

        private void SetupColorBoxes()
        {
            for(int i = 0; i < 256; i++)
            {
                Button btn = this.Controls[i] as Button;
                btn.BackColor = Palette.Entries[i];
                btn.Enabled = true;

                btn.Click += btnColorBox_Click;
            }
        }

        private void btnColorBox_Click(object sender, EventArgs e)
        {
            int btnSizeOffset = 30;
            int btnLocOffset = 16;

            Button btn = sender as Button;

            if (btn != ActiveButton)
            {
                if(ActiveButton != null) //for the first time
                { 
                    //return previous active color button to normal
                    this.ActiveButton.Size = btn.Size;
                    this.ActiveButton.Location = new Point(this.ActiveButton.Location.X + btnLocOffset, this.ActiveButton.Location.Y + btnLocOffset);
                    this.ActiveButton.FlatAppearance.BorderColor = Color.White;
                    this.ActiveButton.FlatAppearance.BorderSize = 1;
                    this.ActiveButton.SendToBack();
                }

                this.ActiveColor = btn.BackColor;

                //set the new button and make it stand out
                this.ActiveButton = btn;
                this.ActiveButton.Size = new Size(btn.Size.Height + btnSizeOffset, btn.Size.Width + btnSizeOffset);
                this.ActiveButton.Location = new Point(btn.Location.X - btnLocOffset, btn.Location.Y - btnLocOffset);
                this.ActiveButton.FlatAppearance.BorderColor = Color.GreenYellow;
                this.ActiveButton.FlatAppearance.BorderSize = 3;
                this.ActiveButton.BringToFront();
            }
        }
    }
}
