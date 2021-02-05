using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Api_face_recognition.Domain;
using Microsoft.Extensions.Options;
using Api_face_recognition.Services;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Api_face_recognition.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacePersonController : ControllerBase
    {
        private readonly ILogger<FacePersonController> _logger;
        private readonly ICognitiveVisionService _cognitivevision;
        private readonly  IFirebaseService _firebase;
        public FacePersonController(ILogger<FacePersonController> logger,IFirebaseService firebase, ICognitiveVisionService cognitivevision)
        {
            _logger = logger;
            _cognitivevision = cognitivevision;
            _firebase = firebase;
        }

        [HttpPost]
        [Authorize]
        public  IActionResult FacePerson (List<IFormFile> files)
        {
            return new ObjectResult(new { HayPersona=true, urlPhoto = "https://www.gstatic.com/tv/thumb/persons/983712/983712_v9_ba.jpg"}) { StatusCode = 200};
        }
        
    }
}
