// Day15.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

/* // example input:
int positions[] = { 0, 4, 1 };
int positionCount[]{ 0, 5, 2 };
*/

int positions[] =  { 0, 0, 0,  2, 2,  0,  7    /* part 2:*/, 0 };
int positionCount[]{ 0, 7, 13, 3, 5, 17, 19    /* part 2: */,11 };


const bool allAligned(const int t)
{
	for (int t0 = 1; t0 < sizeof(positions) / sizeof(int); t0++)
	{
		const int posAtt0 = (positions[t0] + t + t0) % positionCount[t0];
		if (posAtt0 != 0) return false;
	}
	return true;
}

const int getFirstTimeAllAligned()
{
	for (int t = 0; t < 0x7fffffff; t++)
	{
		if (allAligned(t))
			return t;
	}
	return -1;
}

int main()
{
	std::cout << getFirstTimeAllAligned() << std::endl;
    return 0;
}



