using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using OpenCvSharp;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace OnPrem
{
	class Capture
	{
        static void Main(string[] args)
        {
			System.Environment.Exit(0);
		}

		public void startCapture(int cam)
		{

			//ThreadStart captureRef = new ThreadStart(capture);
			Thread captureThread = new Thread(() => capture(cam));
			Console.WriteLine("Trying to start capture...");
			
			Stopwatch sw = new Stopwatch();
			sw.Start();

			try
			{
				//Debug.WriteLine("Haha", "V");
				Console.WriteLine("- Attempting to start capture thread");
				captureThread.Start();
                Console.WriteLine("- Capture started");
				Stopwatch.StartNew();
			}
			catch (Exception)
			{
				Console.WriteLine("Capture failed! :( ");
				return;
			}
			captureThread.Join();
			sw.Stop();
            Console.WriteLine("Capture ended in {0} seconds...",sw.ElapsedMilliseconds/1000);
			return;
		}

		private static void capture (int camera)
		{
			var capture = new VideoCapture(camera);
			var window = new Window("FaceOut capture feed");
			var t = new ResourcesTracker();
			Mat image = t.NewMat();
			int loopCtr = 0;
			Rect[] faces = { };

			MemoryMappedFile MemImage = MemoryMappedFile.CreateNew("FaceOutImage", 1024000);
			//MemoryMappedViewStream access = MemImage.CreateViewStream();

			CascadeClassifier classifier = new CascadeClassifier();
			classifier.Load("C:\\Users\\nimis\\source\\repos\\FaceOut2\\OnPrem\\haarcascade_frontalface_default.xml");

			bool mutexCreated;
			Mutex mutex = new Mutex(false, "FaceOutImgMutex", out mutexCreated);

			//REMEMBER = Find way to remove this machine-specific path

			while (true)
			{
				loopCtr++;
				capture.Read(image);
				if (image.Empty())
					break;
				
				if (Cv2.WaitKey(1) == 113) // Q
					break;

				if (loopCtr >= 30) // Condition so that the detection is only triggered once every 30 frames
				{

					var gray = t.NewMat();
					Cv2.CvtColor(src: image, gray, ColorConversionCodes.BGR2GRAY);
					// window.ShowImage(gray);

					faces = classifier.DetectMultiScale(gray, 1.3, 5);

					// Some testing code below -
					// Mat facecrop = image;
					/*if (faces.Length>1)
					{
						Console.WriteLine("Only one at a time, please! Only the first face will be processed");
					}*/
					// if (faces.Length > 0) facecrop = t.T(new Mat(image, faces[0]));

					

					mutex.WaitOne();
					using (MemoryMappedViewStream stream = MemImage.CreateViewStream())
					{
						BinaryWriter writer = new BinaryWriter(stream);
						writer.Write(image.ToBytes());
					}

					mutex.ReleaseMutex();

					Communicate CommObj = new Communicate();
					CommObj.AzureFaceDetect();

				}

				foreach (var face in faces)
                {
					Cv2.Rectangle(image, face, Scalar.DarkCyan);
					//facecrop = t.T(new Mat(image, face));
				}

				window.ShowImage(image); // to show coloured image

				if (loopCtr>=30) // Stopping the memory leak
                {
					t.Dispose();
					t = new ResourcesTracker();
					image = t.NewMat();
					loopCtr = 0;
                }

				// TODO : Move the capture to a mapped file for the other components

				

			}
			mutex.Close();
			mutex.Dispose();
			t.Dispose();
			MemImage.Dispose();
			capture.Dispose();

		}
		
	}
}
