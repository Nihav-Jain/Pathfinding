#pragma once
#include "VertexData.h"

namespace Pathfinding
{
	public ref class VertexDataCLI
	{
	public:
		VertexDataCLI();
		VertexDataCLI(VertexData* vertexData);
		~VertexDataCLI();

		std::int32_t PosX();
		void PosX(std::int32_t value);

		std::int32_t PosY();
		void PosY(std::int32_t value);

		VertexData* mVertexData;
	};
}

