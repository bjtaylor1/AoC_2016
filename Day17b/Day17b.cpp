// Day17b.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "..\md5helper.h"

using namespace std;

class iteration
{
public:
	const char* passcode;
	string path;
	int x,y;
	bool is_target;
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

	virtual bool operator<(const iteration& rhs)
	{
		if (is_target != rhs.is_target) return is_target < rhs.is_target;
		return path.size() > rhs.path.size();
	}

	void print_result()
	{
		cout << "Longest path length: " << path.size() << endl;
	}

	bool continue_processing()
	{
		return !is_target;
	}
};

template <> bool puzzle_iterator<iteration>::is_cyclic()
{
	return false;
}

ostream& operator<<(ostream& os, const iteration& i)
{
	cout << "\r" << i.path.size();
	return os;
}

int main()
{
	const char* passcode = "pgflpeqp";
	iteration start(passcode, "", 0, 0);
	puzzle_iterator<iteration> puzzle(start);
	iteration best = puzzle.get_best();
	best.print_result();
    return 0;
}

