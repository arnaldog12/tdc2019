using System;
using System.IO;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using ExampleTest.Src.Wrapper;
using FingerExample.Src.Extensions;

namespace ExampleTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ImageView Preview { get; set; }

        private TextView TextResult { get; set; }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            //SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;

            TextResult = FindViewById<TextView>(Resource.Id.text_result);

            Preview = FindViewById<ImageView>(Resource.Id.img_view);

            Button buttonPrevious = FindViewById<Button>(Resource.Id.button_prev);
            buttonPrevious.Click += PreviousClick;

            Button buttonNext = FindViewById<Button>(Resource.Id.button_next);
            buttonNext.Click += NextClick;


            await LoadImagesAsync();
            TextResult.Text = "" + index;

            Log.Debug("", this.GetExternalFilesDir(null).AbsolutePath);
        }

        private void PreviousClick(object sender, EventArgs e)
        {
            --index;
            if (index < 0) index = filesList.Length-1;
            Process();
        }

        private void NextClick(object sender, EventArgs e)
        {
            index = ++index % filesList.Length;
            Process();
        }

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //    return true;
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    int id = item.ItemId;
        //    if (id == Resource.Id.action_settings)
        //    {
        //        return true;
        //    }

        //    return base.OnOptionsItemSelected(item);
        //}


        string folder;
        string[] filesList;
        int index = 0;
        /// <summary>
        ///  Carrega as imagens da pasta 'Samples'
        /// </summary>
        private async System.Threading.Tasks.Task LoadImagesAsync()
        {
            await this.CopyAssetAsync("best_model_ever.pb");

            folder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Java.IO.File.Separator + "Samples";

            if (Directory.Exists(folder))
            {
                filesList = Directory.GetFiles(folder);
            }
            else
            {
                this.ShowAlert("Pasta não existe: ", folder);
            }

            Process();
        }

        async void Process()
        {
            var progress = this.CreateProgress("Progresso", "Processando...");
            progress.Show();

            var filename = Path.GetFileName(filesList[index]);
            var options = new Android.Graphics.BitmapFactory.Options { InScaled = false};

            using (var bmp = await Android.Graphics.BitmapFactory.DecodeFileAsync(folder + Java.IO.File.Separator + filename, options))
            {

                // Chamar dll de arnaldo
                var handle = bmp.LockPixels();
                var info = bmp.GetBitmapInfo();

                Log.Debug("CHANNELS: " , bmp.GetConfig().ToString());
                int predict = ExampleWrapper.predictDigit(handle, (int)info.Height, (int)info.Height);
                TextResult.Text = "" + predict;

                if (Preview.Drawable is Android.Graphics.Drawables.BitmapDrawable temp)
                {
                    temp.Bitmap.Recycle();
                }
                Preview.SetImageBitmap(bmp);
            }

            progress.Dismiss();
        }
    }
}

