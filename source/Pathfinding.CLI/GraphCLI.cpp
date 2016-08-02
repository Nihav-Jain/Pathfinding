#include "stdafx.h"
#include "GraphCLI.h"

namespace Pathfinding
{
	GraphCLI::GraphCLI() :
		mGraph(new Graph())
	{}

	GraphCLI::~GraphCLI()
	{
		delete mGraph;
	}

	std::uint32_t GraphCLI::AddVertex(VertexDataCLI^ data)
	{
		return mGraph->AddVertex(data->mVertexData);
	}

	std::uint32_t GraphCLI::AddVertex(std::uint32_t parentVertexId, VertexDataCLI^ data, EdgeDataCLI^ edgeData)
	{
		return mGraph->AddVertex(parentVertexId, data->mVertexData, edgeData->mEdgeData);
	}

	std::uint32_t GraphCLI::AddParentVertex(std::uint32_t childVertexId, VertexDataCLI^ data, EdgeDataCLI^ edgeData)
	{
		return mGraph->AddParentVertex(childVertexId, data->mVertexData, edgeData->mEdgeData);
	}

	void GraphCLI::CreateEdge(std::uint32_t headVertexId, std::uint32_t tailVertexId, EdgeDataCLI^ data)
	{
		mGraph->CreateEdge(headVertexId, tailVertexId, data->mEdgeData);
	}

	bool GraphCLI::IsEmpty()
	{
		return mGraph->IsEmpty();
	}

	void GraphCLI::Clear()
	{
		mGraph->Clear();
	}
}
