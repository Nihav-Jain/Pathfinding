#pragma once
#include "IPathfinderCLI.h"
#include "GreedyBestFirstPathfinder.h"
#include "VertexDataCLI.h"

namespace Pathfinding
{
	public ref class GreedyBestFirstPathfinderCLI : public IPathfinderCLI
	{
	public:
		GreedyBestFirstPathfinderCLI();
		~GreedyBestFirstPathfinderCLI();

		virtual System::Collections::Generic::List<std::uint32_t>^ FindPath(GraphCLI^ graph, std::uint32_t sourceId, std::uint32_t destinationId, System::Collections::Generic::List<std::uint32_t>^ visited) override;
	private:
		GreedyBestFirstPathfinder* mPathFinder;
	};
}

