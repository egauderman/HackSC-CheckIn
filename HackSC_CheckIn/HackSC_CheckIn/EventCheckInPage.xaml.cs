using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection<>
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Networking.Proximity; // NFC
using Windows.Storage.Streams; // Buffers
using System.Runtime.InteropServices.WindowsRuntime; // For IBuffer to Byte array
using Microsoft.Devices; // Vibrate
using Windows.Foundation; // URL parsing
using System.Text; // Encoding
using Newtonsoft.Json.Linq; // JSON

namespace HackSC_CheckIn
{
	public partial class EventCheckInPage : PhoneApplicationPage
	{
		private const string NoNFCInstructionText = "This phone doesn't have NFC enabled.";

		HackathonEvent Event { get; set; }

		ObservableCollection<Hacker> RegisteredHackers = new ObservableCollection<Hacker>();

		public EventCheckInPage()
		{
			InitializeComponent();

			Event = (App.Current as App).Events_CurrentEvent;

			if (Event == null)
			{
				NavigationService.GoBack();
				return;
			}

			LayoutRoot.DataContext = Event;

			RegisteredHackersItemsControl.ItemsSource = RegisteredHackers;

			// Check if device doesn't have NFC
			if (ProximityDevice.GetDefault() == null)
			{
				InstructionText.Text = NoNFCInstructionText;
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			Dispatcher.BeginInvoke(StartNFCRead);
		}

		private long _subscribedMessageId;
		private void StartNFCRead()
		{
			if (ProximityDevice.GetDefault() != null)
			{
				InstructionText.Visibility = System.Windows.Visibility.Visible;

				_subscribedMessageId = ProximityDevice.GetDefault().SubscribeForMessage(
					"WindowsUri",
					NFCRead_Callback);
			}
		}

		private void NFCRead_Callback(ProximityDevice device, ProximityMessage message)
		{
			Dispatcher.BeginInvoke(() =>
			{
				// Stop NFC read
				device.StopSubscribingForMessage(message.SubscriptionId);
				// Vibrate phone
				VibrateController.Default.Start(TimeSpan.FromSeconds(0.1));

				InstructionText.Visibility = System.Windows.Visibility.Collapsed;
				WaitingText.Visibility = System.Windows.Visibility.Visible;

				//// Parse data to get ID

				var buffer = message.Data.ToArray();
				string uriString = Encoding.Unicode.GetString(buffer, 0, buffer.Length);

				// Remove null character if present
				if (uriString[uriString.Length - 1] == '\0')
				{
					uriString = uriString.Remove(uriString.Length - 1);
				}

				string query = uriString.Split('?')[1];

				string[] queryArray = query.Split('&');

				Dictionary<string, string> queryDict = new Dictionary<string, string>();
				foreach(string s in queryArray)
				{
					string[] split = s.Split('=');
					queryDict.Add(split[0], split[1]);
				}

				string id = queryDict["id"];

				NetworkQuerier.CheckInForEvent(id, Event.Id, EventCheckIn_Callback);
			});
		}

		private void EventCheckIn_Callback(IAsyncResult result)
		{
			Dispatcher.BeginInvoke(() =>
			{
				WaitingText.Visibility = System.Windows.Visibility.Collapsed;

				//// Parse JSON object

				JObject jsonObject = result.AsyncState as JObject;
				//DEBUG:
				//DescriptionText.Text = jsonObject.ToString();

				JToken registration = jsonObject["registration"];
				Hacker hacker = new Hacker
				{
					Id = registration.Value<string>("id"),
					FirstName = registration.Value<string>("first_name"),
					LastName = registration.Value<string>("last_name"),
					Email = registration.Value<string>("email"),
				};

				bool ok = jsonObject.Value<bool>("ok");
				if (!ok)
				{
					addRegisteredHacker(hacker, true);
				}
				else
				{
					addRegisteredHacker(hacker, false);
				}

				// Restart NFC read
				StartNFCRead();
			});
		}

		private void addRegisteredHacker(Hacker hacker, bool alreadyCheckedIn)
		{
			if(alreadyCheckedIn)
			{
				NewPersonText.Text = hacker.ButtonText + "\n(already registered)";
			}
			else
			{
				NewPersonText.Text = hacker.ButtonText;

				RegisteredHackers.Add(hacker);
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			if(ProximityDevice.GetDefault() != null)
			{
				ProximityDevice.GetDefault().StopSubscribingForMessage(_subscribedMessageId);
			}

			base.OnNavigatedFrom(e);
		}
	}
}