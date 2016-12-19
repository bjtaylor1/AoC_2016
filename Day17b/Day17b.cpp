// Day17b.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "..\md5helper.h"

using namespace std;

template<class TPart>
class iteration
{
public:
	const char* passcode;
	string path;
	int x,y;
	bool is_target;
	TPart part;
	iteration(const char* _passcode, const string& _path, int _x, int _y) : passcode(_passcode), path(_path), x(_x), y(_y), is_target(x == 3 && y == 3) {}
	
	static bool is_open(char c)
	{
		return c == 'b' ||
			c == 'c' ||
			c == 'd' ||
			c == 'e' ||
			c == 'f';
	}

	vector<iteration> expand()
	{
		vector<iteration> e;
		string hash = getmd5hash(passcode + path);
		if (is_open(hash[0]) && y > 0) e.push_back(iteration(passcode, path + "U", x, y - 1));
		if (is_open(hash[1]) && y < 3) e.push_back(iteration(passcode, path + "D", x, y + 1));
		if (is_open(hash[2]) && x > 0) e.push_back(iteration(passcode, path + "L", x - 1, y));
		if (is_open(hash[3]) && x < 3) e.push_back(iteration(passcode, path + "R", x + 1, y));
		return e;
	}

	bool operator<(const iteration<TPart>& rhs)
	{
		return part.compare(*this, rhs);
	}

	void print_result()
	{
		part.print_result(*this);
	}

	bool continue_processing()
	{
		return !is_target;
	}
};

class part1
{
public:
	bool compare(const iteration<part1>& lhs, const iteration<part1>& rhs)
	{
		if (lhs.is_target != rhs.is_target) return lhs.is_target > rhs.is_target;
		return lhs.path.size() < rhs.path.size();
	}
	void print_result(const iteration<part1>& it)
	{
		cout << "Shortest path: " << it.path << endl;
	}
};

class part2
{
public:
	bool compare(const iteration<part2>& lhs, const iteration<part2>& rhs)
	{
		if (lhs.is_target != rhs.is_target) return lhs.is_target < rhs.is_target;
		return lhs.path.size() > rhs.path.size();
	}
	void print_result(const iteration<part2>& it)
	{
		cout << endl << "Longest path length: " << it.path.size() << endl;
	}

};

template <> bool puzzle_iterator<iteration<part1>>::is_cyclic()
{
	return false;
}
template <> bool puzzle_iterator<iteration<part2>>::is_cyclic()
{
	return false;
}

ostream& operator<<(ostream& os, const iteration<part1>& i)
{
	cout << i.path << endl;
	return os;
}
ostream& operator<<(ostream& os, const iteration<part2>& i)
{
	cout << "\r" << i.path.size();
	return os;
}

int main()
{
	const char* passcode = "pgflpeqp";
	{
		iteration<part1> start(passcode, "", 0, 0);
		puzzle_iterator<iteration<part1>> puzzle(start);
		iteration<part1> best = puzzle.get_best();
		best.print_result();
	}
	{
		iteration<part2> start(passcode, "", 0, 0);
		puzzle_iterator<iteration<part2>> puzzle(start);
		iteration<part2> best = puzzle.get_best();
		best.print_result();
	}
    return 0;
}

