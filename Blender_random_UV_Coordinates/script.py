import bpy
import copy
from random import random

mesh = bpy.context.object.data

bpy.ops.object.mode_set(mode = 'EDIT') 
bpy.ops.mesh.select_mode(type="VERT")
bpy.ops.mesh.select_all(action = 'SELECT')

uvtex = mesh.uv_textures.new('UVRandom') #UVマップ生成
mesh.uv_textures.active = mesh.uv_textures[len(mesh.uv_textures) - 1] #選択
bpy.ops.uv.smart_project() #スマートUV

location_list = []
uv_list = []

uv_list.clear()
location_list.clear()

obj = bpy.context.active_object
for v in obj.data.vertices:
  vec = copy.copy(v.co)
  vec.x = random()
  vec.y = random()
  vec.z = random()
  location_list.append(vec)

uv_list = [p[:2] for p in location_list]

import bmesh
mesh = obj.data
bm = bmesh.from_edit_mesh(mesh)
uv_layer = bm.loops.layers.uv.active
for bm_face in bm.faces:
 id = bm_face.loops[0].vert.index
 print(id)
 for v in bm_face.loops:
  v[uv_layer].uv[0] = v[uv_layer].uv[0] + uv_list[id][0]
  v[uv_layer].uv[1] = v[uv_layer].uv[1] + uv_list[id][1]

bmesh.update_edit_mesh(mesh)
bpy.ops.object.editmode_toggle()
