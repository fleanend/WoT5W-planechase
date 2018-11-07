using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;

namespace Dothraki_and_Fugue
{
    public class Card
    {
        public string Name;
        public string Path;
        public string ToolTip;
        public bool visited = false;

        public Android.Graphics.Drawables.BitmapDrawable Bm;

        public Card(string path, string name)
        {
            this.Path = path;
            this.Name = name;
        }

        public Card(Card card)
        {
            this.Path = card.Path;
            this.Name = card.Name;
        }

        public static Card MyRegex(string path)
        {
            char[] cName = path.ToCharArray();
            if (cName[0] == 'p' && cName[1] == 'h' && cName[2] == '_')
            { 
                return new Phenomenon(path, path.Replace(".png", "").Substring(3));
            }
            else
            {
                return new Plane(path, path.Replace(".png", ""));
            }
                
        }


        ~Card()
        {

        }
    }

}