using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkaaEditorControls
{
    public class DrawingToolSelectedEventArgs : EventArgs
    {
        private DrawingTools _selectedTool;
        public DrawingTools SelectedTool
        {
            get
            {
                return this._selectedTool;
            }
            private set
            {
                if (this._selectedTool != value)
                    this._selectedTool = value;
            }
        }

        public DrawingToolSelectedEventArgs(DrawingTools selectedTool)
        {
            this.SelectedTool = selectedTool;
        }
    }
}
