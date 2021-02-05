using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Api_face_recognition.Domain;
using Microsoft.Extensions.Options;
using Api_face_recognition.Services;
using Microsoft.AspNetCore.Http;

namespace Api_face_recognition.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FacePersonController : ControllerBase
    {
        private readonly ILogger<FaceRecognitionController> _logger;
        private readonly ICognitiveVisionService _cognitivevision;
        private readonly  IFirebaseService _firebase;
        private readonly AzureStorageConfiguration _azureStorage;
        public FacePersonController(ILogger<FaceRecognitionController> logger,IFirebaseService firebase, ICognitiveVisionService cognitivevision,IOptions<AzureStorageConfiguration> azureStorage)
        {
            _logger = logger;
            _cognitivevision = cognitivevision;
            _firebase = firebase;
            _azureStorage = azureStorage.Value ?? throw new ArgumentNullException(nameof(azureStorage));
        }

        [HttpPost]
        public  async Task<IActionResult> FacePerson (List<IFormFile> files)
        {
            try
            {
                return new ObjectResult(new { HayPersona=true, FaceId='40ce3dba-b616-4747-b3d5-3cabcb8e2b7d'}) { StatusCode = 200};
            }
            catch (System.Exception error)
            {
                return new ObjectResult(new { error = error.Message , facesCount = 0 }) { StatusCode = 500};
            }
            
        }
        
    }
}
