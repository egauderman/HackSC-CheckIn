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

namespace HackSC_CheckIn
{
	public class HackathonEvent
	{
		public string Title { get; set; }
		public string Id { get; set; }

		public string ButtonText
		{
			get
			{
				return Title;
			}
		}
	}

	public partial class ChooseEventPage : PhoneApplicationPage
	{
		public ObservableCollection<HackathonEvent> HackathonEvents = new ObservableCollection<HackathonEvent>();

		public ChooseEventPage()
		{
			InitializeComponent();

			EventsItemsControl.ItemsSource = HackathonEvents;

			HackathonEvents.Add(new HackathonEvent
			{
				Title = "Dinner",
				Id = "0"
			});
			HackathonEvents.Add(new HackathonEvent
			{
				Title = "Midnight Snack",
				Id = "1"
			});
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			// Begin web request for events, show WaitingText
			NetworkQuerier.GetEventList(EventListReceived);
		}

		private void EventListReceived(IAsyncResult result)
		{
			
		}

		private void EventButton_Click(object sender, RoutedEventArgs e)
		{
			(App.Current as App).Events_CurrentEvent = (sender as Button).DataContext as HackathonEvent;

			NavigationService.Navigate(new Uri("/EventCheckInPage.xaml", UriKind.Relative));
		}
	}
}