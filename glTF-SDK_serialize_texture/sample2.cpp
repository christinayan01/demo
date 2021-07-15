// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#define NOMINMAX
#include <windows.h>
#undef min
#undef max

// Replace this with <filesystem> (and use std::filesystem rather than
// std::experimental::filesystem) if your toolchain fully supports C++17
#include <experimental/filesystem>

#include "Sample2.h"

// Usings for glTF
using namespace Microsoft::glTF;

//namespace test1 {
    void Sample2::CreateTriangleResources1(Document& document, BufferBuilder& bufferBuilder,
                                            std::string& accessorIdIndices, std::string& accessorIdPositions,
                                            std::string& accessorIdTexCoords) // yanai add
    {
        // Create all the resource data (e.g. triangle indices and
        // vertex positions) that will be written to the binary buffer
        const char* bufferId = nullptr;

        // Specify the 'special' GLB buffer ID. This informs the GLBResourceWriter that it should use
        // the GLB container's binary chunk (usually the desired buffer location when creating GLBs)
        if (dynamic_cast<const GLBResourceWriter*>(&bufferBuilder.GetResourceWriter()))
        {
            bufferId = GLB_BUFFER_ID;
        }

        // Create a Buffer - it will be the 'current' Buffer that all the BufferViews
        // created by this BufferBuilder will automatically reference
        bufferBuilder.AddBuffer(bufferId);

        // Create a BufferView with a target of ELEMENT_ARRAY_BUFFER (as it will reference index
        // data) - it will be the 'current' BufferView that all the Accessors created by this
        // BufferBuilder will automatically reference
        bufferBuilder.AddBufferView(BufferViewTarget::ELEMENT_ARRAY_BUFFER);

        // Add an Accessor for texCoords -->
        std::vector<float> texcoords = {
            0.0f, 0.0f,
            2.0f, 0.0f,
            0.0f, 2.0f,
            2.0f, 0.0f
        };
        accessorIdTexCoords = bufferBuilder.AddAccessor(texcoords, { TYPE_VEC2, COMPONENT_FLOAT }).id;
        // texCoords end <--
        
        // Add an Accessor for the indices
        std::vector<uint16_t> indices = {
            0, 1, 2,
            0, 2, 3, // yanai
            0, 1, 3,
            1, 2, 3
        };

        // Copy the Accessor's id - subsequent calls to AddAccessor may invalidate the returned reference
        accessorIdIndices = bufferBuilder.AddAccessor(indices, { TYPE_SCALAR, COMPONENT_UNSIGNED_SHORT }).id;

        // Create a BufferView with target ARRAY_BUFFER (as it will reference vertex attribute data)
        bufferBuilder.AddBufferView(BufferViewTarget::ARRAY_BUFFER);

        // Add an Accessor for the positions
        std::vector<float> positions = {
            0.0f, 0.0f, 0.0f, // Vertex 0
            1.0f, 0.0f, 0.0f, // Vertex 1
            0.0f, 1.0f, 0.0f, // Vertex 2
            0.0f, 0.0f, 1.0f  // Vertex 3 yanai
        };

        std::vector<float> minValues(3U, std::numeric_limits<float>::max());
        std::vector<float> maxValues(3U, std::numeric_limits<float>::lowest());

        const size_t positionCount = positions.size();

        // Accessor min/max properties must be set for vertex position data so calculate them here
        for (size_t i = 0U, j = 0U; i < positionCount; ++i, j = (i % 3U))
        {
            minValues[j] = std::min(positions[i], minValues[j]);
            maxValues[j] = std::max(positions[i], maxValues[j]);
        }

        accessorIdPositions = bufferBuilder.AddAccessor(positions,
            { TYPE_VEC3, COMPONENT_FLOAT, false, minValues, maxValues }).id;

        // Add all of the Buffers, BufferViews and Accessors that were created using BufferBuilder to
        // the Document. Note that after this point, no further calls should be made to BufferBuilder
        bufferBuilder.Output(document);
    }

    void Sample2::CreateTriangleEntities1(Document& document,
                                        const std::string& accessorIdIndices, 
                                        const std::string& accessorIdPositions,
                                        const std::string& accessorIdTexCoords) // yanai add
    {
        // Create a very simple glTF Document with the following hierarchy:
        //  Scene
        //     Node
        //       Mesh (Triangle)
        //         MeshPrimitive
        //           Material (Blue)
        // 
        // A Document can be constructed top-down or bottom up. However, if constructed top-down
        // then the IDs of child entities must be known in advance, which prevents using the glTF
        // SDK's automatic ID generation functionality.

        // Construct a image //
        Microsoft::glTF::Image image;
        image.id = "0";
        image.uri = "BoomBox_baseColor.png";
        auto imageId = document.images.Append(image, AppendIdPolicy::GenerateOnEmpty).id;

        // Construct a texture //
        Texture texture;
        texture.imageId = imageId;
        texture.id = "0";
        auto textureId = document.textures.Append(texture, AppendIdPolicy::GenerateOnEmpty).id;        

        // Attach texture id
        // Construct a Material
        Material material;
        material.metallicRoughness.baseColorFactor = Color4(1.0f, 1.0f, 1.0f, 1.0f);//yanai
        material.metallicRoughness.metallicFactor = 0.2f;
        material.metallicRoughness.roughnessFactor = 0.4f;
        material.doubleSided = true;
        material.metallicRoughness.baseColorTexture.textureId = textureId;

        // Add it to the Document and store the generated ID
        auto materialId = document.materials.Append(material, AppendIdPolicy::GenerateOnEmpty).id;

        // Construct a MeshPrimitive. Unlike most types in glTF, MeshPrimitives are direct children
        // of their parent Mesh entity rather than being children of the Document. This is why they
        // don't have an ID member.
        MeshPrimitive meshPrimitive;
        meshPrimitive.materialId = materialId;
        meshPrimitive.indicesAccessorId = accessorIdIndices;
        meshPrimitive.attributes[ACCESSOR_POSITION] = accessorIdPositions;
        meshPrimitive.attributes[ACCESSOR_TEXCOORD_0] = accessorIdTexCoords; // yanai add

        // Construct a Mesh and add the MeshPrimitive as a child
        Mesh mesh;
        mesh.primitives.push_back(meshPrimitive);
        // Add it to the Document and store the generated ID
        auto meshId = document.meshes.Append(mesh, AppendIdPolicy::GenerateOnEmpty).id;

        // Construct a Node adding a reference to the Mesh
        Node node;
        node.meshId = meshId;
        // Add it to the Document and store the generated ID
        auto nodeId = document.nodes.Append(node, AppendIdPolicy::GenerateOnEmpty).id;

        // Construct a Scene
        Scene scene;
        scene.nodes.push_back(nodeId);
        // Add it to the Document, using a utility method that also sets the Scene as the Document's default
        document.SetDefaultScene(std::move(scene), AppendIdPolicy::GenerateOnEmpty);
    }
//}
