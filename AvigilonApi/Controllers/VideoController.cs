using Avigilon.Infrastructure.Validation;

namespace AvigilonApi.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    public class VideoController(IConfiguration configuration, 
                                ITokenProvider tokenProvider,
                                IAvigilonApiCalls avigilonApiCalls,
                                IInputValidations inputValidations) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IAvigilonApiCalls _avigilonApiCalls = avigilonApiCalls;
        private readonly IInputValidations _inputValidations = inputValidations;
        public readonly string clientName = "WebEndpointClient";
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
            if (_inputValidations.ValidateDateInputFromUser(date))
            {
                var isSuccess = await _avigilonApiCalls.SaveMediafile(_session, time, date, camera, isImg);

                return isSuccess ? Ok("Saved file successfully") : BadRequest("Failed to save media");
            }
            return BadRequest();
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
            if (_inputValidations.ValidateDateInputFromUser(bodyContract.StartDate))
            {
                if (_inputValidations.ValidateDateInputFromUser(bodyContract.EndDate))
                {
                    var result = await _avigilonApiCalls.GetListOfTimelines(_session, bodyContract);
                    return Ok(result);
                }
            }
            return BadRequest();

        }

    }
}
