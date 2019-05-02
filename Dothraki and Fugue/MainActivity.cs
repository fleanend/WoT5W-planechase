using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Content.Res;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Util;
using Android.Views;
using System;
using ImageViews.Photo;
using Android.Content;
using System.Linq;
//using Android.Locations.GpsStatus;

namespace Dothraki_and_Fugue {

    [Activity(Label = "Wot5W", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Landscape)]

    public class MainActivity : Activity, GestureDetector.IOnGestureListener
    {
        public static Context mContext = Android.App.Application.Context;
        AppPreferences ap = new AppPreferences(mContext);
        GestureDetector _gestureDetector;
        Card _currentPlane;
        ImageView _currentPlaneView;
        ImageButton _phenomenon_button;
        ImageButton _upleft_button;
        ImageButton _downleft_button;
        AssetManager _assets;
        Dialog _dialog;
        List<Card> _cards;
        List<int> _cards_indices;
        int _index = 0;
        int _done = 0;

        List<string> _wild_effects;

        private PhotoViewAttacher _attacher;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            _assets = this.Assets;
            var content = _assets.List("Cards");
            _cards = new List<Card>();// = ArrayList<Card>
            _cards_indices = new List<int>();
            _currentPlaneView = FindViewById<ImageView>(Resource.Id.imageView1);
            _phenomenon_button = FindViewById<ImageButton>(Resource.Id.imageButton1);
            _upleft_button = FindViewById<ImageButton>(Resource.Id.imageButton2);
            _downleft_button = FindViewById<ImageButton>(Resource.Id.imageButton3);

            _phenomenon_button.Visibility = ViewStates.Invisible;
            _upleft_button.Visibility = ViewStates.Invisible;
            _downleft_button.Visibility = ViewStates.Invisible;

            _wild_effects = new List<string>();

            using (System.IO.StreamReader sr = new System.IO.StreamReader(_assets.Open("effetti.txt")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    _wild_effects.Add(line);
                }
            }

            _gestureDetector = new GestureDetector(this);
           
            if (_done == 1)
                return;

            string[] toolTips = new string[content.Length];

            int ci = 0;
            foreach (string c in content)
            {
                Card cur = Card.MyRegex(c);
                _cards.Add(cur);
                _cards_indices.Add(ci++);
            }


            // get input stream
            System.IO.Stream ims = null;

           
            
            for (int i = 0; i < _cards.Count; i++)
            {
                //multiple a change of seasons
                ims = _assets.Open("Cards/" + _cards[i].Path);
                
                // load image as Drawable
                _cards[i].Bm =  new BitmapDrawable(BitmapFactory.DecodeStream(ims));
            }


            if (ap.loadDeckList() == "")
            {
                _cards_indices = Randomize(_cards_indices);
            }
            else
            {
                _cards_indices.Clear();
                foreach (string card_index in ap.loadDeckList().Split(';'))
                {
                    _cards_indices.Add(Convert.ToInt32(card_index));
                }
                AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
                alertDiag.SetTitle("Restart Anew");
                alertDiag.SetMessage("Do you really want to reset the Planechase?");
                alertDiag.SetPositiveButton("Reset", (senderAlert, args) => { //TOCOMPLETE
                    _cards_indices.Clear();
                    int cid = 0;
                    foreach (var c in _cards)
                    {
                        _cards_indices.Add(cid++);
                    }
                    _cards_indices = Randomize(_cards_indices);
                    Toast.MakeText(this, "Done", ToastLength.Short).Show();
                });
                alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                    alertDiag.Dispose();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
            }


            _phenomenon_button.SetImageResource(Resource.Drawable.sorcerericon);

            // set image to ImageView

            _currentPlaneView.SetScaleType(ImageView.ScaleType.FitCenter);
            _currentPlane = _cards[_cards_indices[0]];
            _currentPlane.visited = true;
            _currentPlaneView.SetImageDrawable(_currentPlane.Bm);
            _attacher = new PhotoViewAttacher(_currentPlaneView);

            _attacher.SingleFling += new EventHandler<SingleFlingEventArgs>(OnSingleFling);

            _attacher.Update();

           
            _done = 1;
            this.Window.AddFlags(WindowManagerFlags.KeepScreenOn);

        }

        protected override void OnPause()
        {
            base.OnPause();
            List<string> index_to_save = new List<string>();
            foreach (int card_i in _cards_indices)
            {
                index_to_save.Add(card_i.ToString());
            }
            ap.saveDeckList(String.Join(";", index_to_save.ToArray()));
        }

        private void OnSingleFling(object sender, SingleFlingEventArgs e)
        {
            var e1 = e.Event1;
            var e2 = e.Event2;
            OnFling(e1, e2, 0, 0);
        }

        public static List<T> Randomize<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            System.Random rnd = new System.Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }


        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
           if(e1.RawX-e2.RawX < 0)
            {
                if (_index > 0)
                    _index--;
               
            }
           else
            {
                if (_index < _cards.Count-1)
                    _index++;
            }
            _currentPlane = _cards[_cards_indices[_index]];
            _currentPlaneView.SetImageDrawable(_currentPlane.Bm);
            _plane_logic();
            _currentPlane.visited = true;
            return true;
        }

        private void _plane_logic()
        {
            if (_currentPlane.Name == "dnd_Tearfall")
            {
                if (_phenomenon_button.Visibility != ViewStates.Visible)
                { 
                    _phenomenon_button.Visibility = ViewStates.Visible;
                    _phenomenon_button.Click += new EventHandler(WildMagic);
                    _phenomenon_button.LongClick += new EventHandler<View.LongClickEventArgs>(FinishWildMagic);
                }
            }
        }

        private void FinishWildMagic(object sender, View.LongClickEventArgs e)
        {
            var button = sender as ImageButton;
            button.Visibility = ViewStates.Invisible;
            button.Click -= WildMagic;
            button.LongClick -= FinishWildMagic;
        }

        private void WildMagic(object sender, EventArgs e)
        {
            _dialog = new Dialog(this);
            _dialog.SetContentView(Resource.Layout.secondlayout);

            System.Random rnd = new System.Random();
            int random_ind = rnd.Next(_wild_effects.Count);
            string effect = _wild_effects[random_ind];

            _dialog.SetTitle("Wild Magic!");

            _dialog.SetCancelable(true);
            TextView text = _dialog.FindViewById<TextView>(Resource.Id.DialogView);
            text.Text = effect;
            _dialog.Show();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _currentPlane = null;
            _currentPlaneView.SetImageDrawable(null);
        }

        public void OnLongPress(MotionEvent e)
        {

        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            _gestureDetector.OnTouchEvent(e);
            return false;
        }

    }

}

