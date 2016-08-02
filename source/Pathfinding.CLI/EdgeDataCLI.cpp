#include "stdafx.h"
#include "EdgeDataCLI.h"

namespace Pathfinding
{
	EdgeDataCLI::EdgeDataCLI(std::uint32_t edgeWeight) :
		mEdgeData(new EdgeData(edgeWeight))
	{}

	EdgeDataCLI::~EdgeDataCLI()
	{
		delete mEdgeData;
	}

	std::int32_t EdgeDataCLI::EdgeWeight()
	{
		return mEdgeData->EdgeWeight();
	}
}
