// Day19.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;

int get_answer(int numElves)
{
	int hpt_pow = (int)log2(numElves);
	int hpt = pow(2, hpt_pow);
	int ans = (numElves - hpt) * 2 + 1;
	return ans;
}

int get_target(vector<int>& elves, int it)
{
	int n = (int)elves.size() / 2;
	int target = (it + n) % elves.size();
	return target;
}

int get_answer_part2_slow(int numElves)
{
	vector<int> elves;
	for (int i = 1; i <= numElves; i++)
	{
		elves.push_back(i);
	}

	

	int it;
	for (it = 0; elves.size() > 1; it = (it + 1) % elves.size())
	{
		if (elves.size() % 1000 == 0) cout << "\r" << elves.size();
		int target = get_target(elves, it);
		if (target < it) it--;
		elves.erase(elves.begin() + target);
	}
	return *elves.begin();

}

int get_answer_part2(int numElves)
{
	if (numElves == 1) return 1;
	int b = (int)(log(numElves - 1) / log(3));
	int c = pow(3, b);
	int d = numElves - c;
	if (d > c) return c + (d - c) * 2;
	else return d;
}


int main()
{
	
	cout << get_answer_part2(3017957);

	return 0;
}


