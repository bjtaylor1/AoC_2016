template <class TIteration>
class exhaustive_iterator
{
public:
	TIteration start;
	puzzle_iterator(const TIteration& _start) : start(_start) {}

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
	}

	TIteration get_best()
	{
		set<TIteration> its;
		its.insert(start);
		set<TIteration>::const_iterator current = its.begin();
		TIteration besttarget;
		while (current->continue_processing())
		{
			if (current->visited)
			{
				throw std::exception("Already visited the best node.");
			}

			vector<TIteration> newitems = current->expand();
			while (newitems.size() == 0)
			{
				current = its.erase(current);
				if (current == its.end())
				{
					throw exception("No more expansions.");
				}
				if (!current->visited) newitems = current->expand();
			}

			if (current->print())
			{
				cout << *current << ", size = " << its.size() << "  ";
				int i = 0;
				cout << endl;*/
			}

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
			for (set<TIteration>::iterator it = its.begin(); it != its.end(); it++)
			{
				if (prune(*it)) it = its.erase(it);
				if (it == its.end()) break;
			}

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

	bool prune(const TIteration& i)
	{
		return false;
	}
};
