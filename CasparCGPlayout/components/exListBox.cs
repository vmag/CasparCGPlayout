using System.Drawing;
using System.Windows.Forms;
using CasparCGPlayout.ItemClasses;

namespace CasparCGPlayout.components
{

    public partial class ExListBox : ListBox
    {

        private readonly Size _imageSize;
        private readonly StringFormat _fmt;
        private readonly Font _timeStartFont;
        private readonly Font _clipIDFont;
        private readonly Font _nameOfClipFont;
        private readonly Font _lengthOfClipFont;

        public ExListBox(Font timeStartFont, Font clipIDFont, Font nameOfClipFont, Font lengthOfClipFont, Size imageSize, 
                         StringAlignment aligment, StringAlignment lineAligment)
        {
            _timeStartFont = timeStartFont;
            _clipIDFont = clipIDFont;
            _nameOfClipFont = nameOfClipFont;
            _lengthOfClipFont = lengthOfClipFont;
            _imageSize = imageSize;
            ItemHeight = _imageSize.Height + Margin.Vertical;
            _fmt = new StringFormat {Alignment = aligment, LineAlignment = lineAligment};
        }

        public ExListBox()
        {
            InitializeComponent();
            _imageSize = new Size(80,60);
            ItemHeight = _imageSize.Height + Margin.Vertical;
            _fmt = new StringFormat {Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near};
            _timeStartFont = new Font(Font, FontStyle.Bold);
            _clipIDFont = new Font(Font, FontStyle.Regular);
            _nameOfClipFont = new Font("Deja Vu MT", 12, FontStyle.Bold);
            _lengthOfClipFont = new Font(Font, FontStyle.Regular);
        }


        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // prevent from error Visual Designer
            if (Items.Count > 0)            
            {                
                var item = (ListBoxItem)Items[e.Index];
                item.drawItem(e, Margin, _timeStartFont, _clipIDFont, _nameOfClipFont, _lengthOfClipFont, _fmt, _imageSize);
            }                            
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
            
    }
}
