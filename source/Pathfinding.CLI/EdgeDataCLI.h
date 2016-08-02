#pragma once
#include "EdgeData.h"

namespace Pathfinding
{
	public ref class EdgeDataCLI
	{
	public:
		EdgeDataCLI(std::uint32_t edgeWeight);
		~EdgeDataCLI();

		std::int32_t EdgeWeight();

		EdgeData* mEdgeData;
	};
}

