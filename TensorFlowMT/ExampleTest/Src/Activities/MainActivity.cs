using System;
using System.IO;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using ExampleTest.Src.Wrapper;
using FingerExample.Src.Extensions;

namespace ExampleTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        private ImageView Preview { get; set; }

        private TextView TextResult { get; set; }

        private View Layout { get; set; }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            TextResult = FindViewById<TextView>(Resource.Id.text_result);
            Preview = FindViewById<ImageView>(Resource.Id.img_view);

            Button buttonPrevious = FindViewById<Button>(Resource.Id.button_prev);
            buttonPrevious.Click += PreviousClick;

            Button buttonNext = FindViewById<Button>(Resource.Id.button_next);
            buttonNext.Click += NextClick;

            Layout = FindViewById(Resource.Layout.activity_main);

            CheckPermissions();
            LoadImagesAsync();
            
            TextResult.Text = "" + index;

            Log.Debug("", this.GetExternalFilesDir(null).AbsolutePath);
        }

        private void CheckPermissions()
        {
            // Read External Storage
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {
                if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage))
                {
                    // Provide an additional rationale to the user if the permission was not granted
                    // and the user would benefit from additional context for the use of the permission.
                    // For example if the user has previously denied the permission.
                    Log.Info("MainActivity", "Displaying camera permission rationale to provide additional context.");
                    var requiredPermissions = new String[] { Manifest.Permission.ReadExternalStorage };
                    Snackbar.Make(Layout,
                                   "Permission message",
                                   Snackbar.LengthIndefinite)
                            .SetAction("OK",
                                       new Action<View>(delegate (View obj)
                                       {
                                           ActivityCompat.RequestPermissions(this, requiredPermissions, 0);
                                       }
                            )
                    ).Show();
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, 0);
                }
            }
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
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            LoadImagesAsync();
        }
        
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
                filesList = Directory.GetFiles(folder);
            else
                this.ShowAlert("Pasta não existe: ", folder);

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

