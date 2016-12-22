// Day22.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;
#include "puzzle_input.h";
#include "pos.h";

bool is_viable(map<long,node>::const_iterator it_A, map<long,node>::const_iterator it_B)
{
	bool isViable = it_A != it_B && it_A->second.used > 0 && it_A->second.used <= it_B->second.avail;
	return isViable;
}

int main()
{
	map<long,node> nodes;
	if (puzzle_input::add_nodes(nodes))
	{
		cout << "there are " << nodes.size() << " nodes." << endl;

	}

	//part 1:
	int viablePairs = 0;
	for (map<long,node>::const_iterator it_A = nodes.begin(); it_A != nodes.end(); it_A++)
	{
		for (map<long,node>::const_iterator it_B = nodes.begin(); it_B != nodes.end(); it_B++)
		{
			if (is_viable(it_A, it_B))
			{
				cout << it_A->second.line << " => " << it_B->second.line << endl;
				viablePairs++;
			}
		}
	}
	cout << "Viable pairs: " << viablePairs << endl;

	//part 2:
	int maxX = 0;
	map<long,node>::const_iterator itMaxX = nodes.end();
	for (map<long,node>::const_iterator it = nodes.begin(); it != nodes.end(); it++)
	{
		if (it->second.x > maxX && it->second.y == 0)
		{
			maxX = it->second.x;
			itMaxX = it;
		}
	}

	

	//itMaxX->is_goal = true;

    return 0;
}

