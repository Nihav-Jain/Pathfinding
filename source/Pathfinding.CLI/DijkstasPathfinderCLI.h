#pragma once
#include "IPathfinderCLI.h"
#include "DijkstrasPathfinder.h"

namespace Pathfinding
{
	public ref class DijkstrasPathfinderCLI : IPathfinderCLI
	{
	public:
		DijkstrasPathfinderCLI();
		~DijkstrasPathfinderCLI();

		virtual System::Collections::Generic::List<std::uint32_t>^ FindPath(GraphCLI^ graph, std::uint32_t sourceId, std::uint32_t destinationId, System::Collections::Generic::List<std::uint32_t>^ visited) override;

	private:
		DijkstrasPathfinder* mPathFinder;
	};
}

