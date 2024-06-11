using Avigilon.Infrastructure.Validation;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpPost("/mediaQuery {camera} {isImg}")]
        public async Task<IActionResult> GetMediaFromApi([FromBody] List<RequestMediaContract> dateTimes, [FromRoute] string camera, [FromRoute] bool isImg)
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

            foreach (var item in dateTimes)
            {                
                if (_inputValidations.ValidateDateInputFromUser(item.StartDate))
                {
                    tasks.Add(HandleMediaSavingAsync(item, camera, isImg, successMessage));
                }
            }

            await Task.WhenAll(tasks);

            return Ok(successMessage);
            
        }
        private async Task HandleMediaSavingAsync(RequestMediaContract item, string camera, bool isImg, string successMessage)
        {
            var isSuccess = await _avigilonApiCalls.SaveMediafile(_session, item.Time, item.StartDate, camera, isImg);

            if (isSuccess)
            {
                successMessage += "Saved file successfully\n";
            }
            else
            {
                successMessage += "Error when saving file\n";
            }
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
            if (_inputValidations.ValidateDateInputFromUser(bodyContract.StartDate) && _inputValidations.ValidateDateInputFromUser(bodyContract.EndDate))
            {     
                var result = await _avigilonApiCalls.GetListOfTimelines(_session, bodyContract);
                return Ok(result);                
            }
            return BadRequest();

        }

    }
}
