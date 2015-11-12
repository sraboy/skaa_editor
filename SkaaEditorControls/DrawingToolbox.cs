using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace SkaaEditorControls
{
    public partial class DrawingToolbox : UserControl
    {
        [NonSerialized]
        private EventHandler _selectedToolChanged;
        public event EventHandler SelectedToolChanged
        {
            add
            {
                if (_selectedToolChanged == null || !_selectedToolChanged.GetInvocationList().Contains(value))
                {
                    _selectedToolChanged += value;
                }
            }
            remove
            {
                _selectedToolChanged -= value;
            }
        }
        protected virtual void OnSelectedToolChanged(DrawingToolSelectedEventArgs e)
        {
            EventHandler handler = _selectedToolChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private DrawingTools _selectedTool = DrawingTools.None;
        public DrawingTools SelectedTool
        {
            get
            {
                return this._selectedTool;
            }
            private set
            {
                if (this._selectedTool != value)
                {
                    this._selectedTool = value;
                    OnSelectedToolChanged(new DrawingToolSelectedEventArgs(this._selectedTool));
                }
            }
        }
        private Cursor _toolCursor;
        public Cursor ToolCursor
        {
            get
            {
                return this._toolCursor;
            }
            private set
            {
                if (this._toolCursor != value)
                    this._toolCursor = value;
            }
        }


        private Stream _panToolCursorStream;
        private Stream _pencilToolCursorStream;
        private Stream _paintBucketToolCursorStream;

        public DrawingToolbox()
        {
            InitializeComponent();
            this._selectedTool = DrawingTools.None;
            this._panToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PanToolCursor.cur"));
            this._pencilToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PencilToolCursor.cur"));
            this._paintBucketToolCursorStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PaintBucketToolCursor.cur"));
        }

        private void btnPanTool_Click(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {
                //var resourceStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PanToolCursor.cur"));
                this.ToolCursor = new Cursor(this._panToolCursorStream);
                this.SelectedTool = DrawingTools.Pan;
                this._panToolCursorStream.Position = 0;
            }
            else
            {
                this.ToolCursor = null;
                this.SelectedTool = DrawingTools.None;
            }

            ToggleCheckBoxes(sender);
        }

        private void btnPencilTool_Click(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked == true)
            {
                //var resourceStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PencilToolCursor.cur"));
                this.ToolCursor = new Cursor(this._pencilToolCursorStream);
                this.SelectedTool = DrawingTools.Pencil;
                this._pencilToolCursorStream.Position = 0;
            }
            else
            {
                this.ToolCursor = null;
                this.SelectedTool = DrawingTools.None;
            }

            ToggleCheckBoxes(sender);
        }

        private void btnFillTool_Click(object sender, EventArgs e)
        {
            if((sender as CheckBox).Checked == true)
            { 
                //var resourceStream = this.GetType().Assembly.GetManifestResourceStream(string.Concat(this.GetType().Assembly.GetName().Name, ".Resources.PaintBucketToolCursor.cur"));
                this.ToolCursor = new Cursor(this._paintBucketToolCursorStream);
                this.SelectedTool = DrawingTools.PaintBucket;
                this._paintBucketToolCursorStream.Position = 0;
            }
            else
            {
                this.ToolCursor = null;
                this.SelectedTool = DrawingTools.None;
            }

            ToggleCheckBoxes(sender);
        }

        private void ToggleCheckBoxes(object sender)
        {
            CheckBox cb = sender as CheckBox;

            foreach(CheckBox c in this.Controls)
            {
                //don't change the sender, set others to false
                c.Checked = c.Name == cb.Name ? c.Checked : false;
            }
        }
    }
}

 


