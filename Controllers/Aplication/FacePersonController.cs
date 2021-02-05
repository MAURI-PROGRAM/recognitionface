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
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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
                BlobContainerClient container = new BlobContainerClient(_azureStorage.ConnectionString, _azureStorage.ContainerName);
                
                //List<DetectedFace> facesDetected;
                //Stream mystream ;
                List<string> imagesStream = new List<string>(); 
                List<Boolean> EyesBlink = new List<Boolean>();
                int isEyesBlink = 0;
                int notEyesBlink = 0;
                foreach (var formFile in files)
                {
                    string fileName = DateTime.Now.ToString("MMddyyyyHHmmssff") + formFile.FileName;
                    BlobClient blob = container.GetBlobClient(fileName);
                    if (formFile.Length > 0)
                    {
                        var filePath = Path.GetTempFileName();

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        blob.Upload(filePath);
                        Uri webazurestorage = new Uri(_azureStorage.UrlStorage );
                        Uri urlPhoto = new Uri(webazurestorage, _azureStorage.ContainerName + "/" +fileName);
                        imagesStream.Add(urlPhoto.ToString());
                        /* var filePath = Path.GetTempFileName();
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        using (Stream s = System.IO.File.OpenRead(filePath))
                        {
                            imagesStream.Add(s);
                        } */
                        
                        
                    } else{
                        return new ObjectResult(new { error = "No se encontro ninguna imagen tipo" }) { StatusCode = 500};
                    }
                }
                foreach (var imageStream in imagesStream)
                {
                    List<DetectedFace> facesDetected = await _cognitivevision.DetectFaceRecognize( imageStream, RecognitionModel.Recognition03);
                    var facelandmarks = facesDetected[0].FaceLandmarks;
                    Double  difLeftX = Math.Sqrt(
                                    Math.Pow(facelandmarks.EyeLeftInner.X-facelandmarks.EyeLeftOuter.X,2)+
                                    Math.Pow(facelandmarks.EyeLeftInner.Y-facelandmarks.EyeLeftOuter.Y,2));
                    Double  difLeftY = Math.Sqrt(
                                    Math.Pow(facelandmarks.EyeLeftBottom.X-facelandmarks.EyeLeftTop.X,2)+
                                    Math.Pow(facelandmarks.EyeLeftBottom.Y-facelandmarks.EyeLeftTop.Y,2));

                    Double difRightX = Math.Sqrt(
                                    Math.Pow(facelandmarks.EyeRightInner.X-facelandmarks.EyeRightOuter.X,2)+
                                    Math.Pow(facelandmarks.EyeRightInner.Y-facelandmarks.EyeRightOuter.Y,2));
                    Double difRightY = Math.Sqrt(
                                    Math.Pow(facelandmarks.EyeRightBottom.X-facelandmarks.EyeRightTop.X,2)+
                                    Math.Pow(facelandmarks.EyeRightBottom.Y-facelandmarks.EyeRightTop.Y,2));
                    
                    Double leftEAR = difLeftY / difLeftX;
                    Double rightEAR = difRightY/difRightX;
                    Double EYE_AR_THRESH = 0.3;
                    bool leftBlink = leftEAR < EYE_AR_THRESH;
                    bool rightBlink = rightEAR < EYE_AR_THRESH;

                    var EyesAspectRatio = new { 
                        leftEAR,
                        rightEAR
                    };
                    /* var EyesBlink = new { 
                        leftBlink,
                        rightBlink
                    }; */
                    var EyeBlink = leftBlink || rightBlink ;

                    Guid guid1 = facesDetected[0].FaceId.Value;
                    isEyesBlink = EyeBlink?isEyesBlink+1:isEyesBlink;
                    notEyesBlink = !EyeBlink?notEyesBlink+1:notEyesBlink;
                    //EyesBlink.Add(EyeBlink);
                }
                //Dictionary<string, object> dataObject = _firebase.TransformObjectRecognition(IMAGE_URL,facesDetected[0].FaceId.Value.ToString());
                //bool save = await _firebase.SaveObject("face-reconigtion",dataObject);
                //return new ObjectResult(new {  faceId = guid1, EyesAspectRatio,EyesBlink }) { StatusCode = 200};
                return new ObjectResult(new { Parpadeo=isEyesBlink, NoParpadeo=notEyesBlink,HayPersona=isEyesBlink>0 && notEyesBlink>0}) { StatusCode = 200};
                //return new ObjectResult(new { error = "error.Message" , facesCount = 0 }) { StatusCode = 500};
            }
            catch (System.Exception error)
            {
                return new ObjectResult(new { error = error.Message , facesCount = 0 }) { StatusCode = 500};
            }
            
        }
        
    }
}
