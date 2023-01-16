using System;
using Google.Apis.YouTube.v3;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3.Data;
using System.Reflection;
using Google.Apis.Upload;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ;
using RabbitMQ.Client.Events;
using Newtonsoft;
using Newtonsoft.Json;

namespace SvideosApi
{
    class Program
    {
        static string basepath = "C:\\Users\\LTF-PRODUCCION\\Desktop\\My Repos\\";
        static async Task  Main(string[] args)
        {



            var factory = new ConnectionFactory() { HostName = "localhost" };
            var messageDefinition = new {Title="",Description="", ChannelId = "",Video="" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hola", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += async (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var convertedMessage = JsonConvert.DeserializeAnonymousType(message, messageDefinition);

                        await Upload(convertedMessage.Title, convertedMessage.Description, convertedMessage.ChannelId, convertedMessage.Video);
                        Console.WriteLine($"[x] Received {message}");
                    };

                    channel.BasicConsume(queue: "hola", autoAck: true, consumer: consumer);

                    Console.WriteLine("Presiona cualquier letra para salir...");
                    Console.ReadLine();

                }
            }
        

            
        }

        
        private static async Task Upload(string Title, string Description, string ChannelId, string videoName) 
        {

            UserCredential credential;
            using (var stream = new FileStream("Secrets.json.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                  
                    new[] { YouTubeService.Scope.YoutubeUpload },
                    "user",
                    CancellationToken.None
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SvideosApi2",
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = Title;
            video.Snippet.Description = Description;
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "22";
            video.Snippet.ChannelId = ChannelId;//"UCpjuL28buAOG0tSVcmMuwFg";
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public";
            video.Status.MadeForKids = false;
            string filePath = basepath + videoName;
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }

            void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
            {
                switch (progress.Status)
                {
                    case UploadStatus.Uploading:
                        Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                        break;

                    case UploadStatus.Failed:
                        Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                        break;
                }
            }

            void videosInsertRequest_ResponseReceived(Video video)
            {
                Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            }
        }
        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }



        private static async Task Search()
        {
            Console.WriteLine("SearchTerm");
            var SearchTerm = Console.ReadLine();
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyAzW0XoMhKqXtTtNtzP0fgLdZUIU_OI5zA",
                ApplicationName = "SvideosApi2",
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = "SearchTerm"; 
            searchListRequest.MaxResults = 50;

            
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();


            
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