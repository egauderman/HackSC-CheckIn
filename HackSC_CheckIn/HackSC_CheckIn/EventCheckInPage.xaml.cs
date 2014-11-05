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

			PageTitle.DataContext = Event;

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

		Hacker _currentHacker;
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

				//// Parse data

				var buffer = message.Data.ToArray();
				string uriString = Encoding.Unicode.GetString(buffer, 0, buffer.Length);

				// Remove null character if present
				if (uriString[uriString.Length - 1] == '\0')
				{
					uriString = uriString.Remove(uriString.Length - 1);
				}
				Uri uri = new Uri(uriString);
				string query = uri.Query;



				_currentHacker = new Hacker
				{
					Id = query.Substring(query.IndexOf("id=")+3, query.IndexOf("&first_name=") - (query.IndexOf("id=")+3)),
					FirstName = query.Substring(query.IndexOf("&first_name") + 12, query.IndexOf("&last_name=") - (query.IndexOf("&first_name") + 12)),
					LastName = query.Substring(query.IndexOf("&last_name=") + 11, query.Length - (query.IndexOf("&last_name=") + 11))
				};

				NetworkQuerier.CheckInForEvent(_currentHacker.Id, Event.Id, EventCheckIn_Callback);
			});
		}

		private void EventCheckIn_Callback(IAsyncResult result)
		{
			WaitingText.Visibility = System.Windows.Visibility.Collapsed;

			if(_currentHacker != null)
			{
				RegisteredHackers.Add(_currentHacker);
				_currentHacker = null;
			}

			// Restart NFC read
			StartNFCRead();
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