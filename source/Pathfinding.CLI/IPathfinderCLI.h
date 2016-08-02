#pragma once
#include <vector>
#include "IPathfinder.h"
#include "GraphCLI.h"

namespace Pathfinding
{
	public ref class IPathfinderCLI abstract
	{
	public:
		IPathfinderCLI();
		//virtual std::vector<std::uint32_t> FindPath(GraphCLI^ graph, std::uint32_t sourceId, std::uint32_t destinationId, std::vector<std::uint32_t>& visited) = 0;
		virtual System::Collections::Generic::List<std::uint32_t>^ FindPath(GraphCLI^ graph, std::uint32_t sourceId, std::uint32_t destinationId, System::Collections::Generic::List<std::uint32_t>^ visited) = 0;
	};
}

