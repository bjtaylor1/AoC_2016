// Day18.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;

int get_safety(const string& s)
{
	int safety = 0;
	for (string::const_iterator it = s.begin(); it != s.end(); it++)
	{
		if ((*it) == '.') safety++;
	}
	return safety;
}

string get_next(const string& s)
{
	string result;
	for (int i = 0; i < s.size(); i++)
	{
		bool l = i > 0 && s[i-1] == '^';
		bool c = s[i] == '^';
		bool r = i < s.size() - 1 && s[i + 1] == '^';

		if ((l && c && !r) ||
			(c && r && !l) ||
			(l && !c && !r) ||
			(r && !c && !l))
		{
			result += '^';
		}
		else
		{
			result += '.';
		}
	}
	return result;
}

int doExample(const string& s, const int rows)
{
	map<string, int> seen;
	
	int safety = 0;
	string current = s;
	for (int row = 0; row < rows; row++)
	{
		//cout << "\r" << row;

		int thissafety = get_safety(current);

		cout << current << "  = " << thissafety << endl;
		safety += thissafety;
		current = get_next(current);
		
		map<string, int>::const_iterator entry = seen.find(current);
		if (entry != seen.end())
		{
			int repetitionsAvailable = (rows - row) / row;
			row += (row * repetitionsAvailable);
			safety += safety * entry->second;
		}
		else
		{
			seen.insert(make_pair(current, safety));
		}
	}
	cout << endl;
	return safety;
}

int main()
{
	cout << "safe tiles: " << doExample("....^....", 10) << endl << endl;
	cout << "safe tiles: " << doExample("..^^.", 3) << endl << endl;
	cout << "safe tiles: " << doExample(".^^.^.^^^^", 10) << endl << endl;
	//cout << "safe tiles: " << doExample("^^^^......^...^..^....^^^.^^^.^.^^^^^^..^...^^...^^^.^^....^..^^^.^.^^...^.^...^^.^^^.^^^^.^^.^..^.^", 40) << endl << endl;
	//cout << "safe tiles: " << doExample("^^^^......^...^..^....^^^.^^^.^.^^^^^^..^...^^...^^^.^^....^..^^^.^.^^...^.^...^^.^^^.^^^^.^^.^..^.^", 400000) << endl << endl;
    return 0;
}

