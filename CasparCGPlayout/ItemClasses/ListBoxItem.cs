using CasparCGPlayout.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CasparCGPlayout.ItemClasses
{
   
        public enum TypeEnum
        {
            Video,
            CG
        }

        class ListBoxItem
        {
            private Image _whatnextimage;
            private readonly WhatNextEnum _whatnext;
            Rectangle _timeStartBounds;
            Rectangle _clipIdBounds;
            Rectangle _display1Bounds;
            Rectangle _display2Bounds;
            Rectangle _lengthOfClipBounds;
           
            

            public ListBoxItem(int id, string timestart, string clipid, string display1, string display2, TimeSpan lengthofclip, TypeEnum type, WhatNextEnum whatnext, int inframes, int outframes, Image itemImage)
            {
                isPlaying = false;
                this.id = id;
                timeStart = timestart;
                clipID = clipid;
                this.display1 = display1;
                this.display2 = display2;
                lengthOfClip = lengthofclip;
                this.type = type;
                _whatnext = whatnext;
                inFrames = inframes;
                outFrames = outframes;
                this.itemImage = itemImage;
                
                
            }

            public int id { get; set; }
            public string timeStart { get; set; }
            public string clipID { get; set; }
            public string display1 { get; set; }
            public string display2 { get; set; }
            public TimeSpan lengthOfClip { get; set; }
            public TypeEnum type { get; set; }
            public bool isPlaying { get; set; }
            public int inFrames { get; set; }
            public int outFrames { get; set; }
            public Image itemImage { get; set; }
            
            public void drawItem(DrawItemEventArgs e, Padding margin,
                                 Font timeStartFont, Font clipIDFont, Font displayFont, Font lengthOfClipFont, StringFormat aligment,
                                 Size imageSize)
            {
                if (timeStartFont == null) throw new ArgumentNullException("timeStartFont");
                //logger.Info(String.Format("{0}, {1}, {2}, {3}", e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
                //e.Graphics.Clip = new Region(new Rectangle(32, 32, 32, 32));

                // if selected, mark the background differently
                if (((e.State & DrawItemState.Selected) == DrawItemState.Selected) && !isPlaying)
                {
                    e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
                }
                else
                {
                    switch (type)
                    {
                        case TypeEnum.Video:
                            e.Graphics.FillRectangle(Brushes.LightPink, e.Bounds);
                            break;
                        case TypeEnum.CG:
                            e.Graphics.FillRectangle(Brushes.PaleVioletRed, e.Bounds);
                            break;
                    }

                }

                if (isPlaying)
                {
                    e.Graphics.FillRectangle(Brushes.DarkOrange, e.Bounds);
                    _whatnextimage = Image.FromStream(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("CasparCGPlayout.images.play.png"));
                }
                else
                {
                    switch (_whatnext)
                    {
                        case WhatNextEnum.Playnext:
                            _whatnextimage = Image.FromStream(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("CasparCGPlayout.images.greendot.png"));
                            break;
                        case WhatNextEnum.Wait:
                            _whatnextimage = Image.FromStream(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("CasparCGPlayout.images.redsquare.png"));
                            break;
                    }
                }
                    
                   

                // draw some item separator
                e.Graphics.DrawLine(Pens.DarkGray, e.Bounds.X, e.Bounds.Y, e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

                // draw item image
                e.Graphics.DrawImage(this.itemImage, e.Bounds.X + margin.Left, e.Bounds.Y + margin.Top, imageSize.Width, imageSize.Height);

                //Draw Type Image
                if (_whatnextimage != null)
                {
                    e.Graphics.DrawImage(_whatnextimage, e.Bounds.Width - imageSize.Width - margin.Right, e.Bounds.Y + margin.Top, imageSize.Width, imageSize.Height);
                }
               
                // calculate bounds for title text drawing
                _timeStartBounds = new Rectangle(e.Bounds.X + margin.Horizontal + imageSize.Width,
                                                      e.Bounds.Y + margin.Top,
                                                      e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                      (int)timeStartFont.GetHeight() + 2);

                // calculate bounds for details text drawing
                _clipIdBounds = new Rectangle(e.Bounds.X + margin.Horizontal + imageSize.Width,
                                                       e.Bounds.Y + (int)timeStartFont.GetHeight() + 2 + margin.Vertical + margin.Top,
                                                       e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                       e.Bounds.Height - margin.Bottom - (int)timeStartFont.GetHeight() - 2 - margin.Vertical - margin.Top);

                if (display2 == "")
                {

                    _display1Bounds = new Rectangle((e.Bounds.X + margin.Horizontal) + 220, (e.Bounds.Y + (int)displayFont.GetHeight() + 5), e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                          (int)displayFont.GetHeight() + 2);
                }
                else
                {
                    _display1Bounds = new Rectangle((e.Bounds.X + margin.Horizontal) + 220, (e.Bounds.Y + (int)displayFont.GetHeight() - 5), e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                          (int)displayFont.GetHeight() + 2);
                }

                _display2Bounds = new Rectangle((e.Bounds.X + margin.Horizontal) + 220, (e.Bounds.Y + (int)displayFont.GetHeight() + 15), e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                      (int)displayFont.GetHeight() + 2);

                _lengthOfClipBounds = new Rectangle(e.Bounds.X + margin.Horizontal + imageSize.Width,
                                                   e.Bounds.Y + margin.Top + 39,
                                                    e.Bounds.Width - margin.Right - imageSize.Width - margin.Horizontal,
                                                      (int)timeStartFont.GetHeight() + 2);



                // draw the text within the bounds
                e.Graphics.DrawString("DUE START: " + timeStart, timeStartFont, Brushes.Black, _timeStartBounds, aligment);
                e.Graphics.DrawString(clipID, clipIDFont, Brushes.Black, _clipIdBounds, aligment);
                e.Graphics.DrawString(display1, displayFont, Brushes.Black, _display1Bounds, aligment);
                e.Graphics.DrawString(display2, displayFont, Brushes.Black, _display2Bounds, aligment);
                e.Graphics.DrawString(TimeUtils.fix0Padding(lengthOfClip.Hours) + ":" + TimeUtils.fix0Padding(lengthOfClip.Minutes) + ":" + TimeUtils.fix0Padding(lengthOfClip.Seconds), lengthOfClipFont, Brushes.Black, _lengthOfClipBounds, aligment);


                // put some focus rectangle
                e.DrawFocusRectangle();

            }

            internal List<Rectangle> getRects()
            {
                var rects = new List<Rectangle> {_timeStartBounds, _clipIdBounds, _display1Bounds, _display2Bounds, _clipIdBounds};
                return rects;

            }
        }

}
