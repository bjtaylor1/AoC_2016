template <class TIteration>
class puzzle_iterator
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
		bool gotBestTarget = false;
		while (is_exhaustive() || current->continue_processing())
		{
			if (current->visited)
			{
				throw std::exception("Already visited the best node.");
			}

			int size = its.size();

			vector<TIteration> newitems = current->expand();
			while (newitems.size() == 0)
			{
				current = its.erase(current);
				if (current == its.end())
				{
					if (is_exhaustive()) return besttarget;
					else throw exception("No more expansions.");
				}
				if (!current->visited) newitems = current->expand();
			}

			if (current->print())
			{
				cout << *current;
			}

			//remember the current node if it's cyclic
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

			//add the new items, and prune any dead ends and targets
			its.insert(newitems.begin(), newitems.end());
			for (set<TIteration>::iterator it = its.begin(); it != its.end(); it++)
			{
				if (prune(*it)) it = its.erase(it);
				if (it == its.end()) break;

				if (is_exhaustive() && it->is_target)
				{
					if (!gotBestTarget || *it < besttarget)
					{
						besttarget = *it;
						gotBestTarget = true;
					}
					it = its.erase(it);
					if (it == its.end()) break;
				}
			}

			//rewind to the current best
			current = its.begin();
			if (current == its.end())
			{
				throw exception("Sanity check failed.");
			}

			//spool through to the next unvisited
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
