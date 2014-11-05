using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;

namespace HackSC_CheckIn
{
	public partial class CheckInParticipantPage : PhoneApplicationPage
	{
		public Hacker Person { get; set; }

		public CheckInParticipantPage()
		{
			InitializeComponent();

			Person = (App.Current as App).CheckIn_CurrentPerson;

			// Ensure that Person is a valid SearchResult
			if (Person == null)
			{
				NavigationService.GoBack();
				return;
			}
			
			LayoutRoot.DataContext = Person;
			HasWaiverCheckbox.DataContext = this;
		}

		private void CheckInButton_Click(object sender, RoutedEventArgs e)
		{
			CheckInButton.IsEnabled = false;
			HasWaiverCheckbox.IsEnabled = false;
			WaitingText.Visibility = System.Windows.Visibility.Visible;

			// Make network post request to check person in
			NetworkQuerier.CheckInUser(Person.Id, CheckInUser_Callback);
		}

		private void CheckInUser_Callback(IAsyncResult result)
		{
			JObject jres = result.AsyncState as JObject;
			Person.ProfileUrl = jres["profile"].Value<string>();

			Dispatcher.BeginInvoke(() =>
			{
				NavigationService.Navigate(new Uri("/NFCWritePage.xaml", UriKind.Relative));
			});
		}
	}
}