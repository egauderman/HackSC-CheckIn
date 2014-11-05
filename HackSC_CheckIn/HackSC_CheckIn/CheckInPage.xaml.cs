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
	public partial class CheckInPage : PhoneApplicationPage
	{
		public ObservableCollection<Hacker> SearchResults = new ObservableCollection<Hacker>();

		public CheckInPage()
		{
			InitializeComponent();

			// SearchResultsItemsControl.DataContext = SearchResults;

			SearchResultsItemsControl.ItemsSource = SearchResults;

			// Initialize display state
			DisplayBlank();
		}

		private void SearchQueryBox_LostFocus(object sender, RoutedEventArgs e)
		{
			// Make sure search box has at least 2 non-whitespace characters
			if (SearchQueryBox.Text.Length - SearchQueryBox.Text.Count(Char.IsWhiteSpace) > 1)
			{
				// Start HTTP request
				// note: NetworkQuerier.GetUserList will URLEncode the query
				NetworkQuerier.GetUserList(SearchQueryBox.Text, SearchQueryCallback);

				// Show "One second" and disable text box until request is received
				Dispatcher.BeginInvoke(() =>
				{
					DisplayWaitingText();
				});
			}
			else
			{
				// Textbox is blank; clear the screen
				Dispatcher.BeginInvoke(() =>
				{
					DisplayBlank();
				});
			}
		}

		#region Display states

		// The initial state of the search page
		private void DisplayBlank()
		{
			SearchResults.Clear();

			WaitingText.Visibility = System.Windows.Visibility.Collapsed;

			NoResultsText.Visibility = System.Windows.Visibility.Collapsed;

			SearchQueryBox.IsEnabled = true;
		}

		private void DisplayNoResults()
		{
			SearchResults.Clear();

			WaitingText.Visibility = System.Windows.Visibility.Collapsed;

			NoResultsText.Visibility = System.Windows.Visibility.Visible;

			SearchQueryBox.IsEnabled = true;
		}

		private void DisplayWaitingText()
		{
			SearchResults.Clear();

			NoResultsText.Visibility = System.Windows.Visibility.Collapsed;

			WaitingText.Visibility = System.Windows.Visibility.Visible;

			SearchQueryBox.IsEnabled = false;
		}

		private void PrepareDisplaySearchResults()
		{
			NoResultsText.Visibility = System.Windows.Visibility.Collapsed;

			WaitingText.Visibility = System.Windows.Visibility.Collapsed;

			SearchQueryBox.IsEnabled = true;
		}

		#endregion Display states

		private void SearchQueryCallback(IAsyncResult result)
		{
            JObject jsonObject = result.AsyncState as JObject;
			Dispatcher.BeginInvoke(() =>
			{
				// Set display state to be ready to display search results
				PrepareDisplaySearchResults();

				JArray resultsArray = jsonObject["registrations"] as JArray;
				for (JToken iterator = resultsArray.First; iterator != null; iterator = iterator.Next)
				{
					Hacker person = new Hacker();
					person.Id = iterator.Value<string>("id");
					person.FirstName = iterator.Value<string>("first_name");
					person.LastName = iterator.Value<string>("last_name");
					person.Email = iterator.Value<string>("email");

					// Add search result to SearchResults
					SearchResults.Add(person);
				}

				// Display No Results if needed
				if (SearchResults.Count == 0)
				{
					DisplayNoResults();
				}
			});
		}

		private void SearchResultButton_Click(object sender, RoutedEventArgs e)
		{
			(App.Current as App).CheckIn_CurrentPerson = (sender as Button).DataContext as Hacker;

			NavigationService.Navigate(new Uri("/CheckInParticipantPage.xaml", UriKind.Relative));
		}

		private void SearchQueryBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if(e.Key == System.Windows.Input.Key.Enter)
			{
				// Make the search box lose focus
				this.Focus();
			}
		}
	}
}