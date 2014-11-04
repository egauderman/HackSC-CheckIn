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
using Newtonsoft.Json.Linq;


namespace HackSC_CheckIn
{
	public class SearchResult
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Name
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
		public string ButtonText
		{
			get
			{
				return FirstName + " " + LastName + ", " + Email;
			}
		}
	}

	public partial class CheckInPage : PhoneApplicationPage
	{
		public ObservableCollection<SearchResult> SearchResults = new ObservableCollection<SearchResult>();

		public CheckInPage()
		{
			InitializeComponent();

			//SearchResultsItemsControl.DataContext = SearchResults;

			SearchResultsItemsControl.ItemsSource = SearchResults;
		}

		private void SearchQueryBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (SearchQueryBox.Text.Length > 0)
			{
				// Query the server
				// Old url: http://louise.codejoust.com/hacksc/people.php?q=

				string searchUri = "http://go.hacksc.com/api/find_reg.json?q=" + SearchQueryBox.Text;

				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(searchUri);

				// Force http authorization
				string authInfo = "gauderma@usc.edu:hack1958";
				authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
				request.Headers["Authorization"] = "Basic " + authInfo;
				
				request.BeginGetResponse(SearchQueryCallback, request);
			}
			else
			{
				Dispatcher.BeginInvoke(() =>
				{
					SearchResults.Clear();

					NoResultsText.Visibility = System.Windows.Visibility.Collapsed;
				});
			}
		}

		private void DisplayNoResults()
		{
			Dispatcher.BeginInvoke(() =>
			{
				SearchResults.Clear();

				NoResultsText.Visibility = System.Windows.Visibility.Visible;
			});
		}

		private void SearchQueryCallback(IAsyncResult result)
		{
			HttpWebRequest request = result.AsyncState as HttpWebRequest;
			if (request != null)
			{
				try
				{
					// Get json string
					WebResponse response = request.EndGetResponse(result);
					StreamReader reader = new StreamReader(response.GetResponseStream());
					string searchResultJsonString = reader.ReadToEnd();

					Dispatcher.BeginInvoke(() =>
					{
						SearchResults.Clear();

						// Parse json string
						JObject jsonObject = JObject.Parse(searchResultJsonString);
						JArray resultsArray = jsonObject["registrations"] as JArray;
						for (JToken iterator = resultsArray.First; iterator != null; iterator = iterator.Next)
						{
							SearchResult person = new SearchResult();
							person.Id = iterator.Value<string>("id");
							person.FirstName = iterator.Value<string>("first_name");
							person.LastName = iterator.Value<string>("last_name");
							person.Email = iterator.Value<string>("email");

							// Add search result to SearchResults
							SearchResults.Add(person);
						}

						// Remove "No Results if needed"
						if (SearchResults.Count == 0)
						{
							DisplayNoResults();
						}
						else
						{
							NoResultsText.Visibility = System.Windows.Visibility.Collapsed;
						}
					});
				}
				catch (WebException)
				{
					DisplayNoResults();
				}
			}
		}

		private void SearchResultButton_Click(object sender, RoutedEventArgs e)
		{
			(App.Current as App).CheckIn_CurrentPerson = (sender as Button).DataContext as SearchResult;

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