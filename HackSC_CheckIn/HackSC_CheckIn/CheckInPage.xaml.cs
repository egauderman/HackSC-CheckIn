using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection<>
using System.IO;
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
	public class SearchResult
	{
		public string Name { get; set; }
	}

	public partial class CheckInPage : PhoneApplicationPage
	{
		public ObservableCollection<SearchResult> SearchResults = new ObservableCollection<SearchResult>();

		public CheckInPage()
		{
			InitializeComponent();

			//SearchResultsItemsControl.DataContext = SearchResults;

			SearchResult s = new SearchResult();
			s.Name = "User";
			SearchResults.Add(s);

			SearchResultsItemsControl.DataContext = SearchResults;
		}

		private void SearchQueryBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (SearchQueryBox.Text.Length > 0)
			{
				// Query the server
				string searchUri = "http://louise.codejoust.com/hacksc/people.php?q=" + SearchQueryBox.Text;

				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(searchUri);
				request.BeginGetResponse(SearchQueryCallback, request); 
			}
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
						JArray resultsArray = jsonObject["data"] as JArray;
						for (JToken iterator = resultsArray.First; iterator != null; iterator = iterator.Next)
						{
							string name = iterator.Value<string>("name");

							// Add search result to SearchResults
							SearchResults.Add(new SearchResult { Name = name });
						}
					});
				}
				catch (WebException)
				{
					return;
				}
			}
		}

		private void SearchResultButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}