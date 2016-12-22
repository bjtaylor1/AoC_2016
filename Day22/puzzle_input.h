#pragma once
#include "node.h"

class puzzle_input
{
private:
	static bool add_node(map<long,node>& nodes, const regex& r, const string& line, int lineNo);
public:
	static bool add_nodes(map<long,node>& nodes);
};