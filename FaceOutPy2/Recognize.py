
## This is to recognize the face that will be passed to this class as a PIL image object
## This class will then contact the Azure CS API and query the API based on the operations requested by the caller
import threading
import globals
import asyncio
import io
import glob
import os
import sys
import time
import uuid
import requests
import multiprocessing
from urllib.parse import urlparse
from io import BytesIO
from PIL import Image, ImageDraw
try:
    from azure.cognitiveservices.vision.face import FaceClient
    from msrest.authentication import CognitiveServicesCredentials
    from azure.cognitiveservices.vision.face.models import TrainingStatusType, Person
except:
    print ("error importing Azure services! \n Probably a Network issue")

KEY = "bd86f063ef1749f7bd21c2de77bfdf38"
ENDPOINT = "https://faceoutfr.cognitiveservices.azure.com/"

class Recognize:
    
    def __init__(self, *args, **kwargs):
        #key and endpoint for Azure Cognitive Services Face API
        #KEY = "bd86f063ef1749f7bd21c2de77bfdf38"
        #ENDPOINT = "https://faceoutfr.cognitiveservices.azure.com/"
        return super().__init__(*args, **kwargs)

    def runRecog(self):
        # Create an authenticated FaceClient, then run recognition against the passed object
        face_client = FaceClient(ENDPOINT, CognitiveServicesCredentials(KEY))
        RecogOut = None
        DetectionOutput = face_client.face.detect_with_stream(globals.CapFace)
        print (DetectionOutput.__dict__)


if __name__ == "__main__":
    recObj= Recognize()
    recObj.runRecog()
