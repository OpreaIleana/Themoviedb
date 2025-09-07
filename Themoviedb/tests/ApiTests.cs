using System.Net;
using Newtonsoft.Json.Linq;

namespace Themoviedb.tests
{
    public class ApiTests

    // TODO: Create a Base Class for api tests and a Util class with all the methods used frequently
    // TODO: Replace hardcoded numbers
    {
        // Assertions

        private void AssertResponseIsNotNull(HttpResponseMessage httpResponseMessage)
        {
            Assert.IsNotNull(httpResponseMessage);
        }

        private void AssertStatusCodeIsOk(HttpStatusCode statusCode)
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That((int)statusCode, Is.EqualTo(200));
        } 

        [Test]
        [Description("Comparing get movies and get filtred movies lists and assert they are different")]

        public void CompareMoviesList()
        {
            // Get all movies (page 1)

            // Make the get all movies request 
            {
                HttpClient httpsClient = new();
                HttpRequestMessage httpRequest = new()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.themoviedb.org/3/discover/movie?include_adult=false&include_video=false&language=en-US"),
                    Headers =
                    {
                    { "accept", "application/json" },
                    { "Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI5ZTllNWY1MWQwODRiOTcwM2FkZmEzYWUwNWViZDI3OSIsIm5iZiI6MTc1NzA1NDg5Ni43NDIsInN1YiI6IjY4YmE4N2IwMWNjODM4MzYzMTFiYjk2MyIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.tPwoR-jnSb5MF1A4uOKNESBosO-uChDIQ3sT0mH4t5A" },
                    },
                };

                // Get response and check that is not null
                Task<HttpResponseMessage> httpResponse = httpsClient.SendAsync(httpRequest);
                HttpResponseMessage httpResponseMessage = httpResponse.Result;
                AssertResponseIsNotNull(httpResponseMessage);

                // Change the response content to String
                HttpContent responseContent = httpResponseMessage.Content;
                Task<string> responseData = responseContent.ReadAsStringAsync();
                string data = responseData.Result;
                Console.WriteLine(data);

                // Get status code and check it
                HttpStatusCode statusCode = httpResponseMessage.StatusCode;
                AssertStatusCodeIsOk(statusCode);

                // Get all movies (page 1) sorted by release date ascending

                // Make the get all movies request sorted by release date ascending
                HttpRequestMessage httpRequestFiltredMovies = new()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://api.themoviedb.org/3/discover/movie?include_adult=false&include_video=false&language=en-US&page=1&sort_by=primary_release_date.asc"),
                    Headers =
            {
                { "accept", "application/json" },
                { "Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI5ZTllNWY1MWQwODRiOTcwM2FkZmEzYWUwNWViZDI3OSIsIm5iZiI6MTc1NzA1NDg5Ni43NDIsInN1YiI6IjY4YmE4N2IwMWNjODM4MzYzMTFiYjk2MyIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.tPwoR-jnSb5MF1A4uOKNESBosO-uChDIQ3sT0mH4t5A" },
            },
                };

                // Get response and check that is not null
                Task<HttpResponseMessage> httpResponseFiltredMovies = httpsClient.SendAsync(httpRequestFiltredMovies);
                HttpResponseMessage httpResponseMessageFiltredMovies = httpResponseFiltredMovies.Result;
                AssertResponseIsNotNull(httpResponseMessageFiltredMovies);

                // Change the response content to String
                HttpContent responseContentFiltredMovies = httpResponseMessageFiltredMovies.Content;
                Task<string> responseData1 = responseContentFiltredMovies.ReadAsStringAsync();
                string dataFiltredMovies = responseData1.Result;
                Console.WriteLine(dataFiltredMovies);

                // Get status code and check it
                HttpStatusCode statusCodeFiltredMovies = httpResponseMessageFiltredMovies.StatusCode;
                AssertStatusCodeIsOk(statusCodeFiltredMovies);

                // Compare the lists: get movies and get filtred movies and assert that they are different
                Assert.That(JToken.DeepEquals(data, dataFiltredMovies), Is.False);

                // Close the connection
                httpsClient.Dispose();
            }
        }
    }
}