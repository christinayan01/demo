#pragma once

#include <GLTFSDK/GLTF.h>
#include <GLTFSDK/BufferBuilder.h>
#include <GLTFSDK/GLTFResourceWriter.h>
#include <GLTFSDK/GLBResourceWriter.h>
#include <GLTFSDK/IStreamWriter.h>
#include <GLTFSDK/Serialize.h>
//#include <GLTFSDK/Document.h>//yanaiadd

// Replace this with <filesystem> (and use std::filesystem rather than
// std::experimental::filesystem) if your toolchain fully supports C++17
#include <experimental/filesystem>

#include <fstream>
#include <sstream>
#include <iostream>

#include <cassert>
#include <cstdlib>

using namespace Microsoft::glTF;

//namespace test1 {

	class Sample2
	{
	public:
		static void CreateTriangleResources1(Document& document, BufferBuilder& bufferBuilder, 
			std::string& accessorIdIndices, 
			std::string& accessorIdPositions,
			std::string& accessorIdTexCoords); // yanai add
		static void CreateTriangleEntities1(Document& document, 
			const std::string& accessorIdIndices, 
			const std::string& accessorIdPositions,
			const std::string& accessorIdTexCoords); // yanai add

	};
//}
