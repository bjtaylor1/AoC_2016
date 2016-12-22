#pragma once

class node
{
public:
	static int MAXX, MAXY;
	int x, y, size, used, avail, line;
	node() {}
	node(int _x, int _y, int _size, int _used, int _avail, int _line) : 
		x(_x), y(_y), size(_size), used(_used), avail(_avail), line(_line){}
	void move_to(node& target);
};