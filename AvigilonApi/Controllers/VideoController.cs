
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
        private static string? _session;
        private string? _savedFiles;

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
        [AllowAnonymous]
        [HttpPost("/saveMedia")]
        public async Task<IActionResult> GetMediaFromApi([FromBody] RequestMediaContract requestMedia)
        {
            var successMessage = "";
            if (_session == null || _session == "")
               _session = await _tokenProvider.GenerateSessionTokenAsync(
                   _configuration.GetValue("Avigilon:Secretkeys:Usernonce", "Usernonce"),
                   _configuration.GetValue("Avigilon:Secretkeys:Userkey", "Userkey"),
                   _configuration.GetValue("Avigilon:Login:Username", "Username"),
                   _configuration.GetValue("Avigilon:Login:Password", "Password"),
                   clientName);

            var tasks = new List<Task>();

            foreach (var item in requestMedia.RequestMediaBodyContracts)
            {                
                if (_inputValidations.ValidateDateInputFromUser(item.Date))
                {
                    tasks.Add(HandleMediaSavingAsync(item, requestMedia.Camera, requestMedia.IsImg, successMessage));
                }
            }

            await Task.WhenAll(tasks);

            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var downloadsFolderPath = Path.Combine(userProfilePath, "Downloads");

            return Ok(_savedFiles + "Media saved at: " + downloadsFolderPath);
            
        }
        private async Task HandleMediaSavingAsync(RequestMediaBodyContract item, string camera, bool isImg, string successMessage)
        {
            var success = await _avigilonApiCalls.SaveMediafile(_session, item.Time, item.Date, camera, isImg);
            if(success)
            {
                _savedFiles += $"Saved file successfully {item.Date} - {item.Time}\n";
            }
            else
            {
                _savedFiles += $"Saved file failed or no video at given timestamp {item.Date} - {item.Time}\n";
            }
        }

        [HttpPost("/getTimeline")]
        public async Task<IActionResult> GetListOfEntities([FromBody] RequestBodyContract bodyContract)
        {
            if (_session == null || _session == "")
                _session = await _tokenProvider.GenerateSessionTokenAsync(
                    _configuration.GetValue("Avigilon:Secretkeys:Usernonce", "Usernonce"),
                    _configuration.GetValue("Avigilon:Secretkeys:Userkey", "Userkey"),
                    _configuration.GetValue("Avigilon:Login:Username", "Username"),
                    _configuration.GetValue("Avigilon:Login:Password", "Password"),
                    clientName);
            if (_inputValidations.ValidateDateInputFromUser(bodyContract.StartDate) && _inputValidations.ValidateDateInputFromUser(bodyContract.EndDate))
            {     
                var result = await _avigilonApiCalls.GetListOfTimelines(_session, bodyContract);
                return Ok(result);                
            }
            return BadRequest();

        }

        [HttpPost("/Logout")]
        public async Task<IActionResult> Logout()
        {            
            if (_session == null || _session == "")
            {
                return BadRequest();
            }
            var logoutSuccess = await _avigilonApiCalls.Logout(_session);
            if (logoutSuccess)
            {
                _session = null;
                return Ok("Logged out");
            }
            return BadRequest("Logout failed");
        }

    }
}
