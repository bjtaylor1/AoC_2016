#pragma once
#include "node.h"

class puzzle_input
{
private:
	static bool add_node(vector<node>& nodes, const regex& r, const string& line, int lineNo);
public:
	static bool add_nodes(vector<node>& nodes);
};