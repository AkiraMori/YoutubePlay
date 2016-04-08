using Android.App;
using Android.Widget;
using Android.OS;
using RestSharp;
using Android.Content;
using Com.Google.Android.Youtube.Player;
using System;
using Android.Views.InputMethods;
using System.Threading.Tasks;

namespace YouTubePlayDemo
{
	[Activity (Label = "YouTubePlayDemo", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : YouTubeBaseActivity, IYouTubePlayerOnInitializedListener
	{
		private const int RegStartStandalonePlayer= 1;
		private int RECOVERY_DIALOG_REQUEST = 1;
		private YouTubePlayerView youTubePlayerView;
		private MyPlaylistEventListener playlistEventListener;
		private MyPlayerStateChangeListener playerStateChangeListener;
		private MyPlaybackEventListener playbackEventListener;
		public IYouTubePlayerOnInitializedListener initializeListenter;
		public static IYouTubePlayer Player {
			get;
			set;
		}

		public Button button;
		public static String VIDEO_ID = "_OBlgSz8sSM";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			button = FindViewById<Button> (Resource.Id.myButton);
			button.Click +=  Button_Click;

			youTubePlayerView = FindViewById<YouTubePlayerView>(Resource.Id.youtube_view);
			youTubePlayerView.Initialize(VIDEO_ID, this);
			playlistEventListener = new MyPlaylistEventListener();
			playerStateChangeListener = new MyPlayerStateChangeListener ();
			playbackEventListener = new MyPlaybackEventListener();
		}

		public void OnInitializationFailure(IYouTubePlayerProvider provider, YouTubeInitializationResult errorReason)
		{
			if (errorReason.IsUserRecoverableError) {
				errorReason.GetErrorDialog(this, RECOVERY_DIALOG_REQUEST).Show();
			} else {
				String errorMessage = String.Format(
					GetString(Resource.String.ErrorMessage), errorReason.ToString());
				Toast.MakeText(this, errorMessage, ToastLength.Long).Show();
			}
		}

		void Button_Click (object sender, System.EventArgs e)
		{
			GetVideoID ();

		}


		public static void GetVideoID()
		{
			var client = new RestClient ("http://vagex.com/aupdater2.php");

			var request = new RestRequest ("aupdater2.php", Method.POST);
			request.AddParameter ("userid", "1111");
			client.ExecuteAsync<Entry> (request, RestResponse => {
				VIDEO_ID = RestResponse.Data.URL;

				Player.LoadVideo(VIDEO_ID);
			});

		}

		public void OnInitializationSuccess (IYouTubePlayerProvider provider, IYouTubePlayer player, bool wasRestored)
		{
			Player = player;
			Player.SetPlaylistEventListener(playlistEventListener);
			Player.SetPlayerStateChangeListener(playerStateChangeListener);
			Player.SetPlaybackEventListener(playbackEventListener);

			player.LoadVideo (VIDEO_ID);
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);
			if (requestCode == RegStartStandalonePlayer && resultCode != Result.Ok) {
				YouTubeInitializationResult errorReason =
					YouTubeStandalonePlayer.GetReturnedInitializationResult (data);
				if (errorReason.IsUserRecoverableError) {
					errorReason.GetErrorDialog (this, 0).Show ();
				} else {
					String errorMessage =
						String.Format (GetString (Resource.String.ErrorMessage), errorReason.ToString ());
					Toast.MakeText (this, errorMessage, ToastLength.Long).Show ();
				}
			}
		}
	}

	class MyPlaybackEventListener : Java.Lang.Object, IYouTubePlayerPlaybackEventListener
	{

		public static string PlaybackState = "NOT_PLAYING";
		public static string BufferingState = "";
		#region IYouTubePlayerPlaybackEventListener implementation

		public void OnBuffering (bool p0)
		{
			BufferingState = p0 ? "(BUFFERING)" : "";
		}

		public void OnPaused ()
		{
			PlaybackState = "PAUSED";
		}

		public void OnPlaying ()
		{
			PlaybackState = "PLAYING";
		}

		public void OnSeekTo (int p0)
		{

		}

		public void OnStopped ()
		{
			PlaybackState = "STOPPED";
		}

		#endregion
	}


	class MyPlaylistEventListener : Java.Lang.Object, IYouTubePlayerPlaylistEventListener 
	{

		#region IYouTubePlayerPlaylistEventListener implementation

		public void OnNext ()
		{
			//PlayerControlsActivity.Log ("NEXT VIDEO");
		}

		public void OnPlaylistEnded ()
		{
			//PlayerControlsActivity.Log ("PLAYLIST ENDED");
		}

		public void OnPrevious ()
		{
			//PlayerControlsActivity.Log ("PREVIOUS VIDEO");
		}

		#endregion


	}

	class MyPlayerStateChangeListener : Java.Lang.Object, IYouTubePlayerPlayerStateChangeListener{

		public static string PlayerState = "UNINITIALIZED";

		#region IYouTubePlayerPlayerStateChangeListener implementation

		public void OnAdStarted ()
		{
			PlayerState = "AD_STARTED";
		}

		public void OnError (YouTubePlayerErrorReason p0)
		{
			PlayerState = "ERROR (" + p0 + ")";
			if (p0 == YouTubePlayerErrorReason.UnexpectedServiceDisconnection) {
				// When this error occurs the player is released and can no longer be used.
				//PlayerControlsActivity.Player = null;
				//TODO: No Access! 
				//setControlsEnabled(false);
			}
		}

		public void OnLoaded (string p0)
		{
			PlayerState = String.Format("VIDEO LOADED");
		}

		public void OnLoading ()
		{
			PlayerState = "LOADING";
		}

		public void OnVideoEnded ()
		{
			PlayerState = "VIDEO_ENDED";
			MainActivity.GetVideoID ();
		}

		public void OnVideoStarted ()
		{
			PlayerState = "VIDEO_STARTED";
		}

		#endregion

	}


	class Entry {
		public string URL {
			get;
			set;
		}

		public int Length {
			get;
			set;
		}
	}
}


