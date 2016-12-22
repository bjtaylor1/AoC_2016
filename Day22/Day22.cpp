// Day22.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;
#include "puzzle_input.h";
#include "pos.h";

bool is_viable(vector<node>::const_iterator it_A, vector<node>::const_iterator it_B)
{
	bool isViable = it_A != it_B && it_A->used > 0 && it_A->used <= it_B->avail;
	return isViable;
}

void testxy(int x, int y)
{
	long xy = pos::xy(x, y);
	cout << "x = " << x << ", y = " << y << endl;
	cout << "xy = " << xy << endl;
	cout << "x = " << pos::x(xy) << ", y = " << pos::y(xy) << endl << endl;
}

int main()
{
	testxy(3, 4);
	testxy(7, 11);
	testxy(715, 42);

	vector<node> nodes;
	if (puzzle_input::add_nodes(nodes))
	{
		cout << "there are " << nodes.size() << " nodes." << endl;

	}

	//part 1:
	int viablePairs = 0;
	for (vector<node>::const_iterator it_A = nodes.begin(); it_A != nodes.end(); it_A++)
	{
		for (vector<node>::const_iterator it_B = nodes.begin(); it_B != nodes.end(); it_B++)
		{
			if (is_viable(it_A, it_B))
			{
				cout << it_A->line << " => " << it_B->line << endl;
				viablePairs++;
			}
		}
	}
	cout << "Viable pairs: " << viablePairs << endl;

	//part 2:
	int maxX = 0;
	vector<node>::const_iterator itMaxX = nodes.end();
	for (vector<node>::const_iterator it = nodes.begin(); it != nodes.end(); it++)
	{
		if (it->x > maxX && it->y == 0)
		{
			maxX = it->x;
			itMaxX = it;
		}
	}

	

	//itMaxX->is_goal = true;

    return 0;
}

