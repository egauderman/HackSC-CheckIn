using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HackSC_CheckIn.Resources;

namespace HackSC_CheckIn
{
	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();
            NetworkQuerier.SetLoginInformation("gauderma@usc.edu", "hack1958");
		}

		private void CheckInPageButton_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/CheckInPage.xaml", UriKind.Relative));
		}

		private void EventCheckInButton_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/ChooseEventPage.xaml", UriKind.Relative));
		}
	}
}