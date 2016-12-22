#pragma once

class node
{
public:
	int x, y, size, used, avail, line;
	bool is_goal;
	node() {}
	node(int _x, int _y, int _size, int _used, int _avail, int _line) : 
		x(_x), y(_y), size(_size), used(_used), avail(_avail), line(_line), is_goal(false) {}
};