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
        //[Authorize]
        public  async Task<IActionResult> FacePerson (List<IFormFile> files)
        {
            try{
                int isEyesBlink = 0;
                int notEyesBlink = 0;
                Boolean EyeBlink = true;
                
                foreach (var formFile in files)
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                    using (Stream imageFileStream = System.IO.File.OpenRead(filePath))
                    {
                        // The second argument specifies to return the faceId, while
                        // the third argument specifies not to return face landmarks.
                        List<DetectedFace> facesDetected = await _cognitivevision.DetectFaceRecognizeStream(imageFileStream, RecognitionModel.Recognition03);
                        EyeBlink = _cognitivevision.EyesBlink(facesDetected);
                        isEyesBlink = EyeBlink?isEyesBlink+1:isEyesBlink;
                        notEyesBlink = !EyeBlink?notEyesBlink+1:notEyesBlink;
                    }
                }
                Boolean HayPersona = isEyesBlink>0 && notEyesBlink>0;
                
                return new ObjectResult(new { HayPersona , urlPhoto = "https://www.gstatic.com/tv/thumb/persons/983712/983712_v9_ba.jpg"}) { StatusCode = 200};
            }
            catch (System.Exception error)
            {
                return new ObjectResult(new { error = error.Message , facesCount = 0 }) { StatusCode = 500};
            }
        }
        
    }
}
