using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Api_face_recognition.Domain;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Api_face_recognition.Services
{
    public class CognitiveVisionService:ICognitiveVisionService
    {
        private readonly IFaceClient _client;
        public CognitiveVisionService(string endpoint, string key)
        {
            _client = new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        public  async Task<List<DetectedFace>> DetectFaceRecognize( string url, string recognition_model)
        {
            IList<DetectedFace> detectedFaces = await _client.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection02);
            return detectedFaces.ToList();
        }  

        public  async Task<VerifyResult> VerifyTwoFaces( Guid FaceId1, Guid FaceId2)
        {
            VerifyResult verifyObjectResult = await _client.Face.VerifyFaceToFaceAsync(FaceId1, FaceId2);
            return verifyObjectResult;
        } 
    }
}
