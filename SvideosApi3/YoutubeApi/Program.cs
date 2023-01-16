using System;
using Google.Apis.YouTube.v3;
using System.Collections.Generic;
using BaseClientService;

namespace SvideosApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");

        }

         private async Task Run()
    {
      var youtubeService = new YouTubeService(new BaseClientService.Initializer()
      {
        ApiKey = "REPLACE_ME",
        ApplicationName = this.GetType().ToString()
      });

      var searchListRequest = youtubeService.Search.List("snippet");
      searchListRequest.Q = "Google"; // Replace with your search term.
      searchListRequest.MaxResults = 50;

      // Call the search.list method to retrieve results matching the specified query term.
      var searchListResponse = await searchListRequest.ExecuteAsync();

      List<string> videos = new List<string>();
    

      // Add each result to the appropriate list, and then display the lists of
      // matching videos, channels, and playlists.
      foreach (var searchResult in searchListResponse.Items)
      {
        switch (searchResult.Id.Kind)
        {
          case "youtube#video":
            videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
            break;

          
        }
      }

      Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
   
    }
    }
}