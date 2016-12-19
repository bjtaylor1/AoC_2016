// Day17b.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "..\md5helper.h"

using namespace std;

class iteration
{
public:
	string path;
	int x,y;
	bool is_target;
	iteration(const string& _path, int _x, int _y) : path(_path), x(_x), y(_y) {}

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
		string hash = getmd5hash(path);
		if (is_open(hash[0]))
		return e;
	}

	bool operator<(const iteration& rhs)
	{
		return true;
	}

	void print_result()
	{

	}
};

ostream& operator<<(ostream& os, const iteration& i)
{
	os << i.path.substr(1, 50);
	return os;
}

int main()
{
	iteration start("ihgpwlah", 0, 0);
	puzzle_iterator<iteration> puzzle(start);
	iteration best = puzzle.get_best();
	best.print_result();
	cout << getmd5hash("hijkl") << endl;
    return 0;
}

