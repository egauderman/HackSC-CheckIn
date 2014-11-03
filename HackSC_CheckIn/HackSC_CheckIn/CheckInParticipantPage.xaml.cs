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

			if (Person != null)
			{
				PageTitle.DataContext = Person;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}