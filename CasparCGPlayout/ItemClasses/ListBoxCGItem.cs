using System;
using CasparCGPlayout.Utils;
using System.Drawing;

namespace CasparCGPlayout.ItemClasses
{
    class ListBoxCGItem : ListBoxItem
    {

        public ListBoxCGItem(int id, string timestart, string clipid, string display1, string display2, TimeSpan lengthofclip, TypeEnum type, WhatNextEnum whatnext, int inframes, int outframes, Image itemImage, CatergoryEnum category)
            : base(id, timestart)
        {
            
        }

    }
}
