namespace AvigilonApi.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class VideoController(IConfiguration configuration, 
                                HttpClientProivider httpClientProivider,
                                TokenProvider tokenProvider,
                                HandleMediaResponse handleMediaResponse) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly HttpClientProivider _httpClientProivider = httpClientProivider;
        private readonly TokenProvider _tokenProvider = tokenProvider;
        private readonly HandleMediaResponse _handleMediaResponse = handleMediaResponse;

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

        [HttpGet("/mediaQuery {date} {time} {camera} {isImg}")]
        public async Task<IActionResult> GetMediaFromApi([FromRoute] string date, [FromRoute] string time, [FromRoute] string camera, [FromRoute] bool isImg)
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
            var dateTime = "live";
            if (!time.Equals("live"))
            {
                dateTime = date + "T" + time + ".057Z,c";
            }

            var content = (new MediaContract
            {
                Session = _session,
                CameraId = camera.ToLower().Equals("vd1") ? vd1 : camera.ToLower().Equals("vd2") ? vd2 : lrf,
                Format = isImg ? "jpeg" : "fmp4",
                Media = "video",
                Range = "F300",
                Quality = "low",
                Burnin = false,
                Size = "640,480,le",
                T = dateTime
            });
            Uri requestUri = new Uri(baseUri + $"media?session={content.Session}&cameraId={content.CameraId}&format={content.Format}&t={content.T}");

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get video: response status code was {httpResponseMessage.StatusCode}");
            }
            var mediaData = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            var isSuccess = await _handleMediaResponse.SaveMediaResponse(camera, mediaData, isImg);


            return isSuccess ? Ok("Saved file successfully") : BadRequest("Failed to save media");

        }

        [HttpPost("/getList")]
        public async Task<IActionResult> GetListOfEntities([FromBody] RequestBodyContract bodyContract)
        {
            if (_session == null || _session == "")
                _session = await _tokenProvider.GenerateSessionTokenAsync(
                    _configuration.GetValue("Avigilon:Secretkeys:Usernonce", "Usernonce"),
                    _configuration.GetValue("Avigilon:Secretkeys:Userkey", "Userkey"),
                    _configuration.GetValue("Avigilon:Login:Username", "Username"),
                    _configuration.GetValue("Avigilon:Login:Password", "Password"),
                    clientName);

            string[] cameraIds = [vd1, vd2, lrf];
            var cameraId = bodyContract.Camera.ToLower().Equals("vd1") ? vd1 : bodyContract.Camera.ToLower().Equals("vd2") ? vd2 : lrf;

            var httpClient = _httpClientProivider.GetHttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "x-avg-session " + _session);

            Uri requestUri = new Uri(baseUri + $"timeline?session={_session}&cameraIds={cameraId}&scope={bodyContract.Interval}_SECONDS&start={bodyContract.StartDate}T06:00:00.0&end{bodyContract.EndDate}T06:00:00.0&storage=ALL");

            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get list of entrys: response status code was {httpResponseMessage.StatusCode}");
            }

            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            var message = JsonSerializer.Deserialize<TimeLineResult>(responseString);

            return Ok(message.Result.Timelines);
        }

    }
}
