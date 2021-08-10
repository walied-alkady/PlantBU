using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace PlantBU.Droid
{
    [Service]
    class LocationService : Service
    {
        // A notification requires an id that is unique to the application.
        const int NOTIFICATION_ID = 9000;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // From shared code or in your PCL
            //string data = scanData.Replace('\n', ' ').TrimEnd();
           // MessagingCenter.Send<Apptring>(my_application, "ScanBarcode", data);

            // Work has finished, now dispatch anotification to let the user know.
            /* Notification.Builder notificationBuilder = new Notification.Builder(this)
                 .SetSmallIcon(Resource.Drawable.ic_notification_small_icon)
                 .SetContentTitle(Resources.GetString(Resource.String.notification_content_title))
                 .SetContentText(Resources.GetString(Resource.String.notification_content_text));

             var notificationManager = (NotificationManager)GetSystemService(NotificationService);
             notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());*/
            return StartCommandResult.NotSticky;
        }
    }
}