#include "stdafx.h"
#include "pos.h"

long pos::xy(int x, int y)
{
	return ((long)x << 16) | ((long)y);
}

int pos::x(long xy)
{
	return (int)(xy >> 16);
}

int pos::y(long xy)
{
	return (xy & ((int)0x7fff));

}