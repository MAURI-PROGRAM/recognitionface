using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Api_face_recognition.Domain;
using Api_face_recognition.Services;
using Microsoft.Extensions.Options;

namespace Api_face_recognition.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
         private readonly TokenStringsConfiguration _tokenstrings;
        //private readonly IAuthenticationService _authentication;

        public AuthenticationController(ILogger<AuthenticationController> logger,IOptions<TokenStringsConfiguration> tokenstrings)
        {
            _logger = logger;
            _tokenstrings = tokenstrings.Value;
            //_authentication = authentication;

        }

        [HttpGet]
        public  IActionResult Get()
        {
            JwtEntity loginResponse = new JwtEntity();
            if(true){
            //if(await _authentication.Login("AppRecognitionKey",KeyScret)){
                loginResponse = JwtServices.GenerarTokenJWT(_tokenstrings.Key,_tokenstrings.ExpiresSeconds);
            }
            return new ObjectResult(loginResponse) { StatusCode = 200};
        }

        
    }
}
