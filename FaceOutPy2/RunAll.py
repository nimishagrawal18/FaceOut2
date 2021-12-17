
import webcam
import Recognize
import globals
import threading

CaptObj = webcam.Capturing(1,"Capture",1)

RecogObj = threading.Thread(target=Recognize.Recognize(), name='Recognition')

CaptObj.start()
RecogObj.start()

CaptObj.join()
RecogObj.join()