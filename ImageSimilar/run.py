#
# https://qiita.com/K-jun/items/cab923d49a939a8486fc
#

from IPython.display import Image
import os

base_dir = "D:/python/annoy/"
thumb_dir = os.path.join(base_dir, "thumb/")
img_path = os.path.join(thumb_dir, "1595153765.jpg")
Image(img_path)

####


#import tensorflow.keras
#print(tensorflow.keras.__version__)
#from tensorflow.keras.applications.vgg19 import VGG19
#base_model = VGG19(weights="imagenet")
#base_model.summary()

####
import tensorflow as tf
print(tf.__version__)


# tensorflowがGPUも使えるかどうか
from tensorflow.python.client import device_lib
print(device_lib.list_local_devices())


# TensorFlowのGPUメモリ使用量の制限
# https://github.com/tensorflow/tensorflow/issues/7072
print("TensorFlowのGPUメモリ使用量の制限")
import tensorflow as tf
#from tensorflow.keras.backend.tensorflow_backend import set_session
from tensorflow.keras.backend import set_session
config = tf.ConfigProto()
config.gpu_options.allow_growth = True  # dynamically grow the memory used on the GPU
config.log_device_placement = True  # to log device placement (on which device the operation ran)
                                    # (nothing gets printed in Jupyter, only if you run it standalone)

config.gpu_options.per_process_gpu_memory_fraction = 0.5
sess = tf.Session(config=config)
set_session(sess)  # set this TensorFlow session as the default session for Keras
#tf.keras.backend.set_sess(tf.Session(config=config))
#tf.keras.backend.tensorflow_backend(tf.Session(config=config))

####
from tensorflow.keras.applications.vgg16 import VGG16
base_model = VGG16(weights='imagenet')
base_model.summary()
print(base_model.input)
print(base_model.output)


####とりあえず1枚の画像を読み込んで、この入力サンプルに対する予測値の出力を生成します。
from tensorflow.keras.preprocessing import image
import numpy as np

img_path = os.path.join(thumb_dir, "1595153765.jpg")
img = image.load_img(img_path, target_size=(224, 224))

input = image.img_to_array(img)
result = base_model.predict(np.array([input]))
print("配列の中身", result)
print("配列の長さ: ", len(result[0]))

####

#中間層の抽出

from tensorflow.keras import Model, layers
model = Model(inputs=base_model.input, outputs=base_model.get_layer("fc2").output)
print(model.input)
print(model.output)

####

#この関数も1枚試してみましょう。

#img_path = "./gdrive/My Drive/Google Colaboratory/ラーメン類似検索/ramen_images/ramen1.jpg"
img = image.load_img(img_path, target_size=(224, 224))
input = image.img_to_array(img)
result = model.predict(np.array([input]))
print("実際の値", result)
print("配列の長さ: ", len(result[0]))

####
#Annoy
#モデルのロード
from annoy import AnnoyIndex
#print(annoy.__version__)

dim = 4096
annoy_model = AnnoyIndex(dim)

#Indexと一緒にベクトルを登録
#from keras.preprocessing import image
#from keras.applications.vgg19 import preprocess_input
from tensorflow.keras.preprocessing import image
from tensorflow.keras.applications.vgg16 import preprocess_input

#img_path = os.path.join(thumb_dir, "1551256482.jpg")
#from tensorflow.keras.preprocessing import image
#img = image.load_img(img_path, target_size=(224, 224))
#x = image.img_to_array(img)
#import numpy as np
#x = np.expand_dims(x, axis=0)
#x = preprocess_input(x)
#print(x)

#numimg=0
#fc2_features = model.predict(x)
#annoy_model.add_item(numimg, fc2_features[0])

####
#import os
import glob

numimg=0
glob_dir = os.path.join(thumb_dir, "*.jpg")
files = glob.glob(glob_dir)
for file in files:
    print(file)
    img_path = file
    img = image.load_img(img_path, target_size=(224, 224))
    x = image.img_to_array(img)
    x = np.expand_dims(x, axis=0)
    x = preprocess_input(x)
    fc2_features = model.predict(x)
    annoy_model.add_item(numimg, fc2_features[0])
    print(id)
    numimg += 1

print('num files=' + str(numimg))

#ビルドして、ファイルとして保存する
annoy_model.build(numimg)
save_path = os.path.join(base_dir, "result.ann")
annoy_model.save(save_path)

# 確認する
#annoy_model.unload()
#trained_model.load("D:/python/annoy/images_next.ann")

trained_model = AnnoyIndex(4096)
trained_model.load('D:/python/annoy/images_next.ann') # モデルを読み込むことも可能です。
print(trained_model.get_nns_by_item(0, 6000)) # インデックス0付近の1000個のデータの返します
items = trained_model.get_nns_by_item(1, 6, search_k=-1, include_distances=False)
print(items)


####txt出力####
txt1 = 'D:/python/annoy/test.csv'
file = open(txt1, "w", encoding = "utf_8")
for j in range(1, numimg+1):
    items = trained_model.get_nns_by_item(j, 7, search_k=-1, include_distances=False)
    file.write(str(items) + "\n")

file.close()


####任意の画像と比較したいとき####
#まずは『関数』でベクトル化

img_path2 = "D:/python/annoy/1605611080.jpg"
img_path2 = "D:/python/annoy/thumb/1551257517.jpg"
img2 = image.load_img(img_path2, target_size=(224, 224))
x2 = image.img_to_array(img2)
x2 = np.expand_dims(x2, axis=0)
x2 = preprocess_input(x2)
fc2_features2 = model.predict(x2)
#続けてAnnoyで検索。get_nns_by_vectorという関数を使います。
result2 = trained_model.get_nns_by_vector(fc2_features2[0], 6, search_k=-1, include_distances=False)
print(result2)
