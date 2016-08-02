#pragma once
#include "IPathfinderCLI.h"
#include "BreadthFirstPathfinder.h"

namespace Pathfinding
{
	public ref class BreadthFirstPathfinderCLI : public IPathfinderCLI
	{
	public:
		BreadthFirstPathfinderCLI();
		~BreadthFirstPathfinderCLI();

		virtual System::Collections::Generic::List<std::uint32_t>^ FindPath(GraphCLI^ graph, std::uint32_t sourceId, std::uint32_t destinationId, System::Collections::Generic::List<std::uint32_t>^ visited) override;

	private:
		BreadthFirstPathfinder* mPathFinder;
	};
}

