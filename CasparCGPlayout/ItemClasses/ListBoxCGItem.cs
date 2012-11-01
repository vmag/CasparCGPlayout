using System;
using CasparCGPlayout.Utils;
using CasparCGPlayout.components;
using System.Drawing;

namespace CasparCGPlayout.ItemClasses
{
    class ListBoxCGItem : ListBoxItem
    {
        public ListBoxCGItem(int id, string timestart, string clipid, string display1, string display2, TimeSpan lengthofclip, TypeEnum type, WhatNextEnum whatnext, int inframes, int outframes, Image itemImage) : base (id, timestart, clipid, display1, display2, lengthofclip, type, whatnext, inframes, outframes, itemImage)
        {
            
        }

    }
}
