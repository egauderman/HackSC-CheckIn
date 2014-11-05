using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Networking.Proximity;
using Windows.Storage.Streams; // NFC
using Microsoft.Devices; // Vibrate

namespace HackSC_CheckIn
{
	public partial class NFCWritePage : PhoneApplicationPage
	{
		private const string TouchPhoneInstructionText = "Touch phone to card...";
		private const string NoNFCInstructionText = "This phone doesn't have NFC enabled. Press Done";
		private const string SuccessfulWriteInstructionText = "NFC write successful";

		private Hacker Person;

		public NFCWritePage()
		{
			InitializeComponent();

			InstructionText.Text = TouchPhoneInstructionText;

			// Check if device doesn't have NFC
			if (ProximityDevice.GetDefault() == null)
			{
				InstructionText.Text = NoNFCInstructionText;
				DoneButton.IsEnabled = true;
			}

			Person = (App.Current as App).CheckIn_CurrentPerson;

			// Ensure that current person isn't null
			if(Person == null)
			{
				NavigationService.GoBack();
				return;
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

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if(ProximityDevice.GetDefault() != null)
			{
				//// Writes URL
				// Set data to write
				var dataWriter = new DataWriter() { UnicodeEncoding = UnicodeEncoding.Utf16LE };
				dataWriter.WriteString(
					Person.ProfileUrl +
					"?id=" + Person.Id +
					"&first_name=" + Person.FirstName +
					"&last_name=" + Person.LastName
					);

				// Begin write
				ProximityDevice.GetDefault().PublishBinaryMessage(
					"WindowsUri:WriteTag",
					dataWriter.DetachBuffer(),
					NFCWriteFinish);

				////// Writes ID
				//// Set data to write
				//var dataWriter = new DataWriter() { UnicodeEncoding = UnicodeEncoding.Utf8 };
				//dataWriter.WriteString("id=" + (App.Current as App).CheckIn_CurrentPerson.Id);

				//// Begin write
				//ProximityDevice.GetDefault().PublishBinaryMessage(
				//	"Windows:WriteTag.HackSC_ID",
				//	dataWriter.DetachBuffer(),
				//	NFCWriteFinish);
			}
		}

		private void NFCWriteFinish(ProximityDevice sender,  long publishedMessageId)
		{
			Dispatcher.BeginInvoke(() =>
			{
				// Vibrate phone
				VibrateController.Default.Start(TimeSpan.FromSeconds(0.1));

				InstructionText.Text = SuccessfulWriteInstructionText;
				DoneButton.IsEnabled = true;

				// Stop NFC write
				sender.StopPublishingMessage(publishedMessageId);
			});
		}
	}
}