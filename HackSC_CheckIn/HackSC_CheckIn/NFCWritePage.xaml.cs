using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Networking.Proximity; // NFC

namespace HackSC_CheckIn
{
	public partial class NFCWritePage : PhoneApplicationPage
	{
		public NFCWritePage()
		{
			InitializeComponent();

			// Check if device doesn't have NFC
			if (ProximityDevice.GetDefault() == null)
			{
				InstructionText.Text = "This phone doesn't have NFC enabled. Press Done";
				DoneButton.IsEnabled = true;
			}
		}

		private void DoneButton_Click(object sender, RoutedEventArgs e)
		{
			// Navigate back to MainPage
			NavigationService.RemoveBackEntry(); // remove CheckInParticipantPage
			NavigationService.RemoveBackEntry(); // remove CheckInPage
			(App.Current as App).CheckIn_CurrentPerson = null; // delete CurrentPerson global variable
			NavigationService.GoBack();
		}
	}
}