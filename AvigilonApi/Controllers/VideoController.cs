namespace AvigilonApi.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class VideoController(IConfiguration configuration, 
                                IHttpClientProvider httpClientProivider,
                                ITokenProvider tokenProvider,
                                IHandleMediaResponse handleMediaResponse,
                                AvigilonApiCalls avigilonApiCalls) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpClientProvider _httpClientProivider = httpClientProivider;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IHandleMediaResponse _handleMediaResponse = handleMediaResponse;
        private readonly AvigilonApiCalls _avigilonApiCalls = avigilonApiCalls;
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

            var isSuccess = await _avigilonApiCalls.SaveMediafile(_session, time, date, camera, isImg);

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
            
            var result = await _avigilonApiCalls.GetListOfTimelines(_session, bodyContract);

            return Ok(result);
        }

    }
}
