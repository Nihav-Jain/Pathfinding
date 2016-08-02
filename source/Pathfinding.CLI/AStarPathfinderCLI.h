#pragma once
#include "IPathfinderCLI.h"
#include "AStarPathfinder.h"

namespace Pathfinding
{
	public ref class AStarPathfinderCLI : public IPathfinderCLI
	{
	public:
		AStarPathfinderCLI();
		~AStarPathfinderCLI();

		virtual System::Collections::Generic::List<std::uint32_t>^ FindPath(GraphCLI^ graph, std::uint32_t sourceId, std::uint32_t destinationId, System::Collections::Generic::List<std::uint32_t>^ visited) override;

	private:
		AStarPathfinder* mPathFinder;
	};
}

