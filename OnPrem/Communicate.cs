using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using OpenCvSharp;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;


namespace OnPrem
{
    class Communicate
    {
        const string SUB_KEY = "b98e8b34b5dc4846906569184eaef5a6";
        const string ENDPOINT = "https://faceouttest.cognitiveservices.azure.com/";

        // MAKE SURE TO REMOVE THESE VALUES IF THIS GOES PUBLIC

        IFaceClient client;
        public Communicate()
        {
            client = new FaceClient(new ApiKeyServiceClientCredentials(SUB_KEY)) { Endpoint = ENDPOINT };
        }

        private Mat getImage ()
        {
            // get face image here using mapped memory

            MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("FaceOutImage");

            Mutex mutex = Mutex.OpenExisting("FaceOutImgMutex");
            mutex.WaitOne();
            Mat mat;
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);

                mat = Mat.FromStream(reader.BaseStream,ImreadModes.Unchanged);
                
            }
            mmf.Dispose();
            mutex.ReleaseMutex();
            mutex.Dispose();
            return mat;
        }

        /*private async IList<DetectedFace> AzureFaceDetect ()
        {

            using Mat image = getImage(); 
            {
                 IList<DetectedFace> faces= await client.Face.DetectWithStreamAsync(image.ToMemoryStream(), true, detectionModel: DetectionModel.Detection03);
                 return faces;
            }
        }*/

        public async void AzureFaceDetect() // non-returning overload for test
        {

            using Mat image = getImage();
            {
                IList<DetectedFace> faces = await client.Face.DetectWithStreamAsync(image.ToMemoryStream(), true,returnFaceLandmarks:true, detectionModel: DetectionModel.Detection01);
                foreach (DetectedFace face in faces)
                {
                    var landmarks = face.FaceLandmarks;
                    var upperLipBottom = landmarks.UpperLipBottom;
                    var underLipTop = landmarks.UnderLipTop;

                    var centerOfMouth = new Point(
                        (upperLipBottom.X + underLipTop.X) / 2,
                        (upperLipBottom.Y + underLipTop.Y) / 2);

                    var eyeLeftInner = landmarks.EyeLeftInner;
                    var eyeRightInner = landmarks.EyeRightInner;

                    var centerOfTwoEyes = new Point(
                        (eyeLeftInner.X + eyeRightInner.X) / 2,
                        (eyeLeftInner.Y + eyeRightInner.Y) / 2);

                    Console.WriteLine("The vector points for face are {0} and {1}",
                        centerOfTwoEyes.X - centerOfMouth.X,
                        centerOfTwoEyes.Y - centerOfMouth.Y);
                }
                //Console.WriteLine(faces);
            }
        }
    }
}
