using System;
using System.IO;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.Provider;
using Android.Util;
using Android.Widget;
using AndroidApp;

namespace AndroidApp.Extensions
{
    public static class AndroidContextExtensions
    {
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

        public static ProgressDialog CreateProgress(this Context context, string title, string message)
        {
            var pd = new ProgressDialog(context);
            pd.SetTitle(title);
            pd.SetMessage(message);
            pd.Indeterminate = true;
            pd.SetProgressStyle(ProgressDialogStyle.Spinner);
            pd.SetCancelable(false);
            pd.SetCanceledOnTouchOutside(false);
            return pd;
        }

        public static void ShowAlert(this Context context, string title, string message)
        {
            new AlertDialog.Builder(context)
                .SetTitle(title)
                .SetCancelable(false)
                .SetMessage(message)
                .SetPositiveButton("ok", (o, a) => { })
                .Create()
                .Show();
        }
    }
}