// Day22.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;
#include "puzzle_input.h"
#include "pos.h"
#include "../puzzle_iterator.h"

extern long primes[];

bool is_viable(map<long, node>::const_iterator it_A, map<long, node>::const_iterator it_B)
{
	bool isViable = it_A != it_B && it_A->second.used > 0 && it_A->second.used <= it_B->second.avail;
	return isViable;
}

bool compare_x(const pair<long, node>& lhs, const pair<long, node>& rhs)
{
	return lhs.second.x < rhs.second.x;
}
bool compare_y(const pair<long, node>& lhs, const pair<long, node>& rhs)
{
	return lhs.second.y < rhs.second.y;
}

void node::move_to(node& target)
{
	if (target.avail < used) throw exception("Not enough space!");
	target.avail -= used;
	target.used += used;
	avail += used;
	used = 0;
}

int xmovements[4];
int ymovements[4];

unsigned long long thecount = 0;

int nextid = 0;

class iteration
{
public:

	map<long, node> nodes;
	long depth;
	long goalX, goalY;
	long score;
	bool visited;
	int id;
	long long hash;
	bool is_target;
	long spacex, spacey, spacetogoaldist, goaldist;
	iteration() {}
	iteration(const map<long, node>& _nodes, long _depth, long _goalX, long _goalY) :
		nodes(_nodes), depth(_depth), goalX(_goalX), goalY(_goalY), visited(false), id(nextid++), is_target(goalX == 0&& _goalY == 0), goaldist(_goalX + goalY)
	{
		bool goalCanMoveLeft = _goalX > 0 && _nodes.at(pos::xy(_goalX - 1, _goalY)).avail >= _nodes.at(pos::xy(_goalX, _goalY)).used;
		bool goalCanMoveUp = _goalY > 0 && _nodes.at(pos::xy(_goalX, _goalY - 1)).avail >= _nodes.at(pos::xy(_goalX, _goalY)).used;
		bool goalCanMoveRight = goalX < node::MAXX && _nodes.at(pos::xy(_goalX + 1, _goalY)).avail >= _nodes.at(pos::xy(_goalX, _goalY)).used;
		bool goalCanMoveDown = goalY < node::MAXY && _nodes.at(pos::xy(_goalX, _goalY + 1)).avail >= _nodes.at(pos::xy(_goalX, _goalY)).used;
		score = 0;
		if (goalCanMoveLeft) score += 9;
		if (goalCanMoveUp) score += 9;
		if (goalCanMoveRight) score += 1;
		if (goalCanMoveDown) score += 1;


		hash = 0; spacetogoaldist = 0x7fffffff;
		for (int x = 0; x <= node::MAXX; x++)
			for (int y = 0; y <= node::MAXY; y++)
			{
				long prime = primes[x + ((node::MAXX + 1)*y)];
				long used = nodes.at(pos::xy(x, y)).used;

				if (used == 0)
				{
					long spacetogoalx = abs(x - goalX);
					long spacetogoaly = abs(y - goalY);
					spacex = x;
					spacey = y;
					spacetogoaldist = abs(x - goalX) + abs(y - goalY);
				}
				hash += prime * used;
			}

	}

	iteration with_visited() const
	{
		iteration vis(*this);
		vis.visited = true;
		return vis;
	}

	vector<iteration> expand() const
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
						if (nodes.at(pos::xy(newx, newy)).avail >= nodes.at(pos::xy(x, y)).used && nodes.at(pos::xy(x, y)).used > 0)
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

	void print_swappability()
	{
		for (int y = 0; y <= node::MAXY; y++)
		{
			for (int x = 0; x <= node::MAXX; x++)
			{
				int swappability = 0;

				for (int move = 0; move < 4; move++)
				{
					int testx = x + xmovements[move];
					int testy = y + ymovements[move];
					if (testx >= 0 && testy >= 0 && testx <= node::MAXX && testy <= node::MAXY &&
						nodes.at(pos::xy(testx, testy)).size >= nodes.at(pos::xy(x, y)).used)
					{
						swappability++;
					}
				}
				if (x == goalX && y == goalY) cout << 'G';
				else if (nodes.at(pos::xy(x, y)).used == 0) cout << "0";
				else if (swappability == 0) cout << '#'; else cout << swappability;
			}
			cout << endl;
		}
	}

	bool continue_processing() const
	{
		return !(goalX == 0 && goalY == 0);
	}

	bool print() const
	{
		return node::MAXX <= 2 || ((++thecount) % 20) == 0;
	}
};
ostream& operator<<(ostream& os, const iteration& i);

bool compare(const iteration& lhs, const iteration& rhs)
{
	if (lhs.goaldist != rhs.goaldist) return lhs.goaldist < rhs.goaldist;
	if (lhs.spacetogoaldist != rhs.spacetogoaldist) return lhs.spacetogoaldist < rhs.spacetogoaldist;
	if (lhs.spacex != rhs.spacex) return  lhs.spacex < rhs.spacex;
	if (lhs.spacey != rhs.spacey) return lhs.spacey < rhs.spacey;
	if (lhs.goalX != rhs.goalX) return lhs.goalX < rhs.goalY;
	if (lhs.goalY != rhs.goalY) return lhs.goalY < rhs.goalY;
	
	return false;

}
bool operator<(const iteration& lhs, const iteration& rhs)
{
	bool res = compare(lhs, rhs);
	return res;
}

#define PRINT
iteration* startiteration;

ostream& operator<<(ostream& os, const iteration& i)
{
#ifdef PRINT

	//if (true || node::MAXX <= 2)
	{
		for (long y = 0; y <= node::MAXY; y++)
		{
			for (long x = 0; x <= node::MAXX; x++)
			{

				node n = i.nodes.at(pos::xy(x, y));
				if (x == i.goalX && y == i.goalY)
					cout << "G";
				else if (n.used == 0)
					cout << " ";
				else if (n.used > 100)
					cout << "#";
				else 
					cout << "-";
				/*
				int charindex = (n.used % 26);
				bool isgoal = x == i.goalX && y == i.goalY;
				int goalmod = ((int)isgoal) * 32;
				int basechar = ((int)'a');
				char usedchar = (char)(basechar + charindex - goalmod);
				cout << usedchar;
				*/
			}
			os << endl;
		}
		os << endl;
	}
#endif
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

class test
{
public:
	int score, id;
	test(int _id, int _score) : id(_id), score(_score) {}
	bool get_a_bool() const
	{
		return true;
	}
};

bool operator<(const test& lhs, const test& rhs)
{
	return lhs.score < rhs.score;

}

int main()
{
	initialize_movements();

	map<long, node> nodes;
	if (puzzle_input::add_nodes(nodes))
	{
		cout << "there are " << nodes.size() << " nodes." << endl;
	}

	//part 1:
	int viablePairs = 0;
	for (map<long, node>::const_iterator it_A = nodes.begin(); it_A != nodes.end(); it_A++)
	{
		for (map<long, node>::const_iterator it_B = nodes.begin(); it_B != nodes.end(); it_B++)
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
	startiteration = &start;

	vector<iteration> firstlevel = start.expand();

	start.print_swappability();

	puzzle_iterator<iteration> iterator(start);

	try
	{
		iteration best = iterator.get_best();
		cout << "Best : " << endl << best << endl << endl;
		cout << "Best steps = " << best.depth << endl;

	}
	catch (exception ex)
	{
		cout << ex.what() << endl;
	}
	cout << thecount << endl;

	return 0;
}

