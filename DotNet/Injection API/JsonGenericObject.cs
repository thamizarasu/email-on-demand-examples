﻿using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using InjectionApi.Sdk.Client;
using RestSharp;

namespace SocketLabs.OnDemand.Api.HttpInjection
{
    static public partial class Samples
    {
        /// <summary>
        /// This example uses generic JsonObject containers to create a POST
        /// request for sending one or more SMTP messages through Email-On-Demand.
        /// 
        /// The JSON request generated by this sample code looks like this:
        /// 
        ///{
        ///    "ApiKey": "YOUR API KEY HERE",
        ///    "Messages": [{
        ///        "Subject": "Email subject line for generic object example.",
        ///        "TextBody": "The text portion of the message.",
        ///        "HtmlBody": "<h1>The HTML portion of the message</h1><br/><p>A paragraph.</p>",
        ///        "To": [{
        ///            "EmailAddress": "to@example.com",
        ///            "FriendlyName": "Customer Name"
        ///        }],
        ///        "From": {
        ///            "EmailAddress": "from@example.com",
        ///            "FriendlyName": "The ABC Company"
        ///        }
        ///    }]
        ///}
        /// </summary>
        public static void SimpleInjectionViaRestSharpAsJson(string yourApiKey, string apiUrl)
        {
            // The client object processes requests to the SocketLabs Injection API.
            var client = new RestClient(apiUrl);

            // Construct the objects used to generate JSON for the POST request.
            var recipient1 = new JsonObject();
            recipient1.Add("EmailAddress", "to@example.com");
            recipient1.Add("FriendlyName", "Customer Name");

            var toList = new JsonObject[1];
            toList[0] = recipient1;

            var fromField = new JsonObject();
            fromField.Add("EmailAddress", "from@example.com");
            fromField.Add("FriendlyName", "The ABC Company");

            var message1 = new JsonObject();
            message1.Add("Subject", "Email subject line for generic object example.");
            message1.Add("TextBody", "The text portion of the message.");
            message1.Add("HtmlBody", "<h1>The HTML portion of the message</h1><br/><p>A paragraph.</p>");
            message1.Add("To", toList);
            message1.Add("From", fromField);

            var messageArray = new JsonObject[1];
            messageArray[0] = message1;

            var body = new JsonObject();
            body.Add("ApiKey", yourApiKey);
            body.Add("Messages", messageArray);

            try
            {
                // Generate a new POST request.
                var request = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };

                // Store the request data in the request object.
                request.AddBody(body);

                // Make the POST request.
                var result = client.ExecuteAsPost(request, "POST");

                // Store the response result in our custom class.
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(result.Content)))
                {
                    var serializer = new DataContractJsonSerializer(typeof (PostResponse));
                    var resp = (PostResponse) serializer.ReadObject(stream);

                    // Display the results.
                    if (resp.ErrorCode.Equals("Success"))
                    {
                        Console.WriteLine("Successful injection!");
                    }
                    else
                    {
                        Console.WriteLine("Failed injection! Returned JSON is: ");
                        Console.WriteLine(result.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error, something bad happened: " + ex.Message);
            }
        }
    }
}