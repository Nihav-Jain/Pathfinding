#pragma once
#include "Graph.h"
#include "VertexDataCLI.h"
#include "EdgeDataCLI.h"

namespace Pathfinding
{
	public ref class GraphCLI
	{
	public:
		GraphCLI();
		~GraphCLI();

		std::uint32_t AddVertex(VertexDataCLI^ data);
		std::uint32_t AddVertex(std::uint32_t parentVertexId, VertexDataCLI^ data, EdgeDataCLI^ edgeData);
		std::uint32_t AddParentVertex(std::uint32_t childVertexId, VertexDataCLI^ data, EdgeDataCLI^ edgeData);

		void CreateEdge(std::uint32_t headVertexId, std::uint32_t tailVertexId, EdgeDataCLI^ data);
		bool IsEmpty();
		void Clear();

		Graph* mGraph;
	};
}

