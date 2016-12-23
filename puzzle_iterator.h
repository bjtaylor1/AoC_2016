template <class TIteration>
class puzzle_iterator
{
public:
	TIteration start;
	puzzle_iterator(const TIteration& _start) : start(_start) {}
	/**/
	
	static void get_best_unvisited(set<TIteration>& its)
	{
		set<TIteration>::const_iterator* it
		for (set<TIteration>::iterator it = its.begin(); it != its.end(); it++)
		{
			if (!it->visited)
			{
				//return it;
			}
		}
		//return its.end();
	}
	
	TIteration get_best()
	{
		set<TIteration> its;
		its.insert(start);
		set<TIteration>::const_iterator current = its.begin();
		while (current->continue_processing())
		{
			if (current->print())
			{
				cout << "size = " << its.size() << endl;
				cout << *current << endl;
			}

			if (current->visited)
			{
				throw std::exception("Already visited the best node.");
			}

			vector<TIteration> newitems = current->expand();
			/*
			cout << "ORIGINAL: " << endl << *current << endl;
			cout << "EXPAND: " << endl;
			for (vector<iteration>::const_iterator it = newitems.begin(); it != newitems.end(); it++)
			{
				cout << endl << *it << endl << endl;
			}
			throw exception("stop.");
			*/

			
			if (is_cyclic())
			{
				TIteration vis = current->with_visited();
				its.erase(current);
				its.insert(vis);
			}
			else
			{
				its.erase(current);
			}

			its.insert(newitems.begin(), newitems.end());

			current = its.begin();
			if (current == its.end())
			{
				throw exception("Sanity check failed.");
			}
			while (current->visited)
			{
				current++;
				if (current == its.end())
				{
					throw exception("No unvisited nodes left.");
				}
			}

		}
		return *current;
	}

	bool is_cyclic()
	{
		return true;
	}

	bool is_exhaustive()
	{
		return false;
	}
};
