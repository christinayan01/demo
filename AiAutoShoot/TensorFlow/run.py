#
# 特定の画像filenameの設計者を推定する
#
# copyright 2020 christinayan by Takahiro Yanai.
# 2020.12.12
#

import tensorflow.keras
print(tensorflow.keras.__version__)

####

classes = ['cool', 'bad']
nb_classes = len(classes)
batch_size_for_data_generator = 20

base_dir = "d:\\python\\bunrui\\"

import os
train_dir = os.path.join(base_dir, 'train_images')
validation_dir = os.path.join(base_dir, 'validation_images')
test_dir = os.path.join(base_dir, 'test_images')

train_cool_dir = os.path.join(train_dir, 'cool')
train_bad_dir = os.path.join(train_dir, 'bad')

validation_cool_dir = os.path.join(validation_dir, 'cool')
validation_bad_dir = os.path.join(validation_dir, 'bad')

test_cool_dir = os.path.join(test_dir, 'cool')
test_bad_dir = os.path.join(test_dir, 'bad')

img_rows, img_cols = 150, 150

print('train_cool_dir:', len(os.listdir(train_cool_dir)), train_cool_dir)
print('train_bad_dir:', len(os.listdir(train_bad_dir)), train_bad_dir)
total_train = len(os.listdir(train_cool_dir)) + len(os.listdir(train_bad_dir))

print('validation_cool_dir:', len(os.listdir(validation_cool_dir)), validation_cool_dir)
print('validation_bad_dir:', len(os.listdir(validation_bad_dir)), validation_bad_dir)
total_val = len(os.listdir(validation_cool_dir)) + len(os.listdir(validation_bad_dir))

print('test_cool_dir:', len(os.listdir(test_cool_dir)), train_cool_dir)
print('test_bad_dir:', len(os.listdir(test_bad_dir)), test_bad_dir)

####
import tensorflow as tf
from tensorflow.keras.preprocessing.image import ImageDataGenerator

train_datagen = ImageDataGenerator(rescale=1.0 / 255,shear_range=0.2,zoom_range=0.2,horizontal_flip=True)
train_generator = train_datagen.flow_from_directory(
    directory=train_dir,
    target_size=(img_rows, img_cols),
    color_mode='rgb',
    classes=classes,
    class_mode='categorical',
    batch_size=batch_size_for_data_generator,
    shuffle=True)

####
test_datagen = ImageDataGenerator(rescale=1.0 / 255)
validation_generator = test_datagen.flow_from_directory(
    directory=validation_dir,
    target_size=(img_rows, img_cols),
    color_mode='rgb',
    classes=classes,
    class_mode='categorical',
    batch_size=batch_size_for_data_generator,
    shuffle=True)

####
validation_image_generator = ImageDataGenerator(rescale=1.0 / 255)
val_data_gen = validation_image_generator.flow_from_directory(
    directory=validation_dir,
    target_size=(img_rows, img_cols),
    color_mode='rgb',
    classes=classes,
    class_mode='categorical',
    batch_size=batch_size_for_data_generator,
    shuffle=True)

####
from tensorflow.keras.layers import Input, Dense, Dropout, Activation, Flatten
from tensorflow.keras.applications.vgg16 import VGG16

input_tensor = Input(shape=(img_rows, img_cols, 3))
vgg16 = tf.keras.applications.vgg16.VGG16(include_top=False, weights='imagenet', input_tensor=input_tensor)
vgg16.summary()

#### VGG16モデルに全結合分類器を追加する
from tensorflow.keras.models import Sequential, Model

top_model = Sequential()
top_model.add(Flatten(input_shape=vgg16.output_shape[1:]))
top_model.add(Dense(256, activation='relu'))
top_model.add(Dropout(0.5))
top_model.add(Dense(nb_classes, activation='softmax'))
model = Model(inputs=vgg16.input, outputs=top_model(vgg16.output))
model.summary()

## hdf5保存ファイルが有る場合は※１にいく

#### VGG16のblock5_conv1以降と追加した全結合分類器のみ訓練する
vgg16.trainable = True
set_trainable = False
for layer in vgg16.layers:
    if layer.name == 'block5_conv1':
        set_trainable = True
    if set_trainable:
        layer.trainable = True
    else:
        layer.trainable = False

for layer in vgg16.layers:
    print(layer, layer.trainable)

for layer in model.layers:
    print(layer, layer.trainable)

print("ok.")

#### 学習
from tensorflow.keras import optimizers

model.compile(loss='categorical_crossentropy', optimizer=optimizers.RMSprop(lr=1e-5), metrics=['acc'])
history = model.fit_generator(train_generator, steps_per_epoch=25, epochs=30, validation_data=validation_generator, validation_steps=10, verbose=1)

#### 学習結果を保存する
hdf5_file = os.path.join(base_dir, 'flower-model.h5')
model.save_weights(hdf5_file)

#### 読み込む場合はここからスタート　※１
#model.load_weights('D:/python/bunbe', 'flower-model.h5')

#### 学習推移をグラフに表示する
# % matplotlib inline   #Jupyter Notebookで、ノートブック上にグラフを描画する際に指定
import matplotlib.pyplot as plt

acc = history.history['acc']
val_acc = history.history['val_acc']

loss = history.history['loss']
val_loss = history.history['val_loss']

epochs = range(len(acc))

plt.plot(epochs, acc, 'bo', label='Training acc')
plt.plot(epochs, val_acc, 'b', label='Validation acc')
plt.title('Training and validation accuracy')
plt.legend()

plt.figure()

plt.plot(epochs, loss, 'bo', label='Training loss')
plt.plot(epochs, val_loss, 'b', label='Validation loss')
plt.title('Training and validation loss')
plt.legend()

plt.show()

#### テストの画像データで正解率を調べる

test_generator = test_datagen.flow_from_directory(directory=test_dir,target_size=(img_rows, img_cols),color_mode='rgb',classes=classes,class_mode='categorical',batch_size=batch_size_for_data_generator)
test_loss, test_acc = model.evaluate_generator(test_generator, steps=50)
print('test acc:', test_acc)

#### 実際にテスト画像を分離してみる
# filename で開いた画像を見てみる

import numpy as np
from tensorflow.keras.preprocessing.image import load_img, img_to_array
from tensorflow.keras.applications.vgg16 import preprocess_input

filename = os.path.join(test_dir, 'ScreenShot00000.png')
print(filename)

#### 
img = load_img(filename, target_size=(img_rows, img_cols))
x = img_to_array(img)
x = np.expand_dims(x, axis=0)

ic=0

#フォルダの画像全部チェックする

txt1 = os.path.join(base_dir, 'result.txt')
csv_file = open(txt1, "w", encoding = "utf_8")

import glob
glob_dir = os.path.join(test_dir, "*.png")
files = glob.glob(glob_dir)
for file in files:
    print(file)
    #...
    img = load_img(file, target_size=(img_rows, img_cols))
    x = img_to_array(img)
    x = np.expand_dims(x, axis=0)
    #...
    predict = model.predict(preprocess_input(x))
    for pre in predict:
        y = pre.argmax()
        #print("test result=",classes[y], pre)
        #val = '{:.1f}'.format(pre)
        csv_file.write(file + '\t' + str(pre[0])  + '\t' + str(pre[1]) + "\n")
        csv_file.flush()

csv_file.close()
print("OK.")

##############################################
##############################################
##############################################
import tensorflow as tf

from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Dense, Conv2D, Flatten, Dropout, MaxPooling2D
from tensorflow.keras.preprocessing.image import ImageDataGenerator

import os
import numpy as np
import matplotlib.pyplot as plt

#
train_cats_dir = os.path.join(train_dir, 'cool')  # 学習用の猫画像のディレクトリ
train_dogs_dir = os.path.join(train_dir, 'bad')  # 学習用の犬画像のディレクトリ
validation_cats_dir = os.path.join(validation_dir, 'cool')  # 検証用の猫画像のディレクトリ
validation_dogs_dir = os.path.join(validation_dir, 'bad')  # 検証用の犬画像のディレクトリ

num_cats_tr = len(os.listdir(train_cats_dir))
num_dogs_tr = len(os.listdir(train_dogs_dir))
num_cats_val = len(os.listdir(validation_cats_dir))
num_dogs_val = len(os.listdir(validation_dogs_dir))
total_train = num_cats_tr + num_dogs_tr
total_val = num_cats_val + num_dogs_val

print('total training cat images:', num_cats_tr)
print('total training dog images:', num_dogs_tr)
print('total validation cat images:', num_cats_val)
print('total validation dog images:', num_dogs_val)
print("Total training images:", total_train)
print("Total validation images:", total_val)

batch_size = 128
epochs = 15
IMG_HEIGHT = 150
IMG_WIDTH = 150

#データの準備
train_image_generator = ImageDataGenerator(rescale=1./255) # 学習データのジェネレータ
validation_image_generator = ImageDataGenerator(rescale=1./255) # 検証データのジェネレータ

train_data_gen = train_image_generator.flow_from_directory(batch_size=batch_size,
                                                           directory=train_dir,
                                                           shuffle=True,
                                                           target_size=(IMG_HEIGHT, IMG_WIDTH),
                                                           classes=classes,
                                                           class_mode='binary')
val_data_gen = validation_image_generator.flow_from_directory(batch_size=batch_size,
                                                              directory=validation_dir,
                                                              target_size=(IMG_HEIGHT, IMG_WIDTH),
                                                              classes=classes,
                                                              class_mode='binary')

#学習用画像の可視化
sample_training_images, _ = next(train_data_gen)

# この関数は、1行5列のグリッド形式で画像をプロットし、画像は各列に配置されます。
def plotImages(images_arr):
    fig, axes = plt.subplots(1, 5, figsize=(20,20))
    axes = axes.flatten()
    for img, ax in zip( images_arr, axes):
        ax.imshow(img)
        ax.axis('off')
    plt.tight_layout()
    plt.show()

#plotImages(sample_training_images[:5])

#モデルの構築
model = Sequential([
    Conv2D(16, 3, padding='same', activation='relu', input_shape=(IMG_HEIGHT, IMG_WIDTH ,3)),
    MaxPooling2D(),
    Conv2D(32, 3, padding='same', activation='relu'),
    MaxPooling2D(),
    Conv2D(64, 3, padding='same', activation='relu'),
    MaxPooling2D(),
    Flatten(),
    Dense(512, activation='relu'),
    Dense(1, activation='sigmoid')
])

#モデルのコンパイル
model.compile(optimizer='adam',
              loss='binary_crossentropy',
              metrics=['accuracy'])
model.summary()

#モデルの学習
history = model.fit_generator(
    train_data_gen,
    steps_per_epoch=total_train // batch_size,
    epochs=epochs,
    validation_data=val_data_gen,
    validation_steps=total_val // batch_size
)

#学習結果の可視化
acc = history.history['acc']
val_acc = history.history['val_acc']

loss = history.history['loss']
val_loss = history.history['val_loss']

epochs_range = range(epochs)

plt.figure(figsize=(8, 8))
plt.subplot(1, 2, 1)
plt.plot(epochs_range, acc, label='Training Accuracy')
plt.plot(epochs_range, val_acc, label='Validation Accuracy')
plt.legend(loc='lower right')
plt.title('Training and Validation Accuracy')

plt.subplot(1, 2, 2)
plt.plot(epochs_range, loss, label='Training Loss')
plt.plot(epochs_range, val_loss, label='Validation Loss')
plt.legend(loc='upper right')
plt.title('Training and Validation Loss')
plt.show()


#### データの拡張と可視化 ####
batch_size = 128
IMG_HEIGHT, IMG_WIDTH = 150, 150

#水平反転の適用
image_gen = ImageDataGenerator(rescale=1./255, horizontal_flip=True)

train_data_gen = image_gen.flow_from_directory(batch_size=batch_size,
                                               directory=train_dir,
                                               shuffle=True,
                                               classes=classes,
                                               target_size=(IMG_HEIGHT, IMG_WIDTH))

augmented_images = [train_data_gen[0][0][0] for i in range(5)]

# 上で学習用画像の可視化のために定義、使用されたおなじカスタムプロット関数を再利用する
#plotImages(augmented_images)

## 画像のランダムな回転
image_gen = ImageDataGenerator(rescale=1./255, rotation_range=45)

train_data_gen = image_gen.flow_from_directory(batch_size=batch_size,
                                               directory=train_dir,
                                               shuffle=True,
                                               classes=classes,
                                               target_size=(IMG_HEIGHT, IMG_WIDTH))

augmented_images = [train_data_gen[0][0][0] for i in range(5)]

##ズームによるデータ拡張の適用
image_gen = ImageDataGenerator(rescale=1./255, zoom_range=0.5)

train_data_gen = image_gen.flow_from_directory(batch_size=batch_size,
                                               directory=train_dir,
                                               shuffle=True,
                                               classes=classes,
                                               target_size=(IMG_HEIGHT, IMG_WIDTH))

augmented_images = [train_data_gen[0][0][0] for i in range(5)]

##すべてのデータ拡張を同時に利用する

image_gen_train = ImageDataGenerator(
                    rescale=1./255,
                    rotation_range=45,
                    width_shift_range=.15,
                    height_shift_range=.15,
                    horizontal_flip=True,
                    zoom_range=0.5
                    )

train_data_gen = image_gen_train.flow_from_directory(batch_size=batch_size,
                                                     directory=train_dir,
                                                     shuffle=True,
                                                     classes=classes,
                                                     target_size=(IMG_HEIGHT, IMG_WIDTH),
                                                     class_mode='binary')

augmented_images = [train_data_gen[0][0][0] for i in range(5)]
#plotImages(augmented_images)

#検証データジェネレータの構築
image_gen_val = ImageDataGenerator(rescale=1./255)

val_data_gen = image_gen_val.flow_from_directory(batch_size=batch_size,
                                                 directory=validation_dir,
                                                 target_size=(IMG_HEIGHT, IMG_WIDTH),
                                                 classes=classes,
                                                 class_mode='binary')

##ドロップアウト（dropout）
#ドロップアウトを追加した新しいネットワークの構築
model_new = Sequential([
    Conv2D(16, 3, padding='same', activation='relu', 
           input_shape=(IMG_HEIGHT, IMG_WIDTH ,3)),
    MaxPooling2D(),
    Dropout(0.2),
    Conv2D(32, 3, padding='same', activation='relu'),
    MaxPooling2D(),
    Conv2D(64, 3, padding='same', activation='relu'),
    MaxPooling2D(),
    Dropout(0.2),
    Flatten(),
    Dense(512, activation='relu'),
    #Dense(1, activation='sigmoid')
    Dense(nb_classes, activation='sigmoid')
])

#モデルのコンパイル
model_new.compile(optimizer='adam',
              loss='binary_crossentropy',
              metrics=['accuracy'])
model_new.summary()

#モデルの学習
history = model_new.fit_generator(
    train_data_gen,
    steps_per_epoch=total_train // batch_size,
    epochs=epochs,
    validation_data=val_data_gen,
    validation_steps=total_val // batch_size
)

#モデルの可視化
acc = history.history['acc']
val_acc = history.history['val_acc']

loss = history.history['loss']
val_loss = history.history['val_loss']

epochs_range = range(epochs)

plt.figure(figsize=(8, 8))
plt.subplot(1, 2, 1)
plt.plot(epochs_range, acc, label='Training Accuracy')
plt.plot(epochs_range, val_acc, label='Validation Accuracy')
plt.legend(loc='lower right')
plt.title('Training and Validation Accuracy')

plt.subplot(1, 2, 2)
plt.plot(epochs_range, loss, label='Training Loss')
plt.plot(epochs_range, val_loss, label='Validation Loss')
plt.legend(loc='upper right')
plt.title('Training and Validation Loss')
plt.show()


#### 実際にテスト画像を分離してみる
# filename で開いた画像を見てみる
import numpy as np
from tensorflow.keras.preprocessing.image import load_img, img_to_array
from tensorflow.keras.applications.vgg16 import preprocess_input

filename = os.path.join(test_dir, 'ScreenShot00000.png')
print(filename)

#### 
img = load_img(filename, target_size=(IMG_HEIGHT, IMG_WIDTH))
x = img_to_array(img)
x = np.expand_dims(x, axis=0)
ic=0

#フォルダの画像全部チェックする

txt1 = os.path.join(base_dir, 'result.txt')
csv_file = open(txt1, "w", encoding = "utf_8")

import glob
glob_dir = os.path.join(test_dir, "*.png")
files = glob.glob(glob_dir)
for file in files:
    print(file)
    #...
    img = load_img(file, target_size=(IMG_HEIGHT, IMG_WIDTH))
    x = img_to_array(img)
    x = np.expand_dims(x, axis=0)
    #...
    predict = model_new.predict(preprocess_input(x))
    for pre in predict:
        y = pre.argmax()
        csv_file.write(file + '\t' + str(pre[0])  + '\t' + str(pre[1]) + "\n")
        csv_file.flush()

csv_file.close()
print("OK.")

    Dropout(0.2),
    Flatten(),
    Dense(512, activation='relu'),
    #Dense(1, activation='sigmoid')
    Dense(nb_classes, activation='relu'))

])

#モデルのコンパイル
model_new.compile(optimizer='adam',
              loss='binary_crossentropy',
              metrics=['accuracy'])
model_new.summary()

#モデルの学習
history = model_new.fit_generator(
    train_data_gen,
    steps_per_epoch=total_train // batch_size,
    epochs=epochs,
    validation_data=val_data_gen,
    validation_steps=total_val // batch_size
)

#モデルの可視化
acc = history.history['acc']
val_acc = history.history['val_acc']

loss = history.history['loss']
val_loss = history.history['val_loss']

epochs_range = range(epochs)

plt.figure(figsize=(8, 8))
plt.subplot(1, 2, 1)
plt.plot(epochs_range, acc, label='Training Accuracy')
plt.plot(epochs_range, val_acc, label='Validation Accuracy')
plt.legend(loc='lower right')
plt.title('Training and Validation Accuracy')

plt.subplot(1, 2, 2)
plt.plot(epochs_range, loss, label='Training Loss')
plt.plot(epochs_range, val_loss, label='Validation Loss')
plt.legend(loc='upper right')
plt.title('Training and Validation Loss')
plt.show()


#### 実際にテスト画像を分離してみる
# filename で開いた画像を見てみる
import numpy as np
from tensorflow.keras.preprocessing.image import load_img, img_to_array
from tensorflow.keras.applications.vgg16 import preprocess_input

filename = os.path.join(test_dir, 'ScreenShot00000.png')
print(filename)

#### 
img = load_img(filename, target_size=(IMG_HEIGHT, IMG_WIDTH))
x = img_to_array(img)
x = np.expand_dims(x, axis=0)
ic=0

#フォルダの画像全部チェックする

txt1 = os.path.join(base_dir, 'result.txt')
csv_file = open(txt1, "w", encoding = "utf_8")

import glob
glob_dir = os.path.join(test_dir, "*.png")
files = glob.glob(glob_dir)
for file in files:
    print(file)
    #...
    img = load_img(file, target_size=(IMG_HEIGHT, IMG_WIDTH))
    x = img_to_array(img)
    x = np.expand_dims(x, axis=0)
    #...
    predict = model_new.predict(preprocess_input(x))
    for pre in predict:
        y = pre.argmax()
        csv_file.write(file + '\t' + str(pre[0])  + '\t' + str(pre[1]) + "\n")
        csv_file.flush()

csv_file.close()
print("OK.")
