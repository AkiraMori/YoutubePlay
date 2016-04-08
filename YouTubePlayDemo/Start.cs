
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;

namespace YouTubePlayDemo
{
	[Activity (Label = "Start",MainLauncher = true)]			
	public class Start : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.Start);
			// Create your application here
			Button button = FindViewById<Button> (Resource.Id.Start); 
			button.Click += Button_Click;
		}

		public void GetVideoID()
		{
			var client = new RestClient ("http://vagex.com/aupdater2.php");

			var request = new RestRequest ("aupdater2.php", Method.POST);
			request.AddParameter ("userid", "1111");
			client.ExecuteAsync<Entry> (request, RestResponse => {
				DevConstants.Video_ID = RestResponse.Data.URL;
				if(!DevConstants.mainState)
					StartActivity (typeof( MainActivity));
				else{
					
				}
			});
		}

		void Button_Click (object sender, EventArgs e)
		{
			//StartActivity (typeof( MainActivity));
			GetVideoID();
		}
	}
}

