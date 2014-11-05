using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HackSC_CheckIn
{
	public partial class CheckInParticipantPage : PhoneApplicationPage
	{
		public SearchResult Person { get; set; }

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

			// TEMP: following network request is temporarily commented out; just go straight to NFCWritePage

			// Make network post request
			//NetworkQuerier.CheckInUser(Person.Id,
			//	(IAsyncResult result) =>
			//	{
					NavigationService.Navigate(new Uri("/NFCWritePage.xaml", UriKind.Relative));
			//	}
			//);
		}
	}
}