#pragma once

class node
{
public:
	int x, y, size, used, avail;
	node(int _x, int _y, int _size, int _used, int _avail) : x(_x), y(_y), size(_size), used(_used), avail(_avail) {}
};