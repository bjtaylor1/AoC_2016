// Day22.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;
#include "puzzle_input.h";
#include "pos.h";
#include "../puzzle_iterator.h"

bool is_viable(map<long,node>::const_iterator it_A, map<long,node>::const_iterator it_B)
{
	bool isViable = it_A != it_B && it_A->second.used > 0 && it_A->second.used <= it_B->second.avail;
	return isViable;
}

bool compare_x(const pair<long, node>& lhs, const pair<long,node>& rhs)
{
	return lhs.second.x < rhs.second.x;
}
bool compare_y(const pair<long, node>& lhs, const pair<long,node>& rhs)
{
	return lhs.second.y < rhs.second.y;
}

void node::move_to(node& target)
{
	if (target.avail < used) throw exception("Not enough space!");
	target.avail -= used;
	target.used += used;
	used = 0;
}

int xmovements[4];
int ymovements[4];

class iteration
{
public:
	map<long, node> nodes;
	long depth;
	long goalX, goalY;
	bool is_target;
	long score;
	iteration(const map<long, node>& _nodes, long _depth, long _goalX, long _goalY) :
		nodes(_nodes), depth(_depth), goalX(_goalX), goalY(_goalY), is_target(_goalX == 0 && _goalY == 0)
	{
		bool goalCanMoveLeft = _goalX > 0 && _nodes.at(pos::xy(_goalX - 1, _goalY)).avail < _nodes.at(pos::xy(_goalX, _goalY)).used;
		bool goalCanMoveUp = _goalY > 0 && _nodes.at(pos::xy(_goalX, _goalY - 1)).avail < _nodes.at(pos::xy(_goalX, _goalY)).used;
		bool goalCanMoveRight = goalX < node::MAXX && _nodes.at(pos::xy(_goalX + 1, _goalY)).avail < _nodes.at(pos::xy(_goalX, _goalY)).used;
		bool goalCanMoveDown = goalY < node::MAXY && _nodes.at(pos::xy(_goalX, _goalY + 1)).avail < _nodes.at(pos::xy(_goalX, _goalY)).used;
		score = 0;
		if (goalCanMoveLeft) score += 9;
		if (goalCanMoveUp) score += 9;
		if (goalCanMoveRight) score += 1;
		if (goalCanMoveDown) score += 1;
	}

	vector<iteration> expand()
	{
		vector<iteration> e;
		for (int x = 0; x <= node::MAXX; x++)
		{
			for (int y = 0; y <= node::MAXY; y++)
			{
				for (int move = 0; move < 4; move++)
				{
					int newx = x + xmovements[move];
					int newy = y + ymovements[move];
					if (newx >= 0 && newy >= 0 && newx <= node::MAXX && newy <= node::MAXY)
					{
						if (nodes[pos::xy(newx, newy)].avail >= nodes[pos::xy(x, y)].used)
						{
							int newGoalX, newGoalY;
							if (x == goalX && y == goalY)
							{
								newGoalX = newx; newGoalY = newy;
							}
							else
							{
								newGoalX = goalX; newGoalY = goalY;
							}
							map<long, node> newnodes = nodes;
							newnodes[pos::xy(x, y)].move_to(newnodes[pos::xy(newx, newy)]);
							iteration next(newnodes, depth + 1, newGoalX, newGoalY);
							e.push_back(next);
						}
					}
				}
			}
		}
		return e;
	}

	bool continue_processing()
	{
		return !is_target;
	}

	bool operator<(const iteration& rhs)
	{
		long lhsGoalDist = goalX + goalY;
		long rhsGoalDist = rhs.goalX + rhs.goalY;
		if (lhsGoalDist != rhsGoalDist) return lhsGoalDist < rhsGoalDist;
	}
};

ostream& operator<<(ostream& os, const iteration& i)
{
	if (node::MAXX <= 2)
	{
		for (long y = 0; y <= node::MAXY; y++)
		{
			for (long x = 0; x <= node::MAXX; x++)
			{
				node n = i.nodes.at(pos::xy(x, y));
				if (x == i.goalX && y == i.goalY) os << " G ";
				else if (n.used > 20) os << " # ";
				else if (n.used >= 6) os << " . ";
				else os << " _ ";
			}
			os << endl;
		}
	}
	return os;
}

void initialize_movements()
{
	xmovements[0] = 0;
	xmovements[1] = 1;
	xmovements[2] = 0;
	xmovements[3] = -1;

	ymovements[0] = -1;
	ymovements[1] = 0;
	ymovements[2] = 1;
	ymovements[3] = 0;
}

int main()
{
	initialize_movements();

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
	int maxX = max_element(nodes.begin(), nodes.end(), compare_x)->second.x;
	int maxY = max_element(nodes.begin(), nodes.end(), compare_y)->second.y;

	iteration start(nodes, 0, node::MAXX, 0);
	cout << start;
	vector<iteration> firstlevel = start.expand();

	for (vector<iteration>::const_iterator it = firstlevel.begin(); it != firstlevel.end(); it++)
	{
		cout << *it << endl << endl;

	}
	//puzzle_iterator<iteration> iterator(start);
	
	//iteration best = iterator.get_best();
	
    return 0;
}

