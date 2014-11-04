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
			// Make network post request, set WaitingText.Visibility to Visible, in callback set it
		}
	}
}