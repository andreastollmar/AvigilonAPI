namespace AvigilonApi.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class VideoController(IConfiguration configuration, 
                                HttpClientProivider httpClientProivider,
                                TokenProvider tokenProvider) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly HttpClientProivider _httpClientProivider = httpClientProivider;
        private readonly TokenProvider _tokenProvider = tokenProvider;

        public readonly string clientName = "WebEndpointClient";
        public readonly string baseUri = "https://srv03367:8443/mt/api/rest/v1/";
        public readonly string vd1 = "4xIx1DMwMLSwMDUxsTRJStRLTsw1MBASyP2n63D3r8-t3Qtf2Uer7GkBAA";
        public readonly string vd2 = "4xIx1DMwMLSwMDUxsTRJStRLTsw1MBQSyP2n63D3r8-t3Qtf2Uer7GkBAA";
        public readonly string lrf = "4xIx1DMwMLSwMDVMMk9OMdFLTsw1MBASkIsQPdOy12nPRp7ZvgcfBs0CAA";
        private string? _session;
        [HttpGet("/Cameras")]
        public async Task<List<CameraContract>> GetVideos()
        {
            var factory = new WebEndpointClientFactory(_configuration.GetValue("Avigilon:Secretkeys:Usernonce", "Usernonce"),
                _configuration.GetValue("Avigilon:Secretkeys:Userkey", "Usernonce"));

            var client = factory.Create(new Uri("https://srv03367:8443"));
            await client.Login(_configuration.GetValue("Avigilon:Login:Username", "Username"), 
                _configuration.GetValue("Avigilon:Login:Password", "Password"));
            
            var cameras = await client.GetCameras();
            return cameras;
        }        

        [HttpGet("/mediaQuery {date} {time} {camera}")]
        public async Task<IActionResult> GetViedoFromApi([FromRoute] string date, [FromRoute] string time, [FromRoute] string camera)
        {
            if (_session == null || _session == "")
               _session = await _tokenProvider.GenerateSessionTokenAsync(
                   _configuration.GetValue("Avigilon:Secretkeys:Usernonce", "Usernonce"),
                   _configuration.GetValue("Avigilon:Secretkeys:Userkey", "Userkey"),
                   _configuration.GetValue("Avigilon:Login:Username", "Username"),
                   _configuration.GetValue("Avigilon:Login:Password", "Password"),
                   clientName);

            var httpClient = _httpClientProivider.GetHttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "x-avg-session " + _session);


            var serializerSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

            var content = (new MediaContract
            {
                Session = _session,
                CameraId = camera.ToLower().Equals("vd1") ? vd1 : camera.ToLower().Equals("vd2") ? vd2 : lrf,
                Format = "fmp4",
                Media = "video",
                Range = "F300",
                Quality = "low",
                Burnin = false,
                Size = "640,480,le",
                T = time
            });
            Uri requestUri = new Uri(baseUri + $"media?session={content.Session}&cameraId={content.CameraId}&format={content.Format}&t={content.T}");
            //StringContent content2 = new StringContent(content, Encoding.UTF8, "application/json");
            Uri requestUri2 = new Uri(baseUri + $"media?session={content.Session}&cameraId={content.CameraId}&format=jpeg&t=live");

            //Uri requestUri = new Uri(baseUri + "media/");
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri2);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get cameras: response status code was {httpResponseMessage.StatusCode}");
            }
            // var mediaResponse = await JsonSerializer.DeserializeAsync<MediaResponseContract>(await httpResponseMessage.Content.ReadAsStreamAsync(), serializerSettings);
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var imgData = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            var directoryPath = "./Images/";
            var fileName = $"{camera}_" + DateTime.Now.ToShortTimeString() + ".jpeg";
            fileName = fileName.Replace(":", "_");
            string filePath = Path.Combine(directoryPath, fileName);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(imgData);
            }



            return Ok(responseString);
        }     

    }
}
