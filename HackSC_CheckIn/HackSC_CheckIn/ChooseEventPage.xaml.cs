using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection<>
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq; // JSON

namespace HackSC_CheckIn
{
	public partial class ChooseEventPage : PhoneApplicationPage
	{
		public ObservableCollection<HackathonEvent> HackathonEvents = new ObservableCollection<HackathonEvent>();

		public ChooseEventPage()
		{
			InitializeComponent();

			EventsItemsControl.ItemsSource = HackathonEvents;

			// Begin web request for events, show WaitingText
			NetworkQuerier.GetEventList(GetEventList_Callback);

			WaitingText.Visibility = System.Windows.Visibility.Visible;
		}

		private void GetEventList_Callback(IAsyncResult result)
		{
			JObject jsonObject = result.AsyncState as JObject;
			Dispatcher.BeginInvoke(() =>
			{
				WaitingText.Visibility = System.Windows.Visibility.Collapsed;

				JArray resultsArray = jsonObject["events"] as JArray;
				for (JToken iterator = resultsArray.First; iterator != null; iterator = iterator.Next)
				{
					HackathonEvent hackathonEvent = new HackathonEvent();
					hackathonEvent.Id = iterator.Value<string>("id");
					hackathonEvent.Title = iterator.Value<string>("title");
					hackathonEvent.Description = iterator.Value<string>("description");

					// Add search result to SearchResults
					HackathonEvents.Add(hackathonEvent);
				}

				// Display No Results if needed
				if (HackathonEvents.Count == 0)
				{
					NoEventsText.Visibility = System.Windows.Visibility.Visible;
				}
			});
		}

		private void EventButton_Click(object sender, RoutedEventArgs e)
		{
			(App.Current as App).Events_CurrentEvent = (sender as Button).DataContext as HackathonEvent;

			NavigationService.Navigate(new Uri("/EventCheckInPage.xaml", UriKind.Relative));
		}
	}
}