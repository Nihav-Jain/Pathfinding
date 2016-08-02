#include "stdafx.h"
#include "IPathfinderCLI.h"
#include "DijkstasPathfinderCLI.h"

using namespace System::Collections::Generic;

namespace Pathfinding
{
	DijkstrasPathfinderCLI::DijkstrasPathfinderCLI() :
		mPathFinder(new DijkstrasPathfinder())
	{}

	DijkstrasPathfinderCLI::~DijkstrasPathfinderCLI()
	{
		delete mPathFinder;
	}

	List<std::uint32_t>^ DijkstrasPathfinderCLI::FindPath(GraphCLI^ graph, std::uint32_t sourceId, std::uint32_t destinationId, List<std::uint32_t>^ visited)
	{
		List<std::uint32_t>^ pathList = gcnew List<std::uint32_t>();
		std::vector<std::uint32_t> visitedNodes;
		std::vector<std::uint32_t> path = mPathFinder->FindPath(*(graph->mGraph), sourceId, destinationId, visitedNodes);

		for (std::uint32_t node : path)
		{
			pathList->Add(node);
		}
		visited->Clear();
		for (std::uint32_t visitedNode : visitedNodes)
		{
			visited->Add(visitedNode);
		}

		return pathList;
	}
}
