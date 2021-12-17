from PIL import Image
import cv2

CapFace = None

def __init__():

    #CapFace = Image.open("lastface.jpg")   # Temp assigning lastface.jpg to CapFace
    #is_success, im_buf_arr = cv2.imencode(".jpg", CapFace)
    #CapFace = im_buf_arr.tobytes()

    im = Image.open('lastface.jpg') 
    buf = io.BytesIO()

    # Converting The Image object to jpeg datastream
    im.save(buf, format='JPEG')
    print(typeof(buf))
    CapFace = buf
    
    
    