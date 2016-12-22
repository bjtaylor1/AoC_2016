// Day22.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;

int main()
{
	regex   r("^\/dev\/grid\/node\-x(\\d+)\-y(\\d+)" 
		"\\s*(\\d+)T"
		"\\s*(\\d+)T"
		"\\s*(\\d+)T");
	string s("/dev/grid/node-x0-y0     94T   73T    21T   77%");
	smatch m;
	if (regex_search(s, m, r))
	{
		cout << "It's ok!" << endl;
		int x = atoi(m[1].str().c_str());
		int y = atoi(m[2].str().c_str());
		int size = atoi(m[3].str().c_str());
		int used = atoi(m[4].str().c_str());
		int avail = atoi(m[5].str().c_str());
		cout << "x = " << x << "; y = " << y << "; size = " << size << "; used = " << used << "; avail = " << avail << endl;
	}

    return 0;
}

