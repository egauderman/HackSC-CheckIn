using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // for ObservableCollection<>
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

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
			
		}
	}
}