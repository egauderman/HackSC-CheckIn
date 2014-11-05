using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HackSC_CheckIn
{
	class NetworkQuerier
	{
		private static string BASE = "http://go.hacksc.com/api/";
		private static string loginparams = null;

		private static void StartRequest(string url, AsyncCallback finalCallback)
		{
			StartRequest(url, finalCallback, null);
		}

		private static bool SetupAuthentication(HttpWebRequest req)
		{
			if (loginparams == null) { return false; }

			// get authentication string from appCurrent.app
			req.Headers[HttpRequestHeader.Authorization] = loginparams;
			return true;
		}

		private static void SetupPostBody(HttpWebRequest req, string postBody)
		{
			req.Method = "POST";

			byte[] byteArray = Encoding.UTF8.GetBytes(postBody);
			req.ContentType = "application/x-www-form-urlencoded";

		}

		private static void StartRequest(string url, AsyncCallback finalCallback, string postBody)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			if (!SetupAuthentication(request))
			{
				// no auth keys!
			}

			if (postBody != null)
			{
				SetupPostBody(request, postBody);
			}

			AsyncCallback resultcb = (IAsyncResult resp) =>
			{
                HttpWebRequest irequest = (HttpWebRequest)resp.AsyncState;
				HttpWebResponse response = (HttpWebResponse)irequest.EndGetResponse(resp);
				if (response.StatusCode == HttpStatusCode.OK)
				{
					string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
					JObject jsonObject = JObject.Parse(responseString);
					MyAsyncResult async_res = new MyAsyncResult(jsonObject);
					finalCallback(async_res);
				}
				else
				{
					// throw up error message.
                    
				}
			};

			if (postBody != null)
			{
                request.Method = "POST";
				request.BeginGetRequestStream((IAsyncResult async_res) =>
				{
					HttpWebRequest webRequest = (HttpWebRequest)async_res.AsyncState;
					Stream postStream = webRequest.EndGetRequestStream(async_res);
					byte[] postData = Encoding.UTF8.GetBytes(postBody);
					postStream.Write(postData, 0, postData.Length);
                    postStream.Close();

					// Get the response.
					webRequest.BeginGetResponse(new AsyncCallback(resultcb), webRequest);

				}, request);
			}
			else
			{
				// Get the response directly.
				request.BeginGetResponse(resultcb, request);
			}
		}

		public class MyAsyncResult : IAsyncResult
		{
			JObject _result;

			public MyAsyncResult(JObject result)
			{
				_result = result;
			}
			public bool IsCompleted
			{
				get { return true; }
			}
			public WaitHandle AsyncWaitHandle
			{
				get { throw new NotImplementedException(); }
			}
			public object AsyncState
			{
				get { return _result; }
			}
			public bool CompletedSynchronously
			{
				get { return true; }
			}
		}

		public static void SetLoginInformation(string username, string password)
		{
			loginparams = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
		}

		public static void GetUserList(string query, AsyncCallback callback)
		{
			query = HttpUtility.UrlEncode(query);
			StartRequest((BASE + "find_reg.json?q=" + query), callback);
		}

		public static void CheckInUser(string userId, AsyncCallback callback)
		{
			var postData = "id=" + userId + "&has_experience=1&has_waiver=1"; // + (waiver ? "1" : "0");
			StartRequest((BASE + "mark_reg"), callback, postData);
		}

		public static void GetEventList(AsyncCallback callback)
		{
			StartRequest((BASE + "events.json"), callback);
		}

		public static void GetEventInfo(string eventId, AsyncCallback callback)
		{
			StartRequest((BASE + "event/" + eventId), callback);
		}

		public static void CheckinForEvent(string userId, string eventId, AsyncCallback callback)
		{
			StartRequest((BASE + "event/" + eventId + "/checkin?user=" + userId), callback);
		}
	}
}
