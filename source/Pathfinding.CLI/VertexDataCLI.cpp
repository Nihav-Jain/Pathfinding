#include "stdafx.h"
#include "VertexDataCLI.h"

namespace Pathfinding
{
	VertexDataCLI::VertexDataCLI() :
		mVertexData(new VertexData())
	{}

	VertexDataCLI::VertexDataCLI(VertexData* vertexData) :
		mVertexData(vertexData)
	{}

	VertexDataCLI::~VertexDataCLI()
	{
		delete mVertexData;
	}

	std::int32_t VertexDataCLI::PosX()
	{
		return mVertexData->PosX;
	}

	void VertexDataCLI::PosX(std::int32_t value)
	{
		mVertexData->PosX = value;
	}

	std::int32_t VertexDataCLI::PosY()
	{
		return mVertexData->PosY;
	}

	void VertexDataCLI::PosY(std::int32_t value)
	{
		mVertexData->PosY = value;
	}

}
