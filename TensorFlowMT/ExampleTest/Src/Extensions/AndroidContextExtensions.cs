using System;
using System.IO;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Database.Sqlite;
using Android.Provider;
using Android.Util;
using Android.Widget;
using ExampleTest;

namespace FingerExample.Src.Extensions
{
    public static class AndroidContextExtensions
    {
        public static string PathToExternalStorage(this Context context, string fileName)
        {
            return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, fileName);
        }

        public static async Task<string> CopyAssetAsync(this Context context, string fileName)
        {

            var file = $"/{fileName}";
            var path = context.GetExternalFilesDir(null).AbsolutePath + file;
            Log.Debug("CopyAssetAsync", path);
            if (!File.Exists(path))
            {
                var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                await context.Assets.Open(fileName).CopyToAsync(fileStream);
            }
            return path;
        }
        public static string ReadAsset(this Context context, string fileName)
        {
            using (var reader = new StreamReader(context.Assets.Open(fileName)))
            {
                return reader.ReadToEnd();
            }
        }

        public static void ShowToast(this Context context, int resId, ToastLength duration = ToastLength.Short)
        {
            Toast.MakeText(context, resId, duration).Show();
        }
        public static void ShowToast(this Context context, string message, ToastLength duration = ToastLength.Short)
        {
            Toast.MakeText(context, message, duration).Show();
        }

        public static void ShowAlert(this Context context, string title, string message)
        {
            new AlertDialog.Builder(context, Resource.Style.CustomAlertStyle)
                .SetTitle(title)
                .SetCancelable(false)
                .SetMessage(message)
                .SetPositiveButton(context.GetString(Resource.String.action_ok), (o, a) => { })
                .Create()
                .Show();
        }
        public static void ShowAlert(this Context context, int resTitle, string message)
        {
            new AlertDialog.Builder(context, Resource.Style.CustomAlertStyle)
                .SetTitle(resTitle)
                .SetCancelable(false)
                .SetMessage(message)
                .SetPositiveButton(context.GetString(Resource.String.action_ok), (o, a) => { })
                .Create()
                .Show();
        }
        public static void ShowAlert(this Context context, int resTitle, int resMessage)
        {
            new AlertDialog.Builder(context, Resource.Style.CustomAlertStyle)
                .SetTitle(resTitle)
                .SetCancelable(false)
                .SetMessage(resMessage)
                .SetPositiveButton(context.GetString(Resource.String.action_ok), (o, a) => { })
                .Create()
                .Show();
        }
        public static void ShowAlert(this Context context, int resTitle, int resMessage, Action callBack)
        {
            new AlertDialog.Builder(context, Resource.Style.CustomAlertStyle)
                .SetTitle(resTitle)
                .SetCancelable(false)
                .SetMessage(resMessage)
                .SetPositiveButton(context.GetString(Resource.String.action_ok), (o, a) => callBack?.Invoke())
                .SetNegativeButton(context.GetString(Resource.String.action_cancel), (o, a) => { })
                .Create()
                .Show();
        }
        public static void ShowAlert(this Context context, int resTitle, int resMessage, int resOk, int resCancel, Action callBack)
        {
            new AlertDialog.Builder(context, Resource.Style.CustomAlertStyle)
                .SetTitle(resTitle)
                .SetCancelable(false)
                .SetMessage(resMessage)
                .SetPositiveButton(context.GetString(resOk), (o, a) => callBack?.Invoke())
                .SetNegativeButton(context.GetString(resCancel), (o, a) => { })
                .Create()
                .Show();
        }

        public static void ShowAlertWithIcon(this Context context, int resTitle, string message, int resIcon)
        {
            new AlertDialog.Builder(context, Resource.Style.CustomAlertStyle)
                .SetTitle(resTitle)
                .SetCancelable(false)
                .SetMessage(message)
                .SetIcon(resIcon)
                .SetPositiveButton(context.GetString(Resource.String.action_ok), (o, a) => { })
                .Create()
                .Show();
        }
        public static void ShowAlertWithIcon(this Context context, int resTitle, int resMessage, int resIcon)
        {
            new AlertDialog.Builder(context, Resource.Style.CustomAlertStyle)
                .SetTitle(resTitle)
                .SetCancelable(false)
                .SetMessage(resMessage)
                .SetIcon(resIcon)
                .SetPositiveButton(context.GetString(Resource.String.action_ok), (o, a) => { })
                .Create()
                .Show();
        }

        public static ProgressDialog CreateProgress(this Context context, string title, string message)
        {
            var pd = new ProgressDialog(context, Resource.Style.CustomAlertStyle);
            pd.SetTitle(title);
            pd.SetMessage(message);
            pd.Indeterminate = true;
            pd.SetProgressStyle(ProgressDialogStyle.Spinner);
            pd.SetCancelable(false);
            pd.SetCanceledOnTouchOutside(false);
            return pd;
        }
        public static ProgressDialog CreateProgress(this Context context, int resTitle, int resMessage)
        {
            var pd = new ProgressDialog(context, Resource.Style.CustomAlertStyle);
            pd.SetTitle(resTitle);
            pd.SetMessage(context.GetString(resMessage));
            pd.Indeterminate = true;
            pd.SetProgressStyle(ProgressDialogStyle.Spinner);
            pd.SetCancelable(false);
            pd.SetCanceledOnTouchOutside(false);
            return pd;
        }

        public static Java.IO.File CreateTemporaryJpgFile(this Context context, string fileName)
        {
            return new Java.IO.File(context.GetExternalFilesDir(null), $"{fileName}.jpg");
        }

        public static void TakePhoto(this Context context, Java.IO.File file, int requestCode)
        {
            if (context is Activity activity)
            {
                var takeIntent = new Intent(MediaStore.ActionImageCapture);
                takeIntent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));
                takeIntent.PutExtra("android.intent.extras.CAMERA_FACING", 1);
                if (takeIntent.ResolveActivity(context.PackageManager) != null)
                {
                    activity.StartActivityForResult(takeIntent, requestCode);
                }
            }
        }

        public static async Task<Tuple<int, int, int, int>> GetFaceRectAsync(this Context context, Java.IO.File file, int maxFaces)
        {
            var options = new Android.Graphics.BitmapFactory.Options { InScaled = false, InPreferredConfig = Android.Graphics.Bitmap.Config.Rgb565 };
            using (var bitmap = await Android.Graphics.BitmapFactory.DecodeFileAsync(file.AbsolutePath, options))
            {
                var detector = new Android.Media.FaceDetector(bitmap.Width, bitmap.Height, maxFaces);
                var faces = new Android.Media.FaceDetector.Face[1];
                var facesFound = await detector.FindFacesAsync(bitmap, faces);
                if (facesFound > 0)
                {
                    var face = faces[0];
                    var eyeDistance = face.EyesDistance();
                    var midPoint = new Android.Graphics.PointF();
                    face.GetMidPoint(midPoint);
                    return new Tuple<int, int, int, int>(
                        (int)(midPoint.X - eyeDistance),
                        (int)(midPoint.Y - eyeDistance),
                        (int)(2 * eyeDistance),
                        (int)(2 * eyeDistance));
                }
                return null;
            }
        }
    }
}